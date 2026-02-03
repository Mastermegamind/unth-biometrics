Imports System.Windows.Forms

Public Class CustomFieldDialog
    Inherits Form

    Private ReadOnly txtLabel As TextBox
    Private ReadOnly cmbType As ComboBox
    Private ReadOnly txtOptions As TextBox
    Private ReadOnly chkRequired As CheckBox
    Private ReadOnly chkActive As CheckBox
    Private ReadOnly numOrder As NumericUpDown
    Private ReadOnly btnOk As Button
    Private ReadOnly btnCancel As Button

    Public Property FieldDef As StudentFieldDef

    Public Sub New(Optional ByVal existing As StudentFieldDef = Nothing)
        Text = If(existing Is Nothing, "Add Custom Field", "Edit Custom Field")
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Size = New Drawing.Size(420, 320)

        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 6,
            .Padding = New Padding(12)
        }
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35.0F))
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65.0F))

        txtLabel = New TextBox With {.Dock = DockStyle.Fill}
        cmbType = New ComboBox With {.Dock = DockStyle.Fill, .DropDownStyle = ComboBoxStyle.DropDownList}
        cmbType.Items.AddRange(New Object() {"text", "multiline", "number", "date", "dropdown"})

        txtOptions = New TextBox With {.Dock = DockStyle.Fill}
        chkRequired = New CheckBox With {.Text = "Required", .AutoSize = True}
        chkActive = New CheckBox With {.Text = "Active", .AutoSize = True}
        numOrder = New NumericUpDown With {.Dock = DockStyle.Left, .Minimum = 0, .Maximum = 1000, .Width = 100}

        layout.Controls.Add(New Label With {.Text = "Label:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 0)
        layout.Controls.Add(txtLabel, 1, 0)
        layout.Controls.Add(New Label With {.Text = "Type:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 1)
        layout.Controls.Add(cmbType, 1, 1)
        layout.Controls.Add(New Label With {.Text = "Options:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 2)
        layout.Controls.Add(txtOptions, 1, 2)
        layout.Controls.Add(New Label With {.Text = "Display Order:", .AutoSize = True, .Anchor = AnchorStyles.Left}, 0, 3)
        layout.Controls.Add(numOrder, 1, 3)

        Dim optionsPanel As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .AutoSize = True}
        optionsPanel.Controls.Add(chkRequired)
        optionsPanel.Controls.Add(chkActive)
        layout.Controls.Add(optionsPanel, 1, 4)

        Dim buttonsPanel As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.RightToLeft, .AutoSize = True}
        btnOk = New Button With {.Text = "OK", .Width = 90}
        btnCancel = New Button With {.Text = "Cancel", .Width = 90}
        AddHandler btnOk.Click, AddressOf SaveField
        AddHandler btnCancel.Click, Sub() DialogResult = DialogResult.Cancel
        buttonsPanel.Controls.Add(btnOk)
        buttonsPanel.Controls.Add(btnCancel)
        layout.Controls.Add(buttonsPanel, 1, 5)

        Controls.Add(layout)

        AddHandler cmbType.SelectedIndexChanged, AddressOf UpdateOptionsState

        If existing IsNot Nothing Then
            txtLabel.Text = existing.FieldLabel
            cmbType.SelectedItem = If(String.IsNullOrWhiteSpace(existing.FieldType), "text", existing.FieldType.ToLowerInvariant())
            txtOptions.Text = existing.Options
            chkRequired.Checked = existing.IsRequired
            chkActive.Checked = existing.IsActive
            numOrder.Value = existing.DisplayOrder
        Else
            cmbType.SelectedIndex = 0
            chkActive.Checked = True
            numOrder.Value = 0
        End If

        UpdateOptionsState(Nothing, EventArgs.Empty)
    End Sub

    Private Sub UpdateOptionsState(sender As Object, e As EventArgs)
        Dim isDropdown As Boolean = cmbType.SelectedItem IsNot Nothing AndAlso cmbType.SelectedItem.ToString().ToLowerInvariant() = "dropdown"
        txtOptions.Enabled = isDropdown
        If Not isDropdown Then
            txtOptions.Text = String.Empty
        End If
    End Sub

    Private Sub SaveField(sender As Object, e As EventArgs)
        Dim label As String = txtLabel.Text.Trim()
        If String.IsNullOrWhiteSpace(label) Then
            MessageBox.Show("Label is required.", "Custom Field", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim fieldType As String = If(cmbType.SelectedItem, "text").ToString().Trim().ToLowerInvariant()
        Dim options As String = txtOptions.Text.Trim()
        If fieldType = "dropdown" AndAlso String.IsNullOrWhiteSpace(options) Then
            MessageBox.Show("Options are required for dropdown fields.", "Custom Field", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        FieldDef = New StudentFieldDef With {
            .FieldLabel = label,
            .FieldType = fieldType,
            .Options = options,
            .IsRequired = chkRequired.Checked,
            .IsActive = chkActive.Checked,
            .DisplayOrder = Convert.ToInt32(numOrder.Value)
        }

        DialogResult = DialogResult.OK
    End Sub
End Class
