<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class setView
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.RadioButton1 = New System.Windows.Forms.RadioButton
        Me.viewtype = New System.Windows.Forms.ComboBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.CheckBoxDoubleMirror = New System.Windows.Forms.CheckBox
        Me.CheckBoxHid = New System.Windows.Forms.CheckBox
        Me.CheckBoxVis = New System.Windows.Forms.CheckBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.CheckBoxPoly = New System.Windows.Forms.CheckBox
        Me.CheckBoxMill = New System.Windows.Forms.CheckBox
        Me.CheckBoxHole = New System.Windows.Forms.CheckBox
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(88, 201)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "次へ >>"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(166, 201)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "キャンセル"
        '
        'RadioButton1
        '
        Me.RadioButton1.AutoSize = True
        Me.RadioButton1.Checked = True
        Me.RadioButton1.Location = New System.Drawing.Point(6, 19)
        Me.RadioButton1.Name = "RadioButton1"
        Me.RadioButton1.Size = New System.Drawing.Size(95, 17)
        Me.RadioButton1.TabIndex = 1
        Me.RadioButton1.TabStop = True
        Me.RadioButton1.Text = "設計面の名前"
        Me.RadioButton1.UseVisualStyleBackColor = True
        '
        'viewtype
        '
        Me.viewtype.FormattingEnabled = True
        Me.viewtype.Items.AddRange(New Object() {"TOP", "BOTTOM", "LEFT", "RIGHT", "FRONT", "BACK"})
        Me.viewtype.Location = New System.Drawing.Point(107, 18)
        Me.viewtype.Name = "viewtype"
        Me.viewtype.Size = New System.Drawing.Size(97, 21)
        Me.viewtype.TabIndex = 3
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CheckBoxDoubleMirror)
        Me.GroupBox1.Controls.Add(Me.CheckBoxHid)
        Me.GroupBox1.Controls.Add(Me.CheckBoxVis)
        Me.GroupBox1.Controls.Add(Me.RadioButton1)
        Me.GroupBox1.Controls.Add(Me.viewtype)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(221, 88)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "設計面"
        '
        'CheckBoxDoubleMirror
        '
        Me.CheckBoxDoubleMirror.AutoSize = True
        Me.CheckBoxDoubleMirror.Location = New System.Drawing.Point(111, 45)
        Me.CheckBoxDoubleMirror.Name = "CheckBoxDoubleMirror"
        Me.CheckBoxDoubleMirror.Size = New System.Drawing.Size(96, 17)
        Me.CheckBoxDoubleMirror.TabIndex = 6
        Me.CheckBoxDoubleMirror.Text = "2 回ミラー反転"
        Me.CheckBoxDoubleMirror.UseVisualStyleBackColor = True
        '
        'CheckBoxHid
        '
        Me.CheckBoxHid.AutoSize = True
        Me.CheckBoxHid.Checked = True
        Me.CheckBoxHid.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxHid.Location = New System.Drawing.Point(14, 68)
        Me.CheckBoxHid.Name = "CheckBoxHid"
        Me.CheckBoxHid.Size = New System.Drawing.Size(61, 17)
        Me.CheckBoxHid.TabIndex = 6
        Me.CheckBoxHid.Text = "隠れ面"
        Me.CheckBoxHid.UseVisualStyleBackColor = True
        '
        'CheckBoxVis
        '
        Me.CheckBoxVis.AutoSize = True
        Me.CheckBoxVis.Checked = True
        Me.CheckBoxVis.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxVis.Enabled = False
        Me.CheckBoxVis.Location = New System.Drawing.Point(14, 45)
        Me.CheckBoxVis.Name = "CheckBoxVis"
        Me.CheckBoxVis.Size = New System.Drawing.Size(66, 17)
        Me.CheckBoxVis.TabIndex = 5
        Me.CheckBoxVis.Text = "おもて面"
        Me.CheckBoxVis.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.CheckBoxPoly)
        Me.GroupBox2.Controls.Add(Me.CheckBoxMill)
        Me.GroupBox2.Controls.Add(Me.CheckBoxHole)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 106)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(223, 89)
        Me.GroupBox2.TabIndex = 5
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "形状認識"
        '
        'CheckBoxPoly
        '
        Me.CheckBoxPoly.AutoSize = True
        Me.CheckBoxPoly.Checked = True
        Me.CheckBoxPoly.CheckState = System.Windows.Forms.CheckState.Checked
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
        Me.CheckBoxMill.Checked = True
        Me.CheckBoxMill.CheckState = System.Windows.Forms.CheckState.Checked
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
        Me.CheckBoxHole.Checked = True
        Me.CheckBoxHole.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxHole.Location = New System.Drawing.Point(14, 19)
        Me.CheckBoxHole.Name = "CheckBoxHole"
        Me.CheckBoxHole.Size = New System.Drawing.Size(62, 17)
        Me.CheckBoxHole.TabIndex = 0
        Me.CheckBoxHole.Text = "穴形状"
        Me.CheckBoxHole.UseVisualStyleBackColor = True
        '
        'setView
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(247, 229)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.OK_Button)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "setView"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "要素選択"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
    Friend WithEvents viewtype As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBoxHid As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxVis As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents CheckBoxPoly As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxMill As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxHole As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBoxDoubleMirror As System.Windows.Forms.CheckBox

End Class
