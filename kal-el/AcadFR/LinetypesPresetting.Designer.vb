<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LinetypesPresetting
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
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.Number = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Layer = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.LineType = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Color = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Top = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Bottom = New System.Windows.Forms.DataGridViewCheckBoxColumn
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Number, Me.Layer, Me.LineType, Me.Color, Me.Top, Me.Bottom})
        Me.DataGridView1.Location = New System.Drawing.Point(12, 12)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(643, 275)
        Me.DataGridView1.TabIndex = 1
        '
        'Number
        '
        Me.Number.HeaderText = "Num"
        Me.Number.Name = "Number"
        '
        'Layer
        '
        Me.Layer.HeaderText = "Layer"
        Me.Layer.Name = "Layer"
        '
        'LineType
        '
        Me.LineType.HeaderText = "Line Type"
        Me.LineType.Name = "LineType"
        '
        'Color
        '
        Me.Color.HeaderText = "Color"
        Me.Color.Name = "Color"
        '
        'Top
        '
        Me.Top.HeaderText = "Top"
        Me.Top.Name = "Top"
        '
        'Bottom
        '
        Me.Bottom.HeaderText = "Bottom"
        Me.Bottom.Name = "Bottom"
        '
        'LinetypesPresetting
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(669, 468)
        Me.Controls.Add(Me.DataGridView1)
        Me.Name = "LinetypesPresetting"
        Me.Text = "Linetypes Pre-setting"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents Number As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Layer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents LineType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Color As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Top As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Bottom As System.Windows.Forms.DataGridViewCheckBoxColumn
End Class
