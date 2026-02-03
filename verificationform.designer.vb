<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class verificationform
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim lblPrompt As System.Windows.Forms.Label
        Dim CloseButton As System.Windows.Forms.Button
        Dim gbReturnValues As System.Windows.Forms.GroupBox
        Dim lblFalseAcceptRate As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(verificationform))
        Me.FalseAcceptRate = New System.Windows.Forms.TextBox()
        Me.VerificationControl = New DPFP.Gui.Verification.VerificationControl()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtSessionName = New System.Windows.Forms.TextBox()
        Me.LabelSession = New System.Windows.Forms.Label()
        Me.txtCourseCode = New System.Windows.Forms.TextBox()
        Me.LabelCourseCode = New System.Windows.Forms.Label()
        Me.txtCourseName = New System.Windows.Forms.TextBox()
        Me.LabelCourseName = New System.Windows.Forms.Label()
        Me.txtDayName = New System.Windows.Forms.TextBox()
        Me.LabelDayName = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.DateClockIn = New System.Windows.Forms.DateTimePicker()
        lblPrompt = New System.Windows.Forms.Label()
        CloseButton = New System.Windows.Forms.Button()
        gbReturnValues = New System.Windows.Forms.GroupBox()
        lblFalseAcceptRate = New System.Windows.Forms.Label()
        gbReturnValues.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblPrompt
        '
        lblPrompt.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        lblPrompt.Font = New System.Drawing.Font("Tahoma", 8.25!)
        lblPrompt.Location = New System.Drawing.Point(30, 9)
        lblPrompt.Name = "lblPrompt"
        lblPrompt.Size = New System.Drawing.Size(235, 36)
        lblPrompt.TabIndex = 8
        lblPrompt.Text = "To Clock In, touch fingerprint reader with any enrolled finger."
        '
        'CloseButton
        '
        CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        CloseButton.BackColor = System.Drawing.Color.Crimson
        CloseButton.DialogResult = System.Windows.Forms.DialogResult.OK
        CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        CloseButton.ForeColor = System.Drawing.Color.White
        CloseButton.Location = New System.Drawing.Point(615, -66)
        CloseButton.Name = "CloseButton"
        CloseButton.Size = New System.Drawing.Size(101, 33)
        CloseButton.TabIndex = 7
        CloseButton.Text = "Close"
        CloseButton.UseVisualStyleBackColor = False
        AddHandler CloseButton.Click, AddressOf Me.CloseButton_Click
        '
        'gbReturnValues
        '
        gbReturnValues.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        gbReturnValues.BackColor = System.Drawing.Color.Transparent
        gbReturnValues.Controls.Add(Me.FalseAcceptRate)
        gbReturnValues.Controls.Add(lblFalseAcceptRate)
        gbReturnValues.ForeColor = System.Drawing.Color.Black
        gbReturnValues.Location = New System.Drawing.Point(1043, 64)
        gbReturnValues.Name = "gbReturnValues"
        gbReturnValues.Size = New System.Drawing.Size(2, 87)
        gbReturnValues.TabIndex = 10
        gbReturnValues.TabStop = False
        gbReturnValues.Text = "Return values"
        '
        'FalseAcceptRate
        '
        Me.FalseAcceptRate.Font = New System.Drawing.Font("Tahoma", 10.0!)
        Me.FalseAcceptRate.Location = New System.Drawing.Point(6, 41)
        Me.FalseAcceptRate.Name = "FalseAcceptRate"
        Me.FalseAcceptRate.ReadOnly = True
        Me.FalseAcceptRate.Size = New System.Drawing.Size(234, 24)
        Me.FalseAcceptRate.TabIndex = 2
        '
        'lblFalseAcceptRate
        '
        lblFalseAcceptRate.ForeColor = System.Drawing.Color.Black
        lblFalseAcceptRate.Location = New System.Drawing.Point(44, 18)
        lblFalseAcceptRate.Name = "lblFalseAcceptRate"
        lblFalseAcceptRate.Size = New System.Drawing.Size(120, 20)
        lblFalseAcceptRate.TabIndex = 1
        lblFalseAcceptRate.Text = "False accept rate"
        lblFalseAcceptRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'VerificationControl
        '
        Me.VerificationControl.Active = True
        Me.VerificationControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.VerificationControl.Location = New System.Drawing.Point(366, 23)
        Me.VerificationControl.Name = "VerificationControl"
        Me.VerificationControl.ReaderSerialNumber = "00000000-0000-0000-0000-000000000000"
        Me.VerificationControl.Size = New System.Drawing.Size(48, 47)
        Me.VerificationControl.TabIndex = 9
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.Label1.Location = New System.Drawing.Point(13, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(63, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Reg No:"
        '
        'TextBox1
        '
        Me.TextBox1.Font = New System.Drawing.Font("Tahoma", 10.0!)
        Me.TextBox1.Location = New System.Drawing.Point(96, 23)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(265, 24)
        Me.TextBox1.TabIndex = 13
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtDayName)
        Me.GroupBox1.Controls.Add(Me.LabelDayName)
        Me.GroupBox1.Controls.Add(Me.txtCourseName)
        Me.GroupBox1.Controls.Add(Me.LabelCourseName)
        Me.GroupBox1.Controls.Add(Me.txtCourseCode)
        Me.GroupBox1.Controls.Add(Me.LabelCourseCode)
        Me.GroupBox1.Controls.Add(Me.txtSessionName)
        Me.GroupBox1.Controls.Add(Me.LabelSession)
        Me.GroupBox1.Controls.Add(Me.txtName)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.VerificationControl)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.TextBox1)
        Me.GroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.GroupBox1.Location = New System.Drawing.Point(33, 41)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(429, 210)
        Me.GroupBox1.TabIndex = 15
        Me.GroupBox1.TabStop = False
        '
        'txtName
        '
        Me.txtName.Font = New System.Drawing.Font("Tahoma", 10.0!)
        Me.txtName.Location = New System.Drawing.Point(96, 53)
        Me.txtName.Name = "txtName"
        Me.txtName.ReadOnly = True
        Me.txtName.Size = New System.Drawing.Size(265, 24)
        Me.txtName.TabIndex = 15
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.Label3.Location = New System.Drawing.Point(13, 58)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Name:"

        'LabelSession
        '
        Me.LabelSession.AutoSize = True
        Me.LabelSession.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.LabelSession.Location = New System.Drawing.Point(13, 86)
        Me.LabelSession.Name = "LabelSession"
        Me.LabelSession.Size = New System.Drawing.Size(48, 13)
        Me.LabelSession.TabIndex = 16
        Me.LabelSession.Text = "Session:"

        'txtSessionName
        '
        Me.txtSessionName.Font = New System.Drawing.Font("Tahoma", 10.0!)
        Me.txtSessionName.Location = New System.Drawing.Point(96, 83)
        Me.txtSessionName.Name = "txtSessionName"
        Me.txtSessionName.Size = New System.Drawing.Size(265, 24)
        Me.txtSessionName.TabIndex = 17

        'LabelCourseCode
        '
        Me.LabelCourseCode.AutoSize = True
        Me.LabelCourseCode.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.LabelCourseCode.Location = New System.Drawing.Point(13, 116)
        Me.LabelCourseCode.Name = "LabelCourseCode"
        Me.LabelCourseCode.Size = New System.Drawing.Size(73, 13)
        Me.LabelCourseCode.TabIndex = 18
        Me.LabelCourseCode.Text = "Course Code:"

        'txtCourseCode
        '
        Me.txtCourseCode.Font = New System.Drawing.Font("Tahoma", 10.0!)
        Me.txtCourseCode.Location = New System.Drawing.Point(96, 113)
        Me.txtCourseCode.Name = "txtCourseCode"
        Me.txtCourseCode.Size = New System.Drawing.Size(265, 24)
        Me.txtCourseCode.TabIndex = 19

        'LabelCourseName
        '
        Me.LabelCourseName.AutoSize = True
        Me.LabelCourseName.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.LabelCourseName.Location = New System.Drawing.Point(13, 146)
        Me.LabelCourseName.Name = "LabelCourseName"
        Me.LabelCourseName.Size = New System.Drawing.Size(74, 13)
        Me.LabelCourseName.TabIndex = 20
        Me.LabelCourseName.Text = "Course Name:"

        'txtCourseName
        '
        Me.txtCourseName.Font = New System.Drawing.Font("Tahoma", 10.0!)
        Me.txtCourseName.Location = New System.Drawing.Point(96, 143)
        Me.txtCourseName.Name = "txtCourseName"
        Me.txtCourseName.Size = New System.Drawing.Size(265, 24)
        Me.txtCourseName.TabIndex = 21

        'LabelDayName
        '
        Me.LabelDayName.AutoSize = True
        Me.LabelDayName.Font = New System.Drawing.Font("Tahoma", 8.0!)
        Me.LabelDayName.Location = New System.Drawing.Point(13, 176)
        Me.LabelDayName.Name = "LabelDayName"
        Me.LabelDayName.Size = New System.Drawing.Size(62, 13)
        Me.LabelDayName.TabIndex = 22
        Me.LabelDayName.Text = "Day Name:"

        'txtDayName
        '
        Me.txtDayName.Font = New System.Drawing.Font("Tahoma", 10.0!)
        Me.txtDayName.Location = New System.Drawing.Point(96, 173)
        Me.txtDayName.Name = "txtDayName"
        Me.txtDayName.Size = New System.Drawing.Size(265, 24)
        Me.txtDayName.TabIndex = 23
        '
        'StatusStrip1
        '
        Me.StatusStrip1.BackColor = System.Drawing.Color.FromArgb(CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 500)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(820, 22)
        Me.StatusStrip1.TabIndex = 22
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(59, 479)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(13, 13)
        Me.Label4.TabIndex = 23
        Me.Label4.Text = "0"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(30, 479)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(34, 13)
        Me.Label5.TabIndex = 24
        Me.Label5.Text = "Total:"
        '
        'BackgroundWorker1
        '
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.Color.White
        Me.PictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox2.Location = New System.Drawing.Point(1176, 84)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(61, 58)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox2.TabIndex = 14
        Me.PictureBox2.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PictureBox1.Image = Global.BiometricFingerprintsAttendanceSystemVB.My.Resources.Resources.photo
        Me.PictureBox1.Location = New System.Drawing.Point(485, 9)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(203, 168)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 11
        Me.PictureBox1.TabStop = False
        '
        'ListView1
        '
        Me.ListView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6, Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader9})
        Me.ListView1.Font = New System.Drawing.Font("Tahoma", 9.0!)
        Me.ListView1.FullRowSelect = True
        Me.ListView1.GridLines = True
        Me.ListView1.HideSelection = False
        Me.ListView1.Location = New System.Drawing.Point(33, 260)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(775, 210)
        Me.ListView1.TabIndex = 29
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Reg No"
        Me.ColumnHeader1.Width = 90
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Name"
        Me.ColumnHeader2.Width = 200
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Date"
        Me.ColumnHeader4.Width = 120
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Day Name"
        Me.ColumnHeader5.Width = 120
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Session"
        Me.ColumnHeader6.Width = 120
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "Course Code"
        Me.ColumnHeader7.Width = 120

        'ColumnHeader8
        '
        Me.ColumnHeader8.Text = "Time-In"
        Me.ColumnHeader8.Width = 120

        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "Time-Out"
        Me.ColumnHeader9.Width = 120
        '
        'DateClockIn
        '
        Me.DateClockIn.CustomFormat = "dd/MM/yyyy"
        Me.DateClockIn.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DateClockIn.Location = New System.Drawing.Point(866, 124)
        Me.DateClockIn.Name = "DateClockIn"
        Me.DateClockIn.Size = New System.Drawing.Size(162, 20)
        Me.DateClockIn.TabIndex = 30
        '
        'verificationform
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(820, 522)
        Me.Controls.Add(Me.DateClockIn)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(gbReturnValues)
        Me.Controls.Add(lblPrompt)
        Me.Controls.Add(CloseButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "verificationform"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Clock In"
        gbReturnValues.ResumeLayout(False)
        gbReturnValues.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents VerificationControl As DPFP.Gui.Verification.VerificationControl
    Private WithEvents FalseAcceptRate As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents txtName As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtSessionName As TextBox
    Friend WithEvents LabelSession As Label
    Friend WithEvents txtCourseCode As TextBox
    Friend WithEvents LabelCourseCode As Label
    Friend WithEvents txtCourseName As TextBox
    Friend WithEvents LabelCourseName As Label
    Friend WithEvents txtDayName As TextBox
    Friend WithEvents LabelDayName As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents ListView1 As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents ColumnHeader6 As ColumnHeader
    Friend WithEvents ColumnHeader7 As ColumnHeader
    Friend WithEvents ColumnHeader8 As ColumnHeader
    Friend WithEvents ColumnHeader9 As ColumnHeader
    Friend WithEvents DateClockIn As DateTimePicker
End Class
