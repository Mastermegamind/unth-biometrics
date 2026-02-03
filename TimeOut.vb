Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Drawing
Imports System.Media
Public Class TimeOut
    Dim mysqlconn As MySqlConnection
    Dim dr As MySqlDataReader
    Dim cmd As MySqlCommand
    Private currentSessionId As Integer
    Private lblScannerStatus As Label
    Private Const AttendancePageSize As Integer = 200
    Private currentPage As Integer
    Private lblPageInfo As Label
    Private btnPrevPage As Button
    Private btnNextPage As Button

    Public Sub New()
        InitializeComponent()
    End Sub

    Sub OnComplete(ByVal Control As Object, ByVal FeatureSet As DPFP.FeatureSet, ByRef EventHandlerStatus As DPFP.Gui.EventHandlerStatus) Handles VerificationControl.OnComplete
        If Me.InvokeRequired Then
            Dim status As DPFP.Gui.EventHandlerStatus = EventHandlerStatus
            Me.Invoke(Sub()
                          HandleVerification(FeatureSet, status)
                      End Sub)
            EventHandlerStatus = status
        Else
            HandleVerification(FeatureSet, EventHandlerStatus)
        End If
    End Sub

    Private Sub HandleVerification(ByVal FeatureSet As DPFP.FeatureSet, ByRef EventHandlerStatus As DPFP.Gui.EventHandlerStatus)
        Dim ver As New DPFP.Verification.Verification()
        Dim res As New DPFP.Verification.Verification.Result()
        Dim plo As Integer = 0
        SetScannerStatus("Capturing", Color.SteelBlue, False)

        For Each template As DPFP.Template In fptemplist   ' Compare feature set with all stored templates:
            If Not template Is Nothing Then                     '   Get template from storage.
                ver.Verify(FeatureSet, template, res)           '   Compare feature set with particular template.
                FalseAcceptRate.Text = res.FARAchieved.ToString          '   Determine the current False Accept Rate
                If res.Verified Then
                    EventHandlerStatus = DPFP.Gui.EventHandlerStatus.Success
                    PictureBox2.Image = My.Resources.check_mark_symbol_transparent_background_22
                    SetScannerStatus("Matched", Color.ForestGreen, True)
                    Dim name As String = listofnames(plo)
                    TextBox1.Text = listofnames(plo)
                    autoselect(name)
                    TimeOut()
                    Exit For ' success
                End If
            End If

            plo += 1
        Next
        If Not res.Verified Then
            EventHandlerStatus = DPFP.Gui.EventHandlerStatus.Failure
            PictureBox2.Image = My.Resources.question_mark_PNG66
            TextBox1.Text = ""
            txtName.Text = ""
            SetScannerStatus("No match", Color.DarkRed, True)
        End If
    End Sub

    Sub autoselect(ByVal identity As String)
        Try
            Dim name As String = ""
            Using mysqlconn As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT name, passport FROM students WHERE regno = @regno", mysqlconn)
                    cmd.Parameters.AddWithValue("@regno", identity.Trim())
                    mysqlconn.Open()
                    Using dr As MySqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then
                            name = dr("name").ToString()
                            Dim passportFile As String = String.Empty
                            If Not dr.IsDBNull(dr.GetOrdinal("passport")) Then
                                passportFile = dr("passport").ToString()
                            End If
                            LoadPassportImage(passportFile)
                        End If
                    End Using
                End Using
            End Using
            If String.IsNullOrWhiteSpace(name) Then
                name = "No Name"
            End If

            Invoke(New FunctionCall(AddressOf setname), name)

        Catch ex As MySqlException
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try


    End Sub
    'Private Sub setjobtitle(ByVal jobtitle As String)
    '    TextBox2.Text = jobtitle
    'End Sub
    Private Sub setname(ByVal name As String)
        txtName.Text = name
    End Sub

    Private Sub LoadPassportImage(ByVal passportFile As String)
        If String.IsNullOrWhiteSpace(passportFile) Then
            PictureBox1.Image = Nothing
            Return
        End If
        Dim cached As Image = Module1.TryGetPassportImage(passportFile)
        If cached Is Nothing Then
            PictureBox1.Image = Nothing
            Return
        End If

        Dim oldImage As Image = PictureBox1.Image
        PictureBox1.Image = cached
        If oldImage IsNot Nothing AndAlso Not Object.ReferenceEquals(oldImage, cached) Then
            oldImage.Dispose()
        End If
    End Sub

    Public Sub TimeOut()
        Try
            ApplySessionSettings()
            Dim attendanceDate As Date = DateClockOut.Value.Date
            Dim timeoutValue As TimeSpan = DateTime.Now.TimeOfDay
            Dim sessionName As String = If(txtSessionName.Text, String.Empty).Trim()
            Dim courseCode As String = If(txtCourseCode.Text, String.Empty).Trim()
            Dim courseName As String = If(txtCourseName.Text, String.Empty).Trim()
            Dim dayName As String = If(txtDayName.Text, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(dayName) Then
                dayName = attendanceDate.ToString("dddd")
            End If
            Dim sessionId As Integer = currentSessionId

            Using cn As New MySqlConnection(Module1.ConnectionString)
                cn.Open()
                Using tx As MySqlTransaction = cn.BeginTransaction()
                    Using comd As New MySqlCommand("INSERT INTO attendance (regno, name, date, day_name, session_name, course_code, course_name, session_id, timein, timeout) " &
                                                  "VALUES (@regno, @name, @date, @day_name, @session_name, @course_code, @course_name, @session_id, @timein, @timeout) " &
                                                  "ON DUPLICATE KEY UPDATE timeout = VALUES(timeout), day_name = VALUES(day_name), course_name = VALUES(course_name), session_id = VALUES(session_id)", cn, tx)
                        comd.Parameters.AddWithValue("@regno", TextBox1.Text.Trim())
                        comd.Parameters.AddWithValue("@name", txtName.Text.Trim())
                        comd.Parameters.AddWithValue("@date", attendanceDate)
                        comd.Parameters.AddWithValue("@day_name", dayName)
                        comd.Parameters.AddWithValue("@session_name", sessionName)
                        comd.Parameters.AddWithValue("@course_code", courseCode)
                        comd.Parameters.AddWithValue("@course_name", If(String.IsNullOrWhiteSpace(courseName), DBNull.Value, courseName))
                        comd.Parameters.AddWithValue("@session_id", If(sessionId > 0, sessionId, DBNull.Value))
                        comd.Parameters.AddWithValue("@timein", timeoutValue)
                        comd.Parameters.AddWithValue("@timeout", timeoutValue)
                        comd.ExecuteNonQuery()
                    End Using
                    tx.Commit()
                End Using
            End Using
            AppLogger.Success("Clock-out: " & TextBox1.Text.Trim() & " | " & sessionName & " | " & courseCode)

            ListView1.Items.Clear()
            RefreshAttendanceList()

        Catch ex As Exception
            AppLogger.LogError("Clock-out error", ex)
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    'Public Sub Reset()
    '    TextBox1.Text = Nothing
    '    'TextBox2.Text = Nothing
    '    txtName.Text = Nothing
    '    PictureBox1.Image = My.Resources.photo
    'End Sub
    Private Sub BackgroundWorker1_DoWork(sender As System.Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim results As New List(Of String())()
        Dim totalCount As Integer = 0
        Dim pageIndex As Integer = currentPage
        Dim sessionId As Integer = currentSessionId

        Try
            Using mysqlconn As New MySqlConnection(Module1.ConnectionString)
                mysqlconn.Open()

                Using cmdCount As New MySqlCommand("SELECT COUNT(*) FROM attendance WHERE date = @date AND (@session_id IS NULL OR session_id = @session_id)", mysqlconn)
                    cmdCount.Parameters.AddWithValue("@date", DateClockOut.Value.Date)
                    cmdCount.Parameters.AddWithValue("@session_id", If(sessionId > 0, sessionId, DBNull.Value))
                    totalCount = Convert.ToInt32(cmdCount.ExecuteScalar())
                End Using

                Dim offset As Integer = pageIndex * AttendancePageSize
                If totalCount > 0 AndAlso offset >= totalCount Then
                    pageIndex = 0
                    offset = 0
                End If

                Using cmd As New MySqlCommand("SELECT regno, name, date, day_name, session_name, course_code, timein, timeout FROM attendance " &
                                              "WHERE date = @date AND (@session_id IS NULL OR session_id = @session_id) " &
                                              "ORDER BY timein DESC, id DESC LIMIT @limit OFFSET @offset", mysqlconn)
                    cmd.Parameters.AddWithValue("@date", DateClockOut.Value.Date)
                    cmd.Parameters.AddWithValue("@session_id", If(sessionId > 0, sessionId, DBNull.Value))
                    cmd.Parameters.AddWithValue("@limit", AttendancePageSize)
                    cmd.Parameters.AddWithValue("@offset", offset)
                    Using sqlReader As MySqlDataReader = cmd.ExecuteReader()
                        While sqlReader.Read()
                            results.Add(New String() {
                                sqlReader("regno").ToString(),
                                sqlReader("name").ToString(),
                                sqlReader("date").ToString(),
                                sqlReader("day_name").ToString(),
                                sqlReader("session_name").ToString(),
                                sqlReader("course_code").ToString(),
                                sqlReader("timein").ToString(),
                                sqlReader("timeout").ToString()
                            })
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        Dim payload As Tuple(Of List(Of String()), Integer, Integer) = New Tuple(Of List(Of String()), Integer, Integer)(results, totalCount, pageIndex)

        Me.Invoke(Sub()
                      currentPage = payload.Item3
                      ListView1.BeginUpdate()
                      ListView1.Items.Clear()
                      For Each row In payload.Item1
                          Dim li As ListViewItem = ListView1.Items.Add(row(0))
                          li.SubItems.Add(row(1))
                          li.SubItems.Add(row(2))
                          li.SubItems.Add(row(3))
                          li.SubItems.Add(row(4))
                          li.SubItems.Add(row(5))
                          li.SubItems.Add(row(6))
                          li.SubItems.Add(row(7))
                          If li.Index Mod 3 = 0 Then
                              li.BackColor = Color.LightBlue
                          End If
                      Next
                      ListView1.EndUpdate()

                      Dim totalPages As Integer = If(payload.Item2 = 0, 0, CInt(Math.Ceiling(payload.Item2 / CDbl(AttendancePageSize))))
                      Dim pageDisplay As Integer = If(totalPages = 0, 0, payload.Item3 + 1)
                      Label4.Text = payload.Item2.ToString()
                      If lblPageInfo IsNot Nothing Then
                          lblPageInfo.Text = "Page " & pageDisplay.ToString() & "/" & totalPages.ToString()
                      End If
                      If btnPrevPage IsNot Nothing Then
                          btnPrevPage.Enabled = payload.Item3 > 0
                      End If
                      If btnNextPage IsNot Nothing Then
                          btnNextPage.Enabled = payload.Item3 + 1 < totalPages
                      End If
                  End Sub)
    End Sub

    Private Sub CloseButton_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub TimeOut_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Module1.LoadEnrolledData()
        Catch ex As Exception
            AppLogger.LogError("Load enrolled data (clock-out) failed", ex)
        End Try
        ApplySessionSettings(True)
        EnsureStatusLabel()
        InitializePagingControls()
        SetScannerStatus("Ready", Color.DimGray, False)
        txtDayName.ReadOnly = True
        txtSessionName.ReadOnly = True
        txtCourseCode.ReadOnly = True
        txtCourseName.ReadOnly = True
        DateClockOut.Enabled = False
    End Sub

    Public Sub TimeOut_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        If Me.TopLevel Then
            SetScannerActive(True)
        End If
        RefreshAttendanceList()
    End Sub

    Private Sub DateClockOut_ValueChanged(sender As Object, e As EventArgs) Handles DateClockOut.ValueChanged
        If Not txtDayName.ReadOnly Then
            txtDayName.Text = DateClockOut.Value.ToString("dddd")
        End If
    End Sub

    Public Sub Reset()
        TextBox1.Text = Nothing
        txtName.Text = Nothing
        PictureBox1.Image = Nothing
    End Sub

    Public Sub SetScannerActive(ByVal isActive As Boolean)
        VerificationControl.Active = isActive
        If isActive Then
            SetScannerStatus("Ready", Color.DimGray, False)
        Else
            SetScannerStatus("Paused", Color.Gray, False)
        End If
    End Sub

    Public Sub RefreshSessionSettings()
        ApplySessionSettings(True)
        RefreshAttendanceList()
    End Sub

    Public Sub ApplySessionInfo(ByVal info As SessionInfo)
        If info.SessionId <> currentSessionId Then
            currentSessionId = info.SessionId
            Module1.ClearPassportCache()
            currentPage = 0
        End If
        DateClockOut.Value = info.SessionDate
        txtDayName.Text = If(String.IsNullOrWhiteSpace(info.DayName), info.SessionDate.ToString("dddd"), info.DayName)
        txtSessionName.Text = info.SessionName
        txtCourseCode.Text = info.CourseCode
        txtCourseName.Text = info.CourseName
    End Sub

    Private Sub ApplySessionSettings(Optional ByVal forceRefresh As Boolean = False)
        Dim info As SessionInfo = SessionStore.GetCurrentSession(forceRefresh)
        ApplySessionInfo(info)
    End Sub

    Private Sub EnsureStatusLabel()
        If lblScannerStatus IsNot Nothing Then
            Return
        End If

        lblScannerStatus = New Label With {
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Tahoma", 9.0!, FontStyle.Bold),
            .ForeColor = Color.DimGray,
            .Text = "Ready",
            .Size = New Size(250, 20)
        }

        lblScannerStatus.Location = New Point(13, 188)
        GroupBox1.Controls.Add(lblScannerStatus)
        lblScannerStatus.BringToFront()
    End Sub

    Private Sub SetScannerStatus(ByVal statusText As String, ByVal statusColor As Color, ByVal playSound As Boolean)
        If lblScannerStatus Is Nothing Then
            Return
        End If
        lblScannerStatus.Text = statusText
        lblScannerStatus.ForeColor = statusColor

        If playSound Then
            If statusColor = Color.ForestGreen Then
                SystemSounds.Asterisk.Play()
            ElseIf statusColor = Color.DarkRed Then
                SystemSounds.Hand.Play()
            End If
        End If
    End Sub

    Private Sub InitializePagingControls()
        If lblPageInfo IsNot Nothing Then
            Return
        End If

        lblPageInfo = New Label With {
            .AutoSize = True,
            .Location = New Point(Label5.Left + 80, Label5.Top),
            .Text = "Page 0/0"
        }
        btnPrevPage = New Button With {
            .Text = "Prev",
            .Size = New Size(60, 22),
            .Location = New Point(lblPageInfo.Right + 10, Label5.Top - 4)
        }
        btnNextPage = New Button With {
            .Text = "Next",
            .Size = New Size(60, 22),
            .Location = New Point(btnPrevPage.Right + 6, Label5.Top - 4)
        }

        AddHandler btnPrevPage.Click, AddressOf PrevPage_Click
        AddHandler btnNextPage.Click, AddressOf NextPage_Click

        Controls.Add(lblPageInfo)
        Controls.Add(btnPrevPage)
        Controls.Add(btnNextPage)
    End Sub

    Private Sub PrevPage_Click(sender As Object, e As EventArgs)
        If currentPage > 0 Then
            currentPage -= 1
            RefreshAttendanceList()
        End If
    End Sub

    Private Sub NextPage_Click(sender As Object, e As EventArgs)
        currentPage += 1
        RefreshAttendanceList()
    End Sub

    Private Sub RefreshAttendanceList()
        If Not BackgroundWorker1.IsBusy Then
            BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub

End Class

