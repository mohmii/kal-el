Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.Windows
Imports System.Windows
Imports System.Math
Imports System.Linq

Imports FR

Public Class adskClass

    ' declare a paletteset object, this will only be created once
    Public Shared myPaletteSet As PaletteSet
    ' we need a palette which will be housed by the paletteSet
    Public Shared myPalette As UserControl3
    Public Shared TempName As String

    'we need an initial workspace value
    Public Shared AppPreferences As New AppPreferences

    ' palette command
    <CommandMethod("palette")> _
    Public Sub palette()

        ' check to see if it is valid
        Try
            If (myPaletteSet = Nothing) Then
                ' create a new palette set, with a unique guid
                myPaletteSet = New PaletteSet("Feature Recognition", New Guid("F5337918-A32C-4e7a-82A7-198F15F26662"))
                ' now create a palette inside, this has our tree control
                myPalette = New UserControl3
                ' now add the palette to the paletteset
                myPaletteSet.Add("Palette1", myPalette)
            Else
                If SelectionCommand.IdentifiedFeature.Count <> 0 Or SelectionCommand.UnIdentifiedFeature.Count <> 0 _
                Or SelectionCommand.ProjectionView.Count <> 0 Then
                    If MsgBox("There were still ongoing feature recognition process for current/last drawing." + vbCrLf + _
                              "Are you going to start a new one?", MsgBoxStyle.YesNo, "AcadFR") _
                              = MsgBoxResult.Yes Then

                        SelectionCommand.IdentifiedFeature.Clear()
                        SelectionCommand.UnIdentifiedFeature.Clear()
                        SelectionCommand.ProjectionView.Clear()

                        myPaletteSet.Dispose()
                        ' create a new palette set, with a unique guid
                        myPaletteSet = New PaletteSet("Feature Recognition", New Guid("F5337918-A32C-4e7a-82A7-198F15F26662"))
                        ' now create a palette inside, this has our tree control
                        myPalette = New UserControl3
                        ' now add the palette to the paletteset
                        myPaletteSet.Add("Palette1", myPalette)
                    End If
                End If

            End If

            'initiate the workspace value
            'AppPreferences.ReadPreferences()

        Catch ex As Exception
            MsgBox("Cannot load the feature list")
        End Try

        ' now display the paletteset
        myPaletteSet.Visible = True
        myPaletteSet.Dock = DockSides.Left
        myPaletteSet.KeepFocus = True

    End Sub

    'method for load the preference setting window
    <CommandMethod("pref")> _
    Public Sub Pref()
        Try
            Using AppPreferencesForm As New AppPreferencesForm
                AppPreferencesForm.ShowDialog()
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Shared ListForm As FRListForm

    'method for load the database interface
    <CommandMethod("loadform")> _
    Public Sub LoadForm()
        Try
            ListForm = New FRListForm
            Using ListForm
                ListForm.ShowDialog()
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private ed As Editor
    Private values2() As TypedValue
    Private sfilter2 As SelectionFilter
    Private Opts As PromptSelectionOptions = Nothing
    Private res As PromptSelectionResult
    Private SS As SelectionSet
    Public Shared IdArray As ObjectId()
    Public Shared ObjIDsClassify As List(Of ObjectId)
    Private AcConnector As AcadConn
    Private DbConnector As DatabaseConn


    Private TmpSetView As setView
    Private LineTypes As LinetypesPresetting
    Private Schematic As SchematicPresetting
    Private zoom As AcadApplication

    <CommandMethod("regent")> _
    Public Sub RegEnt()

        zoom = Application.AcadApplication
        zoom.ZoomAll()
        ed = Application.DocumentManager.MdiActiveDocument.Editor

        'create filter with only processing the surface define in the preferences
        Dim values2() As TypedValue = {New TypedValue(DxfCode.LayerName, 0)}
        sfilter2 = New SelectionFilter(values2)

        'selection option for not to select the same object
        Opts = New PromptSelectionOptions()
        Opts.AllowDuplicates = False
        Opts.MessageForAdding = "Select the entities that need to be checked:"

        'get the result of selection
        res = ed.GetSelection(Opts)
        If res.Status = PromptStatus.OK Then
            SS = res.Value
            IdArray = SS.GetObjectIds
            ObjIDsClassify = New List(Of ObjectId)

            For Each id As ObjectId In IdArray
                ObjIDsClassify.Add(id)
            Next

            AcConnector = New AcadConn

            AcConnector.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)

            Try
                Using AcConnector.myT

                    AcConnector.OpenBlockTableRec()
                    AcConnector.btr.UpgradeOpen()

                    DbConnector = New DatabaseConn
                    DbConnector.InitLinesDb()
                    DbConnector.InitHoleDb()

                    TmpSetView = New setView
                    setView.CBVisible = True
                    setView.CBHidden = True

                    ClasifiyEnt(ObjIDsClassify, AcConnector)

                    If LineEntities.Count <> 0 Then
                        If LineTypesStat = True And adskClass.AppPreferences.AutoRegLine = True Then
                            LineTypes = New LinetypesPresetting
                            LineTypes.OpenLinetypes(LineTypes, UIEntities)
                        End If
                    End If

                    If CircEntities.Count <> 0 Then
                        Dim getGroup = From item In CircEntities _
                                       Group item By CircXCenter = item.Center Into GroupMember = Group _
                                       Select Honor = GroupMember

                        'method for schematic presetting
                        Dim SchematicStat As Boolean = False
                        UI2CircList = New List(Of IEnumerable(Of Circle))
                        UI2CircListAll = New List(Of IEnumerable(Of Circle))
                        Dim UI2CircStat As Boolean
                        For Each result As IEnumerable(Of Circle) In getGroup
                            If result.Count = 2 And DbConnector.CheckTopTap(result) = False Then
                                SchematicStat = True
                                UI2CircStat = True
                                For Each UI2C As IEnumerable(Of Circle) In UI2CircList
                                    If CheckUI2Circle(result, UI2C) = False Then
                                        UI2CircStat = False
                                    End If
                                Next
                                If UI2CircStat = True Then
                                    If result.First.Radius > result.Last.Radius Then
                                        UI2CircList.Add(result)
                                    Else
                                        UI2CircList.Add(result.Reverse)
                                    End If
                                End If

                                If result.First.Radius > result.Last.Radius Then
                                    UI2CircListAll.Add(result)
                                Else
                                    UI2CircListAll.Add(result.Reverse)
                                End If

                            End If
                        Next

                        'membuka form jika ada kelompok 2 lingkaran yg belum teridentifikasi
                        If SchematicStat = True And adskClass.AppPreferences.SchematicSymbol = True Then
                            Schematic = New SchematicPresetting
                            Schematic.OpenSchematic(Schematic, UI2CircList)
                        End If
                    End If
                End Using

            Catch ex As Exception
                MsgBox(ex.ToString)
            Finally
                AcConnector.myT.Dispose()
            End Try
        End If

    End Sub

    Private id As ObjectId
    Private CircEntities As List(Of Circle)
    Private ArcEntities As List(Of Arc)
    Private PLEntities As List(Of Polyline)
    Public Shared LineEntities As List(Of Line)
    Public Shared AllEntities As List(Of Entity)
    Private Entity As Object

    'List of unidentified entities
    Private UIEntities As List(Of Entity)
    Public Shared UIEntitiesAll As List(Of Entity)
    Private UI2CircList As List(Of IEnumerable(Of Circle))
    Public Shared UI2CircListAll As List(Of IEnumerable(Of Circle))

    'Variabel untuk isi kosongnya LineTypes and SchematicPresetting
    Private LineTypesStat As Boolean

    Private MaxPoint, MinPoint As List(Of Point3d)
    Private ListOfDiameter As List(Of Double)
    Private ListOfCenter As List(Of Point3d)

    Private Sub ClasifiyEnt(ByVal Ids As List(Of ObjectId), ByVal AcadConnection As AcadConn)
        'collection of entities
        CircEntities = New List(Of Circle)
        LineEntities = New List(Of Line)
        ArcEntities = New List(Of Arc)
        PLEntities = New List(Of Polyline)
        AllEntities = New List(Of Entity)
        UIEntities = New List(Of Entity)
        UIEntitiesAll = New List(Of Entity)

        Dim ErasedEnt As New List(Of Entity)
        Dim ErasedEntIdx As New List(Of Integer)
        Dim LineTemp As Line
        Dim ent As Entity
        Dim ObjIDAdd As New List(Of ObjectId)
        Dim ObjIDRemoveIdx As New List(Of Integer)
        LineTypesStat = New Boolean

        MaxPoint = New List(Of Point3d)
        MinPoint = New List(Of Point3d)

        ListOfCenter = New List(Of Point3d)
        ListOfDiameter = New List(Of Double)

        Dim AcConnector2 As New AcadConn
        Dim UIEntStat As Boolean

        AcConnector2.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        Using AcConnector2.myT
            AcConnector2.OpenBlockTableRec()
            AcConnector2.btr.UpgradeOpen()

            'classify the entities into three categories (circle, line, and arc)
            For Each id In ObjIDsClassify
                Entity = AcConnector2.myT.GetObject(id, OpenMode.ForWrite, True)
                'if not entity and not auxiliary
                If DbConnector.CheckIfEntity(Entity) = False And DbConnector.CheckIfEntityAuxiliary(Entity) = False Then
                    'penentuan status LineTypePresetting
                    LineTypesStat = True
                    UIEntStat = True
                    For Each UIE As Entity In UIEntities
                        If UIE.Layer = Entity.Layer And UIE.Linetype = Entity.Linetype And UIE.Color = Entity.Color Then
                            UIEntStat = False
                        End If
                    Next

                    If UIEntStat = True Then
                        UIEntities.Add(Entity)
                    End If

                    UIEntitiesAll.Add(Entity)

                ElseIf DbConnector.CheckIfEntity(Entity) = False And DbConnector.CheckIfEntityAuxiliary(Entity) = True _
                And adskClass.AppPreferences.RemoveUEE = True Then
                    'erase auxiliary entity id remove unessential entities was checked
                    Entity.Erase()
                Else
                    'add circle, line and arc entities
                    If DbConnector.CheckIfEntity(Entity) = True And Not (TypeOf (Entity) Is DBPoint) And Not (TypeOf (Entity) Is DBText) Then
                        If TypeOf (Entity) Is Circle Then
                            CircEntities.Add(Entity)
                        ElseIf TypeOf (Entity) Is Line Then
                            LineTemp = Entity
                            If isequal(LineTemp.StartPoint.X, LineTemp.EndPoint.X) = True Then
                                ent = New Line(New Point3d(Round(LineTemp.StartPoint.X, 5), LineTemp.StartPoint.Y, 0), New Point3d(Round(LineTemp.EndPoint.X, 5), LineTemp.EndPoint.Y, 0))
                                ent.Layer = Entity.Layer
                                ent.Linetype = Entity.Linetype
                                ent.ColorIndex = Entity.ColorIndex
                                AcConnector2.btr.AppendEntity(ent)
                                AcConnector2.myT.AddNewlyCreatedDBObject(ent, True)
                                ObjIDAdd.Add(ent.ObjectId)
                                ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(Entity.ObjectId))
                                ErasedEnt.Add(Entity)
                                ErasedEntIdx.Add(ErasedEnt.IndexOf(Entity))
                                Entity.Erase()
                                Entity = ent
                            ElseIf isequal(LineTemp.StartPoint.Y, LineTemp.EndPoint.Y) = True Then
                                ent = New Line(New Point3d(LineTemp.StartPoint.X, Round(LineTemp.StartPoint.Y, 5), 0), New Point3d(LineTemp.EndPoint.X, Round(LineTemp.EndPoint.Y, 5), 0))
                                ent.Layer = Entity.Layer
                                ent.Linetype = Entity.Linetype
                                ent.ColorIndex = Entity.ColorIndex
                                AcConnector2.btr.AppendEntity(ent)
                                AcConnector2.myT.AddNewlyCreatedDBObject(ent, True)
                                ObjIDAdd.Add(ent.ObjectId)
                                ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(Entity.ObjectId))
                                ErasedEnt.Add(Entity)
                                ErasedEntIdx.Add(ErasedEnt.IndexOf(Entity))
                                Entity.Erase()
                                Entity = ent
                            End If
                            LineEntities.Add(Entity)
                        ElseIf TypeOf (Entity) Is Arc Then
                            ArcEntities.Add(Entity)
                        ElseIf TypeOf (Entity) Is Polyline Then
                            PLEntities.Add(Entity)
                        End If
                        MaxPoint.Add(Entity.GeomExtents.MaxPoint)
                        MinPoint.Add(Entity.GeomExtents.MinPoint)
                        AllEntities.Add(Entity)
                    End If
                End If
            Next id

            'remove object id
            ObjIDRemoveIdx.Sort()
            For i As Integer = (ObjIDRemoveIdx.Count - 1) To 0 Step (-1)
                ObjIDsClassify.RemoveAt(ObjIDRemoveIdx(i))
            Next

            'add object id
            For Each id In ObjIDAdd
                ObjIDsClassify.Add(id)
            Next

            AcConnector2.myT.Commit()
        End Using
        AcConnector2.myT.Dispose()
    End Sub

    Private Function isequal(ByVal x As Double, ByVal y As Double) As Boolean
        If Math.Abs(x - y) > adskClass.AppPreferences.ToleranceValues Then
            Return False
        Else
            Return True
        End If
    End Function

    'method for checking unidentifed two circle
    Private Function CheckUI2Circle(ByRef result0 As IEnumerable(Of Circle), ByRef result1 As IEnumerable(Of Circle)) As Boolean
        If result0.First.Radius > result0.Last.Radius Then
            If result0(0).Layer = result1(0).Layer And result0(0).Linetype = result1(0).Linetype And result0(0).Color = result1(0).Color _
            And result0(1).Layer = result1(1).Layer And result0(1).Linetype = result1(1).Linetype And result0(1).Color = result1(1).Color Then
                Return False
            Else
                Return True
            End If
        Else
            If result0(1).Layer = result1(0).Layer And result0(1).Linetype = result1(0).Linetype And result0(1).Color = result1(0).Color _
            And result0(0).Layer = result1(1).Layer And result0(0).Linetype = result1(1).Linetype And result0(0).Color = result1(1).Color Then
                Return False
            Else
                Return True
            End If
        End If
    End Function
End Class