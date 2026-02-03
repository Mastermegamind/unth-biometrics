Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections.Generic

Public Class SessionSetup
    Inherits Form

    Private ReadOnly txtDayName As TextBox
    Private ReadOnly txtSessionName As TextBox
    Private ReadOnly txtCourseCode As TextBox
    Private ReadOnly txtCourseName As TextBox
    Private ReadOnly cmbSessions As ComboBox
    Private ReadOnly dtSessionDate As DateTimePicker
    Private ReadOnly btnSave As Button
    Private ReadOnly btnNew As Button
    Private ReadOnly btnUpdate As Button
    Private ReadOnly btnDelete As Button
    Private ReadOnly btnReload As Button
    Private ReadOnly btnSetActive As Button
    Private isLoadingSessions As Boolean
    Private currentSessionId As Integer

    Public Sub New()
        Text = "Session Setup"
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Size = New Size(520, 320)

        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 7,
            .Padding = New Padding(16)
        }
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35.0F))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65.0F))

        dtSessionDate = New DateTimePicker With {.Format = DateTimePickerFormat.Short, .Dock = DockStyle.Fill}
        txtDayName = New TextBox With {.Dock = DockStyle.Fill}
        txtSessionName = New TextBox With {.Dock = DockStyle.Fill}
        txtCourseCode = New TextBox With {.Dock = DockStyle.Fill}
        txtCourseName = New TextBox With {.Dock = DockStyle.Fill}
        cmbSessions = New ComboBox With {.Dock = DockStyle.Fill, .DropDownStyle = ComboBoxStyle.DropDownList}

        btnSave = New Button With {.Text = "Create Session", .Dock = DockStyle.Fill, .Height = 32}
        btnNew = New Button With {.Text = "New", .Dock = DockStyle.Fill, .Height = 32}
        btnUpdate = New Button With {.Text = "Update", .Dock = DockStyle.Fill, .Height = 32}
        btnDelete = New Button With {.Text = "Delete", .Dock = DockStyle.Fill, .Height = 32}
        btnReload = New Button With {.Text = "Reload", .Dock = DockStyle.Fill, .Height = 32}
        btnSetActive = New Button With {.Text = "Set Active", .Dock = DockStyle.Fill, .Height = 32}

        layout.Controls.Add(New Label With {.Text = "Session Date:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 0)
        layout.Controls.Add(dtSessionDate, 1, 0)
        layout.Controls.Add(New Label With {.Text = "Day Name:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 1)
        layout.Controls.Add(txtDayName, 1, 1)
        layout.Controls.Add(New Label With {.Text = "Session Name:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 2)
        layout.Controls.Add(txtSessionName, 1, 2)
        layout.Controls.Add(New Label With {.Text = "Course Code:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 3)
        layout.Controls.Add(txtCourseCode, 1, 3)
        layout.Controls.Add(New Label With {.Text = "Course Name:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 4)
        layout.Controls.Add(txtCourseName, 1, 4)
        layout.Controls.Add(New Label With {.Text = "Existing Sessions:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 5)
        layout.Controls.Add(cmbSessions, 1, 5)

        Dim buttonPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .AutoSize = True
        }
        buttonPanel.Controls.Add(btnSetActive)
        buttonPanel.Controls.Add(btnReload)
        buttonPanel.Controls.Add(btnDelete)
        buttonPanel.Controls.Add(btnUpdate)
        buttonPanel.Controls.Add(btnSave)
        buttonPanel.Controls.Add(btnNew)
        layout.Controls.Add(buttonPanel, 1, 6)

        Controls.Add(layout)

        AddHandler btnSave.Click, AddressOf SaveSession
        AddHandler btnNew.Click, AddressOf NewSession
        AddHandler btnUpdate.Click, AddressOf UpdateSession
        AddHandler btnDelete.Click, AddressOf DeleteSession
        AddHandler btnReload.Click, AddressOf ReloadSession
        AddHandler btnSetActive.Click, AddressOf SetActiveSession
        AddHandler dtSessionDate.ValueChanged, AddressOf SessionDateChanged
        AddHandler cmbSessions.SelectedIndexChanged, AddressOf SelectedSessionChanged

        ReloadSession(Nothing, EventArgs.Empty)
    End Sub

    Private Sub SessionDateChanged(sender As Object, e As EventArgs)
        txtDayName.Text = dtSessionDate.Value.ToString("dddd")
    End Sub

    Private Sub ReloadSession(sender As Object, e As EventArgs)
        Dim info As SessionInfo = SessionStore.GetCurrentSession(True)
        ApplySessionInfo(info)
        LoadSessionsList(info.SessionId)
    End Sub

    Private Sub SaveSession(sender As Object, e As EventArgs)
        If currentSessionId > 0 Then
            Dim confirmCreate As DialogResult = MessageBox.Show("A session is selected. Create a NEW session instead of updating?" & Environment.NewLine & "Use Update to modify the selected session.", "Create New Session", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If confirmCreate <> DialogResult.Yes Then
                Return
            End If
        End If

        Dim info As New SessionInfo With {
            .SessionDate = dtSessionDate.Value.Date,
            .DayName = txtDayName.Text.Trim(),
            .SessionName = txtSessionName.Text.Trim(),
            .CourseCode = txtCourseCode.Text.Trim(),
            .CourseName = txtCourseName.Text.Trim()
        }

        If String.IsNullOrWhiteSpace(info.DayName) Then
            info.DayName = info.SessionDate.ToString("dddd")
        End If

        SessionStore.SaveCurrentSession(info)
        LoadSessionsList(info.SessionId)
        currentSessionId = info.SessionId
        UpdateCrudState()
        MessageBox.Show("Session settings saved.", "Session Setup", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub NewSession(sender As Object, e As EventArgs)
        isLoadingSessions = True
        cmbSessions.SelectedIndex = -1
        isLoadingSessions = False

        currentSessionId = 0
        dtSessionDate.Value = Date.Today
        txtDayName.Text = dtSessionDate.Value.ToString("dddd")
        txtSessionName.Text = String.Empty
        txtCourseCode.Text = String.Empty
        txtCourseName.Text = String.Empty
        UpdateCrudState()
    End Sub

    Private Sub UpdateSession(sender As Object, e As EventArgs)
        If currentSessionId <= 0 Then
            MessageBox.Show("Select a session to update.", "Session Setup", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim info As New SessionInfo With {
            .SessionId = currentSessionId,
            .SessionDate = dtSessionDate.Value.Date,
            .DayName = txtDayName.Text.Trim(),
            .SessionName = txtSessionName.Text.Trim(),
            .CourseCode = txtCourseCode.Text.Trim(),
            .CourseName = txtCourseName.Text.Trim()
        }

        If String.IsNullOrWhiteSpace(info.DayName) Then
            info.DayName = info.SessionDate.ToString("dddd")
        End If

        Dim updated As Boolean = SessionStore.UpdateSession(currentSessionId, info)
        If updated Then
            LoadSessionsList(currentSessionId)
            MessageBox.Show("Session updated.", "Session Setup", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Update failed. Please try again.", "Session Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub DeleteSession(sender As Object, e As EventArgs)
        If currentSessionId <= 0 Then
            MessageBox.Show("Select a session to delete.", "Session Setup", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim attendanceCount As Integer = SessionStore.GetAttendanceCount(currentSessionId)
        Dim warning As String = "Delete this session?"
        If attendanceCount > 0 Then
            warning = "This session has " & attendanceCount.ToString() & " attendance record(s)." & Environment.NewLine & "Delete anyway?"
        End If

        Dim confirm As DialogResult = MessageBox.Show(warning, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm <> DialogResult.Yes Then
            Return
        End If

        Dim deleted As Boolean = SessionStore.DeleteSession(currentSessionId)
        If deleted Then
            NewSession(Nothing, EventArgs.Empty)
            LoadSessionsList()
            MessageBox.Show("Session deleted.", "Session Setup", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Delete failed. Please try again.", "Session Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub SetActiveSession(sender As Object, e As EventArgs)
        Dim info As SessionInfo = TryCast(cmbSessions.SelectedItem, SessionInfo)
        If info Is Nothing OrElse info.SessionId <= 0 Then
            Return
        End If
        Dim confirm As DialogResult = MessageBox.Show("Set this session as active?" & Environment.NewLine & info.DisplayText, "Confirm Active Session", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm <> DialogResult.Yes Then
            Return
        End If

        SessionStore.SetActiveSession(info.SessionId)
        ApplySessionInfo(info)
        Text = "Session Setup - Active: " & info.DisplayText
    End Sub

    Private Sub SelectedSessionChanged(sender As Object, e As EventArgs)
        If isLoadingSessions Then
            Return
        End If

        Dim info As SessionInfo = TryCast(cmbSessions.SelectedItem, SessionInfo)
        If info Is Nothing Then
            Return
        End If

        ApplySessionInfo(info)
    End Sub

    Private Sub ApplySessionInfo(ByVal info As SessionInfo)
        dtSessionDate.Value = info.SessionDate
        txtDayName.Text = If(String.IsNullOrWhiteSpace(info.DayName), dtSessionDate.Value.ToString("dddd"), info.DayName)
        txtSessionName.Text = info.SessionName
        txtCourseCode.Text = info.CourseCode
        txtCourseName.Text = info.CourseName
        currentSessionId = info.SessionId
        UpdateCrudState()
    End Sub

    Private Sub LoadSessionsList(Optional ByVal selectId As Integer = 0)
        isLoadingSessions = True
        Dim sessions As List(Of SessionInfo) = SessionStore.GetSessionsList()
        cmbSessions.DataSource = Nothing
        cmbSessions.DisplayMember = "DisplayText"
        cmbSessions.ValueMember = "SessionId"
        cmbSessions.DataSource = sessions

        Dim activeInfo As SessionInfo = SessionStore.GetCurrentSession()
        Dim targetId As Integer = If(selectId > 0, selectId, activeInfo.SessionId)
        If targetId > 0 Then
            cmbSessions.SelectedValue = targetId
        ElseIf sessions.Count > 0 Then
            cmbSessions.SelectedIndex = 0
        End If

        btnSetActive.Enabled = (cmbSessions.SelectedItem IsNot Nothing)
        Text = "Session Setup - Active: " & activeInfo.DisplayText
        isLoadingSessions = False
    End Sub

    Private Sub UpdateCrudState()
        Dim hasSelection As Boolean = currentSessionId > 0
        btnUpdate.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
        btnSetActive.Enabled = hasSelection
    End Sub
End Class
