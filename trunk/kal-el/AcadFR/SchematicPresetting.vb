Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports FR
Imports System.Linq
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Public Class SchematicPresetting

    'variabel hole underhole
    'Private UICircleGroup As IEnumerable(Of Circle)
    Private DBConn As DatabaseConn
    Private ProceedStat As Boolean
    Private Counter As Integer

    'setiap isi sel di klik
    Private Sub TapHoleList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles TapHoleList.CellContentClick
        ProceedStat = True
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.TapHoleList.Rows
            If Row.Cells("TopSurface").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("TopSurface").Value = True
                Row.Cells("BottomSurface").Value = False
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("BottomSurface").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("TopSurface").Value = False
                Row.Cells("BottomSurface").Value = True
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Ignore").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("TopSurface").Value = False
                Row.Cells("BottomSurface").Value = False
                Row.Cells("Ignore").Value = True
            End If

            If Row.Cells("TopSurface").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("BottomSurface").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
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
        DBConn = New DatabaseConn
        Counter = New Integer
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.TapHoleList.Rows
            'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
            'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke variabel perantara
            'TopHole = New TopTapLineType
            'TopHole.TapLineName = Row.Cells("HoleLayer").Value.ToString
            'TopHole.TapLineType = Row.Cells("HoleLineType").Value.ToString
            'TopHole.TapLineColor = Row.Cells("HoleColor").Value.ToString
            'TopHole.UnHoleLineName = Row.Cells("UnderholeLayer").Value.ToString
            'TopHole.UnHoleLineType = Row.Cells("UnderholeLineType").Value.ToString
            'TopHole.UnHoleLineColor = Row.Cells("UnderholeColor").Value.ToString
            If Row.Cells("TopSurface").FormattedValue = True Then
                DBConn.AddToTopTapLineDatabase(SelectionCommand.UI2CircList(Counter))
            ElseIf Row.Cells("BottomSurface").FormattedValue = True Then
                DBConn.AddToBottomTapLineDatabase(SelectionCommand.UI2CircList(Counter))
            End If
            Counter = Counter + 1
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