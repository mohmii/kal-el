<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Me.GroupBox1.SuspendLayout()
        CType(Me.Schematic, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Tolerance, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(108, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Workspace directory:"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(281, 23)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(24, 23)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "..."
        Me.Button1.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Schematic)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Tolerance)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 121)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(293, 62)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Tolerance values"
        '
        'Schematic
        '
        Me.Schematic.DecimalPlaces = 2
        Me.Schematic.Enabled = False
        Me.Schematic.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.Schematic.Location = New System.Drawing.Point(236, 33)
        Me.Schematic.Name = "Schematic"
        Me.Schematic.Size = New System.Drawing.Size(51, 20)
        Me.Schematic.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(126, 35)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(95, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Schematic symbol:"
        '
        'Tolerance
        '
        Me.Tolerance.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tolerance.DecimalPlaces = 2
        Me.Tolerance.Enabled = False
        Me.Tolerance.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.Tolerance.Location = New System.Drawing.Point(236, 14)
        Me.Tolerance.Name = "Tolerance"
        Me.Tolerance.Size = New System.Drawing.Size(51, 20)
        Me.Tolerance.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 16)
        Me.Label2.Margin = New System.Windows.Forms.Padding(3, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(215, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Connectivity, tangency, normal, and parallel:"
        '
        'Save
        '
        Me.Save.Location = New System.Drawing.Point(197, 189)
        Me.Save.Name = "Save"
        Me.Save.Size = New System.Drawing.Size(51, 23)
        Me.Save.TabIndex = 4
        Me.Save.Text = "Save"
        Me.Save.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.Location = New System.Drawing.Point(254, 189)
        Me.Cancel.Name = "Cancel"
        Me.Cancel.Size = New System.Drawing.Size(51, 23)
        Me.Cancel.TabIndex = 5
        Me.Cancel.Text = "Cancel"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'Directory
        '
        Me.Directory.Location = New System.Drawing.Point(12, 25)
        Me.Directory.Name = "Directory"
        Me.Directory.Size = New System.Drawing.Size(263, 20)
        Me.Directory.TabIndex = 6
        '
        'PreProcess
        '
        Me.PreProcess.AutoSize = True
        Me.PreProcess.Location = New System.Drawing.Point(12, 98)
        Me.PreProcess.Name = "PreProcess"
        Me.PreProcess.Size = New System.Drawing.Size(132, 17)
        Me.PreProcess.TabIndex = 8
        Me.PreProcess.Text = "Drawing pre-processor"
        Me.PreProcess.UseVisualStyleBackColor = True
        '
        'AutoRegLine
        '
        Me.AutoRegLine.AutoSize = True
        Me.AutoRegLine.Location = New System.Drawing.Point(12, 52)
        Me.AutoRegLine.Name = "AutoRegLine"
        Me.AutoRegLine.Size = New System.Drawing.Size(222, 17)
        Me.AutoRegLine.TabIndex = 9
        Me.AutoRegLine.Text = "Auto registration for undefined line entities"
        Me.AutoRegLine.UseVisualStyleBackColor = True
        '
        'AutoRegScheme
        '
        Me.AutoRegScheme.AutoSize = True
        Me.AutoRegScheme.Location = New System.Drawing.Point(12, 75)
        Me.AutoRegScheme.Name = "AutoRegScheme"
        Me.AutoRegScheme.Size = New System.Drawing.Size(254, 17)
        Me.AutoRegScheme.TabIndex = 10
        Me.AutoRegScheme.Text = "Auto registration for undefined schematic entities"
        Me.AutoRegScheme.UseVisualStyleBackColor = True
        '
        'AppPreferencesForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(317, 224)
        Me.Controls.Add(Me.AutoRegScheme)
        Me.Controls.Add(Me.AutoRegLine)
        Me.Controls.Add(Me.PreProcess)
        Me.Controls.Add(Me.Directory)
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Save)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "AppPreferencesForm"
        Me.Text = "FR Preferences"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
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
End Class
