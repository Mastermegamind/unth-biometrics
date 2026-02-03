Imports System.Security.Cryptography
Imports MySql.Data.MySqlClient
Imports System
Imports System.IO
Imports System.Data

Public Class AdminReg
    Private Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        Try
            Dim constr As String = Module1.ConnectionString
            Using cnn As MySqlConnection = New MySqlConnection(constr)
                Using com As MySqlCommand = New MySqlCommand("SELECT 1 FROM registration WHERE username = @username OR email = @email LIMIT 1", cnn)
                    com.CommandType = CommandType.Text
                    com.Parameters.AddWithValue("@username", txtUsername.Text.Trim())
                    com.Parameters.AddWithValue("@email", txtEmail.Text.Trim())
                    cnn.Open()
                    Using sdr As MySqlDataReader = com.ExecuteReader()
                        If sdr.Read() Then
                            MsgBox("Duplicate Record Detected", MsgBoxStyle.Information, "Biometric Fingerprints Student Attendance System")
                            Return
                        End If
                    End Using
                End Using
            End Using

            Dim hashedPassword As String = Security.HashPassword(txtPassword.Text)

            Using cn As MySqlConnection = New MySqlConnection(constr)
                Using comd As New MySqlCommand("INSERT INTO registration (username, usertype, password, name, contactno, email) VALUES (@username, @usertype, @password, @name, @contactno, @email)", cn)
                    comd.CommandType = CommandType.Text
                    comd.Parameters.AddWithValue("@username", txtUsername.Text.Trim())
                    comd.Parameters.AddWithValue("@usertype", cmbUserType.Text.Trim())
                    comd.Parameters.AddWithValue("@password", hashedPassword)
                    comd.Parameters.AddWithValue("@name", txtName.Text.Trim())
                    comd.Parameters.AddWithValue("@contactno", txtContact_no.Text.Trim())
                    comd.Parameters.AddWithValue("@email", txtEmail.Text.Trim())
                    cn.Open()
                    comd.ExecuteNonQuery()
                End Using
            End Using

            AuditLogger.LogAction("UserCreate", txtUsername.Text.Trim() & " | " & cmbUserType.Text.Trim())

            reset()
            MsgBox("Submitted Successfully", MsgBoxStyle.Information, "Biometric Fingerprints Student Attendance System")

        Catch ex As Exception
            AppLogger.LogError("User registration error", ex)
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Public Sub reset()
        txtEmail.Text = String.Empty
        txtName.Text = String.Empty
        txtContact_no.Text = String.Empty
        txtPassword.Text = String.Empty
        txtUsername.Text = String.Empty

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class
