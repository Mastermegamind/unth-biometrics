Imports System
Imports System.Security.Cryptography
Imports BCrypt.Net

Public Module Security
    Private Const HashPrefix As String = "PBKDF2"
    Private Const Iterations As Integer = 100000
    Private Const SaltSize As Integer = 16
    Private Const KeySize As Integer = 32
    Private Const BcryptWorkFactor As Integer = 12

    Public Function HashPassword(ByVal password As String) As String
        If password Is Nothing Then
            password = ""
        End If
        Return BCrypt.Net.BCrypt.HashPassword(password, workFactor:=BcryptWorkFactor)
    End Function

    Public Function VerifyPassword(ByVal password As String, ByVal stored As String) As Boolean
        If String.IsNullOrEmpty(stored) Then
            Return False
        End If

        If stored.StartsWith("$2", StringComparison.Ordinal) Then
            Try
                Return BCrypt.Net.BCrypt.Verify(password, stored)
            Catch
                Return False
            End Try
        End If

        If stored.StartsWith(HashPrefix & "$", StringComparison.Ordinal) Then
            Return VerifyLegacyPbkdf2(password, stored)
        End If
        Return String.Equals(password, stored, StringComparison.Ordinal)
    End Function

    Public Function IsLegacyPassword(ByVal stored As String) As Boolean
        If String.IsNullOrEmpty(stored) Then
            Return False
        End If
        Return Not stored.StartsWith("$2", StringComparison.Ordinal)
    End Function

    Private Function FixedTimeEquals(ByVal a As Byte(), ByVal b As Byte()) As Boolean
        If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then
            Return False
        End If

        Dim diff As Integer = 0
        For i As Integer = 0 To a.Length - 1
            diff = diff Or (a(i) Xor b(i))
        Next

        Return diff = 0
    End Function

    Private Function VerifyLegacyPbkdf2(ByVal password As String, ByVal stored As String) As Boolean
        Dim parts As String() = stored.Split("$"c)
        If parts.Length <> 4 Then
            Return False
        End If

        Dim iters As Integer
        If Not Integer.TryParse(parts(1), iters) Then
            Return False
        End If

        Dim salt As Byte()
        Dim expected As Byte()
        Try
            salt = Convert.FromBase64String(parts(2))
            expected = Convert.FromBase64String(parts(3))
        Catch ex As FormatException
            Return False
        End Try

        Dim actual As Byte()
        Using pbkdf2 As New Rfc2898DeriveBytes(password, salt, iters, HashAlgorithmName.SHA256)
            actual = pbkdf2.GetBytes(expected.Length)
        End Using

        Return FixedTimeEquals(actual, expected)
    End Function
End Module
