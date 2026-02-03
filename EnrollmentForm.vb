Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Security.Cryptography


Public Class EnrollmentForm

    Public Data As AppData
    Dim counter As Integer = 0
    Dim mysqlconn As MySqlConnection
    Dim dr As MySqlDataReader
    Dim cmd As MySqlCommand
    Dim id As String
    Private enrollmentProgressLabel As Label

    Sub New(ByVal data As AppData)
        InitializeComponent()
        Me.Data = data
        ExchangeData(False)
        AddHandler data.OnChange, AddressOf OnDataChange

    End Sub

    Private Sub OnDataChange()
        ExchangeData(False)
    End Sub

    Public Sub ExchangeData(ByVal read As Boolean)
        If (read) Then
            Data.EnrolledFingersMask = EnrollmentControl.EnrolledFingerMask
            Data.MaxEnrollFingerCount = EnrollmentControl.MaxEnrollFingerCount
            Data.Update()
        Else
            EnrollmentControl.EnrolledFingerMask = Data.EnrolledFingersMask
            EnrollmentControl.MaxEnrollFingerCount = Data.MaxEnrollFingerCount
        End If
    End Sub

    Sub EnrollmentControl_OnEnroll(ByVal Control As Object, ByVal Finger As Integer, ByVal Template As DPFP.Template, ByRef EventHandlerStatus As DPFP.Gui.EventHandlerStatus) Handles EnrollmentControl.OnEnroll
        If (Data.IsEventHandlerSucceeds) Then
            Data.Templates(Finger - 1) = Template
            ExchangeData(True)
            If Not saveme(Finger, Template) Then
                Data.Templates(Finger - 1) = Nothing
                ExchangeData(True)
                EventHandlerStatus = DPFP.Gui.EventHandlerStatus.Failure
                Exit Sub
            End If
            ListEvents.Items.Insert(0, String.Format("OnEnroll: finger {0}", Finger))
            counter += 1
            UpdateEnrollmentProgress()
        Else
            EventHandlerStatus = DPFP.Gui.EventHandlerStatus.Failure
        End If
    End Sub

    Sub EnrollmentControl_OnDelete(ByVal Control As Object, ByVal Finger As Integer, ByRef EventHandlerStatus As DPFP.Gui.EventHandlerStatus) Handles EnrollmentControl.OnDelete
        If (Data.IsEventHandlerSucceeds) Then
            Data.Templates(Finger - 1) = Nothing
            ExchangeData(True)
            ListEvents.Items.Insert(0, String.Format("OnDelete: finger {0}", Finger))
            counter -= 1
            deleteprint(Finger, Data.EnrolledFingersMask)
        Else
            EventHandlerStatus = DPFP.Gui.EventHandlerStatus.Failure
        End If
    End Sub

    Private Sub EnrollmentControl_OnCancelEnroll(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32) Handles EnrollmentControl.OnCancelEnroll
        ListEvents.Items.Insert(0, String.Format("OnCancelEnroll: {0}, finger {1}", ReaderSerialNumber, Finger))
    End Sub

    Private Sub EnrollmentControl_OnComplete(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32) Handles EnrollmentControl.OnComplete
        ListEvents.Items.Insert(0, String.Format("OnComplete: {0}, finger {1}", ReaderSerialNumber, Finger))
    End Sub

    Private Sub EnrollmentControl_OnFingerRemove(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32) Handles EnrollmentControl.OnFingerRemove
        ListEvents.Items.Insert(0, String.Format("OnFingerRemove: {0}, finger {1}", ReaderSerialNumber, Finger))
    End Sub

    Private Sub EnrollmentControl_OnFingerTouch(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32) Handles EnrollmentControl.OnFingerTouch
        ListEvents.Items.Insert(0, String.Format("OnFingerTouch: {0}, finger {1}", ReaderSerialNumber, Finger))
    End Sub

    Private Sub EnrollmentControl_OnReaderConnect(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32) Handles EnrollmentControl.OnReaderConnect
        ListEvents.Items.Insert(0, String.Format("OnReaderConnect: {0}, finger {1}", ReaderSerialNumber, Finger))
    End Sub

    Private Sub EnrollmentControl_OnReaderDisconnect(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32) Handles EnrollmentControl.OnReaderDisconnect
        ListEvents.Items.Insert(0, String.Format("OnReaderDisconnect: {0}, finger {1}", ReaderSerialNumber, Finger))
    End Sub

    Private Sub EnrollmentControl_OnSampleQuality(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32, ByVal CaptureFeedback As DPFP.Capture.CaptureFeedback) Handles EnrollmentControl.OnSampleQuality
        ListEvents.Items.Insert(0, String.Format("OnSampleQuality: {0}, finger {1}, {2}", ReaderSerialNumber, Finger, CaptureFeedback))
    End Sub

    Private Sub EnrollmentControl_OnStartEnroll(ByVal Control As System.Object, ByVal ReaderSerialNumber As System.String, ByVal Finger As System.Int32) Handles EnrollmentControl.OnStartEnroll
        ListEvents.Items.Insert(0, String.Format("OnStartEnroll: {0}, finger {1}", ReaderSerialNumber, Finger))
    End Sub

    Private Sub EnrollmentForm_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim mask As Integer = EnrollmentControl.EnrolledFingerMask
        Dim rightCount As Integer = CountFingersInRange(mask, 1, 5)
        Dim leftCount As Integer = CountFingersInRange(mask, 6, 10)
        If rightCount < 2 OrElse leftCount < 2 Then
            MessageBox.Show("Please enroll at least two right-hand and two left-hand fingers before closing.", "Enrollment Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            e.Cancel = True
            Return
        End If

        Main.loadenrolleddata()

    End Sub

    Private Sub EnrollmentForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ListEvents.Items.Clear()
        InitializeEnrollmentProgress()
        'txtPersonId.Text = Form1.ComboBox1.SelectedItem
    End Sub

    Private Sub CloseButton_Click(sender As System.Object, e As System.EventArgs)

        'If counter <> 3 Then
        'MsgBox("3 different fingers need to be enrolled")
        'Else
        Me.Close()
        'Form1.Panel1.BackgroundImage = My.Resources.punchclock_app
        'End If

    End Sub

    Function saveme(ByVal Finger As Integer, ByVal Templatee As DPFP.Template) As Boolean
        Dim ms1 As New MemoryStream()
        Templatee.Serialize(ms1)
        Dim data As Byte() = ms1.ToArray()
        Dim templateHash As Byte() = ComputeTemplateHash(data)

        If IsTemplateDuplicate(txtPersonId.Text.Trim(), Finger, templateHash) Then
            MessageBox.Show("This fingerprint already exists for another finger or user.", "Duplicate Fingerprint", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Using mysqlconn As New MySqlConnection(Module1.ConnectionString)
            Using cmdSave As New MySqlCommand("INSERT INTO new_enrollment (regno, finger_index, finger_name, template, template_hash) " &
                                              "VALUES (@regno, @finger_index, @finger_name, @template, @template_hash) " &
                                              "ON DUPLICATE KEY UPDATE template = VALUES(template), finger_name = VALUES(finger_name), template_hash = VALUES(template_hash)", mysqlconn)
                cmdSave.CommandType = CommandType.Text
                cmdSave.Parameters.AddWithValue("@regno", txtPersonId.Text.Trim())
                cmdSave.Parameters.AddWithValue("@finger_index", Finger)
                cmdSave.Parameters.AddWithValue("@finger_name", GetFingerName(Finger))
                cmdSave.Parameters.Add("@template", MySqlDbType.LongBlob).Value = data
                cmdSave.Parameters.Add("@template_hash", MySqlDbType.Binary, 32).Value = templateHash
                mysqlconn.Open()
                cmdSave.ExecuteNonQuery()
            End Using
        End Using
        Module1.LoadEnrolledData(True)
        AuditLogger.LogAction("FingerprintEnroll", txtPersonId.Text.Trim() & " | " & GetFingerName(Finger))
        Return True
    End Function


    Sub deleteprint(ByVal finger As Integer, ByVal fingermask As Object)
        Try
            Using mysqlconn As New MySqlConnection(Module1.ConnectionString)
                mysqlconn.Open()
                Using cmd1 As New MySqlCommand("DELETE FROM new_enrollment WHERE regno = @regno AND finger_index = @finger_index", mysqlconn)
                    cmd1.Parameters.AddWithValue("@regno", txtPersonId.Text.Trim())
                    cmd1.Parameters.AddWithValue("@finger_index", finger)
                    cmd1.ExecuteNonQuery()
                End Using
            End Using
            UpdateEnrollmentProgress()
            Module1.LoadEnrolledData(True)
            AuditLogger.LogAction("FingerprintDelete", txtPersonId.Text.Trim() & " | " & GetFingerName(finger))
        Catch ex As Exception
            AppLogger.LogError("Fingerprint delete error", ex)
            MsgBox(ex.ToString)
        End Try
    End Sub


    Sub loadfingeralreadytaken()
        Try
            Using mysqlconn As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT finger_index FROM new_enrollment WHERE regno = @regno", mysqlconn)
                    cmd.Parameters.AddWithValue("@regno", txtPersonId.Text.Trim())
                    mysqlconn.Open()
                    Using dr As MySqlDataReader = cmd.ExecuteReader()
                        Dim mask As Integer = 0
                        While dr.Read()
                            Dim idx As Integer = Convert.ToInt32(dr("finger_index"))
                            If idx >= 1 AndAlso idx <= 10 Then
                                mask = mask Or (1 << (idx - 1))
                            End If
                        End While
                        EnrollmentControl.EnrolledFingerMask = mask
                        Data.EnrolledFingersMask = mask
                    End Using
                End Using
            End Using
            UpdateEnrollmentProgress()
        Catch ex As Exception
            AppLogger.LogError("Enrollment load error", ex)

        End Try
    End Sub

    Private Function GetFingerName(ByVal fingerIndex As Integer) As String
        Select Case fingerIndex
            Case 1
                Return "Right Thumb"
            Case 2
                Return "Right Index"
            Case 3
                Return "Right Middle"
            Case 4
                Return "Right Ring"
            Case 5
                Return "Right Little"
            Case 6
                Return "Left Thumb"
            Case 7
                Return "Left Index"
            Case 8
                Return "Left Middle"
            Case 9
                Return "Left Ring"
            Case 10
                Return "Left Little"
        End Select
        Return "Finger " & fingerIndex.ToString()
    End Function

    Private Function CountFingersInRange(ByVal mask As Integer, ByVal startIndex As Integer, ByVal endIndex As Integer) As Integer
        Dim count As Integer = 0
        For i As Integer = startIndex To endIndex
            Dim bit As Integer = 1 << (i - 1)
            If (mask And bit) <> 0 Then
                count += 1
            End If
        Next
        Return count
    End Function

    Private Sub InitializeEnrollmentProgress()
        If enrollmentProgressLabel IsNot Nothing Then
            Return
        End If

        enrollmentProgressLabel = New Label With {
            .AutoSize = False,
            .Dock = DockStyle.Top,
            .Height = 26,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = Color.DarkSlateGray,
            .BackColor = Color.WhiteSmoke,
            .Padding = New Padding(6, 0, 0, 0)
        }
        GroupEvents.Controls.Add(enrollmentProgressLabel)
        enrollmentProgressLabel.BringToFront()
        UpdateEnrollmentProgress()
    End Sub

    Private Sub UpdateEnrollmentProgress()
        If enrollmentProgressLabel Is Nothing Then
            Return
        End If

        Dim mask As Integer = EnrollmentControl.EnrolledFingerMask
        Dim rightCount As Integer = CountFingersInRange(mask, 1, 5)
        Dim leftCount As Integer = CountFingersInRange(mask, 6, 10)
        Dim rightMissing As Integer = Math.Max(0, 2 - rightCount)
        Dim leftMissing As Integer = Math.Max(0, 2 - leftCount)

        Dim status As String
        If rightMissing = 0 AndAlso leftMissing = 0 Then
            status = "Minimum satisfied."
            enrollmentProgressLabel.ForeColor = Color.ForestGreen
        Else
            status = "Need R+" & rightMissing.ToString() & ", L+" & leftMissing.ToString() & "."
            enrollmentProgressLabel.ForeColor = Color.DarkRed
        End If

        enrollmentProgressLabel.Text = "Enrollment progress - Right: " & rightCount.ToString() & "/2, Left: " & leftCount.ToString() & "/2. " & status
    End Sub

    Private Function ComputeTemplateHash(ByVal templateBytes As Byte()) As Byte()
        Using sha As SHA256 = SHA256.Create()
            Return sha.ComputeHash(templateBytes)
        End Using
    End Function

    Private Function IsTemplateDuplicate(ByVal regno As String, ByVal fingerIndex As Integer, ByVal templateHash As Byte()) As Boolean
        Using mysqlconn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("SELECT regno, finger_index FROM new_enrollment WHERE template_hash = @template_hash LIMIT 1", mysqlconn)
                cmd.Parameters.Add("@template_hash", MySqlDbType.Binary, 32).Value = templateHash
                mysqlconn.Open()
                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    If dr.Read() Then
                        Dim existingRegno As String = dr("regno").ToString()
                        Dim existingFinger As Integer = Convert.ToInt32(dr("finger_index"))
                        If Not String.Equals(existingRegno, regno, StringComparison.OrdinalIgnoreCase) OrElse existingFinger <> fingerIndex Then
                            Return True
                        End If
                    End If
                End Using
            End Using
        End Using
        Return False
    End Function

    

    Private Sub EnrollmentForm_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        loadfingeralreadytaken()
        'id = Form1.ComboBox1.SelectedItem
    End Sub

    'Protected Overridable Sub Process(ByVal Sample As DPFP.Sample)
    '    ConvertSampleToBitmap(Sample)
    'End Sub

    'Protected Function ConvertSampleToBitmap(ByVal Sample As DPFP.Sample) As Bitmap
    '    Dim convertor As New DPFP.Capture.SampleConversion()  ' Create a sample convertor.
    '    Dim bitmap As Bitmap = Nothing              ' TODO: the size doesn't matter
    '    convertor.ConvertToPicture(Sample, bitmap)

    '    Try
    '        Invoke(New FunctionCall(AddressOf _picturebox1draw), bitmap)
    '    Catch ex As Exception

    '    End Try

    '    Return bitmap
    'End Function

    'Sub _picturebox1draw(ByVal bmp)
    '    PictureBox1.Image = New Bitmap(bmp, 157, 168)
    'End Sub
End Class
