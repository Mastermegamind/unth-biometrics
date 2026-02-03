Imports System
Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class LookupManager
    Inherits Form

    Private tabLookups As TabControl
    Private tabSections As TabPage
    Private tabYears As TabPage
    Private tabCustomFields As TabPage

    Private dgvSections As DataGridView
    Private dgvYears As DataGridView
    Private dgvFields As DataGridView

    Private btnAddSection As Button
    Private btnEditSection As Button
    Private btnDeleteSection As Button
    Private btnRefreshSections As Button

    Private btnAddYear As Button
    Private btnEditYear As Button
    Private btnDeleteYear As Button
    Private btnRefreshYears As Button

    Private btnAddField As Button
    Private btnEditField As Button
    Private btnDeleteField As Button
    Private btnRefreshFields As Button

    Public Sub New()
        InitializeComponent()
        LoadSections()
        LoadYears()
        LoadFields()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Manage Sections and Years"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Drawing.Size(700, 450)

        tabLookups = New TabControl()
        tabLookups.Dock = DockStyle.Fill

        tabSections = New TabPage("Sections")
        tabYears = New TabPage("Years")
        tabCustomFields = New TabPage("Custom Fields")

        dgvSections = New DataGridView()
        dgvSections.Dock = DockStyle.Top
        dgvSections.Height = 300
        dgvSections.ReadOnly = True
        dgvSections.AllowUserToAddRows = False
        dgvSections.AllowUserToDeleteRows = False
        dgvSections.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSections.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        dgvYears = New DataGridView()
        dgvYears.Dock = DockStyle.Top
        dgvYears.Height = 300
        dgvYears.ReadOnly = True
        dgvYears.AllowUserToAddRows = False
        dgvYears.AllowUserToDeleteRows = False
        dgvYears.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvYears.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        dgvFields = New DataGridView()
        dgvFields.Dock = DockStyle.Top
        dgvFields.Height = 300
        dgvFields.ReadOnly = True
        dgvFields.AllowUserToAddRows = False
        dgvFields.AllowUserToDeleteRows = False
        dgvFields.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvFields.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        btnAddSection = New Button()
        btnAddSection.Text = "Add"
        btnAddSection.Width = 80
        AddHandler btnAddSection.Click, AddressOf btnAddSection_Click

        btnEditSection = New Button()
        btnEditSection.Text = "Edit"
        btnEditSection.Width = 80
        AddHandler btnEditSection.Click, AddressOf btnEditSection_Click

        btnDeleteSection = New Button()
        btnDeleteSection.Text = "Delete"
        btnDeleteSection.Width = 80
        AddHandler btnDeleteSection.Click, AddressOf btnDeleteSection_Click

        btnRefreshSections = New Button()
        btnRefreshSections.Text = "Refresh"
        btnRefreshSections.Width = 80
        AddHandler btnRefreshSections.Click, AddressOf btnRefreshSections_Click

        Dim panelSections As New FlowLayoutPanel()
        panelSections.Dock = DockStyle.Bottom
        panelSections.Height = 40
        panelSections.FlowDirection = FlowDirection.LeftToRight
        panelSections.Controls.AddRange(New Control() {btnAddSection, btnEditSection, btnDeleteSection, btnRefreshSections})

        btnAddYear = New Button()
        btnAddYear.Text = "Add"
        btnAddYear.Width = 80
        AddHandler btnAddYear.Click, AddressOf btnAddYear_Click

        btnEditYear = New Button()
        btnEditYear.Text = "Edit"
        btnEditYear.Width = 80
        AddHandler btnEditYear.Click, AddressOf btnEditYear_Click

        btnDeleteYear = New Button()
        btnDeleteYear.Text = "Delete"
        btnDeleteYear.Width = 80
        AddHandler btnDeleteYear.Click, AddressOf btnDeleteYear_Click

        btnRefreshYears = New Button()
        btnRefreshYears.Text = "Refresh"
        btnRefreshYears.Width = 80
        AddHandler btnRefreshYears.Click, AddressOf btnRefreshYears_Click

        Dim panelYears As New FlowLayoutPanel()
        panelYears.Dock = DockStyle.Bottom
        panelYears.Height = 40
        panelYears.FlowDirection = FlowDirection.LeftToRight
        panelYears.Controls.AddRange(New Control() {btnAddYear, btnEditYear, btnDeleteYear, btnRefreshYears})

        btnAddField = New Button()
        btnAddField.Text = "Add"
        btnAddField.Width = 80
        AddHandler btnAddField.Click, AddressOf btnAddField_Click

        btnEditField = New Button()
        btnEditField.Text = "Edit"
        btnEditField.Width = 80
        AddHandler btnEditField.Click, AddressOf btnEditField_Click

        btnDeleteField = New Button()
        btnDeleteField.Text = "Delete"
        btnDeleteField.Width = 80
        AddHandler btnDeleteField.Click, AddressOf btnDeleteField_Click

        btnRefreshFields = New Button()
        btnRefreshFields.Text = "Refresh"
        btnRefreshFields.Width = 80
        AddHandler btnRefreshFields.Click, AddressOf btnRefreshFields_Click

        Dim panelFields As New FlowLayoutPanel()
        panelFields.Dock = DockStyle.Bottom
        panelFields.Height = 40
        panelFields.FlowDirection = FlowDirection.LeftToRight
        panelFields.Controls.AddRange(New Control() {btnAddField, btnEditField, btnDeleteField, btnRefreshFields})

        tabSections.Controls.Add(dgvSections)
        tabSections.Controls.Add(panelSections)

        tabYears.Controls.Add(dgvYears)
        tabYears.Controls.Add(panelYears)

        tabCustomFields.Controls.Add(dgvFields)
        tabCustomFields.Controls.Add(panelFields)

        tabLookups.Controls.Add(tabSections)
        tabLookups.Controls.Add(tabYears)
        tabLookups.Controls.Add(tabCustomFields)

        Me.Controls.Add(tabLookups)
    End Sub

    Private Sub LoadSections()
        Dim dt As New DataTable()
        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("SELECT id, name FROM sections ORDER BY name", cn)
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        dgvSections.DataSource = dt
    End Sub

    Private Sub LoadYears()
        Dim dt As New DataTable()
        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("SELECT id, name FROM years ORDER BY name", cn)
                Using da As New MySqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        dgvYears.DataSource = dt
    End Sub

    Private Sub LoadFields()
        dgvFields.DataSource = StudentFieldStore.GetAllFieldsTable()
    End Sub

    Private Sub btnAddSection_Click(sender As Object, e As EventArgs)
        Dim name As String = InputBox("Enter section name:", "Add Section")
        If String.IsNullOrWhiteSpace(name) Then
            Return
        End If

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("INSERT INTO sections (name) VALUES (@name)", cn)
                cmd.Parameters.AddWithValue("@name", name.Trim())
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadSections()
    End Sub

    Private Sub btnEditSection_Click(sender As Object, e As EventArgs)
        Dim row As DataGridViewRow = GetSelectedRow(dgvSections)
        If row Is Nothing Then
            Return
        End If
        Dim id As Integer = Convert.ToInt32(row.Cells("id").Value)
        Dim currentName As String = row.Cells("name").Value.ToString()
        Dim name As String = InputBox("Edit section name:", "Edit Section", currentName)
        If String.IsNullOrWhiteSpace(name) Then
            Return
        End If

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("UPDATE sections SET name = @name WHERE id = @id", cn)
                cmd.Parameters.AddWithValue("@name", name.Trim())
                cmd.Parameters.AddWithValue("@id", id)
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadSections()
    End Sub

    Private Sub btnDeleteSection_Click(sender As Object, e As EventArgs)
        Dim row As DataGridViewRow = GetSelectedRow(dgvSections)
        If row Is Nothing Then
            Return
        End If
        Dim id As Integer = Convert.ToInt32(row.Cells("id").Value)
        If MessageBox.Show("Delete selected section?", "Confirm", MessageBoxButtons.YesNo) <> DialogResult.Yes Then
            Return
        End If

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("DELETE FROM sections WHERE id = @id", cn)
                cmd.Parameters.AddWithValue("@id", id)
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadSections()
    End Sub

    Private Sub btnRefreshSections_Click(sender As Object, e As EventArgs)
        LoadSections()
    End Sub

    Private Sub btnAddYear_Click(sender As Object, e As EventArgs)
        Dim name As String = InputBox("Enter year name:", "Add Year")
        If String.IsNullOrWhiteSpace(name) Then
            Return
        End If

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("INSERT INTO years (name) VALUES (@name)", cn)
                cmd.Parameters.AddWithValue("@name", name.Trim())
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadYears()
    End Sub

    Private Sub btnEditYear_Click(sender As Object, e As EventArgs)
        Dim row As DataGridViewRow = GetSelectedRow(dgvYears)
        If row Is Nothing Then
            Return
        End If
        Dim id As Integer = Convert.ToInt32(row.Cells("id").Value)
        Dim currentName As String = row.Cells("name").Value.ToString()
        Dim name As String = InputBox("Edit year name:", "Edit Year", currentName)
        If String.IsNullOrWhiteSpace(name) Then
            Return
        End If

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("UPDATE years SET name = @name WHERE id = @id", cn)
                cmd.Parameters.AddWithValue("@name", name.Trim())
                cmd.Parameters.AddWithValue("@id", id)
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadYears()
    End Sub

    Private Sub btnDeleteYear_Click(sender As Object, e As EventArgs)
        Dim row As DataGridViewRow = GetSelectedRow(dgvYears)
        If row Is Nothing Then
            Return
        End If
        Dim id As Integer = Convert.ToInt32(row.Cells("id").Value)
        If MessageBox.Show("Delete selected year?", "Confirm", MessageBoxButtons.YesNo) <> DialogResult.Yes Then
            Return
        End If

        Using cn As New MySqlConnection(Module1.ConnectionString)
            Using cmd As New MySqlCommand("DELETE FROM years WHERE id = @id", cn)
                cmd.Parameters.AddWithValue("@id", id)
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadYears()
    End Sub

    Private Sub btnRefreshYears_Click(sender As Object, e As EventArgs)
        LoadYears()
    End Sub

    Private Sub btnAddField_Click(sender As Object, e As EventArgs)
        Using dialog As New CustomFieldDialog()
            If dialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Try
                Dim newId As Integer = StudentFieldStore.AddField(dialog.FieldDef)
                If newId > 0 Then
                    LoadFields()
                End If
            Catch ex As Exception
                MessageBox.Show("Unable to add field: " & ex.Message, "Custom Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Using
    End Sub

    Private Sub btnEditField_Click(sender As Object, e As EventArgs)
        Dim row As DataGridViewRow = GetSelectedRow(dgvFields)
        If row Is Nothing Then
            Return
        End If

        Dim def As StudentFieldDef = BuildFieldFromRow(row)
        If def Is Nothing Then
            Return
        End If

        Using dialog As New CustomFieldDialog(def)
            If dialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Try
                dialog.FieldDef.Id = def.Id
                dialog.FieldDef.FieldKey = def.FieldKey
                Dim ok As Boolean = StudentFieldStore.UpdateField(dialog.FieldDef)
                If ok Then
                    LoadFields()
                End If
            Catch ex As Exception
                MessageBox.Show("Unable to update field: " & ex.Message, "Custom Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Using
    End Sub

    Private Sub btnDeleteField_Click(sender As Object, e As EventArgs)
        Dim row As DataGridViewRow = GetSelectedRow(dgvFields)
        If row Is Nothing Then
            Return
        End If

        Dim id As Integer = Convert.ToInt32(row.Cells("id").Value)
        Dim label As String = row.Cells("field_label").Value.ToString()
        If MessageBox.Show("Delete custom field '" & label & "'? This will remove all saved values for it.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Return
        End If

        Try
            Dim ok As Boolean = StudentFieldStore.DeleteField(id)
            If ok Then
                LoadFields()
            End If
        Catch ex As Exception
            MessageBox.Show("Unable to delete field: " & ex.Message, "Custom Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub btnRefreshFields_Click(sender As Object, e As EventArgs)
        LoadFields()
    End Sub

    Private Function BuildFieldFromRow(ByVal row As DataGridViewRow) As StudentFieldDef
        Try
            Dim def As New StudentFieldDef With {
                .Id = Convert.ToInt32(row.Cells("id").Value),
                .FieldKey = row.Cells("field_key").Value.ToString(),
                .FieldLabel = row.Cells("field_label").Value.ToString(),
                .FieldType = row.Cells("field_type").Value.ToString(),
                .Options = If(row.Cells("options").Value Is DBNull.Value, String.Empty, row.Cells("options").Value.ToString()),
                .IsRequired = Convert.ToBoolean(row.Cells("is_required").Value),
                .IsActive = Convert.ToBoolean(row.Cells("is_active").Value),
                .DisplayOrder = Convert.ToInt32(row.Cells("display_order").Value)
            }
            Return def
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function GetSelectedRow(ByVal grid As DataGridView) As DataGridViewRow
        If grid.SelectedRows Is Nothing OrElse grid.SelectedRows.Count = 0 Then
            Return Nothing
        End If
        Return grid.SelectedRows(0)
    End Function
End Class
