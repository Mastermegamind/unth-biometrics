Imports System.Drawing
Imports System.Windows.Forms
Imports System.Configuration

Public Class AttendanceKiosk
    Inherits Form

    Private ReadOnly refreshTimer As Timer
    Private ReadOnly lblSessionInfo As Label
    Private ReadOnly btnRefresh As Button
    Private ReadOnly splitContainer As SplitContainer
    Private lblClockInHeader As Label
    Private lblClockOutHeader As Label
    Private btnActivateIn As Button
    Private btnActivateOut As Button
    Private ReadOnly clockInForm As verificationform
    Private ReadOnly clockOutForm As TimeOut
    Private activeIsClockIn As Boolean = True

    Public Sub New(Optional ByVal initialTabIndex As Integer = 0)
        Text = "Attendance Clock"
        WindowState = FormWindowState.Maximized
        StartPosition = FormStartPosition.CenterScreen
        FormBorderStyle = FormBorderStyle.None
        ControlBox = False
        TopMost = True
        ShowInTaskbar = True

        Dim topPanel As New Panel With {.Dock = DockStyle.Top, .Height = 70, .BackColor = Color.WhiteSmoke}
        lblSessionInfo = New Label With {
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 16.0F, FontStyle.Bold),
            .ForeColor = Color.Black,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(12, 0, 0, 0)
        }
        btnRefresh = New Button With {.Text = "Refresh Session", .Dock = DockStyle.Right, .Width = 160, .Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)}
        topPanel.Controls.Add(btnRefresh)
        topPanel.Controls.Add(lblSessionInfo)

        splitContainer = New SplitContainer With {
            .Dock = DockStyle.Fill,
            .Orientation = Orientation.Vertical,
            .SplitterWidth = 6
        }
        Controls.Add(splitContainer)
        Controls.Add(topPanel)

        clockInForm = New verificationform()
        clockOutForm = New TimeOut()

        Dim panelIn As Panel = BuildPanel("CLOCK IN", lblClockInHeader, btnActivateIn)
        Dim panelOut As Panel = BuildPanel("CLOCK OUT", lblClockOutHeader, btnActivateOut)
        splitContainer.Panel1.Controls.Add(panelIn)
        splitContainer.Panel2.Controls.Add(panelOut)

        Dim hostIn As Panel = TryCast(panelIn.Tag, Panel)
        Dim hostOut As Panel = TryCast(panelOut.Tag, Panel)
        If hostIn IsNot Nothing Then
            AttachForm(clockInForm, hostIn)
        End If
        If hostOut IsNot Nothing Then
            AttachForm(clockOutForm, hostOut)
        End If

        activeIsClockIn = (initialTabIndex <= 0)
        AddHandler btnActivateIn.Click, Sub() SetActiveSide(True)
        AddHandler btnActivateOut.Click, Sub() SetActiveSide(False)
        AddHandler btnRefresh.Click, AddressOf ManualRefresh

        refreshTimer = New Timer() With {.Interval = GetRefreshIntervalMs()}
        AddHandler refreshTimer.Tick, AddressOf RefreshTimerTick
        refreshTimer.Start()

        SetActiveSide(activeIsClockIn)
        UpdateSessionBanner()
    End Sub

    Private Sub AttachForm(ByVal child As Form, ByVal host As Control)
        child.TopLevel = False
        child.FormBorderStyle = FormBorderStyle.None
        child.Dock = DockStyle.Fill
        host.Controls.Add(child)
        child.Show()
    End Sub

    Private Function BuildPanel(ByVal title As String, ByRef headerLabel As Label, ByRef activateButton As Button) As Panel
        Dim container As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White}
        Dim header As New Panel With {.Dock = DockStyle.Top, .Height = 42, .BackColor = Color.Gainsboro}
        headerLabel = New Label With {
            .Text = title,
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(12, 0, 0, 0),
            .BackColor = Color.Gainsboro
        }
        activateButton = New Button With {
            .Text = "Activate",
            .Dock = DockStyle.Right,
            .Width = 110,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        }
        header.Controls.Add(activateButton)
        header.Controls.Add(headerLabel)

        Dim content As New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White}
        container.Controls.Add(content)
        container.Controls.Add(header)
        container.Tag = content
        Return container
    End Function
    Private Sub SetActiveSide(ByVal isClockInActive As Boolean)
        activeIsClockIn = isClockInActive
        clockInForm.SetScannerActive(isClockInActive)
        clockOutForm.SetScannerActive(Not isClockInActive)
        clockInForm.RefreshSessionSettings()
        clockOutForm.RefreshSessionSettings()
        UpdateHeaderStyles()
    End Sub

    Private Sub UpdateHeaderStyles()
        If lblClockInHeader IsNot Nothing Then
            lblClockInHeader.Text = If(activeIsClockIn, "CLOCK IN (ACTIVE)", "CLOCK IN")
            lblClockInHeader.BackColor = If(activeIsClockIn, Color.ForestGreen, Color.Gainsboro)
            lblClockInHeader.ForeColor = If(activeIsClockIn, Color.White, Color.Black)
        End If
        If lblClockOutHeader IsNot Nothing Then
            lblClockOutHeader.Text = If(activeIsClockIn, "CLOCK OUT", "CLOCK OUT (ACTIVE)")
            lblClockOutHeader.BackColor = If(activeIsClockIn, Color.Gainsboro, Color.ForestGreen)
            lblClockOutHeader.ForeColor = If(activeIsClockIn, Color.Black, Color.White)
        End If
        If btnActivateIn IsNot Nothing Then
            btnActivateIn.Enabled = Not activeIsClockIn
        End If
        If btnActivateOut IsNot Nothing Then
            btnActivateOut.Enabled = activeIsClockIn
        End If
    End Sub

    Private Sub RefreshTimerTick(sender As Object, e As EventArgs)
        RefreshSessionSettings(True)
    End Sub

    Private Sub ManualRefresh(sender As Object, e As EventArgs)
        RefreshSessionSettings(True)
    End Sub

    Private Sub RefreshSessionSettings(ByVal forceRefresh As Boolean)
        Dim info As SessionInfo = SessionStore.GetCurrentSession(forceRefresh)
        clockInForm.ApplySessionInfo(info)
        clockOutForm.ApplySessionInfo(info)
        UpdateSessionBanner(info)
    End Sub

    Private Sub UpdateSessionBanner(Optional ByVal info As SessionInfo = Nothing)
        Dim current As SessionInfo = info
        If current Is Nothing OrElse current.SessionDate = Date.MinValue Then
            current = SessionStore.GetCurrentSession()
        End If
        lblSessionInfo.Text = "Current Session: " & current.SessionDate.ToString("yyyy-MM-dd") & " | " &
                              current.DayName & " | " & current.SessionName & " | " &
                              current.CourseCode & " " & current.CourseName
    End Sub

    Private Function GetRefreshIntervalMs() As Integer
        Dim defaultSeconds As Integer = 5
        Dim value As String = ConfigurationManager.AppSettings("SessionRefreshSeconds")
        Dim seconds As Integer = defaultSeconds
        If Not String.IsNullOrWhiteSpace(value) Then
            Integer.TryParse(value, seconds)
        End If
        If seconds < 2 Then
            seconds = 2
        End If
        Return seconds * 1000
    End Function

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        refreshTimer.Stop()
        clockInForm.SetScannerActive(False)
        clockOutForm.SetScannerActive(False)
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)
        splitContainer.SplitterDistance = splitContainer.Width \ 2
        BringToFront()
        Activate()
    End Sub
End Class
