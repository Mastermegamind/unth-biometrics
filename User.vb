Imports MySql.Data.MySqlClient
Public Class User
    Sub loadenrolleddata()
        Module1.LoadEnrolledData(True)
    End Sub

    Private Sub User_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        loadenrolleddata()
    End Sub
    Private Sub verificationToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles verificationToolStripMenuItem1.Click
        'verificationform.Close()
        'verificationform.ShowDialog()
    End Sub

    Private Sub LogoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogoutToolStripMenuItem.Click
        Me.Close()
        Dim login As New Login()
        login.Show()
    End Sub

    Private Sub TimeInToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TimeInToolStripMenuItem.Click
        Try
            AppLogger.Info("Open kiosk: Time-In")
            Dim kiosk As New AttendanceKiosk(0)
            kiosk.ShowDialog()
        Catch ex As Exception
            AppLogger.LogError("Open kiosk Time-In failed", ex)
            MessageBox.Show("Unable to open Time-In. Check logs/app.log.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TimeOutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TimeOutToolStripMenuItem.Click
        Try
            AppLogger.Info("Open kiosk: Time-Out")
            Dim kiosk As New AttendanceKiosk(1)
            kiosk.ShowDialog()
        Catch ex As Exception
            AppLogger.LogError("Open kiosk Time-Out failed", ex)
            MessageBox.Show("Unable to open Time-Out. Check logs/app.log.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub User_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
