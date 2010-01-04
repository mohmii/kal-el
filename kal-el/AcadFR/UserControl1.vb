Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Colors


Public Class UserControl1

    Private uiSetView As setView

    'works when the add button was being pressed
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        uiSetView = New setView

        Using uiSetView
            uiSetView.ShowDialog()
        End Using

        uiSetView.Dispose()

    End Sub

    'works when user want to use filter by feature type
    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged

        'make the filtering option by type become true
        If CheckBox1.Checked = True Then
            Me.FilterByType.Enabled = True
        Else
            Me.FilterByType.Enabled = False
        End If

    End Sub

    'works when user want to use filter by view type
    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged

        'make the filtering option by surface become true
        If CheckBox2.Checked = True Then
            Me.FilterBySurface.Enabled = True
        Else
            Me.FilterBySurface.Enabled = False
        End If

    End Sub

    ''clear all the feature that listed on machining feature list
    'Private Sub ClearAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ClearAll.Click

    '    adskClass.myPalette.ExistingList.Items.Clear()

    'End Sub

    'Private Sub Delete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete.Click
    '    adskClass.myPalette.ExistingList.SelectedItems.Clear()
    'End Sub

    Private Shared db As Database
    Private Shared tm As Autodesk.AutoCAD.DatabaseServices.TransactionManager
    Private Shared myT As Transaction

    Private Shared bt As BlockTable
    Private Shared btrId As ObjectId
    Private Shared btr As BlockTableRecord

    Private Shared Entity As Entity

    Private RedColor As Color = Color.FromColor(Drawing.Color.Red)
    Private StdCOlor As Color

    'works for highlighting the object in the drawing when the mouse hovering each feature list
    Private Shared Sub ExistingList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExistingList.MouseHover

        db = Application.DocumentManager.MdiActiveDocument.Database
        tm = db.TransactionManager
        myT = tm.StartTransaction()

        Try
            bt = myT.GetObject(db.BlockTableId, OpenMode.ForRead)
            btrId = bt.Item(BlockTableRecord.ModelSpace)
            btr = myT.GetObject(btrId, OpenMode.ForRead, True)

            For Each idTmp As ObjectId In btr
                Entity = myT.GetObject(idTmp, OpenMode.ForRead)

                'If SelectionCommand.MachiningFeature.FindIndex(adskClass.myPalette.ExistingList.SelectedIndex()) = Entity.ObjectId Then
                '    MsgBox("at last")
                'End If
            Next

            myT.Commit()

        Catch ex As Exception
            myT.Dispose()

        Finally
            myT.Dispose()
        End Try

    End Sub

    Private FontRegular As System.Drawing.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25, Drawing.FontStyle.Regular)
    Private FontBold As System.Drawing.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25, Drawing.FontStyle.Bold)

    'Private tmp As Forms.ListViewItem = adskClass.myPalette.ExistingList.SelectedItem

    Private Sub ExistingList2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExistingList2.SelectedIndexChanged

    End Sub
End Class
