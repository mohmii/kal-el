﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.LinetypesList = New System.Windows.Forms.DataGridView
        Me.Number = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Layer = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.LineType = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Color = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Solid = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Hidden = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Auxiliary = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Ignore = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Proceed = New System.Windows.Forms.Button
        Me.Cancel = New System.Windows.Forms.Button
        CType(Me.LinetypesList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LinetypesList
        '
        Me.LinetypesList.AllowUserToAddRows = False
        Me.LinetypesList.AllowUserToDeleteRows = False
        Me.LinetypesList.AllowUserToOrderColumns = True
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.LinetypesList.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.LinetypesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.LinetypesList.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Number, Me.Layer, Me.LineType, Me.Color, Me.Solid, Me.Hidden, Me.Auxiliary, Me.Ignore})
        Me.LinetypesList.Location = New System.Drawing.Point(12, 12)
        Me.LinetypesList.Name = "LinetypesList"
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.LinetypesList.RowHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.LinetypesList.RowHeadersVisible = False
        Me.LinetypesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.LinetypesList.Size = New System.Drawing.Size(619, 295)
        Me.LinetypesList.TabIndex = 1
        '
        'Number
        '
        Me.Number.HeaderText = "No"
        Me.Number.Name = "Number"
        Me.Number.Width = 50
        '
        'Layer
        '
        Me.Layer.HeaderText = "Layer"
        Me.Layer.Name = "Layer"
        Me.Layer.Width = 75
        '
        'LineType
        '
        Me.LineType.HeaderText = "Line Type"
        Me.LineType.Name = "LineType"
        Me.LineType.Width = 150
        '
        'Color
        '
        Me.Color.HeaderText = "Color"
        Me.Color.Name = "Color"
        '
        'Solid
        '
        Me.Solid.HeaderText = "Solid"
        Me.Solid.Name = "Solid"
        Me.Solid.Width = 60
        '
        'Hidden
        '
        Me.Hidden.HeaderText = "Hidden"
        Me.Hidden.Name = "Hidden"
        Me.Hidden.Width = 60
        '
        'Auxiliary
        '
        Me.Auxiliary.HeaderText = "Auxiliary"
        Me.Auxiliary.Name = "Auxiliary"
        Me.Auxiliary.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Auxiliary.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        Me.Auxiliary.Width = 60
        '
        'Ignore
        '
        Me.Ignore.HeaderText = "Nothing"
        Me.Ignore.Name = "Ignore"
        Me.Ignore.Width = 60
        '
        'Proceed
        '
        Me.Proceed.Enabled = False
        Me.Proceed.Location = New System.Drawing.Point(473, 313)
        Me.Proceed.Name = "Proceed"
        Me.Proceed.Size = New System.Drawing.Size(75, 23)
        Me.Proceed.TabIndex = 2
        Me.Proceed.Text = "Proceed >>"
        Me.Proceed.UseVisualStyleBackColor = True
        '
        'Cancel
        '
        Me.Cancel.Location = New System.Drawing.Point(554, 313)
        Me.Cancel.Name = "Cancel"
        Me.Cancel.Size = New System.Drawing.Size(75, 23)
        Me.Cancel.TabIndex = 3
        Me.Cancel.Text = "Cancel"
        Me.Cancel.UseVisualStyleBackColor = True
        '
        'LinetypesPresetting
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(651, 468)
        Me.Controls.Add(Me.Cancel)
        Me.Controls.Add(Me.Proceed)
        Me.Controls.Add(Me.LinetypesList)
        Me.Name = "LinetypesPresetting"
        Me.Text = "Linetypes Pre-setting"
        CType(Me.LinetypesList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LinetypesList As System.Windows.Forms.DataGridView
    Friend WithEvents Proceed As System.Windows.Forms.Button
    Friend WithEvents Cancel As System.Windows.Forms.Button
    Friend WithEvents Number As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Layer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents LineType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Color As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Solid As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Hidden As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Auxiliary As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Ignore As System.Windows.Forms.DataGridViewCheckBoxColumn
End Class
