<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SchematicPresetting
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.Proceed = New System.Windows.Forms.Button
        Me.Cancel = New System.Windows.Forms.Button
        Me.TapHoleList = New System.Windows.Forms.DataGridView
        Me.Number = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.HoleLayer = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.HoleLineType = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.HoleColor = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.UnderholeLayer = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.UnderholeLineType = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.UnderholeColor = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.TopSurface = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.BottomSurface = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Ignore = New System.Windows.Forms.DataGridViewCheckBoxColumn
        CType(Me.TapHoleList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Proceed
        '
        Me.Proceed.Location = New System.Drawing.Point(603, 316)
        Me.Proceed.Name = "Proceed"
        Me.Proceed.Size = New System.Drawing.Size(75, 23)
        Me.Proceed.TabIndex = 2
        Me.Proceed.Text = "Proceed >>"
        Me.Proceed.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.Location = New System.Drawing.Point(702, 316)
        Me.Cancel.Name = "Cancel"
        Me.Cancel.Size = New System.Drawing.Size(75, 23)
        Me.Cancel.TabIndex = 3
        Me.Cancel.Text = "Cancel"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'TapHoleList
        '
        Me.TapHoleList.AllowUserToAddRows = False
        Me.TapHoleList.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TapHoleList.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.TapHoleList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TapHoleList.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Number, Me.HoleLayer, Me.HoleLineType, Me.HoleColor, Me.UnderholeLayer, Me.UnderholeLineType, Me.UnderholeColor, Me.TopSurface, Me.BottomSurface, Me.Ignore})
        Me.TapHoleList.Location = New System.Drawing.Point(4, 12)
        Me.TapHoleList.Name = "TapHoleList"
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.TapHoleList.RowHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.TapHoleList.RowHeadersVisible = False
        Me.TapHoleList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.TapHoleList.Size = New System.Drawing.Size(780, 289)
        Me.TapHoleList.TabIndex = 4
        '
        'Number
        '
        Me.Number.HeaderText = "No"
        Me.Number.Name = "Number"
        Me.Number.Width = 30
        '
        'HoleLayer
        '
        Me.HoleLayer.HeaderText = "Layer"
        Me.HoleLayer.Name = "HoleLayer"
        '
        'HoleLineType
        '
        Me.HoleLineType.HeaderText = "Line Type"
        Me.HoleLineType.Name = "HoleLineType"
        '
        'HoleColor
        '
        Me.HoleColor.HeaderText = "Color"
        Me.HoleColor.Name = "HoleColor"
        '
        'UnderholeLayer
        '
        Me.UnderholeLayer.HeaderText = "Layer"
        Me.UnderholeLayer.Name = "UnderholeLayer"
        '
        'UnderholeLineType
        '
        Me.UnderholeLineType.HeaderText = "Line Type"
        Me.UnderholeLineType.Name = "UnderholeLineType"
        '
        'UnderholeColor
        '
        Me.UnderholeColor.HeaderText = "Color"
        Me.UnderholeColor.Name = "UnderholeColor"
        '
        'TopSurface
        '
        Me.TopSurface.HeaderText = "Top"
        Me.TopSurface.Name = "TopSurface"
        Me.TopSurface.Width = 50
        '
        'BottomSurface
        '
        Me.BottomSurface.HeaderText = "Bottom"
        Me.BottomSurface.Name = "BottomSurface"
        Me.BottomSurface.Width = 50
        '
        'Ignore
        '
        Me.Ignore.HeaderText = "Nothing"
        Me.Ignore.Name = "Ignore"
        Me.Ignore.Width = 50
        '
        'SchematicPresetting
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(789, 360)
        Me.Controls.Add(Me.TapHoleList)
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Proceed)
        Me.Name = "SchematicPresetting"
        Me.Text = "Tap Hole Schematic Pre-setting"
        CType(Me.TapHoleList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Proceed As System.Windows.Forms.Button
    Friend WithEvents Cancel As System.Windows.Forms.Button
    Friend WithEvents TapHoleList As System.Windows.Forms.DataGridView
    Friend WithEvents Number As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HoleLayer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HoleLineType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HoleColor As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UnderholeLayer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UnderholeLineType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UnderholeColor As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TopSurface As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents BottomSurface As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Ignore As System.Windows.Forms.DataGridViewCheckBoxColumn
End Class
