Imports MySql.Data.MySqlClient
Imports System.Configuration
Imports System.Drawing
Imports System.IO

Module Module1
    Public fptemplist As New List(Of DPFP.Template)
    Public listofnames As New List(Of String)
    Delegate Sub FunctionCall(ByVal param)
    Delegate Sub FunctionCall2(ByVal param, ByVal param)

    Public report1datatable As New DataTable
    Public report1source As New DataSet
    Public nameemodule As String

    Public date1 As String
    Public date2 As String

    Public CurrentUserName As String = "system"
    Public CurrentUserType As String = String.Empty

    Private LastEnrollmentLoad As DateTime = DateTime.MinValue
    Private ReadOnly EnrollmentCacheDuration As TimeSpan = TimeSpan.FromMinutes(2)

    Private ReadOnly PassportCacheLock As New Object()
    Private ReadOnly PassportCache As New Dictionary(Of String, Byte())(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly PassportCacheOrder As New Queue(Of String)()
    Private Const PassportCacheMax As Integer = 100

    Private ReadOnly DefaultConnectionString As String = "SERVER=localhost; DATABASE=fingerprints; userid=root; PASSWORD=; PORT=3306;"

    Public ReadOnly Property ConnectionString As String
        Get
            Dim cs = ConfigurationManager.ConnectionStrings("MarvBiometric")
            If cs IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cs.ConnectionString) Then
                Return cs.ConnectionString
            End If
            Return DefaultConnectionString
        End Get
    End Property

    Public ReadOnly con As New MySqlConnection(ConnectionString)

    Public Sub SetCurrentUser(ByVal username As String, ByVal userType As String)
        If String.IsNullOrWhiteSpace(username) Then
            CurrentUserName = "system"
        Else
            CurrentUserName = username.Trim()
        End If
        CurrentUserType = If(userType, String.Empty).Trim()
    End Sub

    Public Sub LoadEnrolledData(Optional ByVal force As Boolean = False)
        If Not force AndAlso fptemplist.Count > 0 AndAlso (DateTime.Now - LastEnrollmentLoad) < EnrollmentCacheDuration Then
            Return
        End If

        Dim drt As MySqlDataReader
        Dim nm As String
        fptemplist.Clear()
        listofnames.Clear()
        Using mysqlconn As New MySqlConnection(ConnectionString)
            mysqlconn.Open()
            nm = "select regno, template from new_enrollment where template is not null order by regno, finger_index"

            Using cmd As New MySqlCommand(nm, mysqlconn)
                drt = cmd.ExecuteReader
                While drt.Read()
                    Dim value As Object = drt("template")
                    If value IsNot DBNull.Value Then
                        Dim fpbytes As Byte() = CType(value, Byte())
                        If fpbytes.Length > 0 Then
                            Dim mstram As New IO.MemoryStream(fpbytes)
                            Dim temp8 As DPFP.Template = New DPFP.Template
                            temp8.DeSerialize(mstram)
                            fptemplist.Add(temp8)
                            listofnames.Add(drt("regno").ToString())
                        End If
                    End If
                End While
                drt.Close()
            End Using
        End Using

        LastEnrollmentLoad = DateTime.Now
    End Sub

    Public Function TryGetPassportImage(ByVal passportFile As String) As Image
        If String.IsNullOrWhiteSpace(passportFile) Then
            Return Nothing
        End If

        Dim key As String = passportFile.Trim()
        Dim cached As Byte() = Nothing
        SyncLock PassportCacheLock
            If PassportCache.TryGetValue(key, cached) Then
                Return BytesToImage(cached)
            End If
        End SyncLock

        Dim fullPath As String = Path.Combine(Application.StartupPath, "Passports", key)
        If Not File.Exists(fullPath) Then
            Return Nothing
        End If

        Dim bytes As Byte() = File.ReadAllBytes(fullPath)
        SyncLock PassportCacheLock
            If Not PassportCache.ContainsKey(key) Then
                PassportCache(key) = bytes
                PassportCacheOrder.Enqueue(key)
                If PassportCacheOrder.Count > PassportCacheMax Then
                    Dim oldest As String = PassportCacheOrder.Dequeue()
                    PassportCache.Remove(oldest)
                End If
            End If
        End SyncLock

        Return BytesToImage(bytes)
    End Function

    Public Sub ClearPassportCache()
        SyncLock PassportCacheLock
            PassportCache.Clear()
            PassportCacheOrder.Clear()
        End SyncLock
    End Sub

    Private Function BytesToImage(ByVal bytes As Byte()) As Image
        If bytes Is Nothing OrElse bytes.Length = 0 Then
            Return Nothing
        End If

        Using ms As New MemoryStream(bytes)
            Using img As Image = Image.FromStream(ms)
                Return New Bitmap(img)
            End Using
        End Using
    End Function
End Module
