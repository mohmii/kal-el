Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Colors
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.Geometry

Imports FR
Imports System.Linq
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Public Class SchematicPresetting

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
            If Row.Cells("TopSurface").FormattedValue = True Then
                DBConn.AddToTopTapLineDatabase(SelectionCommand.UI2CircList(Counter))
                'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
                'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke database top tap
            ElseIf Row.Cells("BottomSurface").FormattedValue = True Then
                'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
                'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke database top tap
                DBConn.AddToBottomTapLineDatabase(SelectionCommand.UI2CircList(Counter))
            End If
            Counter = Counter + 1
        Next

        Try
            'create a document lock and acquire the information from the current drawing editor
            DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

            'initiate a new connection
            AcadConnection = New AcadConn

            AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
            Using AcadConnection.myT
                'initial setting for opening the connection for read the autocad database
                AcadConnection.OpenBlockTableRec()

                If Not (PastEntityColor.Count = 0) Then
                    RollbackColor(PastEntityColor, AcadConnection.btr)
                    PastEntityColor.Clear()
                End If
                'committing the autocad transaction
                AcadConnection.myT.Commit()
            End Using

        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            DrawEditor.UpdateScreen()
            AcadConnection.myT.Dispose()
        End Try

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Dispose()
    End Sub

    'jika cancel diklik
    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Try
            'create a document lock and acquire the information from the current drawing editor
            DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

            'initiate a new connection
            AcadConnection = New AcadConn

            AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
            Using AcadConnection.myT
                'initial setting for opening the connection for read the autocad database
                AcadConnection.OpenBlockTableRec()

                If Not (PastEntityColor.Count = 0) Then
                    RollbackColor(PastEntityColor, AcadConnection.btr)
                    PastEntityColor.Clear()
                End If
                'committing the autocad transaction
                AcadConnection.myT.Commit()
            End Using

        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            DrawEditor.UpdateScreen()
            AcadConnection.myT.Dispose()
        End Try

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Dispose()
    End Sub

    'hilangkan selection ketika diload
    Private Sub SchematicPresetting_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TapHoleList.ClearSelection()
    End Sub

    Private AcadConnection As AcadConn
    Private DocLock As DocumentLock
    Private DrawEditor As Editor
    Private Entity As Entity
    Private CircGroup As IEnumerable(Of Circle)
    Private PastEntityColor As New List(Of InitialColor)
    Private EntityColor As InitialColor
    Private TempId As String

    'method for setting the entity color defaults
    Private Sub RollbackColor(ByVal PastEntColor As List(Of InitialColor), _
                              ByVal BlockTableRecInstances As BlockTableRecord)

        If Not (PastEntColor.Count = 0) Then
            For Each ObjectColorId As InitialColor In PastEntColor
                For Each idTmp As ObjectId In BlockTableRecInstances
                    'acquire the entity from the object id
                    Entity = AcadConnection.myT.GetObject(idTmp, OpenMode.ForWrite)
                    If Entity.ObjectId = ObjectColorId.ColorId Then
                        Entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByAci, ObjectColorId.ColorIndex)
                    End If
                Next
            Next
        End If

    End Sub

    'method for highlighting the selected entities
    Private Sub HighlightEntity(ByVal ObjectIdListTmp As List(Of ObjectId), _
                                ByVal BlockTableRecInstances As BlockTableRecord, _
                                ByVal PastEntColor As List(Of InitialColor))

        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Dim FeatureListId As List(Of ObjectId)
        FeatureListId = ObjectIdListTmp

        'create loop checking for each element in selected item
        For Each ObjectIdTmp As ObjectId In ObjectIdListTmp
            'create a loop checking for each objectid id autocad database
            For Each idTmp As Autodesk.AutoCAD.DatabaseServices.ObjectId In BlockTableRecInstances
                'acquire the entity from the object id
                Entity = AcadConnection.myT.GetObject(idTmp, OpenMode.ForWrite)

                'test if the entity id were the same as entity id in the selected item
                If Entity.ObjectId = ObjectIdTmp Then

                    EntityColor = New InitialColor
                    EntityColor.ColorId = Entity.ObjectId
                    EntityColor.ColorIndex = Entity.ColorIndex
                    PastEntColor.Add(EntityColor)

                    'change the color and the lineweight for making the highlights
                    Entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByColor, 10)

                    'save the changed entity id
                    TempId = Entity.Id.ToString
                End If
            Next
        Next
    End Sub

End Class