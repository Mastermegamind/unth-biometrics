<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Register
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim lblPrompt As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Register))
        Me.btnCapture = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.pictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbGender = New System.Windows.Forms.ComboBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSubmit = New System.Windows.Forms.Button()
        Me.label14 = New System.Windows.Forms.Label()
        Me.txtMatricNo = New System.Windows.Forms.TextBox()
        Me.label8 = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cmbDept = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmbFaculty = New System.Windows.Forms.ComboBox()
        Me.btnImportStudents = New System.Windows.Forms.Button()
        Me.btnExportStudents = New System.Windows.Forms.Button()
        Me.btnImportPhotos = New System.Windows.Forms.Button()
        Me.btnUploadPhoto = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        lblPrompt = New System.Windows.Forms.Label()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblPrompt
        '
        lblPrompt.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        lblPrompt.Font = New System.Drawing.Font("Tahoma", 8.25!)
        lblPrompt.Location = New System.Drawing.Point(445, 26)
        lblPrompt.Name = "lblPrompt"
        lblPrompt.Size = New System.Drawing.Size(253, 33)
        lblPrompt.TabIndex = 364
        lblPrompt.Text = "To Capture Image, Click on Start Webcam button then click on the Capture Button."
        '
        'btnCapture
        '
        Me.btnCapture.BackColor = System.Drawing.Color.MediumSeaGreen
        Me.btnCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCapture.Font = New System.Drawing.Font("Palatino Linotype", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnCapture.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnCapture.Location = New System.Drawing.Point(579, 272)
        Me.btnCapture.Name = "btnCapture"
        Me.btnCapture.Size = New System.Drawing.Size(119, 34)
        Me.btnCapture.TabIndex = 362
        Me.btnCapture.Text = "Capture Image"
        Me.btnCapture.UseVisualStyleBackColor = False
        '
        'btnStart
        '
        Me.btnStart.BackColor = System.Drawing.Color.CornflowerBlue
        Me.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStart.Font = New System.Drawing.Font("Palatino Linotype", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnStart.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnStart.Location = New System.Drawing.Point(470, 272)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(97, 34)
        Me.btnStart.TabIndex = 361
        Me.btnStart.Text = "Start Webcam"
        Me.btnStart.UseVisualStyleBackColor = False
        '
        'pictureBox1
        '
        Me.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pictureBox1.Image = Global.BiometricFingerprintsAttendanceSystemVB.My.Resources.Resources.photo
        Me.pictureBox1.Location = New System.Drawing.Point(471, 62)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(240, 240)
        Me.pictureBox1.TabIndex = 363
        Me.pictureBox1.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(18, 285)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(57, 19)
        Me.Label3.TabIndex = 393
        Me.Label3.Text = "Gender:"
        '
        'cmbGender
        '
        Me.cmbGender.FormattingEnabled = True
        Me.cmbGender.Items.AddRange(New Object() {"Male", "Female"})
        Me.cmbGender.Location = New System.Drawing.Point(110, 282)
        Me.cmbGender.Name = "cmbGender"
        Me.cmbGender.Size = New System.Drawing.Size(147, 27)
        Me.cmbGender.TabIndex = 392
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.Color.Maroon
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.Font = New System.Drawing.Font("Palatino Linotype", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnCancel.Location = New System.Drawing.Point(118, 10)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(97, 31)
        Me.btnCancel.TabIndex = 390
        Me.btnCancel.Text = "&Close"
        Me.btnCancel.UseVisualStyleBackColor = False
        '
        'btnSubmit
        '
        Me.btnSubmit.BackColor = System.Drawing.Color.DarkSlateGray
        Me.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSubmit.Font = New System.Drawing.Font("Palatino Linotype", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSubmit.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnSubmit.Location = New System.Drawing.Point(15, 10)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(97, 31)
        Me.btnSubmit.TabIndex = 389
        Me.btnSubmit.Text = "&Submit"
        Me.btnSubmit.UseVisualStyleBackColor = False
        '
        'label14
        '
        Me.label14.AutoSize = True
        Me.label14.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label14.Location = New System.Drawing.Point(17, 120)
        Me.label14.Name = "label14"
        Me.label14.Size = New System.Drawing.Size(56, 19)
        Me.label14.TabIndex = 354
        Me.label14.Text = "Section:"
        '
        'txtMatricNo
        '
        Me.txtMatricNo.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMatricNo.Location = New System.Drawing.Point(110, 36)
        Me.txtMatricNo.Name = "txtMatricNo"
        Me.txtMatricNo.Size = New System.Drawing.Size(235, 26)
        Me.txtMatricNo.TabIndex = 358
        '
        'label8
        '
        Me.label8.AutoSize = True
        Me.label8.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label8.Location = New System.Drawing.Point(17, 39)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(76, 19)
        Me.label8.TabIndex = 357
        Me.label8.Text = "Reg No:"
        '
        'txtName
        '
        Me.txtName.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtName.Location = New System.Drawing.Point(110, 76)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(235, 26)
        Me.txtName.TabIndex = 351
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label2.Location = New System.Drawing.Point(17, 78)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(49, 19)
        Me.label2.TabIndex = 352
        Me.label2.Text = "Name:"
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.btnCancel)
        Me.Panel2.Controls.Add(Me.btnSubmit)
        Me.Panel2.Location = New System.Drawing.Point(102, 417)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(226, 52)
        Me.Panel2.TabIndex = 395
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cmbDept)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.cmbFaculty)
        Me.GroupBox1.Controls.Add(Me.btnImportStudents)
        Me.GroupBox1.Controls.Add(Me.btnExportStudents)
        Me.GroupBox1.Controls.Add(Me.btnImportPhotos)
        Me.GroupBox1.Controls.Add(Me.btnUploadPhoto)
        Me.GroupBox1.Controls.Add(Me.btnCapture)
        Me.GroupBox1.Controls.Add(Me.btnStart)
        Me.GroupBox1.Controls.Add(Me.pictureBox1)
        Me.GroupBox1.Controls.Add(lblPrompt)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.cmbGender)
        Me.GroupBox1.Controls.Add(Me.label14)
        Me.GroupBox1.Controls.Add(Me.txtMatricNo)
        Me.GroupBox1.Controls.Add(Me.label8)
        Me.GroupBox1.Controls.Add(Me.txtName)
        Me.GroupBox1.Controls.Add(Me.label2)
        Me.GroupBox1.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(37, 26)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(751, 368)
        Me.GroupBox1.TabIndex = 394
        Me.GroupBox1.TabStop = False
        '
        'cmbDept
        '
        Me.cmbDept.FormattingEnabled = True
        Me.cmbDept.Location = New System.Drawing.Point(110, 161)
        Me.cmbDept.Name = "cmbDept"
        Me.cmbDept.Size = New System.Drawing.Size(235, 27)
        Me.cmbDept.TabIndex = 396
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(17, 163)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 19)
        Me.Label1.TabIndex = 395
        Me.Label1.Text = "Year:"
        '
        'cmbFaculty
        '
        Me.cmbFaculty.FormattingEnabled = True
        Me.cmbFaculty.Location = New System.Drawing.Point(110, 118)
        Me.cmbFaculty.Name = "cmbFaculty"
        Me.cmbFaculty.Size = New System.Drawing.Size(235, 27)
        Me.cmbFaculty.TabIndex = 394

        'btnImportStudents
        '
        Me.btnImportStudents.BackColor = System.Drawing.Color.SteelBlue
        Me.btnImportStudents.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnImportStudents.Font = New System.Drawing.Font("Palatino Linotype", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnImportStudents.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnImportStudents.Location = New System.Drawing.Point(21, 318)
        Me.btnImportStudents.Name = "btnImportStudents"
        Me.btnImportStudents.Size = New System.Drawing.Size(120, 31)
        Me.btnImportStudents.TabIndex = 401
        Me.btnImportStudents.Text = "Import CSV/XLSX"
        Me.btnImportStudents.UseVisualStyleBackColor = False

        'btnExportStudents
        '
        Me.btnExportStudents.BackColor = System.Drawing.Color.DarkSlateGray
        Me.btnExportStudents.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExportStudents.Font = New System.Drawing.Font("Palatino Linotype", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnExportStudents.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnExportStudents.Location = New System.Drawing.Point(147, 318)
        Me.btnExportStudents.Name = "btnExportStudents"
        Me.btnExportStudents.Size = New System.Drawing.Size(120, 31)
        Me.btnExportStudents.TabIndex = 402
        Me.btnExportStudents.Text = "Export Students"
        Me.btnExportStudents.UseVisualStyleBackColor = False

        'btnImportPhotos
        '
        Me.btnImportPhotos.BackColor = System.Drawing.Color.OliveDrab
        Me.btnImportPhotos.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnImportPhotos.Font = New System.Drawing.Font("Palatino Linotype", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnImportPhotos.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnImportPhotos.Location = New System.Drawing.Point(273, 318)
        Me.btnImportPhotos.Name = "btnImportPhotos"
        Me.btnImportPhotos.Size = New System.Drawing.Size(120, 31)
        Me.btnImportPhotos.TabIndex = 403
        Me.btnImportPhotos.Text = "Import Photos"
        Me.btnImportPhotos.UseVisualStyleBackColor = False
        '
        'btnUploadPhoto
        '
        Me.btnUploadPhoto.BackColor = System.Drawing.Color.DarkSlateGray
        Me.btnUploadPhoto.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnUploadPhoto.Font = New System.Drawing.Font("Palatino Linotype", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUploadPhoto.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.btnUploadPhoto.Location = New System.Drawing.Point(470, 318)
        Me.btnUploadPhoto.Name = "btnUploadPhoto"
        Me.btnUploadPhoto.Size = New System.Drawing.Size(227, 31)
        Me.btnUploadPhoto.TabIndex = 391
        Me.btnUploadPhoto.Text = "Browse Photo"
        Me.btnUploadPhoto.UseVisualStyleBackColor = False
        '
        'Timer1
        '
        '
        'Register
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(800, 488)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Register"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Registration Form"
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents btnCapture As Button
    Private WithEvents btnStart As Button
    Private WithEvents pictureBox1 As PictureBox
    Private WithEvents Label3 As Label
    Friend WithEvents cmbGender As ComboBox
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSubmit As Button
    Private WithEvents label14 As Label
    Private WithEvents txtMatricNo As TextBox
    Private WithEvents label8 As Label
    Private WithEvents txtName As TextBox
    Private WithEvents label2 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnUploadPhoto As Button
    Friend WithEvents Timer1 As Timer
    Friend WithEvents cmbDept As ComboBox
    Private WithEvents Label1 As Label
    Friend WithEvents cmbFaculty As ComboBox
    Friend WithEvents btnImportStudents As Button
    Friend WithEvents btnExportStudents As Button
    Friend WithEvents btnImportPhotos As Button
End Class
