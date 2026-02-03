Imports MySql.Data.MySqlClient
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Net.Http
Imports System.Text

Public Module BackupService
    Private ReadOnly TableOrder As String() = {
        "attendance",
        "new_enrollment",
        "students",
        "sessions",
        "session_settings",
        "sections",
        "years",
        "registration",
        "admin_logs"
    }

    Public Function BackupToApi() As Boolean
        Dim url As String = ConfigurationManager.AppSettings("BackupApiUrl")
        If String.IsNullOrWhiteSpace(url) Then
            AppLogger.LogError("Backup API URL is not configured.")
            Return False
        End If

        Try
            Dim xml As String = BuildBackupXml()
            Dim apiKey As String = ConfigurationManager.AppSettings("BackupApiKey")
            Dim timeoutSeconds As Integer = GetTimeoutSeconds(30)

            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds)
                If Not String.IsNullOrWhiteSpace(apiKey) Then
                    client.DefaultRequestHeaders.Add("X-Api-Key", apiKey)
                End If

                Dim content As New StringContent(xml, Encoding.UTF8, "application/xml")
                Dim response As HttpResponseMessage = client.PostAsync(url, content).Result
                If response.IsSuccessStatusCode Then
                    AppLogger.Success("Backup upload succeeded: " & response.StatusCode.ToString())
                    AuditLogger.LogAction("BackupUpload", response.StatusCode.ToString())
                    Return True
                End If

                Dim respBody As String = response.Content.ReadAsStringAsync().Result
                AppLogger.LogError("Backup upload failed: " & response.StatusCode.ToString() & " | " & respBody)
                Return False
            End Using
        Catch ex As Exception
            AppLogger.LogError("Backup upload error", ex)
            Return False
        End Try
    End Function

    Public Function RestoreFromApi() As Boolean
        Dim url As String = ConfigurationManager.AppSettings("RestoreApiUrl")
        If String.IsNullOrWhiteSpace(url) Then
            AppLogger.LogError("Restore API URL is not configured.")
            Return False
        End If

        Try
            Dim apiKey As String = ConfigurationManager.AppSettings("BackupApiKey")
            Dim timeoutSeconds As Integer = GetTimeoutSeconds(60)

            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds)
                If Not String.IsNullOrWhiteSpace(apiKey) Then
                    client.DefaultRequestHeaders.Add("X-Api-Key", apiKey)
                End If

                Dim response As HttpResponseMessage = client.GetAsync(url).Result
                If Not response.IsSuccessStatusCode Then
                    Dim respBody As String = response.Content.ReadAsStringAsync().Result
                    AppLogger.LogError("Restore download failed: " & response.StatusCode.ToString() & " | " & respBody)
                    Return False
                End If

                Dim xml As String = response.Content.ReadAsStringAsync().Result
                ApplyBackupXml(xml)
                AppLogger.Success("Restore completed.")
                AuditLogger.LogAction("RestoreFromApi", "OK")
                Return True
            End Using
        Catch ex As Exception
            AppLogger.LogError("Restore error", ex)
            Return False
        End Try
    End Function

    Private Function BuildBackupXml() As String
        Dim ds As New DataSet("biometric_backup")
        ds.EnforceConstraints = False

        Dim info As New DataTable("backup_info")
        info.Columns.Add("created_at")
        info.Columns.Add("machine")
        info.Columns.Add("app_version")
        Dim row As DataRow = info.NewRow()
        row("created_at") = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        row("machine") = Environment.MachineName
        row("app_version") = Application.ProductVersion
        info.Rows.Add(row)
        ds.Tables.Add(info)

        Using conn As New MySqlConnection(Module1.ConnectionString)
            conn.Open()
            For Each tableName As String In TableOrder
                Dim dt As New DataTable(tableName)
                Using cmd As New MySqlCommand("SELECT * FROM `" & tableName & "`", conn)
                    Using adapter As New MySqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using
                End Using
                ds.Tables.Add(dt)
            Next
        End Using

        Using sw As New StringWriter()
            ds.WriteXml(sw, XmlWriteMode.WriteSchema)
            Return sw.ToString()
        End Using
    End Function

    Private Sub ApplyBackupXml(ByVal xml As String)
        If String.IsNullOrWhiteSpace(xml) Then
            Throw New Exception("Restore payload is empty.")
        End If

        Dim ds As New DataSet()
        Using sr As New StringReader(xml)
            ds.ReadXml(sr, XmlReadMode.ReadSchema)
        End Using

        Using conn As New MySqlConnection(Module1.ConnectionString)
            conn.Open()
            Using tx As MySqlTransaction = conn.BeginTransaction()
                Using cmd As New MySqlCommand("SET FOREIGN_KEY_CHECKS=0;", conn, tx)
                    cmd.ExecuteNonQuery()
                End Using

                For Each tableName As String In TableOrder
                    Using cmd As New MySqlCommand("TRUNCATE TABLE `" & tableName & "`;", conn, tx)
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                For Each tableName As String In TableOrder
                    If Not ds.Tables.Contains(tableName) Then
                        Continue For
                    End If
                    InsertTableRows(conn, tx, ds.Tables(tableName))
                Next

                Using cmd As New MySqlCommand("SET FOREIGN_KEY_CHECKS=1;", conn, tx)
                    cmd.ExecuteNonQuery()
                End Using

                tx.Commit()
            End Using
        End Using
    End Sub

    Private Sub InsertTableRows(ByVal conn As MySqlConnection, ByVal tx As MySqlTransaction, ByVal table As DataTable)
        If table.Rows.Count = 0 Then
            Return
        End If

        Dim colNames As New List(Of String)()
        Dim paramNames As New List(Of String)()
        For Each col As DataColumn In table.Columns
            colNames.Add("`" & col.ColumnName & "`")
            paramNames.Add("@" & col.ColumnName)
        Next

        Dim sql As String = "INSERT INTO `" & table.TableName & "` (" & String.Join(",", colNames) & ") VALUES (" & String.Join(",", paramNames) & ")"
        Using cmd As New MySqlCommand(sql, conn, tx)
            For Each col As DataColumn In table.Columns
                cmd.Parameters.AddWithValue("@" & col.ColumnName, DBNull.Value)
            Next

            For Each row As DataRow In table.Rows
                For Each col As DataColumn In table.Columns
                    Dim value As Object = row(col)
                    If value Is Nothing OrElse value Is DBNull.Value Then
                        cmd.Parameters("@" & col.ColumnName).Value = DBNull.Value
                    Else
                        cmd.Parameters("@" & col.ColumnName).Value = value
                    End If
                Next
                cmd.ExecuteNonQuery()
            Next
        End Using
    End Sub

    Private Function GetTimeoutSeconds(ByVal fallback As Integer) As Integer
        Dim value As String = ConfigurationManager.AppSettings("BackupTimeoutSeconds")
        Dim seconds As Integer = fallback
        If Not String.IsNullOrWhiteSpace(value) Then
            Integer.TryParse(value, seconds)
        End If
        If seconds < 5 Then
            seconds = 5
        End If
        Return seconds
    End Function
End Module
