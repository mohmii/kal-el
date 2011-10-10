<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddToOppositeMsg
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
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Delete_Button = New System.Windows.Forms.Button
        Me.Keep_Button = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(225, 79)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 3
        Me.Cancel_Button.Text = "キャンセル"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(309, 17)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "元の形状を元の面に残しますか、または削除しますか？"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Delete_Button
        '
        Me.Delete_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Delete_Button.Location = New System.Drawing.Point(131, 79)
        Me.Delete_Button.Name = "Delete_Button"
        Me.Delete_Button.Size = New System.Drawing.Size(67, 23)
        Me.Delete_Button.TabIndex = 2
        Me.Delete_Button.Text = "削除"
        '
        'Keep_Button
        '
        Me.Keep_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Keep_Button.Location = New System.Drawing.Point(34, 79)
        Me.Keep_Button.Name = "Keep_Button"
        Me.Keep_Button.Size = New System.Drawing.Size(67, 23)
        Me.Keep_Button.TabIndex = 2
        Me.Keep_Button.Text = "残す"
        '
        'AddToOppositeMsg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(325, 114)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Delete_Button)
        Me.Controls.Add(Me.Keep_Button)
        Me.Controls.Add(Me.Cancel_Button)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AddToOppositeMsg"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AcadFR - 反対側の面に追加"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Delete_Button As System.Windows.Forms.Button
    Friend WithEvents Keep_Button As System.Windows.Forms.Button

End Class
