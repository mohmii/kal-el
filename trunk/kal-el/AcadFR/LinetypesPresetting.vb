Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Colors
Imports Autodesk.AutoCAD.Interop
Imports FR

Public Class LinetypesPresetting

    Private ProceedStat As Boolean
    Private DBConn As DatabaseConn

    'setiap isi sel di klik
    Private Sub LinetypesList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles LinetypesList.CellContentClick
        CheckProceed()
    End Sub

    'checking the availability of proceed botton
    Private Sub CheckProceed()
        ProceedStat = True
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.LinetypesList.Rows

            If Row.Cells("Solid").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = True
                Row.Cells("Hidden").Value = False
                Row.Cells("Auxiliary").Value = False
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Hidden").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = False
                Row.Cells("Hidden").Value = True
                Row.Cells("Auxiliary").Value = False
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Auxiliary").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = False
                Row.Cells("Hidden").Value = False
                Row.Cells("Auxiliary").Value = True
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Ignore").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = False
                Row.Cells("Hidden").Value = False
                Row.Cells("Auxiliary").Value = False
                Row.Cells("Ignore").Value = True
            End If

            If Row.Cells("Solid").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("Hidden").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("Auxiliary").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
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

    Private DLock As DocumentLock

    Private Sub LinetypesList_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LinetypesList.SelectionChanged
        'highlight jika row dipilih
        If Me.LinetypesList.Focused = True Then

            'create a document lock and acquire the information from the current drawing editor
            DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

            'initiate a new connection
            AcadConnection = New AcadConn

            AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
            DLock = Application.DocumentManager.MdiActiveDocument.LockDocument
            Try
                Using DLock
                    Using AcadConnection.myT
                        'initial setting for opening the connection for read the autocad database
                        AcadConnection.OpenBlockTableRec()

                        'dummy variable for preventing clearing the current highlighted entity/entities
                        TempId = Nothing

                        'roleback each precious selected entities to their default color
                        RollbackColor(PastEntityColor, AcadConnection.btr)

                        PastEntityColor = New List(Of InitialColor)

                        HighlightEntity(LinetypesList.SelectedRows(0).Cells("ObjectID").Value, AcadConnection.btr, PastEntityColor)

                        'committing the autocad transaction
                        AcadConnection.myT.Commit()
                    End Using
                End Using
            Catch ex As Exception
                MsgBox(ex.ToString)
            Finally
                DrawEditor.UpdateScreen()
                AcadConnection.myT.Dispose()
            End Try
        End If
    End Sub

    'jika Proceed diklik
    Private Sub Proceed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Proceed.Click
        'balikin ke warna original

        'create a document lock and acquire the information from the current drawing editor
        DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

        'initiate a new connection
        AcadConnection = New AcadConn

        AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        DLock = Application.DocumentManager.MdiActiveDocument.LockDocument

        Try
            Using DLock
                Using AcadConnection.myT
                    'initial setting for opening the connection for read the autocad database
                    AcadConnection.OpenBlockTableRec()

                    If Not (PastEntityColor.Count = 0) Then
                        RollbackColor(PastEntityColor, AcadConnection.btr)
                        PastEntityColor.Clear()
                    End If
                    'committing the autocad transaction
                    AcadConnection.myT.Commit()

                    'masukin ke database
                    DBConn = New DatabaseConn
                    For Each Row As System.Windows.Forms.DataGridViewRow In Me.LinetypesList.Rows
                        'masukkan Row.Cells("Layer").Value , Row.Cells("LineType").Value , Row.Cells("Color").Value ke variabel perantara
                        If Row.Cells("Solid").FormattedValue = True Then
                            DBConn.AddToSolidLineDatabase(Row)
                        ElseIf Row.Cells("Hidden").FormattedValue = True Then
                            DBConn.AddToHiddenLineDatabase(Row)
                        ElseIf Row.Cells("Auxiliary").FormattedValue = True Then
                            DBConn.AddToAuxiliaryLineDatabase(Row)
                            If adskClass.AppPreferences.RemoveUEE = True Then
                                EraseUEE(Row, adskClass.UIEntitiesAll)
                            End If
                        ElseIf Row.Cells("Ignore").FormattedValue = True Then
                            If adskClass.AppPreferences.RemoveUEE = True Then
                                EraseUEE(Row, adskClass.UIEntitiesAll)
                            End If
                        End If
                    Next

                    Me.DialogResult = System.Windows.Forms.DialogResult.OK
                    Me.Dispose()
                End Using
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            DrawEditor.UpdateScreen()
            AcadConnection.myT.Dispose()
        End Try
    End Sub

    'method for erasing unessential entities
    Private Sub EraseUEE(ByVal TableRow As System.Windows.Forms.DataGridViewRow, ByRef UIEntAll As List(Of Entity))
        Dim UEEIndex As New List(Of Integer)
        Dim UEEObjIDIndex As New List(Of Integer)
        Dim EntityToErase As Entity
        AcadConnection = New AcadConn
        AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        AcadConnection.OpenBlockTableRec()
        AcadConnection.btr.UpgradeOpen()

        Using AcadConnection.myT

            For Each UnessentialEntity As Entity In UIEntAll
                If UnessentialEntity.Layer = TableRow.Cells("Layer").Value _
                And UnessentialEntity.Color.ColorNameForDisplay.ToLower = TableRow.Cells("Color").Value.ToString.ToLower Then
                    If UnessentialEntity.Linetype.ToString.ToLower = "bylayer" Then
                        If TableRow.Cells("Linetype").Value.ToString.ToLower = "null" Then
                            UEEIndex.Add(UIEntAll.IndexOf(UnessentialEntity))
                            UEEObjIDIndex.Add(adskClass.ObjIDsClassify.IndexOf(UnessentialEntity.ObjectId))
                        End If
                    Else
                        If UnessentialEntity.Linetype.ToString.ToLower = TableRow.Cells("Linetype").Value.ToString.ToLower Then
                            UEEIndex.Add(UIEntAll.IndexOf(UnessentialEntity))
                            UEEObjIDIndex.Add(adskClass.ObjIDsClassify.IndexOf(UnessentialEntity.ObjectId))
                        End If
                    End If
                End If
            Next

            UEEIndex.Sort()
            For i As Integer = (UEEIndex.Count - 1) To 0 Step (-1)
                EntityToErase = AcadConnection.myT.GetObject(UIEntAll(UEEIndex(i)).ObjectId, OpenMode.ForWrite, True)
                EntityToErase.Erase()
                UIEntAll.RemoveAt(UEEIndex(i))
            Next

            UEEObjIDIndex.Sort()
            For i As Integer = (UEEObjIDIndex.Count - 1) To 0 Step (-1)
                adskClass.ObjIDsClassify.RemoveAt(UEEObjIDIndex(i))
            Next

            AcadConnection.myT.Commit()

        End Using

    End Sub

    'jika cancel diklik
    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click

        'create a document lock and acquire the information from the current drawing editor
        DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

        'initiate a new connection
        AcadConnection = New AcadConn

        AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        DLock = Application.DocumentManager.MdiActiveDocument.LockDocument
        Try
            Using DLock
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
    Private Sub LinetypesPresetting_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckProceed()
        LinetypesList.ClearSelection()
    End Sub

    'method open window linetypes presetting
    Public Sub OpenLinetypes(ByRef Linetypes As LinetypesPresetting, ByRef UIEnt As List(Of Entity))
        Dim LTCount As New Integer
        Dim LTNothingStat As Boolean = True

        'Using Linetypes
        For Each Ent As Entity In UIEnt
            AddToLinetypesTable(LTCount, Ent, Linetypes, LTNothingStat)
            LTCount = LTCount + 1
        Next

        Linetypes.Show()
        Linetypes.SetTopLevel(True)

        'If LTNothingStat = False And adskClass.AppPreferences.AutoRegLine = True Then
        '    Linetypes.Show()
        '    Linetypes.SetTopLevel(True)
        'Else
        '    Proceed_Click(Nothing, Nothing)
        'End If

        'End Using
    End Sub

    'add new line in LinetypesTable
    Private Sub AddToLinetypesTable(ByVal Count As Integer, ByVal AddedEnt As Entity, _
                       ByRef Table As LinetypesPresetting, ByRef LTNothingStat As Boolean)

        Dim NewRow As New System.Windows.Forms.DataGridViewRow
        Table.LinetypesList.Rows.Add(NewRow)
        Table.LinetypesList.Rows(Count).Cells("ObjectID").Value = AddedEnt.ObjectId
        Table.LinetypesList.Rows(Count).Cells("Number").Value = Count + 1
        Table.LinetypesList.Rows(Count).Cells("Layer").Value = AddedEnt.Layer.ToString

        If AddedEnt.Linetype.ToString.ToLower = "bylayer" Then
            Table.LinetypesList.Rows(Count).Cells("Linetype").Value = "NULL"
        ElseIf AddedEnt.Linetype.ToString.ToLower = "byblock" Then
            Table.LinetypesList.Rows(Count).Cells("Linetype").Value = AddedEnt.Linetype.ToString.ToUpper
            Table.LinetypesList.Rows(Count).Cells("Ignore").Value = True
            Table.LinetypesList.Rows(Count).DefaultCellStyle.BackColor = Drawing.Color.Gold
            Table.LinetypesList.Rows(Count).ReadOnly = True
        Else
            Table.LinetypesList.Rows(Count).Cells("Linetype").Value = AddedEnt.Linetype.ToString
        End If

        If AddedEnt.Color.ColorNameForDisplay.ToLower = "bylayer" Or AddedEnt.Color.ColorNameForDisplay.ToLower = "byblock" Then
            Table.LinetypesList.Rows(Count).Cells("Color").Value = AddedEnt.Color.ColorNameForDisplay.ToUpper
            Table.LinetypesList.Rows(Count).Cells("Ignore").Value = True
            Table.LinetypesList.Rows(Count).DefaultCellStyle.BackColor = Drawing.Color.Gold
        Else
            Table.LinetypesList.Rows(Count).Cells("Color").Value = AddedEnt.Color.ColorNameForDisplay.ToUpper
            Table.LinetypesList.Rows(Count).Cells("Ignore").Value = False
        End If

        If Table.LinetypesList.Rows(Count).Cells("Ignore").Value = False Then
            LTNothingStat = False
        End If

        Table.LinetypesList.Rows(Count).Cells("Solid").Value = False
        Table.LinetypesList.Rows(Count).Cells("Hidden").Value = False
        Table.LinetypesList.Rows(Count).Cells("Auxiliary").Value = False

    End Sub

    'highlight
    Private AcadConnection As AcadConn
    Private DrawEditor As Editor
    Private Entity As Entity
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


    Private zoom As AcadApplication

    'method for highlighting the selected entities
    Private Sub HighlightEntity(ByVal ObjectIdTmp As ObjectId, _
                                ByVal BlockTableRecInstances As BlockTableRecord, _
                                ByVal PastEntColor As List(Of InitialColor))

        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

        'create a loop checking for each objectid id autocad database
        For Each idTmp As ObjectId In BlockTableRecInstances
            'acquire the entity from the object id
            Entity = AcadConnection.myT.GetObject(idTmp, OpenMode.ForWrite)

            'test if the entity id were the same as entity id in the selected item
            If Entity.ObjectId = ObjectIdTmp Then

                EntityColor = New InitialColor
                EntityColor.ColorId = Entity.ObjectId
                EntityColor.ColorIndex = Entity.ColorIndex
                PastEntColor.Add(EntityColor)

                'change the color and the lineweight for making the highlights
                Entity.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByColor, 30)

                'zoom entity
                zoom = Application.AcadApplication
                Dim Min() As Double = New Double() {Entity.GeomExtents.MinPoint.X - 100, Entity.GeomExtents.MinPoint.Y - 100, 0}
                Dim Max() As Double = New Double() {Entity.GeomExtents.MaxPoint.X + 100, Entity.GeomExtents.MaxPoint.Y + 100, 0}
                zoom.ZoomWindow(Min, Max)

                'save the changed entity id
                TempId = Entity.Id.ToString
                Exit For
            End If
        Next
    End Sub
End Class