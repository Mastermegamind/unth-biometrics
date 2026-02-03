Imports MySql.Data.MySqlClient

Public Module AuditLogger
    Public Sub LogAction(ByVal actionName As String, ByVal details As String)
        If String.IsNullOrWhiteSpace(actionName) Then
            Return
        End If

        Dim username As String = Module1.CurrentUserName
        If String.IsNullOrWhiteSpace(username) Then
            username = "system"
        End If

        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("INSERT INTO admin_logs (username, action_name, details) VALUES (@username, @action, @details)", conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    cmd.Parameters.AddWithValue("@action", actionName.Trim())
                    cmd.Parameters.AddWithValue("@details", If(details, String.Empty))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            AppLogger.Success("Audit: " & actionName.Trim() & " | " & username & " | " & If(details, String.Empty))
        Catch ex As Exception
            ' Logging must never block core flows.
            AppLogger.LogError("Audit failed: " & actionName.Trim(), ex)
        End Try
    End Sub
End Module
