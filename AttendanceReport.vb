Imports MySql.Data.MySqlClient
Imports System.Data.OleDb
Imports System.Linq

Public Class AttendanceReport
    Private lastReport As DataTable

    Private Sub btnView_Click(sender As Object, e As EventArgs) Handles btnView.Click
        SelectGridView()
        groupBox1.Visible = True
    End Sub

    Sub SelectGridView()
        Try
            Dim sessionId As Integer = SessionStore.GetCurrentSession().SessionId

            Using con As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT name as 'Student Name', regno as 'Reg No', session_id as 'Session ID', date, day_name as 'Day Name', session_name as 'Session', course_code as 'Course Code', course_name as 'Course Name', timein as 'Time-In', timeout as 'Time-Out' " &
                                              "FROM attendance WHERE date BETWEEN @dateFrom AND @dateTo AND (@session_id IS NULL OR session_id = @session_id)", con)
                    cmd.Parameters.AddWithValue("@dateFrom", dtpDateFrom.Value.Date)
                    cmd.Parameters.AddWithValue("@dateTo", dtpDateTo.Value.Date)
                    cmd.Parameters.AddWithValue("@session_id", If(sessionId > 0, sessionId, DBNull.Value))

                    con.Open()
                    Dim da As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    lastReport = dt
                    Dim bs As New BindingSource()
                    bs.DataSource = dt
                    dataGridViewEmployee.DataSource = dt
                End Using
            End Using
            'btnExportCSV.Enabled = True;
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Reset()
        dataGridViewEmployee.DataSource = ""
        txtStudentName.Text = ""
        lblNumCount.Text = ""
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        Reset()
    End Sub

    Public Sub AutoDisplayName()
        Try
            Dim sessionId As Integer = SessionStore.GetCurrentSession().SessionId
            Using con As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT name FROM attendance WHERE (@session_id IS NULL OR session_id = @session_id)", con)
                    cmd.Parameters.AddWithValue("@session_id", If(sessionId > 0, sessionId, DBNull.Value))
                    Dim ds As New DataSet()
                    Dim da As New MySqlDataAdapter(cmd)
                    con.Open()
                    da.Fill(ds, "list") 'list can be any name u want

                    Dim col As New AutoCompleteStringCollection
                    Dim i As Integer
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        col.Add(ds.Tables(0).Rows(i)("name").ToString())  'columnname same As In query

                    Next

                    txtStudentName.AutoCompleteSource = AutoCompleteSource.CustomSource
                    txtStudentName.AutoCompleteCustomSource = col
                    txtStudentName.AutoCompleteMode = AutoCompleteMode.Suggest
                End Using
            End Using
        Catch ex As MySqlException
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try

    End Sub

    Private Sub AttendanceReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AutoDisplayName()
    End Sub

    Public Sub CalculateAttendance()
        Try
            Dim sessionId As Integer = SessionStore.GetCurrentSession().SessionId
            Using constring As New MySqlConnection(Module1.ConnectionString)
                Using comd As New MySqlCommand("SELECT COUNT(*) FROM attendance WHERE name = @name AND date BETWEEN @dateFrom AND @dateTo AND (@session_id IS NULL OR session_id = @session_id)", constring)
                    comd.CommandType = CommandType.Text
                    comd.Parameters.AddWithValue("@name", txtStudentName.Text.Trim())
                    comd.Parameters.AddWithValue("@dateFrom", dtpDateFrom.Value.Date)
                    comd.Parameters.AddWithValue("@dateTo", dtpDateTo.Value.Date)
                    comd.Parameters.AddWithValue("@session_id", If(sessionId > 0, sessionId, DBNull.Value))
                    constring.Open()

                    Dim o As Object = comd.ExecuteScalar()

                    If (Not o Is Nothing) Then
                        lblNumCount.Text = o.ToString()
                        panel1.Visible = True
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub txtStudentName_TextChanged(sender As Object, e As EventArgs) Handles txtStudentName.TextChanged
        CalculateAttendance()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If lastReport Is Nothing OrElse lastReport.Rows.Count = 0 Then
            MessageBox.Show("No data to export. Run a report first.")
            Return
        End If

        Using dialog As New SaveFileDialog()
            dialog.Filter = "CSV files (*.csv)|*.csv|Excel files (*.xlsx)|*.xlsx"
            dialog.Title = "Export Attendance"
            dialog.FileName = "attendance_export.csv"
            If dialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Try
                If dialog.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                    ExportCsv(lastReport, dialog.FileName)
                Else
                    ExportExcel(lastReport, dialog.FileName, "Attendance")
                End If
                MessageBox.Show("Export completed.")
            Catch ex As Exception
                MessageBox.Show("Export failed: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub ExportCsv(ByVal dt As DataTable, ByVal filePath As String)
        Dim sb As New System.Text.StringBuilder()
        Dim headers = dt.Columns.Cast(Of DataColumn)().Select(Function(c) EscapeCsv(c.ColumnName))
        sb.AppendLine(String.Join(",", headers))

        For Each row As DataRow In dt.Rows
            Dim fields = dt.Columns.Cast(Of DataColumn)().Select(Function(c) EscapeCsv(row(c).ToString()))
            sb.AppendLine(String.Join(",", fields))
        Next

        System.IO.File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8)
    End Sub

    Private Sub ExportExcel(ByVal dt As DataTable, ByVal filePath As String, ByVal sheetName As String)
        Try
            Dim connStr As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties='Excel 12.0 Xml;HDR=YES';"
            Using conn As New OleDb.OleDbConnection(connStr)
                conn.Open()
                Using cmd As New OleDb.OleDbCommand(BuildCreateTableSql(sheetName, dt), conn)
                    cmd.ExecuteNonQuery()
                End Using

                For Each row As DataRow In dt.Rows
                    Using cmd As New OleDb.OleDbCommand(BuildInsertSql(sheetName, dt), conn)
                        For Each col As DataColumn In dt.Columns
                            cmd.Parameters.AddWithValue("@" & col.ColumnName, row(col))
                        Next
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Using
        Catch ex As Exception
            Throw New Exception("Excel export failed. Install Microsoft Access Database Engine or export to CSV. " & ex.Message)
        End Try
    End Sub

    Private Function BuildCreateTableSql(ByVal sheetName As String, ByVal dt As DataTable) As String
        Dim cols = dt.Columns.Cast(Of DataColumn)().Select(Function(c) "[" & c.ColumnName & "] TEXT")
        Return "CREATE TABLE [" & sheetName & "] (" & String.Join(",", cols) & ")"
    End Function

    Private Function BuildInsertSql(ByVal sheetName As String, ByVal dt As DataTable) As String
        Dim colNames = dt.Columns.Cast(Of DataColumn)().Select(Function(c) "[" & c.ColumnName & "]")
        Dim placeholders = dt.Columns.Cast(Of DataColumn)().Select(Function(c) "?")
        Return "INSERT INTO [" & sheetName & "] (" & String.Join(",", colNames) & ") VALUES (" & String.Join(",", placeholders) & ")"
    End Function

    Private Function EscapeCsv(ByVal value As String) As String
        If value Is Nothing Then
            Return ""
        End If
        Dim needsQuotes As Boolean = value.Contains(",") OrElse value.Contains("""") OrElse value.Contains(ControlChars.Cr) OrElse value.Contains(ControlChars.Lf)
        If needsQuotes Then
            value = value.Replace("""", """""")
            Return """" & value & """"
        End If
        Return value
    End Function
End Class
