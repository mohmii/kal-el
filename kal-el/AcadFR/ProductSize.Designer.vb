<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProductSize
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
        Me.Length1 = New System.Windows.Forms.NumericUpDown
        Me.Label1 = New System.Windows.Forms.Label
        Me.Width1 = New System.Windows.Forms.NumericUpDown
        Me.Height1 = New System.Windows.Forms.NumericUpDown
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Button1 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        CType(Me.Length1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Width1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Height1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Length1
        '
        Me.Length1.DecimalPlaces = 3
        Me.Length1.Location = New System.Drawing.Point(109, 29)
        Me.Length1.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.Length1.Name = "Length1"
        Me.Length1.Size = New System.Drawing.Size(99, 20)
        Me.Length1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(228, 17)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "次へ進む前に、正確な寸法を入力してください"
        Me.Label1.UseCompatibleTextRendering = True
        '
        'Width1
        '
        Me.Width1.DecimalPlaces = 3
        Me.Width1.Location = New System.Drawing.Point(109, 55)
        Me.Width1.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.Width1.Name = "Width1"
        Me.Width1.Size = New System.Drawing.Size(99, 20)
        Me.Width1.TabIndex = 2
        '
        'Height1
        '
        Me.Height1.DecimalPlaces = 3
        Me.Height1.Location = New System.Drawing.Point(109, 81)
        Me.Height1.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.Height1.Name = "Height1"
        Me.Height1.Size = New System.Drawing.Size(99, 20)
        Me.Height1.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(47, 31)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(43, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "長さ (X)"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(47, 57)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(35, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "幅 (Y)"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(47, 83)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "高さ (Z)"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(50, 118)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 7
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(131, 118)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 8
        Me.Button2.Text = "キャンセル"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'ProductSize
        '
        Me.AcceptButton = Me.Button1
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Button2
        Me.ClientSize = New System.Drawing.Size(252, 153)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Height1)
        Me.Controls.Add(Me.Width1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Length1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "ProductSize"
        Me.Text = "製品寸法"
        CType(Me.Length1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Width1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Height1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Length1 As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Width1 As System.Windows.Forms.NumericUpDown
    Friend WithEvents Height1 As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
End Class
