﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppPreferencesForm
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.Button1 = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.UnderholeDiaTol = New System.Windows.Forms.NumericUpDown
        Me.HoleDiaTol = New System.Windows.Forms.NumericUpDown
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Schematic = New System.Windows.Forms.NumericUpDown
        Me.Label3 = New System.Windows.Forms.Label
        Me.Tolerance = New System.Windows.Forms.NumericUpDown
        Me.Label2 = New System.Windows.Forms.Label
        Me.Save = New System.Windows.Forms.Button
        Me.Cancel = New System.Windows.Forms.Button
        Me.Directory = New System.Windows.Forms.TextBox
        Me.PreProcess = New System.Windows.Forms.CheckBox
        Me.AutoRegLine = New System.Windows.Forms.CheckBox
        Me.AutoRegScheme = New System.Windows.Forms.CheckBox
        Me.MultiAnalysis = New System.Windows.Forms.CheckBox
        Me.RemoveEntities = New System.Windows.Forms.CheckBox
        Me.GroupBox1.SuspendLayout()
        CType(Me.UnderholeDiaTol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.HoleDiaTol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Schematic, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Tolerance, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "設定デイレクトリ:"
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.Location = New System.Drawing.Point(229, 23)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(24, 23)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "..."
        Me.Button1.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.UnderholeDiaTol)
        Me.GroupBox1.Controls.Add(Me.HoleDiaTol)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Schematic)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Tolerance)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 168)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(248, 96)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "許容値"
        '
        'UnderholeDiaTol
        '
        Me.UnderholeDiaTol.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UnderholeDiaTol.DecimalPlaces = 2
        Me.UnderholeDiaTol.Enabled = False
        Me.UnderholeDiaTol.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.UnderholeDiaTol.Location = New System.Drawing.Point(191, 71)
        Me.UnderholeDiaTol.Name = "UnderholeDiaTol"
        Me.UnderholeDiaTol.Size = New System.Drawing.Size(51, 20)
        Me.UnderholeDiaTol.TabIndex = 3
        Me.UnderholeDiaTol.Value = New Decimal(New Integer() {5, 0, 0, 65536})
        '
        'HoleDiaTol
        '
        Me.HoleDiaTol.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.HoleDiaTol.DecimalPlaces = 2
        Me.HoleDiaTol.Enabled = False
        Me.HoleDiaTol.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.HoleDiaTol.Location = New System.Drawing.Point(191, 52)
        Me.HoleDiaTol.Name = "HoleDiaTol"
        Me.HoleDiaTol.Size = New System.Drawing.Size(51, 20)
        Me.HoleDiaTol.TabIndex = 3
        Me.HoleDiaTol.Value = New Decimal(New Integer() {1, 0, 0, 65536})
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 73)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(106, 13)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "タップ下穴径許容値:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 54)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(82, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "タップ径許容値:"
        '
        'Schematic
        '
        Me.Schematic.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Schematic.DecimalPlaces = 2
        Me.Schematic.Enabled = False
        Me.Schematic.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.Schematic.Location = New System.Drawing.Point(191, 33)
        Me.Schematic.Name = "Schematic"
        Me.Schematic.Size = New System.Drawing.Size(51, 20)
        Me.Schematic.TabIndex = 3
        Me.Schematic.Value = New Decimal(New Integer() {1, 0, 0, 65536})
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 35)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(94, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "単独穴径許容値:"
        '
        'Tolerance
        '
        Me.Tolerance.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tolerance.DecimalPlaces = 2
        Me.Tolerance.Enabled = False
        Me.Tolerance.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.Tolerance.Location = New System.Drawing.Point(191, 14)
        Me.Tolerance.Name = "Tolerance"
        Me.Tolerance.Size = New System.Drawing.Size(51, 20)
        Me.Tolerance.TabIndex = 1
        Me.Tolerance.Value = New Decimal(New Integer() {1, 0, 0, 65536})
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 16)
        Me.Label2.Margin = New System.Windows.Forms.Padding(3, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(151, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "接続、接線、直角および平行:"
        '
        'Save
        '
        Me.Save.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Save.Location = New System.Drawing.Point(99, 278)
        Me.Save.Name = "Save"
        Me.Save.Size = New System.Drawing.Size(74, 23)
        Me.Save.TabIndex = 4
        Me.Save.Text = "保存"
        Me.Save.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel.Location = New System.Drawing.Point(179, 278)
        Me.Cancel.Name = "Cancel"
        Me.Cancel.Size = New System.Drawing.Size(76, 23)
        Me.Cancel.TabIndex = 5
        Me.Cancel.Text = "キャンセル"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Directory
        '
        Me.Directory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Directory.Location = New System.Drawing.Point(12, 25)
        Me.Directory.Name = "Directory"
        Me.Directory.Size = New System.Drawing.Size(211, 20)
        Me.Directory.TabIndex = 6
        '
        'PreProcess
        '
        Me.PreProcess.AutoSize = True
        Me.PreProcess.Checked = True
        Me.PreProcess.CheckState = System.Windows.Forms.CheckState.Checked
        Me.PreProcess.Location = New System.Drawing.Point(12, 145)
        Me.PreProcess.Name = "PreProcess"
        Me.PreProcess.Size = New System.Drawing.Size(108, 17)
        Me.PreProcess.TabIndex = 8
        Me.PreProcess.Text = "図面の事前処理"
        Me.PreProcess.UseVisualStyleBackColor = True
        '
        'AutoRegLine
        '
        Me.AutoRegLine.AutoSize = True
        Me.AutoRegLine.Checked = True
        Me.AutoRegLine.CheckState = System.Windows.Forms.CheckState.Checked
        Me.AutoRegLine.Location = New System.Drawing.Point(12, 52)
        Me.AutoRegLine.Name = "AutoRegLine"
        Me.AutoRegLine.Size = New System.Drawing.Size(154, 17)
        Me.AutoRegLine.TabIndex = 9
        Me.AutoRegLine.Text = "未認定の線素の自動登録"
        Me.AutoRegLine.UseVisualStyleBackColor = True
        '
        'AutoRegScheme
        '
        Me.AutoRegScheme.AutoSize = True
        Me.AutoRegScheme.Checked = True
        Me.AutoRegScheme.CheckState = System.Windows.Forms.CheckState.Checked
        Me.AutoRegScheme.Location = New System.Drawing.Point(12, 75)
        Me.AutoRegScheme.Name = "AutoRegScheme"
        Me.AutoRegScheme.Size = New System.Drawing.Size(178, 17)
        Me.AutoRegScheme.TabIndex = 10
        Me.AutoRegScheme.Text = "未認定の形状記号の自動登録"
        Me.AutoRegScheme.UseVisualStyleBackColor = True
        '
        'MultiAnalysis
        '
        Me.MultiAnalysis.AutoSize = True
        Me.MultiAnalysis.Location = New System.Drawing.Point(12, 98)
        Me.MultiAnalysis.Name = "MultiAnalysis"
        Me.MultiAnalysis.Size = New System.Drawing.Size(193, 17)
        Me.MultiAnalysis.TabIndex = 11
        Me.MultiAnalysis.Text = "複数投影図によるミリング形状認識"
        Me.MultiAnalysis.UseVisualStyleBackColor = True
        '
        'RemoveEntities
        '
        Me.RemoveEntities.AutoSize = True
        Me.RemoveEntities.Checked = True
        Me.RemoveEntities.CheckState = System.Windows.Forms.CheckState.Checked
        Me.RemoveEntities.Location = New System.Drawing.Point(12, 121)
        Me.RemoveEntities.Name = "RemoveEntities"
        Me.RemoveEntities.Size = New System.Drawing.Size(108, 17)
        Me.RemoveEntities.TabIndex = 8
        Me.RemoveEntities.Text = "不要線素の削除"
        Me.RemoveEntities.UseVisualStyleBackColor = True
        '
        'AppPreferencesForm
        '
        Me.AcceptButton = Me.Save
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel
        Me.ClientSize = New System.Drawing.Size(265, 317)
        Me.Controls.Add(Me.MultiAnalysis)
        Me.Controls.Add(Me.AutoRegScheme)
        Me.Controls.Add(Me.AutoRegLine)
        Me.Controls.Add(Me.RemoveEntities)
        Me.Controls.Add(Me.PreProcess)
        Me.Controls.Add(Me.Directory)
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Save)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label1)
        Me.Name = "AppPreferencesForm"
        Me.Text = "FRの設定"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.UnderholeDiaTol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.HoleDiaTol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Schematic, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Tolerance, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Tolerance As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Save As System.Windows.Forms.Button
    Friend WithEvents Cancel As System.Windows.Forms.Button
    Friend WithEvents Directory As System.Windows.Forms.TextBox
    Friend WithEvents Schematic As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents PreProcess As System.Windows.Forms.CheckBox
    Friend WithEvents AutoRegLine As System.Windows.Forms.CheckBox
    Friend WithEvents AutoRegScheme As System.Windows.Forms.CheckBox
    Friend WithEvents MultiAnalysis As System.Windows.Forms.CheckBox
    Friend WithEvents RemoveEntities As System.Windows.Forms.CheckBox
    Friend WithEvents UnderholeDiaTol As System.Windows.Forms.NumericUpDown
    Friend WithEvents HoleDiaTol As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
End Class
