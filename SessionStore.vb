Imports MySql.Data.MySqlClient
Imports System.Collections.Generic

Public Class SessionInfo
    Public Property SessionId As Integer
    Public Property SessionDate As Date
    Public Property DayName As String
    Public Property SessionName As String
    Public Property CourseCode As String
    Public Property CourseName As String

    Public ReadOnly Property DisplayText As String
        Get
            Dim parts As New List(Of String)()
            If SessionDate <> Date.MinValue Then
                parts.Add(SessionDate.ToString("yyyy-MM-dd"))
            End If
            If Not String.IsNullOrWhiteSpace(DayName) Then
                parts.Add(DayName)
            End If
            If Not String.IsNullOrWhiteSpace(SessionName) Then
                parts.Add(SessionName)
            End If
            Dim course As String = (If(CourseCode, String.Empty) & " " & If(CourseName, String.Empty)).Trim()
            If Not String.IsNullOrWhiteSpace(course) Then
                parts.Add(course)
            End If
            If parts.Count = 0 Then
                Return "Session"
            End If
            Return String.Join(" | ", parts)
        End Get
    End Property
End Class

Public Module SessionStore
    Private cachedInfo As SessionInfo = Nothing
    Private lastLoad As DateTime = DateTime.MinValue
    Private ReadOnly cacheDuration As TimeSpan = TimeSpan.FromSeconds(2)

    Public Function GetCurrentSession(Optional ByVal forceRefresh As Boolean = False) As SessionInfo
        If Not forceRefresh AndAlso cachedInfo IsNot Nothing AndAlso (DateTime.Now - lastLoad) < cacheDuration Then
            Return cachedInfo
        End If

        Dim info As SessionInfo = Nothing
        Try
            info = LoadActiveSession()
        Catch ex As Exception
            AppLogger.LogError("Session load error", ex)
            info = Nothing
        End Try

        If info Is Nothing Then
            info = BuildDefaultSession()
        End If

        NormalizeInfo(info)

        cachedInfo = info
        lastLoad = DateTime.Now
        Return info
    End Function

    Public Sub SaveCurrentSession(ByVal info As SessionInfo)
        If info Is Nothing Then
            Return
        End If

        NormalizeInfo(info)
        Dim newId As Integer = 0

        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                conn.Open()
                EnsureSessionSettingsRow(conn)
                newId = InsertSession(conn, info)
                UpdateSessionSettings(conn, info, newId)
            End Using
        Catch ex As Exception
            AppLogger.LogError("Session save error", ex)
        End Try

        If newId > 0 Then
            info.SessionId = newId
        End If

        cachedInfo = info
        lastLoad = DateTime.Now
        AuditLogger.LogAction("SessionSaved", info.DisplayText)
    End Sub

    Public Function GetSessionsList() As List(Of SessionInfo)
        Dim result As New List(Of SessionInfo)()

        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                conn.Open()
                Using cmd As New MySqlCommand("SELECT id, session_date, day_name, session_name, course_code, course_name FROM sessions ORDER BY session_date DESC, id DESC", conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            result.Add(ReadSession(reader, 0, 1, 2, 3, 4, 5))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            AppLogger.LogError("Session list load error", ex)
        End Try

        If result.Count = 0 Then
            result.Add(GetCurrentSession())
        End If

        Return result
    End Function

    Public Sub SetActiveSession(ByVal sessionId As Integer)
        If sessionId <= 0 Then
            Return
        End If

        Dim info As SessionInfo = Nothing

        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                conn.Open()
                info = GetSessionById(conn, sessionId)
                If info Is Nothing Then
                    Return
                End If
                EnsureSessionSettingsRow(conn)
                UpdateSessionSettings(conn, info, sessionId)
            End Using
        Catch ex As Exception
            AppLogger.LogError("Session set active error", ex)
        End Try

        If info IsNot Nothing Then
            cachedInfo = info
            lastLoad = DateTime.Now
            AuditLogger.LogAction("SessionSetActive", info.DisplayText)
        End If
    End Sub

    Private Function LoadActiveSession() As SessionInfo
        Using conn As New MySqlConnection(Module1.ConnectionString)
            conn.Open()

            Dim activeSessionId As Integer = 0
            Dim settingsInfo As SessionInfo = Nothing

            Try
                Using cmd As New MySqlCommand("SELECT active_session_id, session_date, day_name, session_name, course_code, course_name FROM session_settings LIMIT 1", conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            If Not reader.IsDBNull(0) Then
                                activeSessionId = Convert.ToInt32(reader.GetValue(0))
                            End If
                            settingsInfo = ReadSession(reader, -1, 1, 2, 3, 4, 5)
                        End If
                    End Using
                End Using
            Catch ex As MySqlException When ex.Number = 1054
                Using cmd As New MySqlCommand("SELECT session_date, day_name, session_name, course_code, course_name FROM session_settings LIMIT 1", conn)
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            settingsInfo = ReadSession(reader, -1, 0, 1, 2, 3, 4)
                        End If
                    End Using
                End Using
            End Try

            If activeSessionId > 0 Then
                Dim activeInfo As SessionInfo = GetSessionById(conn, activeSessionId)
                If activeInfo IsNot Nothing Then
                    Return activeInfo
                End If
            End If

            Return settingsInfo
        End Using
    End Function

    Private Function GetSessionById(ByVal conn As MySqlConnection, ByVal sessionId As Integer) As SessionInfo
        Try
            Using cmd As New MySqlCommand("SELECT id, session_date, day_name, session_name, course_code, course_name FROM sessions WHERE id = @id", conn)
                cmd.Parameters.AddWithValue("@id", sessionId)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Return ReadSession(reader, 0, 1, 2, 3, 4, 5)
                    End If
                End Using
            End Using
        Catch ex As MySqlException
        End Try

        Return Nothing
    End Function

    Private Function InsertSession(ByVal conn As MySqlConnection, ByVal info As SessionInfo) As Integer
        Try
            Using cmd As New MySqlCommand("INSERT INTO sessions (session_date, day_name, session_name, course_code, course_name) VALUES (@date, @day, @session, @code, @name)", conn)
                cmd.Parameters.AddWithValue("@date", info.SessionDate.Date)
                cmd.Parameters.AddWithValue("@day", info.DayName)
                cmd.Parameters.AddWithValue("@session", info.SessionName)
                cmd.Parameters.AddWithValue("@code", info.CourseCode)
                cmd.Parameters.AddWithValue("@name", If(String.IsNullOrWhiteSpace(info.CourseName), DBNull.Value, info.CourseName))
                cmd.ExecuteNonQuery()
                Dim insertedId As Integer = Convert.ToInt32(cmd.LastInsertedId)
                Return insertedId
            End Using
        Catch ex As MySqlException
            Return 0
        End Try
    End Function

    Private Sub UpdateSessionSettings(ByVal conn As MySqlConnection, ByVal info As SessionInfo, ByVal activeSessionId As Integer)
        Dim hasActive As Boolean = activeSessionId > 0
        Try
            Using cmd As New MySqlCommand("UPDATE session_settings SET session_date = @date, day_name = @day, session_name = @session, course_code = @code, course_name = @name, active_session_id = @active", conn)
                cmd.Parameters.AddWithValue("@date", info.SessionDate.Date)
                cmd.Parameters.AddWithValue("@day", info.DayName)
                cmd.Parameters.AddWithValue("@session", info.SessionName)
                cmd.Parameters.AddWithValue("@code", info.CourseCode)
                cmd.Parameters.AddWithValue("@name", If(String.IsNullOrWhiteSpace(info.CourseName), DBNull.Value, info.CourseName))
                cmd.Parameters.AddWithValue("@active", If(hasActive, activeSessionId, DBNull.Value))
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As MySqlException When ex.Number = 1054
            Using cmd As New MySqlCommand("UPDATE session_settings SET session_date = @date, day_name = @day, session_name = @session, course_code = @code, course_name = @name", conn)
                cmd.Parameters.AddWithValue("@date", info.SessionDate.Date)
                cmd.Parameters.AddWithValue("@day", info.DayName)
                cmd.Parameters.AddWithValue("@session", info.SessionName)
                cmd.Parameters.AddWithValue("@code", info.CourseCode)
                cmd.Parameters.AddWithValue("@name", If(String.IsNullOrWhiteSpace(info.CourseName), DBNull.Value, info.CourseName))
                cmd.ExecuteNonQuery()
            End Using
        End Try
    End Sub

    Private Sub EnsureSessionSettingsRow(ByVal conn As MySqlConnection)
        Try
            Using cmd As New MySqlCommand("SELECT id FROM session_settings LIMIT 1", conn)
                Dim existing As Object = cmd.ExecuteScalar()
                If existing Is Nothing Then
                    Dim info As SessionInfo = BuildDefaultSession()
                    Using insertCmd As New MySqlCommand("INSERT INTO session_settings (session_date, day_name, session_name, course_code, course_name) VALUES (@date, @day, @session, @code, @name)", conn)
                        insertCmd.Parameters.AddWithValue("@date", info.SessionDate.Date)
                        insertCmd.Parameters.AddWithValue("@day", info.DayName)
                        insertCmd.Parameters.AddWithValue("@session", info.SessionName)
                        insertCmd.Parameters.AddWithValue("@code", info.CourseCode)
                        insertCmd.Parameters.AddWithValue("@name", If(String.IsNullOrWhiteSpace(info.CourseName), DBNull.Value, info.CourseName))
                        insertCmd.ExecuteNonQuery()
                    End Using
                End If
            End Using
        Catch ex As MySqlException
        End Try
    End Sub

    Private Function BuildDefaultSession() As SessionInfo
        Dim today As Date = Date.Today
        Return New SessionInfo With {
            .SessionId = 0,
            .SessionDate = today,
            .DayName = today.ToString("dddd"),
            .SessionName = "Session 1",
            .CourseCode = String.Empty,
            .CourseName = String.Empty
        }
    End Function

    Private Sub NormalizeInfo(ByVal info As SessionInfo)
        If info.SessionDate = Date.MinValue Then
            info.SessionDate = Date.Today
        End If
        If String.IsNullOrWhiteSpace(info.DayName) Then
            info.DayName = info.SessionDate.ToString("dddd")
        End If
        If info.SessionName Is Nothing Then
            info.SessionName = String.Empty
        End If
        If info.CourseCode Is Nothing Then
            info.CourseCode = String.Empty
        End If
        If info.CourseName Is Nothing Then
            info.CourseName = String.Empty
        End If
    End Sub

    Private Function ReadSession(ByVal reader As MySqlDataReader, ByVal idOrdinal As Integer, ByVal dateOrdinal As Integer, ByVal dayOrdinal As Integer, ByVal sessionOrdinal As Integer, ByVal codeOrdinal As Integer, ByVal nameOrdinal As Integer) As SessionInfo
        Dim info As New SessionInfo()
        If idOrdinal >= 0 AndAlso Not reader.IsDBNull(idOrdinal) Then
            info.SessionId = Convert.ToInt32(reader.GetValue(idOrdinal))
        End If
        info.SessionDate = ReadDate(reader, dateOrdinal)
        info.DayName = ReadString(reader, dayOrdinal)
        info.SessionName = ReadString(reader, sessionOrdinal)
        info.CourseCode = ReadString(reader, codeOrdinal)
        info.CourseName = ReadString(reader, nameOrdinal)
        Return info
    End Function

    Private Function ReadString(ByVal reader As MySqlDataReader, ByVal ordinal As Integer) As String
        If ordinal < 0 OrElse reader.IsDBNull(ordinal) Then
            Return String.Empty
        End If
        Return reader.GetValue(ordinal).ToString()
    End Function

    Private Function ReadDate(ByVal reader As MySqlDataReader, ByVal ordinal As Integer) As Date
        If ordinal < 0 OrElse reader.IsDBNull(ordinal) Then
            Return Date.Today
        End If
        Dim value As Object = reader.GetValue(ordinal)
        If TypeOf value Is Date Then
            Return CType(value, Date)
        End If
        Dim parsed As Date
        If Date.TryParse(value.ToString(), parsed) Then
            Return parsed
        End If
        Return Date.Today
    End Function
End Module
