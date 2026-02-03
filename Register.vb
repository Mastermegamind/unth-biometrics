Imports System.Diagnostics
Imports Emgu.CV.Structure
Imports Emgu.CV
Imports Emgu.CV.CvEnum
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Windows.Forms
Imports System.IO
Imports System.Security.Cryptography
Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.Data.OleDb
Imports Microsoft.VisualBasic.FileIO
Imports System.Text
Imports System.Linq

Public Class Register
    'Declaration of all variables, vectors And haarcascades
    Dim currentFrame As Image(Of Bgr, [Byte])
    Dim grabber As Capture
    Dim face As HaarCascade
    Dim eye As HaarCascade
    'Dim font As New MCvFont(CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5, 0.5)

    Private ReadOnly SectionLookup As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly YearLookup As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly CustomFields As New List(Of StudentFieldDef)()
    Private ReadOnly CustomFieldControls As New Dictionary(Of Integer, Control)()

    Private Function GetPassportFolder() As String
        Dim folder As String = Path.Combine(Application.StartupPath, "Passports")
        If Not Directory.Exists(folder) Then
            Directory.CreateDirectory(folder)
        End If
        Return folder
    End Function

    Private Function SanitizeFileName(ByVal value As String) As String
        If String.IsNullOrWhiteSpace(value) Then
            Return String.Empty
        End If

        Dim invalidChars As Char() = Path.GetInvalidFileNameChars()
        Dim sb As New StringBuilder()
        For Each ch As Char In value.Trim()
            If Array.IndexOf(invalidChars, ch) = -1 Then
                sb.Append(ch)
            End If
        Next
        Return sb.ToString()
    End Function

    Private Function IsAllowedImageExtension(ByVal filePath As String) As Boolean
        If String.IsNullOrWhiteSpace(filePath) Then
            Return False
        End If

        Dim ext As String = Path.GetExtension(filePath)
        If String.IsNullOrWhiteSpace(ext) Then
            Return False
        End If

        Select Case ext.ToLowerInvariant()
            Case ".jpg", ".jpeg", ".png", ".bmp", ".gif"
                Return True
        End Select
        Return False
    End Function

    Private Function IsValidRegNo(ByVal value As String) As Boolean
        If String.IsNullOrWhiteSpace(value) Then
            Return False
        End If
        Dim pattern As String = "^[A-Za-z0-9][A-Za-z0-9/_-]{2,}$"
        Return System.Text.RegularExpressions.Regex.IsMatch(value.Trim(), pattern)
    End Function

    Private Function SavePassportImage(ByVal regNo As String, ByVal image As Image) As String
        If image Is Nothing Then
            Return String.Empty
        End If

        Dim safeRegNo As String = SanitizeFileName(regNo)
        If String.IsNullOrWhiteSpace(safeRegNo) Then
            Return String.Empty
        End If

        Dim fileName As String = safeRegNo & ".jpg"
        Dim fullPath As String = Path.Combine(GetPassportFolder(), fileName)
        image.Save(fullPath, ImageFormat.Jpeg)
        Return fileName
    End Function

    Private Function CopyPassportFile(ByVal regNo As String, ByVal sourcePath As String) As String
        If String.IsNullOrWhiteSpace(sourcePath) OrElse Not File.Exists(sourcePath) Then
            Return String.Empty
        End If

        If Not IsAllowedImageExtension(sourcePath) Then
            Return String.Empty
        End If

        Dim safeRegNo As String = SanitizeFileName(regNo)
        If String.IsNullOrWhiteSpace(safeRegNo) Then
            Return String.Empty
        End If

        Dim extension As String = Path.GetExtension(sourcePath)
        If String.IsNullOrWhiteSpace(extension) Then
            extension = ".jpg"
        End If
        extension = extension.ToLowerInvariant()

        Dim fileName As String = safeRegNo & extension
        Dim destPath As String = Path.Combine(GetPassportFolder(), fileName)

        Dim sourceFull As String = Path.GetFullPath(sourcePath)
        Dim destFull As String = Path.GetFullPath(destPath)
        If Not String.Equals(sourceFull, destFull, StringComparison.OrdinalIgnoreCase) Then
            File.Copy(sourcePath, destPath, True)
        End If

        Return fileName
    End Function

    Private Sub label8_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs)
        Try
            grabber = New Capture()
            grabber.QueryFrame()
            Timer1.Start()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs)
        Try
            'Label3.Text = "0"

            'Get the current frame form capture device
            currentFrame = grabber.QueryFrame()
            If currentFrame Is Nothing Then
                Return
            End If

            pictureBox1.Image = currentFrame.ToBitmap()

        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try
    End Sub

    Private Sub btnCapture_Click(sender As Object, e As EventArgs)
        Try

            grabber.Dispose()
            Timer1.Stop()

        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try
    End Sub

    Public Sub GenerateID()
        Dim numbers As String = "1234567890"

        Dim characters As String = numbers
        characters += Convert.ToString(numbers)
        Dim length As Integer = Integer.Parse("4")
        Dim id As String = String.Empty
        For i As Integer = 0 To length - 1
            Dim character As String = String.Empty
            Do
                Dim index As Integer = New Random().Next(0, characters.Length)
                character = characters.ToCharArray()(index).ToString()
            Loop While id.IndexOf(character) <> -1
            id += character
        Next
        txtMatricNo.Text = id.Trim()
    End Sub

    Private Sub Register_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbFaculty.DropDownStyle = ComboBoxStyle.DropDownList
        cmbDept.DropDownStyle = ComboBoxStyle.DropDownList
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        LoadSections()
        LoadYears()
        LoadCustomFields()
        GenerateID()
    End Sub

    Public Sub RegisterEmp()
        Try
            Dim regNo As String = txtMatricNo.Text.Trim()
            If String.IsNullOrWhiteSpace(regNo) Then
                MessageBox.Show("Reg No is required.")
                Return
            End If
            If Not IsValidRegNo(regNo) Then
                MessageBox.Show("Reg No format is invalid. Use letters, numbers, '-' or '/'.")
                Return
            End If
            If String.IsNullOrWhiteSpace(txtName.Text) Then
                MessageBox.Show("Name is required.")
                Return
            End If
            If cmbGender.SelectedIndex < 0 Then
                MessageBox.Show("Gender is required.")
                Return
            End If
            If GetSelectedSectionId() Is DBNull.Value Then
                MessageBox.Show("Section is required.")
                Return
            End If
            If GetSelectedYearId() Is DBNull.Value Then
                MessageBox.Show("Year is required.")
                Return
            End If

            Dim customValues As Dictionary(Of Integer, String) = CollectCustomFieldValues()
            If customValues Is Nothing Then
                Return
            End If

            Dim constr As String = Module1.ConnectionString
            Using cnn As New MySqlConnection(constr)
                Using com As New MySqlCommand("SELECT 1 FROM students WHERE regno = @regno LIMIT 1", cnn)
                    com.CommandType = CommandType.Text
                    com.Parameters.AddWithValue("@regno", regNo)
                    cnn.Open()
                    Dim exists As Object = com.ExecuteScalar()
                    If exists IsNot Nothing Then
                        MsgBox("Duplicate Record Detected", MsgBoxStyle.Information, "Biometric Fingerprints Student Attendance System")
                        AppLogger.Info("Student duplicate: " & regNo)
                        Return
                    End If
                End Using
            End Using

            Dim passportValue As Object = DBNull.Value
            Dim passportFileName As String = SavePassportImage(regNo, pictureBox1.Image)
            If Not String.IsNullOrWhiteSpace(passportFileName) Then
                passportValue = passportFileName
            End If

            Using cn As New MySqlConnection(Module1.ConnectionString)
                Using comd As New MySqlCommand("INSERT INTO students (regno, name, section_id, year_id, gender, passport) VALUES (@regno, @name, @section_id, @year_id, @gender, @passport)", cn)
                    comd.CommandType = CommandType.Text
                    comd.Parameters.AddWithValue("@regno", regNo)
                    comd.Parameters.AddWithValue("@name", txtName.Text.Trim())
                    comd.Parameters.AddWithValue("@section_id", GetSelectedSectionId())
                    comd.Parameters.AddWithValue("@year_id", GetSelectedYearId())
                    comd.Parameters.AddWithValue("@gender", cmbGender.Text.Trim())
                    comd.Parameters.AddWithValue("@passport", passportValue)
                    cn.Open()
                    comd.ExecuteNonQuery()
                End Using
            End Using

            If customValues.Count > 0 Then
                StudentFieldStore.UpsertFieldValues(regNo, customValues)
            End If

            AuditLogger.LogAction("StudentRegister", regNo & " | " & txtName.Text.Trim())

            Reset()
            MsgBox("Submitted Successfully", MsgBoxStyle.Information, "Biometric Fingerprints Student Attendance System")
            GenerateID()
        Catch ex As Exception
            AppLogger.LogError("Student register error", ex)
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
    Public Sub Reset()
        txtName.Text = Nothing
        cmbGender.SelectedIndex = -1
        cmbDept.SelectedIndex = -1
        cmbFaculty.SelectedIndex = -1
        cmbGender.SelectedIndex = -1
        pictureBox1.Image = Nothing
        ResetCustomFields()

    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        RegisterEmp()
    End Sub

    Private Sub btnUploadPhoto_Click(sender As Object, e As EventArgs) Handles btnUploadPhoto.Click
        Dim OpenFileDialog As OpenFileDialog = New OpenFileDialog()
        OpenFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp"
        If (OpenFileDialog.ShowDialog() = DialogResult.OK) Then
            If Not IsAllowedImageExtension(OpenFileDialog.FileName) Then
                MessageBox.Show("Unsupported image type.")
                Return
            End If
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom
            pictureBox1.Image = New Bitmap(OpenFileDialog.FileName)
        End If
    End Sub

    Private Sub btnImportStudents_Click(sender As Object, e As EventArgs) Handles btnImportStudents.Click
        Using dialog As New OpenFileDialog()
            dialog.Filter = "CSV files (*.csv)|*.csv|Excel files (*.xlsx)|*.xlsx"
            dialog.Title = "Import Students"
            If dialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Dim dt As DataTable
            Try
                If dialog.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                    dt = LoadCsv(dialog.FileName)
                Else
                    dt = LoadExcel(dialog.FileName)
                End If
                ImportStudents(dt)
                LoadSections()
                LoadYears()
                MessageBox.Show("Import completed.")
            Catch ex As Exception
                MessageBox.Show("Import failed: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub btnExportStudents_Click(sender As Object, e As EventArgs) Handles btnExportStudents.Click
        Try
            Dim dt As DataTable = GetStudentsForExport()
            Using dialog As New SaveFileDialog()
                dialog.Filter = "CSV files (*.csv)|*.csv|Excel files (*.xlsx)|*.xlsx"
                dialog.Title = "Export Students"
                dialog.FileName = "students_export.csv"
                If dialog.ShowDialog() <> DialogResult.OK Then
                    Return
                End If

                If dialog.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                    ExportCsv(dt, dialog.FileName)
                Else
                    ExportExcel(dt, dialog.FileName, "Students")
                End If

                MessageBox.Show("Export completed.")
            End Using
        Catch ex As Exception
            MessageBox.Show("Export failed: " & ex.Message)
        End Try
    End Sub

    Private Sub btnImportPhotos_Click(sender As Object, e As EventArgs) Handles btnImportPhotos.Click
        Using dialog As New FolderBrowserDialog()
            dialog.Description = "Select folder containing photos named by Reg No (e.g., 1752.jpg)"
            If dialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Try
                BulkImportPhotos(dialog.SelectedPath)
                MessageBox.Show("Photo import completed.")
            Catch ex As Exception
                MessageBox.Show("Photo import failed: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub btnCancel_Click_1(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub label8_Click_1(sender As Object, e As EventArgs) Handles label8.Click

    End Sub

    Private Sub btnStart_Click_1(sender As Object, e As EventArgs) Handles btnStart.Click
        Try
            grabber = New Capture()
            grabber.QueryFrame()
            Timer1.Start()
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try
    End Sub

    Private Sub btnCapture_Click_1(sender As Object, e As EventArgs) Handles btnCapture.Click
        Try

            grabber.Dispose()
            Timer1.Stop()

        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try
    End Sub

    Private Sub pictureBox1_Click(sender As Object, e As EventArgs) Handles pictureBox1.Click

    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            currentFrame = grabber.QueryFrame()
            If currentFrame Is Nothing Then
                Return
            End If
            pictureBox1.Image = currentFrame.ToBitmap()

        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "")
        End Try
    End Sub

    Private Sub LoadSections()
        SectionLookup.Clear()
        cmbFaculty.Items.Clear()

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("SELECT id, name FROM sections ORDER BY name", cn)
                cn.Open()
                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Dim id As Integer = Convert.ToInt32(dr("id"))
                        Dim name As String = dr("name").ToString()
                        SectionLookup(name) = id
                        cmbFaculty.Items.Add(name)
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub LoadYears()
        YearLookup.Clear()
        cmbDept.Items.Clear()

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("SELECT id, name FROM years ORDER BY name", cn)
                cn.Open()
                Using dr As MySqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Dim id As Integer = Convert.ToInt32(dr("id"))
                        Dim name As String = dr("name").ToString()
                        YearLookup(name) = id
                        cmbDept.Items.Add(name)
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub LoadCustomFields()
        CustomFields.Clear()
        CustomFieldControls.Clear()
        If flpCustomFields Is Nothing Then
            Return
        End If

        flpCustomFields.SuspendLayout()
        flpCustomFields.Controls.Clear()

        Dim fields As List(Of StudentFieldDef) = StudentFieldStore.GetActiveFields()
        If fields Is Nothing OrElse fields.Count = 0 Then
            flpCustomFields.ResumeLayout()
            Return
        End If

        For Each field As StudentFieldDef In fields
            CustomFields.Add(field)
            Dim rowWidth As Integer = Math.Max(300, flpCustomFields.ClientSize.Width - 25)
            Dim row As New Panel With {.Width = rowWidth, .Height = 32}

            Dim labelText As String = field.FieldLabel
            If field.IsRequired Then
                labelText &= " *"
            End If

            Dim lbl As New Label With {
                .AutoSize = False,
                .Text = labelText,
                .Width = 140,
                .TextAlign = ContentAlignment.MiddleLeft,
                .Location = New Point(0, 5)
            }

            Dim input As Control = BuildCustomFieldControl(field)
            input.Location = New Point(lbl.Width + 6, 2)
            input.Width = Math.Max(120, rowWidth - lbl.Width - 10)

            row.Controls.Add(lbl)
            row.Controls.Add(input)
            row.Height = Math.Max(32, input.Height + 6)
            flpCustomFields.Controls.Add(row)
            CustomFieldControls(field.Id) = input
        Next

        flpCustomFields.ResumeLayout()
    End Sub

    Private Function BuildCustomFieldControl(ByVal field As StudentFieldDef) As Control
        Dim fieldType As String = If(field.FieldType, String.Empty).Trim().ToLowerInvariant()
        Select Case fieldType
            Case "date"
                Dim picker As New DateTimePicker With {
                    .Format = DateTimePickerFormat.Short,
                    .ShowCheckBox = Not field.IsRequired
                }
                If picker.ShowCheckBox Then
                    picker.Checked = False
                End If
                Return picker
            Case "dropdown"
                Dim combo As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList}
                If Not String.IsNullOrWhiteSpace(field.Options) Then
                    Dim items As String() = field.Options.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
                    For Each item As String In items
                        combo.Items.Add(item.Trim())
                    Next
                End If
                combo.SelectedIndex = -1
                Return combo
            Case "multiline"
                Dim box As New TextBox With {.Multiline = True, .Height = 50, .ScrollBars = ScrollBars.Vertical}
                Return box
            Case Else
                Dim text As New TextBox()
                Return text
        End Select
    End Function

    Private Function CollectCustomFieldValues() As Dictionary(Of Integer, String)
        Dim values As New Dictionary(Of Integer, String)()
        For Each field As StudentFieldDef In CustomFields
            If Not CustomFieldControls.ContainsKey(field.Id) Then
                Continue For
            End If
            Dim value As String = GetCustomFieldValue(field, CustomFieldControls(field.Id))
            If field.IsRequired AndAlso String.IsNullOrWhiteSpace(value) Then
                MessageBox.Show(field.FieldLabel & " is required.")
                Return Nothing
            End If
            If Not String.IsNullOrWhiteSpace(value) Then
                values(field.Id) = value
            End If
        Next
        Return values
    End Function

    Private Function GetCustomFieldValue(ByVal field As StudentFieldDef, ByVal control As Control) As String
        Dim fieldType As String = If(field.FieldType, String.Empty).Trim().ToLowerInvariant()
        Select Case fieldType
            Case "date"
                Dim picker As DateTimePicker = TryCast(control, DateTimePicker)
                If picker IsNot Nothing Then
                    If Not picker.ShowCheckBox OrElse picker.Checked Then
                        Return picker.Value.ToString("yyyy-MM-dd")
                    End If
                End If
            Case "dropdown"
                Dim combo As ComboBox = TryCast(control, ComboBox)
                If combo IsNot Nothing AndAlso combo.SelectedItem IsNot Nothing Then
                    Return combo.SelectedItem.ToString().Trim()
                End If
            Case "multiline"
                Dim box As TextBox = TryCast(control, TextBox)
                If box IsNot Nothing Then
                    Return box.Text.Trim()
                End If
            Case Else
                Dim box As TextBox = TryCast(control, TextBox)
                If box IsNot Nothing Then
                    Return box.Text.Trim()
                End If
        End Select
        Return String.Empty
    End Function

    Private Sub ResetCustomFields()
        For Each ctrl As Control In CustomFieldControls.Values
            If TypeOf ctrl Is TextBox Then
                CType(ctrl, TextBox).Text = String.Empty
            ElseIf TypeOf ctrl Is ComboBox Then
                CType(ctrl, ComboBox).SelectedIndex = -1
            ElseIf TypeOf ctrl Is DateTimePicker Then
                Dim picker As DateTimePicker = CType(ctrl, DateTimePicker)
                picker.Value = Date.Today
                If picker.ShowCheckBox Then
                    picker.Checked = False
                End If
            ElseIf TypeOf ctrl Is CheckBox Then
                CType(ctrl, CheckBox).Checked = False
            End If
        Next
    End Sub

    Private Function GetSelectedSectionId() As Object
        If cmbFaculty.SelectedItem Is Nothing Then
            Return DBNull.Value
        End If

        Dim name As String = cmbFaculty.SelectedItem.ToString()
        If SectionLookup.ContainsKey(name) Then
            Return SectionLookup(name)
        End If

        Return DBNull.Value
    End Function

    Private Function GetSelectedYearId() As Object
        If cmbDept.SelectedItem Is Nothing Then
            Return DBNull.Value
        End If

        Dim name As String = cmbDept.SelectedItem.ToString()
        If YearLookup.ContainsKey(name) Then
            Return YearLookup(name)
        End If

        Return DBNull.Value
    End Function

    Private Function LoadCsv(ByVal filePath As String) As DataTable
        Dim dt As New DataTable()
        Using parser As New TextFieldParser(filePath)
            parser.SetDelimiters(",")
            parser.HasFieldsEnclosedInQuotes = True
            If parser.EndOfData Then
                Return dt
            End If

            Dim headers As String() = parser.ReadFields()
            For Each header In headers
                dt.Columns.Add(header)
            Next

            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()
                Dim row As DataRow = dt.NewRow()
                For i As Integer = 0 To headers.Length - 1
                    If i < fields.Length Then
                        row(i) = fields(i)
                    End If
                Next
                dt.Rows.Add(row)
            End While
        End Using

        Return dt
    End Function

    Private Function LoadExcel(ByVal filePath As String) As DataTable
        Dim dt As New DataTable()
        Dim connStr As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1';"
        Using conn As New OleDbConnection(connStr)
            conn.Open()
            Dim schema As DataTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
            If schema Is Nothing OrElse schema.Rows.Count = 0 Then
                Throw New Exception("No sheets found in Excel file.")
            End If

            Dim sheetName As String = schema.Rows(0)("TABLE_NAME").ToString()
            Using cmd As New OleDbCommand("SELECT * FROM [" & sheetName & "]", conn)
                Using adapter As New OleDbDataAdapter(cmd)
                    adapter.Fill(dt)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Private Sub ImportStudents(ByVal dt As DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Throw New Exception("No rows found to import.")
        End If

        Using cn As New MySqlConnection(Module1.ConnectionString)
            cn.Open()
            Using tx As MySqlTransaction = cn.BeginTransaction()
                Dim customFields As List(Of StudentFieldDef) = StudentFieldStore.GetActiveFields()
                Dim processed As Integer = 0
                For Each row As DataRow In dt.Rows
                    Dim regno As String = GetField(row, "regno", "reg_no", "registration_no", "registration number")
                    If String.IsNullOrWhiteSpace(regno) Then
                        Continue For
                    End If

                    Dim name As String = GetField(row, "name", "student_name")
                    Dim sectionName As String = GetField(row, "section")
                    Dim yearName As String = GetField(row, "year")
                    Dim gender As String = GetField(row, "gender")
                    Dim photoPath As String = GetField(row, "photo_path", "passport_path", "image_path")

                    If Not IsValidRegNo(regno) OrElse String.IsNullOrWhiteSpace(name) Then
                        Continue For
                    End If
                    If String.IsNullOrWhiteSpace(sectionName) OrElse String.IsNullOrWhiteSpace(yearName) Then
                        Continue For
                    End If

                    Dim customValues As Dictionary(Of Integer, String) = ExtractCustomFieldValues(row, customFields)
                    If customValues Is Nothing Then
                        Continue For
                    End If

                    Dim sectionId As Object = EnsureSectionId(sectionName, cn, tx)
                    Dim yearId As Object = EnsureYearId(yearName, cn, tx)

                    Dim passportValue As Object = DBNull.Value
                    If Not String.IsNullOrWhiteSpace(photoPath) Then
                        Dim passportFileName As String = String.Empty
                        If File.Exists(photoPath) AndAlso IsAllowedImageExtension(photoPath) Then
                            passportFileName = CopyPassportFile(regno, photoPath)
                        Else
                            Dim candidatePath As String = Path.Combine(GetPassportFolder(), photoPath)
                            If File.Exists(candidatePath) AndAlso IsAllowedImageExtension(candidatePath) Then
                                passportFileName = Path.GetFileName(photoPath)
                            End If
                        End If

                        If Not String.IsNullOrWhiteSpace(passportFileName) Then
                            passportValue = passportFileName
                        End If
                    End If

                    Using cmd As New MySqlCommand("INSERT INTO students (regno, name, section_id, year_id, gender, passport) VALUES (@regno, @name, @section_id, @year_id, @gender, @passport) ON DUPLICATE KEY UPDATE name = VALUES(name), section_id = VALUES(section_id), year_id = VALUES(year_id), gender = VALUES(gender), passport = IFNULL(VALUES(passport), passport)", cn, tx)
                        cmd.Parameters.AddWithValue("@regno", regno.Trim())
                        cmd.Parameters.AddWithValue("@name", name)
                        cmd.Parameters.AddWithValue("@section_id", sectionId)
                        cmd.Parameters.AddWithValue("@year_id", yearId)
                        cmd.Parameters.AddWithValue("@gender", gender)
                        cmd.Parameters.AddWithValue("@passport", passportValue)
                        cmd.ExecuteNonQuery()
                    End Using

                    If customValues.Count > 0 Then
                        StudentFieldStore.UpsertFieldValues(regno.Trim(), customValues, cn, tx)
                    End If
                    processed += 1
                Next
                tx.Commit()
                AuditLogger.LogAction("StudentImport", "Processed rows: " & processed.ToString())
            End Using
        End Using
    End Sub

    Private Function EnsureSectionId(ByVal name As String, ByVal cn As MySqlConnection, ByVal tx As MySqlTransaction) As Object
        If String.IsNullOrWhiteSpace(name) Then
            Return DBNull.Value
        End If

        Dim trimmed As String = name.Trim()
        If SectionLookup.ContainsKey(trimmed) Then
            Return SectionLookup(trimmed)
        End If

        Using cmd As New MySqlCommand("SELECT id FROM sections WHERE name = @name LIMIT 1", cn, tx)
            cmd.Parameters.AddWithValue("@name", trimmed)
            Dim existing As Object = cmd.ExecuteScalar()
            If existing IsNot Nothing Then
                Dim id As Integer = Convert.ToInt32(existing)
                SectionLookup(trimmed) = id
                Return id
            End If
        End Using

        Using cmd As New MySqlCommand("INSERT INTO sections (name) VALUES (@name)", cn, tx)
            cmd.Parameters.AddWithValue("@name", trimmed)
            cmd.ExecuteNonQuery()
            Dim newId As Integer = Convert.ToInt32(cmd.LastInsertedId)
            SectionLookup(trimmed) = newId
            Return newId
        End Using
    End Function

    Private Function EnsureYearId(ByVal name As String, ByVal cn As MySqlConnection, ByVal tx As MySqlTransaction) As Object
        If String.IsNullOrWhiteSpace(name) Then
            Return DBNull.Value
        End If

        Dim trimmed As String = name.Trim()
        If YearLookup.ContainsKey(trimmed) Then
            Return YearLookup(trimmed)
        End If

        Using cmd As New MySqlCommand("SELECT id FROM years WHERE name = @name LIMIT 1", cn, tx)
            cmd.Parameters.AddWithValue("@name", trimmed)
            Dim existing As Object = cmd.ExecuteScalar()
            If existing IsNot Nothing Then
                Dim id As Integer = Convert.ToInt32(existing)
                YearLookup(trimmed) = id
                Return id
            End If
        End Using

        Using cmd As New MySqlCommand("INSERT INTO years (name) VALUES (@name)", cn, tx)
            cmd.Parameters.AddWithValue("@name", trimmed)
            cmd.ExecuteNonQuery()
            Dim newId As Integer = Convert.ToInt32(cmd.LastInsertedId)
            YearLookup(trimmed) = newId
            Return newId
        End Using
    End Function

    Private Function GetField(ByVal row As DataRow, ParamArray names() As String) As String
        For Each fieldName As String In names
            Dim col As DataColumn = row.Table.Columns.Cast(Of DataColumn)().FirstOrDefault(Function(c) String.Equals(c.ColumnName, fieldName, StringComparison.OrdinalIgnoreCase))
            If col IsNot Nothing AndAlso row(col) IsNot DBNull.Value Then
                Return row(col).ToString()
            End If
        Next
        Return String.Empty
    End Function

    Private Function ExtractCustomFieldValues(ByVal row As DataRow, ByVal fields As List(Of StudentFieldDef)) As Dictionary(Of Integer, String)
        Dim values As New Dictionary(Of Integer, String)()
        If fields Is Nothing OrElse fields.Count = 0 Then
            Return values
        End If

        For Each field As StudentFieldDef In fields
            Dim value As String = GetField(row, field.FieldKey, field.FieldLabel)
            If String.IsNullOrWhiteSpace(value) Then
                If field.IsRequired Then
                    Return Nothing
                End If
                Continue For
            End If
            values(field.Id) = value.Trim()
        Next

        Return values
    End Function

    Private Sub BulkImportPhotos(ByVal folderPath As String)
        Dim extensions As String() = {".jpg", ".jpeg", ".png", ".bmp", ".gif"}
        Dim files As IEnumerable(Of String) = Directory.EnumerateFiles(folderPath).Where(Function(f) extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))

        Using cn As New MySqlConnection(Module1.ConnectionString)
            cn.Open()
            Using tx As MySqlTransaction = cn.BeginTransaction()
                Dim processed As Integer = 0
                For Each filePath As String In files
                    Dim regno As String = Path.GetFileNameWithoutExtension(filePath)
                    If String.IsNullOrWhiteSpace(regno) Then
                        Continue For
                    End If

                    Dim passportFileName As String = CopyPassportFile(regno, filePath)
                    If String.IsNullOrWhiteSpace(passportFileName) Then
                        Continue For
                    End If

                    Using cmd As New MySqlCommand("UPDATE students SET passport = @passport WHERE regno = @regno", cn, tx)
                        cmd.Parameters.AddWithValue("@passport", passportFileName)
                        cmd.Parameters.AddWithValue("@regno", regno.Trim())
                        cmd.ExecuteNonQuery()
                    End Using
                    processed += 1
                Next
                tx.Commit()
                AuditLogger.LogAction("PhotoImport", "Processed photos: " & processed.ToString())
            End Using
        End Using
    End Sub

    Private Function GetStudentsForExport() As DataTable
        Dim dt As New DataTable()
        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("SELECT s.regno AS RegNo, s.name AS Name, sec.name AS Section, yr.name AS Year, s.gender AS Gender FROM students s LEFT JOIN sections sec ON sec.id = s.section_id LEFT JOIN years yr ON yr.id = s.year_id ORDER BY s.regno", cn)
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        Dim customFields As List(Of StudentFieldDef) = StudentFieldStore.GetActiveFields()
        If customFields Is Nothing OrElse customFields.Count = 0 Then
            Return dt
        End If

        Dim fieldColumnMap As New Dictionary(Of Integer, String)()
        For Each field As StudentFieldDef In customFields
            Dim colName As String = EnsureUniqueColumnName(dt, field.FieldLabel)
            fieldColumnMap(field.Id) = colName
            If Not dt.Columns.Contains(colName) Then
                dt.Columns.Add(colName)
            End If
        Next

        Dim valueMap As Dictionary(Of String, Dictionary(Of Integer, String)) = StudentFieldStore.GetFieldValuesMap()
        For Each row As DataRow In dt.Rows
            Dim regno As String = row("RegNo").ToString()
            If String.IsNullOrWhiteSpace(regno) Then
                Continue For
            End If
            If Not valueMap.ContainsKey(regno) Then
                Continue For
            End If
            Dim values As Dictionary(Of Integer, String) = valueMap(regno)
            For Each field As StudentFieldDef In customFields
                Dim colName As String = fieldColumnMap(field.Id)
                If values.ContainsKey(field.Id) Then
                    row(colName) = values(field.Id)
                End If
            Next
        Next
        Return dt
    End Function

    Private Sub ExportCsv(ByVal dt As DataTable, ByVal filePath As String)
        Dim sb As New StringBuilder()
        Dim headers As IEnumerable(Of String) = dt.Columns.Cast(Of DataColumn)().Select(Function(c) EscapeCsv(c.ColumnName))
        sb.AppendLine(String.Join(",", headers))

        For Each row As DataRow In dt.Rows
            Dim fields As IEnumerable(Of String) = dt.Columns.Cast(Of DataColumn)().Select(Function(c) EscapeCsv(row(c).ToString()))
            sb.AppendLine(String.Join(",", fields))
        Next

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
    End Sub

    Private Sub ExportExcel(ByVal dt As DataTable, ByVal filePath As String, ByVal sheetName As String)
        Try
            Dim connStr As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties='Excel 12.0 Xml;HDR=YES';"
            Using conn As New OleDbConnection(connStr)
                conn.Open()
                Using cmd As New OleDbCommand(BuildCreateTableSql(sheetName, dt), conn)
                    cmd.ExecuteNonQuery()
                End Using

                For Each row As DataRow In dt.Rows
                    Using cmd As New OleDbCommand(BuildInsertSql(sheetName, dt), conn)
                        For Each col As DataColumn In dt.Columns
                            cmd.Parameters.AddWithValue("@" & col.ColumnName, row(col))
                        Next
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Using
        Catch ex As Exception
            Throw New Exception("Excel export failed. Install Microsoft Access Database Engine or export to CSV. " & ex.Message)
        End Try
    End Sub

    Private Function BuildCreateTableSql(ByVal sheetName As String, ByVal dt As DataTable) As String
        Dim cols As IEnumerable(Of String) = dt.Columns.Cast(Of DataColumn)().Select(Function(c) "[" & c.ColumnName & "] TEXT")
        Return "CREATE TABLE [" & sheetName & "] (" & String.Join(",", cols) & ")"
    End Function

    Private Function BuildInsertSql(ByVal sheetName As String, ByVal dt As DataTable) As String
        Dim colNames As IEnumerable(Of String) = dt.Columns.Cast(Of DataColumn)().Select(Function(c) "[" & c.ColumnName & "]")
        Dim placeholders As IEnumerable(Of String) = dt.Columns.Cast(Of DataColumn)().Select(Function(c) "?")
        Return "INSERT INTO [" & sheetName & "] (" & String.Join(",", colNames) & ") VALUES (" & String.Join(",", placeholders) & ")"
    End Function

    Private Function EnsureUniqueColumnName(ByVal dt As DataTable, ByVal baseName As String) As String
        Dim safeName As String = If(String.IsNullOrWhiteSpace(baseName), "Field", baseName)
        Dim name As String = safeName
        Dim index As Integer = 2
        While dt.Columns.Contains(name)
            name = safeName & " (" & index.ToString() & ")"
            index += 1
        End While
        Return name
    End Function

    Private Function EscapeCsv(ByVal value As String) As String
        If value Is Nothing Then
            Return ""
        End If
        Dim needsQuotes As Boolean = value.Contains(",") OrElse value.Contains("""") OrElse value.Contains(ControlChars.Cr) OrElse value.Contains(ControlChars.Lf)
        If needsQuotes Then
            value = value.Replace("""", """""")
            Return """" & value & """"
        End If
        Return value
    End Function
End Class
