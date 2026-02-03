Imports MySql.Data.MySqlClient

Public Class Login
    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try
            txtUserType.Text = ""

            Dim userType As String = Nothing
            Dim storedPassword As String = Nothing

            Using con As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT usertype, password FROM registration WHERE username = @username", con)
                    cmd.Parameters.AddWithValue("@username", txtusername.Text.Trim())
                    con.Open()
                    Using rd As MySqlDataReader = cmd.ExecuteReader()
                        If rd.Read() Then
                            userType = rd.GetString("usertype")
                            storedPassword = rd.GetString("password")
                        End If
                    End Using
                End Using
            End Using

            If String.IsNullOrEmpty(storedPassword) Then
                MessageBox.Show("Invalid username or password")
                AppLogger.Info("Login failed: " & txtusername.Text.Trim())
                Return
            End If

            If Not Security.VerifyPassword(txtpass.Text, storedPassword) Then
                MessageBox.Show("Invalid username or password")
                AppLogger.Info("Login failed: " & txtusername.Text.Trim())
                Return
            End If

            txtUserType.Text = userType

            If Security.IsLegacyPassword(storedPassword) Then
                Try
                    Dim newHash As String = Security.HashPassword(txtpass.Text)
                    Using con As New MySqlConnection(Module1.ConnectionString)
                        Using cmd As New MySqlCommand("UPDATE registration SET password = @password WHERE username = @username", con)
                            cmd.Parameters.AddWithValue("@password", newHash)
                            cmd.Parameters.AddWithValue("@username", txtusername.Text.Trim())
                            con.Open()
                            cmd.ExecuteNonQuery()
                        End Using
                    End Using
                Catch
                    ' Ignore upgrade errors to avoid blocking login.
                End Try
            End If

            Module1.SetCurrentUser(txtusername.Text.Trim(), txtUserType.Text.Trim())

            If (txtUserType.Text.Trim() = "Administrator") Then
                Me.Hide()
                Dim parentForm As New Main()
                parentForm.Show()

                parentForm.lblUser.Text = txtusername.Text
                parentForm.lblUserType.Text = txtUserType.Text
                AppLogger.Success("Login success: " & txtusername.Text.Trim() & " | " & txtUserType.Text.Trim())

            End If

            If (txtUserType.Text.Trim() = "Staff") Then
                Me.Hide()
                Dim usa As New User()
                usa.Show()

                usa.lblUser.Text = txtusername.Text
                usa.lblUserType.Text = txtUserType.Text
                AppLogger.Success("Login success: " & txtusername.Text.Trim() & " | " & txtUserType.Text.Trim())

            End If

        Catch ex As Exception
            AppLogger.LogError("Login error", ex)
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtusername.Text = "Username"
        txtpass.Text = "Password"
    End Sub
    Private Sub txtusername_MouseEnter(sender As Object, e As System.EventArgs) Handles txtusername.MouseEnter
        If (txtusername.Text = "Username") Then
            txtusername.Text = ""
        End If
    End Sub
    Private Sub txtusername_MouseLeave(sender As Object, e As System.EventArgs) Handles txtusername.MouseLeave
        If (txtusername.Text = "") Then
            txtusername.Text = "Username"
        End If
    End Sub
    Private Sub txtpass_MouseEnter(sender As Object, e As System.EventArgs) Handles txtpass.MouseEnter
        If (txtpass.Text = "Password") Then
            txtpass.Text = ""
        End If
    End Sub

    Private Sub txtpass_MouseLeave(sender As Object, e As System.EventArgs) Handles txtpass.MouseLeave
        If (txtpass.Text = "") Then
            txtpass.Text = "Password"
        End If
    End Sub

    Private Sub btnCancel_Click_1(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class
