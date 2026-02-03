Imports System
Imports System.Windows.Forms
Imports MySql.Data.MySqlClient

Public Class LookupManager
    Inherits Form

    Private tabLookups As TabControl
    Private tabSections As TabPage
    Private tabYears As TabPage

    Private dgvSections As DataGridView
    Private dgvYears As DataGridView

    Private btnAddSection As Button
    Private btnEditSection As Button
    Private btnDeleteSection As Button
    Private btnRefreshSections As Button

    Private btnAddYear As Button
    Private btnEditYear As Button
    Private btnDeleteYear As Button
    Private btnRefreshYears As Button

    Public Sub New()
        InitializeComponent()
        LoadSections()
        LoadYears()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Manage Sections and Years"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Drawing.Size(700, 450)

        tabLookups = New TabControl()
        tabLookups.Dock = DockStyle.Fill

        tabSections = New TabPage("Sections")
        tabYears = New TabPage("Years")

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

        tabSections.Controls.Add(dgvSections)
        tabSections.Controls.Add(panelSections)

        tabYears.Controls.Add(dgvYears)
        tabYears.Controls.Add(panelYears)

        tabLookups.Controls.Add(tabSections)
        tabLookups.Controls.Add(tabYears)

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

    Private Function GetSelectedRow(ByVal grid As DataGridView) As DataGridViewRow
        If grid.SelectedRows Is Nothing OrElse grid.SelectedRows.Count = 0 Then
            Return Nothing
        End If
        Return grid.SelectedRows(0)
    End Function
End Class
