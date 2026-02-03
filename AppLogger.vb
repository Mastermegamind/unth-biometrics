Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Public Module AppLogger
    Private ReadOnly LogLock As New Object()

    Public Sub Info(ByVal message As String)
        WriteLog("INFO", message, Nothing)
    End Sub

    Public Sub Success(ByVal message As String)
        WriteLog("SUCCESS", message, Nothing)
    End Sub

    Public Sub LogError(ByVal message As String, Optional ByVal ex As Exception = Nothing)
        WriteLog("ERROR", message, ex)
    End Sub

    Private Sub WriteLog(ByVal level As String, ByVal message As String, ByVal ex As Exception)
        Try
            Dim folder As String = GetLogFolder()
            If Not Directory.Exists(folder) Then
                Directory.CreateDirectory(folder)
            End If

            Dim logPath As String = Path.Combine(folder, "app.log")
            Dim sb As New StringBuilder()
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
            sb.Append(" | ")
            sb.Append(level)
            sb.Append(" | ")
            sb.Append(If(message, String.Empty))

            If ex IsNot Nothing Then
                sb.Append(" | ")
                sb.Append(ex.GetType().Name)
                sb.Append(": ")
                sb.Append(ex.Message)
            End If

            SyncLock LogLock
                File.AppendAllText(logPath, sb.ToString() & Environment.NewLine)
            End SyncLock
        Catch
            ' Avoid throwing from logger.
        End Try
    End Sub

    Private Function GetLogFolder() As String
        Dim basePath As String = Application.StartupPath
        Dim lowerPath As String = basePath.ToLowerInvariant()
        If lowerPath.EndsWith("\bin\debug") OrElse lowerPath.EndsWith("\bin\release") Then
            Dim parent As DirectoryInfo = Directory.GetParent(basePath)
            If parent IsNot Nothing AndAlso parent.Parent IsNot Nothing Then
                basePath = parent.Parent.FullName
            End If
        End If
        Return Path.Combine(basePath, "logs")
    End Function
End Module
