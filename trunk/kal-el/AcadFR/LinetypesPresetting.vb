Imports FR
Imports System.Linq
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Public Class LinetypesPresetting

    'variabel yg dibutuhkan
    Private CheckBox1, CheckBox2, CheckBox3, CheckBox4 As System.Windows.Forms.CheckBox
    Private SolidUIF As SolidLine
    Private HiddenUIF As HiddenLine
    Private AuxUIF As AuxiliaryLine
    Private ProceedStat As New Boolean

    'setiap isi sel di klik
    Private Sub LinetypesList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles LinetypesList.CellContentClick
        ProceedStat = True
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.LinetypesList.Rows
            CheckBox1 = New System.Windows.Forms.CheckBox
            CheckBox2 = New System.Windows.Forms.CheckBox
            CheckBox3 = New System.Windows.Forms.CheckBox
            CheckBox4 = New System.Windows.Forms.CheckBox
            CheckBox1 = Row.Cells("Solid").Value
            CheckBox2 = Row.Cells("Hidden").Value
            CheckBox3 = Row.Cells("Auxiliary").Value
            CheckBox4 = Row.Cells("Ignore").Value

            If CheckBox1.Checked = True Then
                CheckBox2.Checked = False
                CheckBox3.Checked = False
                CheckBox4.Checked = False
            End If

            If CheckBox2.Checked = True Then
                CheckBox1.Checked = False
                CheckBox3.Checked = False
                CheckBox4.Checked = False
            End If

            If CheckBox3.Checked = True Then
                CheckBox1.Checked = False
                CheckBox2.Checked = False
                CheckBox4.Checked = False
            End If

            If CheckBox4.Checked = True Then
                CheckBox1.Checked = False
                CheckBox2.Checked = False
                CheckBox3.Checked = False
            End If

            If CheckBox1.Checked = False And CheckBox2.Checked = False _
            And CheckBox3.Checked = False And CheckBox4.Checked = False Then
                ProceedStat = False
            End If
        Next

        If ProceedStat = True Then
            Me.Proceed.Enabled = True
        Else
            Me.Proceed.Enabled = False
        End If

    End Sub

    'jika Proceed diklik
    Private Sub Proceed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Proceed.Click
        'masukin ke database
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.LinetypesList.Rows
            CheckBox1 = New System.Windows.Forms.CheckBox
            CheckBox2 = New System.Windows.Forms.CheckBox
            CheckBox3 = New System.Windows.Forms.CheckBox
            CheckBox4 = New System.Windows.Forms.CheckBox
            CheckBox1 = Row.Cells("Solid").Value
            CheckBox2 = Row.Cells("Hidden").Value
            CheckBox3 = Row.Cells("Auxiliary").Value
            CheckBox4 = Row.Cells("Ignore").Value

            If CheckBox1.Checked = True Then
                'masukkan Row.Cells("Layer").Value , Row.Cells("LineType").Value , Row.Cells("Color").Value ke database Solid
                SolidUIF = New SolidLine
            ElseIf CheckBox2.Checked = True Then
                'masukkan Row.Cells("Layer").Value , Row.Cells("LineType").Value , Row.Cells("Color").Value ke database Hidden
                HiddenUIF = New HiddenLine
            ElseIf CheckBox3.Checked = True Then
                'masukkan Row.Cells("Layer").Value , Row.Cells("LineType").Value , Row.Cells("Color").Value ke database Auxiliary
                AuxUIF = New AuxiliaryLine
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