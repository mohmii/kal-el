Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Colors
Imports FR
Imports System.Linq

Public Class SchematicPresetting

    Private DBConn As DatabaseConn
    Private ProceedStat As Boolean

    'setiap isi sel di klik
    Private Sub TapHoleList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles TapHoleList.CellContentClick
        CheckProceed()
    End Sub

    'checking the availability of proceed botton
    Private Sub CheckProceed()
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
        'balikin ke warna original
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

        'masukin ke database
        DBConn = New DatabaseConn
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.TapHoleList.Rows
            If Row.Cells("TopSurface").FormattedValue = True Then
                'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
                'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke database top tap
                DBConn.AddToTopTapLineDatabase(Row)
            ElseIf Row.Cells("BottomSurface").FormattedValue = True Then
                'masukkan Row.Cells("HoleLayer").Value , Row.Cells("HoleLineType").Value , Row.Cells("HoleColor").Value 
                'masukkan Row.Cells("UnderholeLayer").Value , Row.Cells("UnderholeLineType").Value , Row.Cells("UnderholeColor").Value ke database bottom tap
                DBConn.AddToBottomTapLineDatabase(Row)
            ElseIf Row.Cells("Ignore").FormattedValue = True Then
                If adskClass.AppPreferences.RemoveUEE = True Then
                    EraseUEE(Row, SelectionCommand.UI2CircListAll)
                End If
            End If
        Next

        

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Dispose()
    End Sub

    'method for erasing unessential entities
    Private Sub EraseUEE(ByVal TableRow As System.Windows.Forms.DataGridViewRow, ByRef UI2CircListAll As List(Of IEnumerable(Of Circle)))
        Dim UEEIndex As New List(Of Integer)
        Dim EntityToErase1, EntityToErase2 As Entity
        AcadConnection = New AcadConn
        AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        AcadConnection.OpenBlockTableRec()
        AcadConnection.btr.UpgradeOpen()

        Using AcadConnection.myT

            For Each UnessentialEntity As IEnumerable(Of Circle) In UI2CircListAll
                If UnessentialEntity(0).Layer = TableRow.Cells("HoleLayer").Value And UnessentialEntity(1).Layer = TableRow.Cells("UnderholeLayer").Value _
                And UnessentialEntity(0).Color.ColorNameForDisplay.ToLower = TableRow.Cells("HoleColor").Value.ToString.ToLower And UnessentialEntity(1).Color.ColorNameForDisplay.ToLower = TableRow.Cells("UnderholeColor").Value.ToString.ToLower Then
                    If UnessentialEntity(0).Linetype.ToString.ToLower = "bylayer" Then
                        If UnessentialEntity(1).Linetype.ToString.ToLower = "bylayer" Then
                            If TableRow.Cells("HoleLineType").Value.ToString.ToLower = "null" _
                            And TableRow.Cells("UnderholeLineType").Value.ToString.ToLower = "null" Then
                                UEEIndex.Add(UI2CircListAll.IndexOf(UnessentialEntity))
                            End If
                        Else
                            If TableRow.Cells("HoleLineType").Value.ToString.ToLower = "null" _
                            And UnessentialEntity(1).Linetype.ToString.ToLower = TableRow.Cells("UnderholeLineType").Value.ToString.ToLower Then
                                UEEIndex.Add(UI2CircListAll.IndexOf(UnessentialEntity))
                            End If
                        End If

                    Else
                        If UnessentialEntity(0).Linetype.ToString.ToLower = TableRow.Cells("HoleLineType").Value.ToString.ToLower Then
                            If UnessentialEntity(1).Linetype.ToString.ToLower = "bylayer" Then
                                If TableRow.Cells("UnderholeLineType").Value.ToString.ToLower = "null" Then
                                    UEEIndex.Add(UI2CircListAll.IndexOf(UnessentialEntity))
                                End If
                            Else
                                If UnessentialEntity(1).Linetype.ToString.ToLower = TableRow.Cells("UnderholeLineType").Value.ToString.ToLower Then
                                    UEEIndex.Add(UI2CircListAll.IndexOf(UnessentialEntity))
                                End If
                            End If
                        End If
                    End If
                End If
            Next

            UEEIndex.Sort()
            For i As Integer = (UEEIndex.Count - 1) To 0 Step (-1)
                EntityToErase1 = AcadConnection.myT.GetObject(UI2CircListAll(UEEIndex(i))(0).ObjectId, OpenMode.ForWrite, True)
                EntityToErase1.Erase()
                EntityToErase2 = AcadConnection.myT.GetObject(UI2CircListAll(UEEIndex(i))(1).ObjectId, OpenMode.ForWrite, True)
                EntityToErase2.Erase()
                UI2CircListAll.RemoveAt(UEEIndex(i))
            Next

            AcadConnection.myT.Commit()

        End Using

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
        CheckProceed()
        TapHoleList.ClearSelection()
    End Sub

    'method open window schematic presetting
    Public Sub OpenSchematic(ByRef Schematic As SchematicPresetting, ByVal UI2C As List(Of IEnumerable(Of Circle)))
        Dim SchemCount As New Integer
        Dim LTNothingStat As Boolean = True

        Using Schematic
            For Each result As IEnumerable(Of Circle) In UI2C
                AddToSchematicTable(SchemCount, result, Schematic, LTNothingStat)
                SchemCount = SchemCount + 1
            Next
            If LTNothingStat = False And adskClass.AppPreferences.AutoRegSchem = True Then
                Schematic.ShowDialog()
            Else
                Proceed_Click(Nothing, Nothing)
            End If

        End Using
    End Sub

    'add new line in Schematic Table
    Private Sub AddToSchematicTable(ByVal Count As Integer, ByVal AddedResult As IEnumerable(Of Circle), _
                       ByRef Table As SchematicPresetting, ByRef LTNothingStat As Boolean)

        Dim NewRow As New System.Windows.Forms.DataGridViewRow
        Dim ObjectIDList As New List(Of ObjectId)
        ObjectIDList.Add(AddedResult(0).ObjectId)
        ObjectIDList.Add(AddedResult(1).ObjectId)

        Table.TapHoleList.Rows.Add(NewRow)
        Table.TapHoleList.Rows(Count).Cells("ObjectID").Value = ObjectIDList
        Table.TapHoleList.Rows(Count).Cells("Number").Value = Count + 1
        Table.TapHoleList.Rows(Count).Cells("HoleLayer").Value = AddedResult(0).Layer.ToString

        If AddedResult(0).Linetype.ToString.ToLower = "bylayer" Then
            Table.TapHoleList.Rows(Count).Cells("HoleLineType").Value = "NULL"
        ElseIf AddedResult(0).Linetype.ToString.ToLower = "byblock" Then
            Table.TapHoleList.Rows(Count).DefaultCellStyle.BackColor = Drawing.Color.Gold
            Table.TapHoleList.Rows(Count).Cells("Ignore").Value = True
            Table.TapHoleList.Rows(Count).ReadOnly = True
        Else
            Table.TapHoleList.Rows(Count).Cells("HoleLineType").Value = AddedResult(0).Linetype.ToString
        End If

        If AddedResult(0).Color.ColorNameForDisplay.ToLower = "bylayer" Or AddedResult(0).Color.ColorNameForDisplay.ToLower = "byblock" Then
            Table.TapHoleList.Rows(Count).Cells("Ignore").Value = True
            Table.TapHoleList.Rows(Count).DefaultCellStyle.BackColor = Drawing.Color.Gold
            Table.TapHoleList.Rows(Count).ReadOnly = True
        Else
            Table.TapHoleList.Rows(Count).Cells("HoleColor").Value = AddedResult(0).Color.ColorNameForDisplay.ToUpper
            Table.TapHoleList.Rows(Count).Cells("Ignore").Value = False
        End If

        Table.TapHoleList.Rows(Count).Cells("UnderholeLayer").Value = AddedResult(1).Layer.ToString

        If AddedResult(1).Linetype.ToString.ToLower = "bylayer" Then
            Table.TapHoleList.Rows(Count).Cells("UnderholeLineType").Value = "NULL"
        ElseIf AddedResult(1).Linetype.ToString.ToLower = "byblock" Then
            Table.TapHoleList.Rows(Count).Cells("Ignore").Value = True
            Table.TapHoleList.Rows(Count).DefaultCellStyle.BackColor = Drawing.Color.Gold
            Table.TapHoleList.Rows(Count).ReadOnly = True
        Else
            Table.TapHoleList.Rows(Count).Cells("UnderholeLineType").Value = AddedResult(1).Linetype.ToString
        End If

        If AddedResult(1).Color.ColorNameForDisplay.ToLower = "bylayer" Or AddedResult(1).Color.ColorNameForDisplay.ToLower = "byblock" Then
            Table.TapHoleList.Rows(Count).Cells("Ignore").Value = True
            Table.TapHoleList.Rows(Count).DefaultCellStyle.BackColor = Drawing.Color.Gold
            Table.TapHoleList.Rows(Count).ReadOnly = True
        Else
            Table.TapHoleList.Rows(Count).Cells("UnderholeColor").Value = AddedResult(1).Color.ColorNameForDisplay.ToUpper
            Table.TapHoleList.Rows(Count).Cells("Ignore").Value = False
        End If

        If Table.TapHoleList.Rows(Count).Cells("Ignore").Value = False Then
            LTNothingStat = False
        End If

        Table.TapHoleList.Rows(Count).Cells("TopSurface").Value = False
        Table.TapHoleList.Rows(Count).Cells("BottomSurface").Value = False

    End Sub

    'highlight
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