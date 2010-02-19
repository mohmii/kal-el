Imports FR
Imports System.Linq
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Public Class SchematicPresetting

    'variabel yg dibutuhkan
    Private CheckBox1, CheckBox2, CheckBox3 As System.Windows.Forms.CheckBox

    'variabel hole underhole
    Private TopHole As TopTapLineType
    Private BottomHole As BottomTapLineType

    Private ProceedStat As New Boolean

    'setiap isi sel di klik
    Private Sub TapHoleList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles TapHoleList.CellContentClick
        ProceedStat = True
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.TapHoleList.Rows
            CheckBox1 = New System.Windows.Forms.CheckBox
            CheckBox2 = New System.Windows.Forms.CheckBox
            CheckBox3 = New System.Windows.Forms.CheckBox
            CheckBox1 = Row.Cells("Top").Value
            CheckBox2 = Row.Cells("Bottom").Value
            CheckBox3 = Row.Cells("Ignore").Value

            If CheckBox1.Checked = True Then
                CheckBox2.Checked = False
                CheckBox3.Checked = False
            End If

            If CheckBox2.Checked = True Then
                CheckBox1.Checked = False
                CheckBox3.Checked = False
            End If

            If CheckBox3.Checked = True Then
                CheckBox1.Checked = False
                CheckBox2.Checked = False
            End If

            If CheckBox1.Checked = False And CheckBox2.Checked = False And CheckBox3.Checked = False Then
                ProceedStat = False
            End If
        Next

        If ProceedStat = True Then
            Me.Proceed.Enabled = True
        Else
            Me.Proceed.Enabled = False
        End If
    End Sub


    Private Sub Proceed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Proceed.Click
        'masukin ke database
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.TapHoleList.Rows
            CheckBox1 = New System.Windows.Forms.CheckBox
            CheckBox2 = New System.Windows.Forms.CheckBox
            CheckBox3 = New System.Windows.Forms.CheckBox
            CheckBox1 = Row.Cells("Top").Value
            CheckBox2 = Row.Cells("Bottom").Value
            CheckBox3 = Row.Cells("Ignore").Value

            If CheckBox1.Checked = True Then
                TopHole = New TopTapLineType
                'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
                'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke database yg top
            ElseIf CheckBox2.Checked = True Then
                BottomHole = New BottomTapLineType
                'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
                'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke database yg bottom
            End If
        Next

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Dispose()
    End Sub

    'jika cancel diklik
    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Dispose()
    End Sub

End Class