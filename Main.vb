Imports MySql.Data.MySqlClient
Public Class Main
    Dim mysqlconn As MySqlConnection
    Dim dr As MySqlDataReader
    Dim cmd As MySqlCommand
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Sub loadenrolleddata()
        Module1.LoadEnrolledData(True)
    End Sub

    Private Sub Main_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        loadenrolleddata()
    End Sub

    Private Sub RegistrationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RegistrationToolStripMenuItem.Click
        Dim register As New Register()
        register.Show()
    End Sub

    Private Sub EnrollmentMenuItem1_Click(sender As Object, e As EventArgs) Handles EnrollmentMenuItem1.Click
        Form1.Close()
        Form1.ShowDialog()
    End Sub

    Private Sub LogoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogoutToolStripMenuItem.Click
        Me.Close()
        Dim login As New Login()
        login.Show()
    End Sub

    Private Sub verificationToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles verificationToolStripMenuItem1.Click

    End Sub

    Private Sub AddUserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddUserToolStripMenuItem.Click
        AdminReg.Close()
        AdminReg.ShowDialog()
    End Sub

    Private Sub ClockInToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClockInToolStripMenuItem.Click
        Try
            AppLogger.Info("Open kiosk: Time-In")
            Dim kiosk As New AttendanceKiosk(0)
            kiosk.ShowDialog()
        Catch ex As Exception
            AppLogger.LogError("Open kiosk Time-In failed", ex)
            MessageBox.Show("Unable to open Time-In. Check logs/app.log.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ClockOutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClockOutToolStripMenuItem.Click
        Try
            AppLogger.Info("Open kiosk: Time-Out")
            Dim kiosk As New AttendanceKiosk(1)
            kiosk.ShowDialog()
        Catch ex As Exception
            AppLogger.LogError("Open kiosk Time-Out failed", ex)
            MessageBox.Show("Unable to open Time-Out. Check logs/app.log.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AttendanceReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AttendanceReportToolStripMenuItem.Click
        AttendanceReport.Close()
        AttendanceReport.ShowDialog()
    End Sub

    Private Sub ManageLookupsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManageLookupsToolStripMenuItem.Click
        Dim frm As New LookupManager()
        frm.ShowDialog()
    End Sub

    Private Sub SessionSetupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SessionSetupToolStripMenuItem.Click
        Dim frm As New SessionSetup()
        frm.ShowDialog()
    End Sub

    Private Sub BackupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackupToolStripMenuItem.Click
        Dim confirm As DialogResult = MessageBox.Show("Backup all data to the cloud now?", "Backup", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm <> DialogResult.Yes Then
            Return
        End If

        Dim ok As Boolean = BackupService.BackupToApi()
        If ok Then
            MessageBox.Show("Backup completed.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Backup failed. Check logs/app.log for details.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub RestoreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestoreToolStripMenuItem.Click
        Dim confirm As DialogResult = MessageBox.Show("Restore will overwrite local data. Continue?", "Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirm <> DialogResult.Yes Then
            Return
        End If

        Dim ok As Boolean = BackupService.RestoreFromApi()
        If ok Then
            MessageBox.Show("Restore completed.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Restore failed. Check logs/app.log for details.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub
End Class
