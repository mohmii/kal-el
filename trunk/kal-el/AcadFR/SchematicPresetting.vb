Imports FR
Imports System.Linq
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Public Class SchematicPresetting

    'variabel hole underhole
    Private TopHole As TopTapLineType
    Private BottomHole As BottomTapLineType

    Private ProceedStat As New Boolean

    'setiap isi sel di klik
    Private Sub TapHoleList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles TapHoleList.CellContentClick
        ProceedStat = True
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.TapHoleList.Rows
            If Row.Cells("Bottom").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Top").Value = False
                Row.Cells("Bottom").Value = True
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Ignore").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Top").Value = False
                Row.Cells("Bottom").Value = False
                Row.Cells("Ignore").Value = True
            End If

            If Row.Cells("Top").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("Bottom").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("Ignore").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False Then
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
            If Row.Cells("Top").FormattedValue = True Then
                TopHole = New TopTapLineType
                'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
                'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke database yg top
            ElseIf Row.Cells("Bottom").FormattedValue = True Then
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