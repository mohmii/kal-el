<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddManualSurface
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.CheckBoxPoly = New System.Windows.Forms.CheckBox
        Me.CheckBoxMill = New System.Windows.Forms.CheckBox
        Me.CheckBoxHole = New System.Windows.Forms.CheckBox
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.OK_Button = New System.Windows.Forms.Button
        Me.surfacetype = New System.Windows.Forms.ComboBox
        Me.RadioButton1 = New System.Windows.Forms.RadioButton
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CheckBoxPoly)
        Me.GroupBox1.Controls.Add(Me.CheckBoxMill)
        Me.GroupBox1.Controls.Add(Me.CheckBoxHole)
        Me.GroupBox1.Location = New System.Drawing.Point(16, 57)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(210, 89)
        Me.GroupBox1.TabIndex = 6
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "形状認識"
        '
        'CheckBoxPoly
        '
        Me.CheckBoxPoly.AutoSize = True
        Me.CheckBoxPoly.Location = New System.Drawing.Point(14, 65)
        Me.CheckBoxPoly.Name = "CheckBoxPoly"
        Me.CheckBoxPoly.Size = New System.Drawing.Size(69, 17)
        Me.CheckBoxPoly.TabIndex = 2
        Me.CheckBoxPoly.Text = "ポリライン"
        Me.CheckBoxPoly.UseVisualStyleBackColor = True
        '
        'CheckBoxMill
        '
        Me.CheckBoxMill.AutoSize = True
        Me.CheckBoxMill.Location = New System.Drawing.Point(14, 42)
        Me.CheckBoxMill.Name = "CheckBoxMill"
        Me.CheckBoxMill.Size = New System.Drawing.Size(82, 17)
        Me.CheckBoxMill.TabIndex = 1
        Me.CheckBoxMill.Text = "ミリング形状"
        Me.CheckBoxMill.UseVisualStyleBackColor = True
        '
        'CheckBoxHole
        '
        Me.CheckBoxHole.AutoSize = True
        Me.CheckBoxHole.Location = New System.Drawing.Point(14, 19)
        Me.CheckBoxHole.Name = "CheckBoxHole"
        Me.CheckBoxHole.Size = New System.Drawing.Size(62, 17)
        Me.CheckBoxHole.TabIndex = 0
        Me.CheckBoxHole.Text = "穴形状"
        Me.CheckBoxHole.UseVisualStyleBackColor = True
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(157, 156)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 8
        Me.Cancel_Button.Text = "キャンセル"
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Enabled = False
        Me.OK_Button.Location = New System.Drawing.Point(79, 156)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 7
        Me.OK_Button.Text = "次へ >>"
        '
        'surfacetype
        '
        Me.surfacetype.FormattingEnabled = True
        Me.surfacetype.Location = New System.Drawing.Point(126, 21)
        Me.surfacetype.Name = "surfacetype"
        Me.surfacetype.Size = New System.Drawing.Size(100, 21)
        Me.surfacetype.TabIndex = 9
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Checked = True
        Me.RadioButton1.Location = New System.Drawing.Point(16, 21)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(95, 17)
        Me.RadioButton1.TabIndex = 10
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "設計面の名前"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'AddManualSurface
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(236, 196)
        Me.Controls.Add(Me.RadioButton1)
        Me.Controls.Add(Me.surfacetype)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "AddManualSurface"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "形状追加認識"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBoxPoly As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxMill As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxHole As System.Windows.Forms.CheckBox
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents surfacetype As System.Windows.Forms.ComboBox
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
End Class
