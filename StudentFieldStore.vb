Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Text

Public Class StudentFieldDef
    Public Property Id As Integer
    Public Property FieldKey As String
    Public Property FieldLabel As String
    Public Property FieldType As String
    Public Property Options As String
    Public Property IsRequired As Boolean
    Public Property IsActive As Boolean
    Public Property DisplayOrder As Integer
End Class

Public Module StudentFieldStore
    Public Function GetActiveFields() As List(Of StudentFieldDef)
        Dim result As New List(Of StudentFieldDef)()
        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT id, field_key, field_label, field_type, options, is_required, is_active, display_order FROM student_fields WHERE is_active = 1 ORDER BY display_order, field_label", conn)
                    conn.Open()
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            result.Add(ReadField(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException When ex.Number = 1146
            Return result
        Catch ex As Exception
            AppLogger.LogError("Custom fields load error", ex)
        End Try
        Return result
    End Function

    Public Function GetAllFieldsTable() As DataTable
        Dim dt As New DataTable()
        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT id, field_key, field_label, field_type, options, is_required, is_active, display_order FROM student_fields ORDER BY display_order, field_label", conn)
                    Using da As New MySqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As MySqlException When ex.Number = 1146
            Return dt
        Catch ex As Exception
            AppLogger.LogError("Custom fields table load error", ex)
        End Try
        Return dt
    End Function

    Public Function AddField(ByVal def As StudentFieldDef) As Integer
        If def Is Nothing OrElse String.IsNullOrWhiteSpace(def.FieldLabel) Then
            Throw New ArgumentException("Field label is required.")
        End If

        Dim newId As Integer = 0
        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                conn.Open()

                If LabelExists(conn, def.FieldLabel, 0) Then
                    Throw New InvalidOperationException("Field label already exists.")
                End If

                Dim key As String = def.FieldKey
                If String.IsNullOrWhiteSpace(key) Then
                    key = BuildUniqueFieldKey(conn, def.FieldLabel)
                ElseIf FieldKeyExists(conn, key, 0) Then
                    Throw New InvalidOperationException("Field key already exists.")
                End If

                Using cmd As New MySqlCommand("INSERT INTO student_fields (field_key, field_label, field_type, options, is_required, is_active, display_order) VALUES (@key, @label, @type, @options, @required, @active, @order)", conn)
                    cmd.Parameters.AddWithValue("@key", key)
                    cmd.Parameters.AddWithValue("@label", def.FieldLabel.Trim())
                    cmd.Parameters.AddWithValue("@type", If(def.FieldType, String.Empty))
                    cmd.Parameters.AddWithValue("@options", If(String.IsNullOrWhiteSpace(def.Options), DBNull.Value, def.Options))
                    cmd.Parameters.AddWithValue("@required", If(def.IsRequired, 1, 0))
                    cmd.Parameters.AddWithValue("@active", If(def.IsActive, 1, 0))
                    cmd.Parameters.AddWithValue("@order", def.DisplayOrder)
                    cmd.ExecuteNonQuery()
                    newId = Convert.ToInt32(cmd.LastInsertedId)
                End Using
            End Using
        Catch ex As Exception
            AppLogger.LogError("Custom field add error", ex)
            Throw
        End Try

        Return newId
    End Function

    Public Function UpdateField(ByVal def As StudentFieldDef) As Boolean
        If def Is Nothing OrElse def.Id <= 0 Then
            Throw New ArgumentException("Field ID is required.")
        End If
        If String.IsNullOrWhiteSpace(def.FieldLabel) Then
            Throw New ArgumentException("Field label is required.")
        End If

        Dim updated As Boolean = False
        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                conn.Open()
                If LabelExists(conn, def.FieldLabel, def.Id) Then
                    Throw New InvalidOperationException("Field label already exists.")
                End If

                Using cmd As New MySqlCommand("UPDATE student_fields SET field_label = @label, field_type = @type, options = @options, is_required = @required, is_active = @active, display_order = @order WHERE id = @id", conn)
                    cmd.Parameters.AddWithValue("@label", def.FieldLabel.Trim())
                    cmd.Parameters.AddWithValue("@type", If(def.FieldType, String.Empty))
                    cmd.Parameters.AddWithValue("@options", If(String.IsNullOrWhiteSpace(def.Options), DBNull.Value, def.Options))
                    cmd.Parameters.AddWithValue("@required", If(def.IsRequired, 1, 0))
                    cmd.Parameters.AddWithValue("@active", If(def.IsActive, 1, 0))
                    cmd.Parameters.AddWithValue("@order", def.DisplayOrder)
                    cmd.Parameters.AddWithValue("@id", def.Id)
                    updated = (cmd.ExecuteNonQuery() > 0)
                End Using
            End Using
        Catch ex As Exception
            AppLogger.LogError("Custom field update error", ex)
            Throw
        End Try

        Return updated
    End Function

    Public Function DeleteField(ByVal fieldId As Integer) As Boolean
        If fieldId <= 0 Then
            Return False
        End If

        Dim deleted As Boolean = False
        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                conn.Open()
                Using tx As MySqlTransaction = conn.BeginTransaction()
                    Using cmd As New MySqlCommand("DELETE FROM student_field_values WHERE field_id = @id", conn, tx)
                        cmd.Parameters.AddWithValue("@id", fieldId)
                        cmd.ExecuteNonQuery()
                    End Using
                    Using cmd As New MySqlCommand("DELETE FROM student_fields WHERE id = @id", conn, tx)
                        cmd.Parameters.AddWithValue("@id", fieldId)
                        deleted = (cmd.ExecuteNonQuery() > 0)
                    End Using
                    tx.Commit()
                End Using
            End Using
        Catch ex As Exception
            AppLogger.LogError("Custom field delete error", ex)
            Throw
        End Try
        Return deleted
    End Function

    Public Function UpsertFieldValues(ByVal regno As String, ByVal values As Dictionary(Of Integer, String)) As Boolean
        If String.IsNullOrWhiteSpace(regno) OrElse values Is Nothing OrElse values.Count = 0 Then
            Return False
        End If

        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                conn.Open()
                Using tx As MySqlTransaction = conn.BeginTransaction()
                    UpsertFieldValues(regno, values, conn, tx)
                    tx.Commit()
                End Using
            End Using
            Return True
        Catch ex As Exception
            AppLogger.LogError("Custom field value save error", ex)
            Return False
        End Try
    End Function

    Public Sub UpsertFieldValues(ByVal regno As String, ByVal values As Dictionary(Of Integer, String), ByVal conn As MySqlConnection, ByVal tx As MySqlTransaction)
        If String.IsNullOrWhiteSpace(regno) OrElse values Is Nothing OrElse values.Count = 0 Then
            Return
        End If

        Using cmd As New MySqlCommand("INSERT INTO student_field_values (regno, field_id, field_value) VALUES (@regno, @field_id, @value) ON DUPLICATE KEY UPDATE field_value = VALUES(field_value)", conn, tx)
            cmd.Parameters.AddWithValue("@regno", regno.Trim())
            cmd.Parameters.AddWithValue("@field_id", 0)
            cmd.Parameters.AddWithValue("@value", DBNull.Value)

            For Each pair As KeyValuePair(Of Integer, String) In values
                cmd.Parameters("@field_id").Value = pair.Key
                cmd.Parameters("@value").Value = If(String.IsNullOrWhiteSpace(pair.Value), DBNull.Value, pair.Value)
                cmd.ExecuteNonQuery()
            Next
        End Using
    End Sub

    Public Function GetFieldValuesMap() As Dictionary(Of String, Dictionary(Of Integer, String))
        Dim result As New Dictionary(Of String, Dictionary(Of Integer, String))(StringComparer.OrdinalIgnoreCase)
        Try
            Using conn As New MySqlConnection(Module1.ConnectionString)
                Using cmd As New MySqlCommand("SELECT regno, field_id, field_value FROM student_field_values", conn)
                    conn.Open()
                    Using reader As MySqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim regno As String = reader.GetString(0)
                            Dim fieldId As Integer = Convert.ToInt32(reader.GetValue(1))
                            Dim value As String = If(reader.IsDBNull(2), String.Empty, reader.GetValue(2).ToString())

                            If Not result.ContainsKey(regno) Then
                                result(regno) = New Dictionary(Of Integer, String)()
                            End If
                            result(regno)(fieldId) = value
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException When ex.Number = 1146
            Return result
        Catch ex As Exception
            AppLogger.LogError("Custom field value map error", ex)
        End Try
        Return result
    End Function

    Private Function ReadField(ByVal reader As MySqlDataReader) As StudentFieldDef
        Dim def As New StudentFieldDef()
        def.Id = Convert.ToInt32(reader.GetValue(0))
        def.FieldKey = reader.GetValue(1).ToString()
        def.FieldLabel = reader.GetValue(2).ToString()
        def.FieldType = reader.GetValue(3).ToString()
        def.Options = If(reader.IsDBNull(4), String.Empty, reader.GetValue(4).ToString())
        def.IsRequired = Not reader.IsDBNull(5) AndAlso Convert.ToInt32(reader.GetValue(5)) = 1
        def.IsActive = Not reader.IsDBNull(6) AndAlso Convert.ToInt32(reader.GetValue(6)) = 1
        def.DisplayOrder = If(reader.IsDBNull(7), 0, Convert.ToInt32(reader.GetValue(7)))
        Return def
    End Function

    Private Function BuildUniqueFieldKey(ByVal conn As MySqlConnection, ByVal label As String) As String
        Dim baseKey As String = NormalizeFieldKey(label)
        If String.IsNullOrWhiteSpace(baseKey) Then
            baseKey = "field"
        End If
        Dim key As String = baseKey
        Dim index As Integer = 2
        While FieldKeyExists(conn, key, 0)
            key = baseKey & "_" & index.ToString()
            index += 1
        End While
        Return key
    End Function

    Private Function NormalizeFieldKey(ByVal label As String) As String
        If String.IsNullOrWhiteSpace(label) Then
            Return String.Empty
        End If
        Dim sb As New StringBuilder()
        For Each ch As Char In label.Trim().ToLowerInvariant()
            If Char.IsLetterOrDigit(ch) Then
                sb.Append(ch)
            ElseIf ch = " "c OrElse ch = "-"c OrElse ch = "_"c Then
                sb.Append("_")
            End If
        Next
        Dim key As String = sb.ToString().Trim("_"c)
        While key.Contains("__")
            key = key.Replace("__", "_")
        End While
        Return key
    End Function

    Private Function FieldKeyExists(ByVal conn As MySqlConnection, ByVal key As String, ByVal excludeId As Integer) As Boolean
        Using cmd As New MySqlCommand("SELECT 1 FROM student_fields WHERE field_key = @key AND id <> @id LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@key", key)
            cmd.Parameters.AddWithValue("@id", excludeId)
            Dim exists As Object = cmd.ExecuteScalar()
            Return exists IsNot Nothing
        End Using
    End Function

    Private Function LabelExists(ByVal conn As MySqlConnection, ByVal label As String, ByVal excludeId As Integer) As Boolean
        Using cmd As New MySqlCommand("SELECT 1 FROM student_fields WHERE field_label = @label AND id <> @id LIMIT 1", conn)
            cmd.Parameters.AddWithValue("@label", label.Trim())
            cmd.Parameters.AddWithValue("@id", excludeId)
            Dim exists As Object = cmd.ExecuteScalar()
            Return exists IsNot Nothing
        End Using
    End Function
End Module
