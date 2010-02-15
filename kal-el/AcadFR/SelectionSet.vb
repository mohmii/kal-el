Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Interop
Imports System.Linq
Imports System.Math
Imports FR

'Namespace Selection
Public Class SelectionCommand

    'save the idarray as object id
    Public Shared IdArray As ObjectId()
    Private RefPoint As Point3d

    Public Shared CircMember As Integer

    'variable for pre-selection
    Private ed As Editor
    Private values2() As TypedValue
    Private sfilter2 As SelectionFilter
    Private Opts As PromptSelectionOptions = Nothing
    Private res As PromptSelectionResult
    Private SS As SelectionSet
    Private AcadConn As AcadConn

    Private id As ObjectId
    Private CircEntities As List(Of Circle)

    Public Shared LineEntities As List(Of Line)
    Public Shared AllEntities As List(Of Entity)

    Private ArcEntities As List(Of Arc)

    Private PLEntities As List(Of Polyline)
    Private MaxPoint, MinPoint As List(Of Point3d)
    Private obj As DBObject
    Private entity As Entity

    Private ListOfDiameter As List(Of Double)
    Private ListOfCenter As List(Of Point3d)

    Private Sub ClasifyEntities(ByVal AcadConnection As AcadConn, _
                                ByVal IdArrayName As ObjectId())

        'collection of entities
        CircEntities = New List(Of Circle)
        LineEntities = New List(Of Line)
        ArcEntities = New List(Of Arc)
        PLEntities = New List(Of Polyline)
        AllEntities = New List(Of Entity)

        MaxPoint = New List(Of Point3d)
        MinPoint = New List(Of Point3d)

        ListOfCenter = New List(Of Point3d)
        ListOfDiameter = New List(Of Double)

        'classify the entities into three categories (circle, line, and arc)
        For Each id In IdArrayName

            entity = AcadConn.myT.GetObject(id, OpenMode.ForWrite, True)
            If Check2Database.CheckIfEntity(entity) And Not (TypeOf (entity) Is DBPoint) Then
                If TypeOf (entity) Is Circle Then
                    CircEntities.Add(entity)
                ElseIf TypeOf (entity) Is Line Then
                    Dim LineTemp As Line
                    Dim ent As New List(Of Entity)
                    LineTemp = entity
                    If isequal(LineTemp.StartPoint.X, LineTemp.EndPoint.X) = True Then
                        ent.Add(New Line(New Point3d(Round(LineTemp.StartPoint.X, 5), LineTemp.StartPoint.Y, 0), New Point3d(Round(LineTemp.EndPoint.X, 5), LineTemp.EndPoint.Y, 0)))
                        ent(0).Layer = entity.Layer
                        ent(0).Linetype = entity.Linetype
                        ent(0).ColorIndex = entity.ColorIndex
                        entity.Erase()
                        entity = ent(0)
                        AcadConn.btr.AppendEntity(entity)
                        AcadConn.myT.AddNewlyCreatedDBObject(entity, True)
                    ElseIf isequal(LineTemp.StartPoint.Y, LineTemp.EndPoint.Y) = True Then
                        ent.Add(New Line(New Point3d(LineTemp.StartPoint.X, Round(LineTemp.StartPoint.Y, 5), 0), New Point3d(LineTemp.EndPoint.X, Round(LineTemp.EndPoint.Y, 5), 0)))
                        ent(0).Layer = entity.Layer
                        ent(0).Linetype = entity.Linetype
                        ent(0).ColorIndex = entity.ColorIndex
                        entity.Erase()
                        entity = ent(0)
                        AcadConn.btr.AppendEntity(entity)
                        AcadConn.myT.AddNewlyCreatedDBObject(entity, True)
                    End If
                    LineEntities.Add(entity)
                ElseIf TypeOf (entity) Is Arc Then
                    ArcEntities.Add(entity)
                ElseIf TypeOf (entity) Is Polyline Then
                    PLEntities.Add(entity)
                End If
                MaxPoint.Add(entity.GeomExtents.MaxPoint)
                MinPoint.Add(entity.GeomExtents.MinPoint)
                AllEntities.Add(entity)
            End If
        Next id
    End Sub

    'checking whether if the point is on the line or not
    Private Function PointOnline(ByVal PointToCheck As Point3d, ByVal StartPoint As Point3d, ByVal EndPoint As Point3d) As Integer

        If Abs((Round(EndPoint.Y, 3) - Round(StartPoint.Y, 3)) * (Round(PointToCheck.X, 3) - Round(StartPoint.X, 3)) - (Round(PointToCheck.Y, 3) - Round(StartPoint.Y, 3)) * (Round(EndPoint.X, 3) - Round(StartPoint.X, 3))) _
        >= Max(Abs(Round(EndPoint.X, 3) - Round(StartPoint.X, 3)), Abs(Round(EndPoint.Y, 3) - Round(StartPoint.Y, 3))) Then Return 0

        If (Round(EndPoint.X, 3) < Round(StartPoint.X, 3) And Round(StartPoint.X, 3) < Round(PointToCheck.X, 3)) Or (Round(EndPoint.Y, 3) < Round(StartPoint.Y, 3) And Round(StartPoint.Y, 3) < Round(PointToCheck.Y, 3)) _
        Then Return 1

        If (Round(PointToCheck.X, 3) < Round(StartPoint.X, 3) And Round(StartPoint.X, 3) < Round(EndPoint.X, 3)) Or (Round(PointToCheck.Y, 3) < Round(StartPoint.Y, 3) And Round(StartPoint.Y, 3) < Round(EndPoint.Y, 3)) _
        Then Return 1

        If (Round(StartPoint.X, 3) < Round(EndPoint.X, 3) And Round(EndPoint.X, 3) < Round(PointToCheck.X, 3)) Or (Round(StartPoint.Y, 3) < Round(EndPoint.Y, 3) And Round(EndPoint.Y, 3) < Round(PointToCheck.Y, 3)) _
        Then Return 3

        If (Round(PointToCheck.X, 3) < Round(EndPoint.X, 3) And Round(EndPoint.X, 3) < Round(StartPoint.X, 3)) Or (Round(PointToCheck.Y, 3) < Round(EndPoint.Y, 3) And Round(EndPoint.Y, 3) < Round(StartPoint.Y, 3)) _
        Then Return 3

        Return 2

    End Function

    Private BBMaxX, BBMaxY, BBMinX, BBMinY As Double
    Private line1, line2, line3, line4 As Line
    Private BoundingBoxLine(3) As Line

    Private Sub DefineBoundBox(ByVal MaxPList As List(Of Point3d), _
                               ByVal MinPList As List(Of Point3d))

        'create some dummy variables
        BBMaxX = New Double
        BBMaxX = Round(MaxPList(0).X, 5)
        BBMaxY = New Double
        BBMaxY = Round(MaxPList(0).Y, 5)
        BBMinX = New Double
        BBMinX = Round(MinPList(0).X, 5)
        BBMinY = New Double
        BBMinY = Round(MinPList(0).Y, 5)

        'search the bounding box maximum points
        For Each BoundPoint As Point3d In MaxPList
            If BBMaxX < BoundPoint.X Then
                BBMaxX = Round(BoundPoint.X, 5)
            End If

            If BBMaxY < BoundPoint.Y Then
                BBMaxY = Round(BoundPoint.Y, 5)
            End If
        Next

        'search the bounding box minimum points
        For Each BoundPoint As Point3d In MinPList
            If BoundPoint.X < BBMinX Then
                BBMinX = Round(BoundPoint.X, 5)
            End If

            If BoundPoint.Y < BBMinY Then
                BBMinY = Round(BoundPoint.Y, 5)
            End If
        Next
    End Sub

    Private PrPointOptions As PromptPointOptions
    Private PrPointResult As PromptPointResult

    Private Check2Database As DatabaseConn
    Private CheckResult() As String

    Private SplitResult As DBObjectCollection
    Private HiddenEntity As List(Of Entity)

    'method for processing selected entities
    <CommandMethod("OptionSel")> _
    Public Sub OptionSel()
        Dim Temp1 As New UserControl3
        'set the editor
        ed = Application.DocumentManager.MdiActiveDocument.Editor

        'create filter with only processing the surface define in the preferences
        Dim values2() As TypedValue = {New TypedValue(DxfCode.LayerName, 0)}
        sfilter2 = New SelectionFilter(values2)

        'selection option for not to select the same object
        Opts = New PromptSelectionOptions()
        Opts.AllowDuplicates = False

        'get the result of selection
        res = ed.GetSelection(Opts)
        If res.Status = PromptStatus.OK Then
            SS = res.Value
            IdArray = SS.GetObjectIds
        End If

        AcadConn = New AcadConn

        AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        'StartTransaction()

        Try
            Using AcadConn.myT
                Check2Database = New DatabaseConn

                AcadConn.OpenBlockTableRec()
                AcadConn.btr.UpgradeOpen()
                ClasifyEntities(AcadConn, IdArray)

                If MaxPoint.Count = 0 Then
                    MsgBox("[ERROR001] The recognizing process could not be continued." + vbCrLf + _
                           "There were problems in finding the appropriate outer vertices for creating the bounding box for this " + _
                           setView.viewis + " view." + vbCrLf + vbCrLf + _
                           "Hint: Please check the database for solid and hidden line definition.", MsgBoxStyle.Critical)
                    Exit Try
                Else
                    DefineBoundBox(MaxPoint, MinPoint)
                End If

                If (BBMaxX = -10000) Or (BBMaxY = -10000) Or (BBMinX = 10000) Or (BBMinY = 10000) Then
                    MsgBox("指定された投影図中の形状認識に問題が起こりました。" + vbCrLf + _
                           "FRデータベースを調べてください。" + vbCrLf + "登録されていない線種のプロパティがあるようです。" _
                           , MsgBoxStyle.Exclamation)
                Else
                    'AcadConn.OpenBlockTableRec()
                    Dim text As MText = New MText

                    Dim point As Point3d

                    point = New Point3d(BBMinX, (BBMaxY + 40), 0)

                    text.Contents = setView.viewis
                    text.ColorIndex = 10
                    text.TextHeight = 20
                    text.Location = point

                    BoundingBoxLine(0) = New Line(New Point3d(BBMinX, BBMinY, 0), New Point3d(BBMaxX, BBMinY, 0))
                    BoundingBoxLine(1) = New Line(New Point3d(BBMaxX, BBMinY, 0), New Point3d(BBMaxX, BBMaxY, 0))
                    BoundingBoxLine(2) = New Line(New Point3d(BBMaxX, BBMaxY, 0), New Point3d(BBMinX, BBMaxY, 0))
                    BoundingBoxLine(3) = New Line(New Point3d(BBMinX, BBMaxY, 0), New Point3d(BBMinX, BBMinY, 0))

                    'drawing the bounding box
                    For Each lineTmp As Line In BoundingBoxLine
                        lineTmp.ColorIndex = 10
                        lineTmp.Linetype = "DASHED"
                        'LineEntities.Add(lineTmp)
                        AcadConn.btr.AppendEntity(lineTmp)
                        AcadConn.myT.AddNewlyCreatedDBObject(lineTmp, True)
                    Next

                    AcadConn.btr.AppendEntity(text)
                    AcadConn.myT.AddNewlyCreatedDBObject(text, True)

                    AcadConn.myT.Commit()

                    'Save the reference points
                    PrPointResult = ed.GetPoint("基準点を指定:" + vbNewLine)
                    RefPoint = PrPointResult.Value

                    'set new instances for this view
                    ProjectView = New ViewProp

                    'set the bounding box parameter
                    SetTheBoundaryParameter(ProjectView, setView.viewis)

                    'set the reference point
                    SetTheReferencePoint(ProjectView, RefPoint, setView.viewis)

                    'set the view generation status to be true
                    ProjectView.GenerationStat = True

                    'register the new view to projection view collection
                    RegisterToViewCollection(ProjectionView, ProjectView)

                    'initiate a variable for placing the hidden feature
                    HiddenFeature = New List(Of OutputFormat)
                    HiddenEntity = New List(Of Entity)

                    'start processing for circle base entities
                    If CircEntities.Count <> 0 And setView.CBHole = True Then
                        Dim getGroup = From item In CircEntities _
                                       Group item By CircXCenter = item.Center Into GroupMember = Group _
                                       Select Honor = GroupMember

                        'initiate the supporting variable

                        CheckResult = Nothing
                        IdentifiedCounter = Nothing
                        UnIdentifiedCounter = Nothing

                        'create condition if the group consist of two circle, cause this group _
                        ' will be considered as tap c'bore
                        For Each result As IEnumerable(Of Circle) In getGroup

                            CircMember = result.Count()
                            Dim Surface As Integer = 3
                            If CircMember = 2 Then

                                'continue to identify tap and cbore hole with the fr database
                                CheckResult = Check2Database.CheckDoubleHole(result, Surface)

                                'checking the result
                                If IsNothing(CheckResult) Then

                                    'loop for each circle in group of circles
                                    For Each circle As Circle In result
                                        'create new unidentified feature
                                        Feature = New OutputFormat
                                        'set the feature property
                                        Feature.EntityMember = CircMember
                                        Feature.FeatureName = "Entity " + UnIdentifiedFeature.Count.ToString
                                        Feature.ObjectId.Add(circle.ObjectId)
                                        'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                                        Feature.MiscProp(0) = FindTheirJapsName("Drill")
                                        Feature.MiscProp(1) = CheckTheSurface(setView.viewis, Surface)
                                        Feature.OriginAndAddition(0) = SetXPosition(circle.Center.X, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(1) = SetYPosition(circle.Center.Y, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(2) = 0
                                        Feature.OriginAndAddition(3) = Round(circle.Radius * 2, 3)
                                        Feature.OriginAndAddition(4) = 0
                                        Feature.OriginAndAddition(5) = 0
                                        Feature.OriginAndAddition(6) = 0
                                        Feature.OriginAndAddition(7) = 0
                                        ListLoopTemp = New List(Of List(Of Entity))
                                        ListLoopTemp.Add(LoopTemp)
                                        Feature.ListLoop = ListLoopTemp
                                        If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(circle) Then
                                            HiddenFeature.Add(Feature)
                                            HiddenEntity.Add(circle)
                                        Else
                                            'add to the unidentified feature list
                                            UnIdentifiedFeature.Add(Feature)
                                            TmpUnidentifiedFeature.Add(Feature)
                                            'OrganizeList.AddListToExisting2(Feature)
                                            UnIdentifiedCounter = UnIdentifiedCounter + 1
                                            AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                                        End If
                                    Next
                                Else
                                    'create new feature for cbore or tap
                                    Feature = New OutputFormat
                                    'set the feature property
                                    Feature.EntityMember = CircMember
                                    Feature.FeatureName = CheckResult(0)
                                    Feature.ObjectId.Add(result.First.ObjectId)
                                    Feature.ObjectId.Add(result.Last.ObjectId)
                                    'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                                    Feature.MiscProp(0) = FindTheirJapsName(CheckResult(0))
                                    Feature.MiscProp(1) = CheckTheSurface(setView.viewis, Surface)
                                    Feature.OriginAndAddition(0) = SetXPosition(result.FirstOrDefault.Center.X, Feature.MiscProp(1))
                                    Feature.OriginAndAddition(1) = SetYPosition(result.FirstOrDefault.Center.Y, Feature.MiscProp(1))
                                    Feature.OriginAndAddition(2) = 0
                                    Feature.OriginAndAddition(3) = CheckResult(3)
                                    Feature.OriginAndAddition(4) = CheckResult(4)
                                    Feature.OriginAndAddition(5) = CheckResult(5)
                                    Feature.OriginAndAddition(6) = CheckResult(6)
                                    Feature.OriginAndAddition(7) = 0
                                    ListLoopTemp = New List(Of List(Of Entity))
                                    ListLoopTemp.Add(LoopTemp)
                                    Feature.ListLoop = ListLoopTemp
                                    If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(result.FirstOrDefault) Then
                                        HiddenFeature.Add(Feature)
                                        HiddenEntity.Add(result.FirstOrDefault)
                                    Else
                                        'add to the identified feature list
                                        IdentifiedFeature.Add(Feature)
                                        TmpIdentifiedFeature.Add(Feature)
                                        'OrganizeList.AddListToExisting(Feature)
                                        IdentifiedCounter = IdentifiedCounter + 1
                                        AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)
                                    End If
                                End If
                            Else
                                If CircMember = 1 Then
                                    'check if the group is ream or not

                                    If Check2Database.CheckIfReam(result) Then
                                        'set the checkresult to ream
                                        'CheckResult = New String() {"Ream", result.SingleOrDefault.Id.ToString(), ""}
                                        CheckResult = Check2Database.WhichReam(result)
                                        'create new feature
                                        Feature = New OutputFormat
                                        'set the feature property
                                        Feature.EntityMember = CircMember
                                        Feature.FeatureName = CheckResult(0)
                                        Feature.ObjectId.Add(result.FirstOrDefault.ObjectId)
                                        'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                                        Feature.MiscProp(0) = FindTheirJapsName(CheckResult(0))
                                        Feature.MiscProp(1) = setView.viewis
                                        Feature.OriginAndAddition(0) = SetXPosition(result.FirstOrDefault.Center.X, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(1) = SetYPosition(result.FirstOrDefault.Center.Y, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(2) = 0
                                        Feature.OriginAndAddition(3) = CheckResult(2)
                                        Feature.OriginAndAddition(4) = CheckResult(3)
                                        Feature.OriginAndAddition(5) = 0
                                        Feature.OriginAndAddition(6) = 0
                                        Feature.OriginAndAddition(7) = 0
                                        ListLoopTemp = New List(Of List(Of Entity))
                                        ListLoopTemp.Add(LoopTemp)
                                        Feature.ListLoop = ListLoopTemp
                                        If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(result.FirstOrDefault) Then
                                            HiddenFeature.Add(Feature)
                                            HiddenEntity.Add(result.FirstOrDefault)
                                        Else
                                            'add to the identified feature list
                                            IdentifiedFeature.Add(Feature)
                                            TmpIdentifiedFeature.Add(Feature)
                                            'OrganizeList.AddListToExisting(Feature)
                                            IdentifiedCounter = IdentifiedCounter + 1
                                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)
                                        End If
                                    Else
                                        'create a new unidentified feature
                                        Feature = New OutputFormat
                                        'set the faeture property
                                        Feature.EntityMember = CircMember
                                        Feature.FeatureName = "Drill"
                                        Feature.ObjectId.Add(result.FirstOrDefault.ObjectId)
                                        'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                                        Feature.MiscProp(0) = FindTheirJapsName("Drill")
                                        Feature.MiscProp(1) = setView.viewis
                                        Feature.OriginAndAddition(0) = SetXPosition(result.FirstOrDefault.Center.X, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(1) = SetYPosition(result.FirstOrDefault.Center.Y, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(2) = 0
                                        Feature.OriginAndAddition(3) = Round(result.FirstOrDefault.Radius * 2, 3)
                                        Feature.OriginAndAddition(4) = 0
                                        Feature.OriginAndAddition(5) = 0
                                        Feature.OriginAndAddition(6) = 0
                                        Feature.OriginAndAddition(7) = 0
                                        ListLoopTemp = New List(Of List(Of Entity))
                                        ListLoopTemp.Add(LoopTemp)
                                        Feature.ListLoop = ListLoopTemp
                                        If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(result.FirstOrDefault) Then
                                            HiddenFeature.Add(Feature)
                                            HiddenEntity.Add(result.FirstOrDefault)
                                        Else
                                            'add to the unidentified feature list
                                            IdentifiedFeature.Add(Feature)
                                            TmpIdentifiedFeature.Add(Feature)
                                            'OrganizeList.AddListToExisting(Feature)
                                            IdentifiedCounter = IdentifiedCounter + 1
                                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)
                                        End If
                                    End If

                                Else
                                    'loop for each circle in group of circles
                                    For Each circle As Circle In result
                                        'create new unidentified feature
                                        Feature = New OutputFormat
                                        'set the feature property
                                        Feature.EntityMember = CircMember
                                        Feature.FeatureName = "Entity " + UnIdentifiedFeature.Count.ToString
                                        Feature.ObjectId.Add(circle.ObjectId)
                                        'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                                        Feature.MiscProp(0) = FindTheirJapsName("Drill")
                                        Feature.MiscProp(1) = setView.viewis
                                        Feature.OriginAndAddition(0) = SetXPosition(circle.Center.X, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(1) = SetYPosition(circle.Center.Y, Feature.MiscProp(1))
                                        Feature.OriginAndAddition(2) = 0
                                        Feature.OriginAndAddition(3) = Round(circle.Radius * 2, 3)
                                        Feature.OriginAndAddition(4) = 0
                                        Feature.OriginAndAddition(5) = 0
                                        Feature.OriginAndAddition(6) = 0
                                        Feature.OriginAndAddition(7) = 0
                                        ListLoopTemp = New List(Of List(Of Entity))
                                        ListLoopTemp.Add(LoopTemp)
                                        Feature.ListLoop = ListLoopTemp
                                        If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(circle) Then
                                            HiddenFeature.Add(Feature)
                                            HiddenEntity.Add(circle)
                                        Else
                                            'add to the unidentified feature list
                                            UnIdentifiedFeature.Add(Feature)
                                            TmpUnidentifiedFeature.Add(Feature)
                                            'OrganizeList.AddListToExisting2(Feature)
                                            UnIdentifiedCounter = UnIdentifiedCounter + 1
                                            AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                                        End If
                                    Next
                                End If
                            End If

                            'set the current feature to current view
                            RegisterToView(Feature)

                        Next
                    End If

                    'start processing for milling features
                    If setView.CBMill = True Then
                        Try

                            AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
                            AcadConn.OpenBlockTableRec()
                            AcadConn.btr.UpgradeOpen()

                            'variable for breaking lines
                            Dim PointStatus As Boolean = True
                            Dim PointCol As Point3dCollection
                            Dim EntityIndex, LineIndex As List(Of Integer)
                            Dim AddedEntities, RemovedEntities As New List(Of Entity)

                            'break point in the middle of the line
                            While PointStatus = True
                                PointStatus = False
                                AllPoints = New List(Of Point3d)
                                GroupOfEntity = New List(Of AllPoints)
                                UnAdjacentPoints = New List(Of Point3d)
                                GetPoints = New GetPoints
                                EntityIndex = New List(Of Integer)
                                LineIndex = New List(Of Integer)
                                GeometryProcessor = New GeometryProcessor
                                AddedEntities = New List(Of Entity)
                                RemovedEntities = New List(Of Entity)

                                GetPoints.UnAdjacentPointExtractor(AllEntities, AllPoints, GroupOfEntity, UnAdjacentPoints)

                                For Each PointTemp As Point3d In AllPoints
                                    If GroupOfEntity(AllPoints.IndexOf(PointTemp)).EntityList.Count = 1 Then
                                        For Each EntityTemp As Entity In AllEntities
                                            If TypeOf EntityTemp Is Line Then
                                                Dim LineTemp As New Line
                                                LineTemp = EntityTemp
                                                If isequalpoint(PointTemp, LineTemp.StartPoint) = False _
                                                And isequalpoint(PointTemp, LineTemp.EndPoint) = False And _
                                                PointOnline(PointTemp, LineTemp.StartPoint, LineTemp.EndPoint) = 2 _
                                                And (Not RemovedEntities.Contains(EntityTemp)) Then
                                                    PointCol = New Point3dCollection

                                                    If isequal(LineTemp.StartPoint.X, LineTemp.EndPoint.X) = True Then
                                                        PointCol.Add(PointTemp)
                                                    ElseIf isequal(LineTemp.StartPoint.Y, LineTemp.EndPoint.Y) = True Then
                                                        PointCol.Add(PointTemp)
                                                    Else
                                                        Dim LineTemp2 As Line
                                                        LineTemp2 = GroupOfEntity(AllPoints.IndexOf(PointTemp)).EntityList.Item(0).Line
                                                        If isequalpoint(PointTemp, LineTemp2.StartPoint) = True Then
                                                            PointCol.Add(LineTemp2.StartPoint)
                                                        ElseIf isequalpoint(PointTemp, LineTemp2.EndPoint) = True Then
                                                            PointCol.Add(LineTemp2.EndPoint)
                                                        End If
                                                    End If

                                                    SplitResult = GeometryProcessor.BreakingLine(LineTemp, PointCol)

                                                    AddedEntities.Add(SplitResult(0))
                                                    AddedEntities.Add(SplitResult(1))
                                                    RemovedEntities.Add(EntityTemp)
                                                    EntityIndex.Add(AllEntities.IndexOf(EntityTemp))
                                                    LineIndex.Add(LineEntities.IndexOf(LineTemp))

                                                    PointStatus = True
                                                    Exit For
                                                End If
                                            End If
                                            'end if type of entity
                                        Next
                                        ' for untuk all entities
                                    End If
                                    'end if jumlah entitas di titik = 1
                                Next

                                'membuang dan menambah jika ada entitas yang di-split
                                If PointStatus = True Then
                                    'membuang entitas yang di-split
                                    EntityIndex.Sort()
                                    For i As Integer = (EntityIndex.Count - 1) To 0 Step (-1)
                                        entity = AcadConn.myT.GetObject(AllEntities(EntityIndex(i)).ObjectId, OpenMode.ForWrite, True)
                                        entity.Erase()
                                        AllEntities.RemoveAt(EntityIndex(i))
                                    Next

                                    'membuang garis yang di-split
                                    LineIndex.Sort()
                                    For j As Integer = (LineIndex.Count - 1) To 0 Step (-1)
                                        LineEntities.RemoveAt(LineIndex(j))
                                    Next

                                    'menambah entitas hasil split
                                    For k As Integer = 0 To AddedEntities.Count - 1
                                        AllEntities.Add(AddedEntities(k))
                                        LineEntities.Add(AddedEntities(k))
                                    Next
                                End If

                            End While

                            'breaking two intersecting lines
                            PointStatus = True
                            While PointStatus = True
                                PointStatus = False
                                AllPoints = New List(Of Point3d)
                                GroupOfEntity = New List(Of AllPoints)
                                UnAdjacentPoints = New List(Of Point3d)
                                GetPoints = New GetPoints
                                EntityIndex = New List(Of Integer)
                                LineIndex = New List(Of Integer)
                                AddedEntities = New List(Of Entity)
                                RemovedEntities = New List(Of Entity)

                                GetPoints.UnAdjacentPointExtractor(AllEntities, AllPoints, GroupOfEntity, UnAdjacentPoints)

                                For i As Integer = 0 To AllEntities.Count - 1
                                    If TypeOf AllEntities(i) Is Line Then
                                        Dim LineTemp1 As New Line
                                        LineTemp1 = AllEntities(i)
                                        For j As Integer = i + 1 To AllEntities.Count - 1
                                            If TypeOf AllEntities(j) Is Line Then
                                                Dim LineTemp2 As New Line
                                                LineTemp2 = AllEntities(j)
                                                If GeometryProcessor.IsIntersect(LineTemp1, LineTemp2) = True And isequalpoint(LineTemp1.StartPoint, LineTemp2.StartPoint) = False _
                                                And isequalpoint(LineTemp1.StartPoint, LineTemp2.EndPoint) = False And isequalpoint(LineTemp1.EndPoint, LineTemp2.StartPoint) = False _
                                                And isequalpoint(LineTemp1.EndPoint, LineTemp2.EndPoint) = False And (Not RemovedEntities.Contains(AllEntities(i))) And (Not RemovedEntities.Contains(AllEntities(j))) Then
                                                    PointCol = New Point3dCollection
                                                    PointCol.Add(GeometryProcessor.IntersectionPoint(LineTemp1, LineTemp2))
                                                    If (PointOnline(PointCol(0), LineTemp1.StartPoint, LineTemp1.EndPoint) <> 2 Or PointOnline(PointCol(0), LineTemp2.StartPoint, LineTemp2.EndPoint) <> 2) _
                                                    And GeometryProcessor.IsIntersect(LineTemp2, LineTemp1) = True Then
                                                        PointCol = New Point3dCollection
                                                        PointCol.Add(GeometryProcessor.IntersectionPoint(LineTemp2, LineTemp1))
                                                    End If

                                                    SplitResult = GeometryProcessor.BreakingLine(LineTemp1, PointCol)
                                                    AddedEntities.Add(SplitResult(0))
                                                    AddedEntities.Add(SplitResult(1))
                                                    RemovedEntities.Add(AllEntities(i))
                                                    EntityIndex.Add(i)
                                                    LineIndex.Add(LineEntities.IndexOf(LineTemp1))

                                                    SplitResult = GeometryProcessor.BreakingLine(LineTemp2, PointCol)
                                                    AddedEntities.Add(SplitResult(0))
                                                    AddedEntities.Add(SplitResult(1))
                                                    RemovedEntities.Add(AllEntities(j))
                                                    EntityIndex.Add(j)
                                                    LineIndex.Add(LineEntities.IndexOf(LineTemp2))

                                                    PointStatus = True
                                                    Exit For
                                                End If
                                            End If
                                        Next

                                    End If
                                Next

                                'membuang dan menambah jika ada entitas yang di-split
                                If PointStatus = True Then
                                    'membuang entitas yang di-split
                                    EntityIndex.Sort()
                                    For i As Integer = (EntityIndex.Count - 1) To 0 Step (-1)
                                        entity = AcadConn.myT.GetObject(AllEntities(EntityIndex(i)).ObjectId, OpenMode.ForWrite, True)
                                        entity.Erase()
                                        AllEntities.RemoveAt(EntityIndex(i))
                                    Next

                                    'membuang garis yang di-split
                                    LineIndex.Sort()
                                    For j As Integer = (LineIndex.Count - 1) To 0 Step (-1)
                                        LineEntities.RemoveAt(LineIndex(j))
                                    Next

                                    'menambah entitas hasil split
                                    For k As Integer = 0 To AddedEntities.Count - 1
                                        AllEntities.Add(AddedEntities(k))
                                        LineEntities.Add(AddedEntities(k))
                                    Next

                                End If
                            End While

                            'variables prepared for generate virtual lines
                            Dim LineTrans As Line
                            Dim LinesOnBound As New List(Of Line)
                            Dim PointsOnBound As New List(Of Point3d)
                            Dim VirtualLines As New List(Of Line)
                            Dim BoundingBoxPoints As New List(Of Point3d)

                            BoundingBoxPoints.Add(New Point3d(BBMinX, BBMinY, 0))
                            BoundingBoxPoints.Add(New Point3d(BBMaxX, BBMinY, 0))
                            BoundingBoxPoints.Add(New Point3d(BBMinX, BBMaxY, 0))
                            BoundingBoxPoints.Add(New Point3d(BBMaxX, BBMaxY, 0))

                            For Each EntityTemp As Entity In AllEntities
                                If TypeOf EntityTemp Is Line Then
                                    LineTrans = New Line
                                    LineTrans = EntityTemp
                                    If (isequal(LineTrans.StartPoint.X, LineTrans.EndPoint.X) = True And (isequal(LineTrans.StartPoint.X, BBMaxX) = True Or isequal(LineTrans.StartPoint.X, BBMinX) = True)) _
                                    Or (isequal(LineTrans.StartPoint.Y, LineTrans.EndPoint.Y) = True And (isequal(LineTrans.StartPoint.Y, BBMaxY) = True Or isequal(LineTrans.StartPoint.Y, BBMinY) = True)) Then
                                        LinesOnBound.Add(LineTrans)
                                        If (Not PointsOnBound.Contains(LineTrans.StartPoint)) And isequalpoint(LineTrans.StartPoint, BoundingBoxPoints(0)) = False _
                                        And isequalpoint(LineTrans.StartPoint, BoundingBoxPoints(1)) = False And isequalpoint(LineTrans.StartPoint, BoundingBoxPoints(2)) = False _
                                        And isequalpoint(LineTrans.StartPoint, BoundingBoxPoints(3)) = False Then
                                            PointsOnBound.Add(LineTrans.StartPoint)
                                        End If

                                        If (Not PointsOnBound.Contains(LineTrans.EndPoint)) And isequalpoint(LineTrans.EndPoint, BoundingBoxPoints(0)) = False _
                                        And isequalpoint(LineTrans.EndPoint, BoundingBoxPoints(1)) = False And isequalpoint(LineTrans.EndPoint, BoundingBoxPoints(2)) = False _
                                        And isequalpoint(LineTrans.EndPoint, BoundingBoxPoints(3)) = False Then
                                            PointsOnBound.Add(LineTrans.EndPoint)
                                        End If

                                    End If
                                End If
                            Next

                            For Each LineTmp As Line In BoundingBoxLine
                                VirtualLines.Add(LineTmp)
                            Next

                            'break bounding box in every point
                            Dim PointIndex As List(Of Integer)
                            Dim SplitBB As List(Of Line)

                            While PointsOnBound.Count > 0
                                PointStatus = False
                                EntityIndex = New List(Of Integer)
                                PointIndex = New List(Of Integer)
                                GeometryProcessor = New GeometryProcessor

                                For Each PointTemp As Point3d In PointsOnBound
                                    For Each LineTemp As Line In VirtualLines
                                        If PointOnline(PointTemp, LineTemp.StartPoint, LineTemp.EndPoint) = 2 And isequalpoint(PointTemp, LineTemp.StartPoint) = False _
                                        And isequalpoint(PointTemp, LineTemp.EndPoint) = False Then
                                            SplitBB = New List(Of Line)
                                            SplitBB = LineBreak(LineTemp, PointTemp)

                                            VirtualLines.Add(SplitBB(0))
                                            VirtualLines.Add(SplitBB(1))
                                            EntityIndex.Add(VirtualLines.IndexOf(LineTemp))
                                            PointIndex.Add(PointsOnBound.IndexOf(PointTemp))

                                            PointStatus = True
                                            Exit For
                                        End If

                                    Next

                                    If PointStatus = True Then
                                        VirtualLines.RemoveAt(EntityIndex(0))
                                        Exit For
                                    End If
                                Next

                                If PointStatus = True Then
                                    PointsOnBound.RemoveAt(PointIndex(0))
                                End If

                            End While

                            Dim LineOnBoundIndex, VirtualLineIndex As List(Of Integer)
                            While LinesOnBound.Count > 0
                                LineOnBoundIndex = New List(Of Integer)
                                VirtualLineIndex = New List(Of Integer)
                                For Each LineTemp As Line In LinesOnBound
                                    For Each LineTemp2 As Line In VirtualLines
                                        If (isequalpoint(LineTemp.StartPoint, LineTemp2.StartPoint) = True And isequalpoint(LineTemp.EndPoint, LineTemp2.EndPoint) = True) _
                                        Or (isequalpoint(LineTemp.StartPoint, LineTemp2.EndPoint) = True And isequalpoint(LineTemp.EndPoint, LineTemp2.StartPoint) = True) Then
                                            VirtualLineIndex.Add(VirtualLines.IndexOf(LineTemp2))
                                            Exit For
                                        End If
                                    Next
                                    VirtualLines.RemoveAt(VirtualLineIndex(0))
                                    LineOnBoundIndex.Add(LinesOnBound.IndexOf(LineTemp))
                                    Exit For
                                Next
                                LinesOnBound.RemoveAt(LineOnBoundIndex(0))
                            End While

                            'drawing the virtual lines
                            For Each lineTmp As Line In VirtualLines
                                lineTmp.ColorIndex = 6
                                'lineTmp.Layer = 2
                                lineTmp.Linetype = "CONTINUOUS"
                                AcadConn.btr.AppendEntity(lineTmp)
                                AcadConn.myT.AddNewlyCreatedDBObject(lineTmp, True)
                                LineEntities.Add(lineTmp)
                                AllEntities.Add(lineTmp)
                            Next

                            For Each lineTmp As Line In BoundingBoxLine
                                entity = AcadConn.myT.GetObject(lineTmp.ObjectId, OpenMode.ForWrite, True)
                                entity.Erase()
                            Next

                            BoundingBoxLine(0) = New Line(New Point3d(BBMinX, BBMinY, 0), New Point3d(BBMaxX, BBMinY, 0))
                            BoundingBoxLine(1) = New Line(New Point3d(BBMaxX, BBMinY, 0), New Point3d(BBMaxX, BBMaxY, 0))
                            BoundingBoxLine(2) = New Line(New Point3d(BBMaxX, BBMaxY, 0), New Point3d(BBMinX, BBMaxY, 0))
                            BoundingBoxLine(3) = New Line(New Point3d(BBMinX, BBMaxY, 0), New Point3d(BBMinX, BBMinY, 0))

                            'redrawing the bounding box
                            For Each lineTmp As Line In BoundingBoxLine
                                lineTmp.ColorIndex = 10
                                lineTmp.Linetype = "DASHED"
                                'LineEntities.Add(lineTmp)
                                AcadConn.btr.AppendEntity(lineTmp)
                                AcadConn.myT.AddNewlyCreatedDBObject(lineTmp, True)
                            Next

                            AcadConn.myT.Commit()

                        Catch ex As Exception
                            'show the error
                            MsgBox(ex.ToString)
                        Finally
                            'dispose the transaction
                            AcadConn.myT.Dispose()
                        End Try

                        ProjectionView(ProjectionView.IndexOf(ProjectView)).LineEntities = LineEntities
                        ProjectionView(ProjectionView.IndexOf(ProjectView)).Entities = AllEntities

                        If ProjectionView.Count = 1 Then
                            'Counter = New Integer
                            MillingProcessor = New MillingProcessor

                            MainLoop = New List(Of Entity)
                            GroupLoop = New List(Of List(Of Entity))
                            GroupLoopPoints = New List(Of List(Of Point3d))

                            MillingProcessor.LoopFinder(ProjectionView(0).Entities, GroupLoop, GroupLoopPoints, MainLoop)
                            ProjectionView(0).MainLoop = MainLoop
                            ProjectionView(0).GroupLoop = GroupLoop
                            ProjectionView(0).GroupLoopPoints = GroupLoopPoints

                            ViewProcessor = New ViewProcessor
                            ViewProcessor.SingleViewProcessor(ProjectionView(0), UnIdentifiedFeature, TmpUnidentifiedFeature, UnIdentifiedCounter)

                        ElseIf ProjectionView.Count > 1 Then
                            'Loop Finder for new view added
                            MillingProcessor = New MillingProcessor

                            MainLoop = New List(Of Entity)
                            GroupLoop = New List(Of List(Of Entity))
                            GroupLoopPoints = New List(Of List(Of Point3d))

                            MillingProcessor.LoopFinder(ProjectionView(ProjectionView.Count - 1).Entities, GroupLoop, GroupLoopPoints, MainLoop)
                            ProjectionView(ProjectionView.Count - 1).MainLoop = MainLoop
                            ProjectionView(ProjectionView.Count - 1).GroupLoop = GroupLoop
                            ProjectionView(ProjectionView.Count - 1).GroupLoopPoints = GroupLoopPoints

                            'FR multiple vuew milling
                            ViewProcessor = New ViewProcessor
                            ViewProcessor.MultipleViewProcessor(ProjectionView, ProjectionView.Count - 1, UnIdentifiedFeature, TmpUnidentifiedFeature, _
                                                                UnIdentifiedCounter, IdentifiedFeature, TmpIdentifiedFeature, IdentifiedCounter)

                        End If
                    End If

                    'check the polyline features
                    If PLEntities.Count <> 0 And setView.CBPoly = True Then
                        For Each PolyTemp As Polyline In PLEntities
                            Feature = New OutputFormat

                            Feature.ObjectId.Add(PolyTemp.ObjectId)
                            Feature.MiscProp(0) = "POLYLINE"
                            Feature.MiscProp(1) = setView.viewis

                            If adskClass.AppPreferences.HiddenSurface = True And Check2Database.CheckIfEntityHidden(PolyTemp) Then
                                HiddenFeature.Add(Feature)
                                HiddenEntity.Add(PolyTemp)
                            Else
                                'add to the unidentified feature list
                                UnIdentifiedFeature.Add(Feature)
                                TmpUnidentifiedFeature.Add(Feature)
                                UnIdentifiedCounter = UnIdentifiedCounter + 1
                                AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                            End If
                        Next
                    End If

                    'check the hidden feature
                    If HiddenFeature.Count <> 0 And setView.CBHidden = True Then
                        If MsgBox(HiddenFeature.Count.ToString + " 個の隠れ線形状が見つかりました。" _
                               + vbCrLf + vbCrLf _
                               + "これらを無視しても良いですか？", MsgBoxStyle.YesNo, "AcadFR - Hidden Feature") = MsgBoxResult.No Then

                            If MsgBox("これらの形状を反対側の面に設定しますがよいですか？", MsgBoxStyle.YesNo, _
                                                          "AcadFR - Hidden Feature") = MsgBoxResult.Yes Then

                                ChangeTheRefPoint(RefPoint)
                                ProjectView = New ViewProp
                                SetTheBoundaryParameter(ProjectView, SearchOppositeSurf(setView.viewis))
                                SetTheReferencePoint(ProjectView, RefPoint, ProjectView.ViewType)

                                ProjectView.GenerationStat = False

                                RegisterToViewCollection(ProjectionView, ProjectView, HiddenFeature)

                                'ProjectionView.Add(ProjectView)

                                'For Each HideFeature As OutputFormat In HiddenFeature
                                '    If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Circle Then
                                '        HideFeature.OriginAndAddition(0) = SetXHiddenFeaturePosition(HideFeature.OriginAndAddition(0), ProjectView.BoundProp(4), ProjectView.ViewType)
                                '        HideFeature.OriginAndAddition(1) = SetYHiddenFeaturePosition(HideFeature.OriginAndAddition(1), ProjectView.BoundProp(5), ProjectView.ViewType)
                                '    End If

                                '    If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Polyline Then

                                '    End If

                                '    HideFeature.MiscProp(1) = ProjectView.ViewType.ToUpper
                                '    UnIdentifiedFeature.Add(HideFeature)
                                '    UnIdentifiedCounter = UnIdentifiedCounter + 1
                                '    AddToTable(HideFeature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)

                                '    RegisterToView(HideFeature)

                                'Next

                                adskClass.myPalette.ComboBox2.Items.Add(ProjectView.ViewType.ToUpper)
                            End If
                        End If
                    End If

                    'report the recognition results
                    If IdentifiedFeature.Count <> 0 Or UnIdentifiedFeature.Count <> 0 Then
                        ReportTheFindings(IdentifiedCounter, UnIdentifiedCounter)
                    Else
                        MsgBox("この投影図には、円が見当たりませんでした。" + setView.viewis + " view")
                    End If
                End If
            End Using
        Catch ex As Exception
            'show the error
            MsgBox(ex.ToString)
        Finally
            'dispose the transaction
            AcadConn.myT.Dispose()
        End Try

    End Sub

    'report how many features that was add to the table
    Private Sub ReportTheFindings(ByVal Identified As String, ByVal Unidentified As String)
        adskClass.myPalette.IdentifiedFeature.ClearSelection()
        adskClass.myPalette.UnidentifiedFeature.ClearSelection()
        adskClass.myPalette.MakeItBlank()

        MsgBox(Identified.ToString + "  認識できた形状のリストに追加される形状" + vbCrLf _
                               + Unidentified.ToString + "  その他の形状のリストに追加される形状")
    End Sub

    Private TmpValue As Double

    'Change the features coordinates inside the saved view
    Private Overloads Sub ChangeFeatCoordinate(ByRef ListedView As ViewProp, ByVal NewView As ViewProp)

        For Each FeatureTmp As OutputFormat In ListedView.GetFeature
            'change the X coordinate by the current reference point position
            TmpValue = New Double
            TmpValue = FeatureTmp.OriginAndAddition(0) 'get the X coordinate

            If ListedView.RefProp(0) - NewView.RefProp(0) > 0 Then
                'new reference point is in X minimum
                FeatureTmp.OriginAndAddition(0) = NewView.BoundProp(4) - Abs(TmpValue)
            ElseIf ListedView.RefProp(0) - NewView.RefProp(0) < 0 Then
                'new reference point is in X maximum
                FeatureTmp.OriginAndAddition(0) = -1 * (NewView.BoundProp(4) - TmpValue)
            End If

            'change the Y coordinate by the current reference point position
            TmpValue = New Double
            TmpValue = FeatureTmp.OriginAndAddition(1) 'get the Y coordinate

            If ListedView.RefProp(1) - NewView.RefProp(1) > 0 Then
                'new reference point is in Y minimum
                FeatureTmp.OriginAndAddition(1) = NewView.BoundProp(5) - Abs(TmpValue)
            ElseIf ListedView.RefProp(1) - NewView.RefProp(1) < 0 Then
                'new reference point is in Y maximum
                FeatureTmp.OriginAndAddition(1) = -1 * (NewView.BoundProp(5) - TmpValue)
            End If
        Next
    End Sub

    'change the faeture coordinates inside the hidden view
    Private Overloads Sub ChangeFeatCoordinate(ByVal ListedView As ViewProp, ByVal HiddenView As ViewProp, _
                                               ByRef HiddenFeature As OutputFormat)

        'change the X coordinate by the current reference point position
        TmpValue = New Double
        TmpValue = HiddenFeature.OriginAndAddition(0) 'get the X coordinate

        If HiddenView.RefProp(0) - ListedView.RefProp(0) > 0 Then
            'new reference point is in X minimum
            HiddenFeature.OriginAndAddition(0) = ListedView.BoundProp(4) - Abs(TmpValue)
        ElseIf HiddenView.RefProp(0) - ListedView.RefProp(0) < 0 Then
            'new reference point is in X maximum
            HiddenFeature.OriginAndAddition(0) = -1 * (ListedView.BoundProp(4) - TmpValue)
        End If

        'change the Y coordinate by the current reference point position
        TmpValue = New Double
        TmpValue = HiddenFeature.OriginAndAddition(1) 'get the Y coordinate

        If HiddenView.RefProp(1) - ListedView.RefProp(1) > 0 Then
            'new reference point is in Y minimum
            HiddenFeature.OriginAndAddition(1) = ListedView.BoundProp(5) - Abs(TmpValue)
        ElseIf HiddenView.RefProp(1) - ListedView.RefProp(1) < 0 Then
            'new reference point is in Y maximum
            HiddenFeature.OriginAndAddition(1) = -1 * (ListedView.BoundProp(5) - TmpValue)
        End If

    End Sub

    'Register the new visible view to the projection view collection
    Private Overloads Sub RegisterToViewCollection(ByRef ProjectionViewList As List(Of ViewProp), ByVal NewView As ViewProp)

        'checking if current view already exist inside the projection views collection
        If CheckIfSurfaceExists(ProjectionViewList, NewView.ViewType) = True Then

            For Each ProjectViewTmp As ViewProp In ProjectionViewList
                If ProjectViewTmp.ViewType.ToLower.Equals(NewView.ViewType.ToLower) Then 'check if current view already exist
                    'check if generation status is true or false, _
                    'true means view generation is conducted by the user, _
                    'false means view generation is conducted by the system
                    If ProjectViewTmp.GenerationStat = False Then
                        ProjectViewTmp.GenerationStat = NewView.GenerationStat
                        If ProjectViewTmp.GetFeature.Count <> 0 Then

                            'change the saved features coordinates 
                            ChangeFeatCoordinate(ProjectViewTmp, NewView)

                        End If

                        'copy all the current view properties into the saved view properties
                        ProjectViewTmp.RefProp = NewView.RefProp
                        ProjectViewTmp.BoundingBox = NewView.BoundingBox
                        ProjectViewTmp.BoundProp = NewView.BoundProp

                    End If

                End If
            Next

        Else
            'add the to the list of projection view
            ProjectionViewList.Add(NewView)
        End If
    End Sub

    'Register the new hidden view to the projection view collection
    Private Overloads Sub RegisterToViewCollection(ByRef ProjectionViewList As List(Of ViewProp), ByVal HiddenView As ViewProp, _
                                                   ByRef HiddenFeatureList As List(Of OutputFormat))

        'checking if current view already exist inside the projection views collection
        If CheckIfSurfaceExists(ProjectionViewList, HiddenView.ViewType) = True Then

            For Each ProjectViewTmp As ViewProp In ProjectionViewList
                If ProjectViewTmp.ViewType.ToLower.Equals(HiddenView.ViewType.ToLower) Then 'check if current view already exist
                    'check if generation status is true or false, _
                    'true means view generation is conducted by the user, _
                    'false means view generation is conducted by the system
                    If ProjectViewTmp.GenerationStat = True Then

                        For Each HideFeature As OutputFormat In HiddenFeatureList
                            If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Circle Then

                                'change the hidden features coordinates
                                ChangeFeatCoordinate(ProjectViewTmp, HiddenView, HideFeature)

                            End If

                            If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Polyline Then

                            End If

                            HideFeature.MiscProp(1) = HiddenView.ViewType.ToUpper
                            UnIdentifiedFeature.Add(HideFeature)
                            UnIdentifiedCounter = UnIdentifiedCounter + 1
                            AddToTable(HideFeature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)

                            RegisterToView(HideFeature)
                        Next

                    End If
                End If
            Next
        Else
            'add the to the list of projection view
            ProjectionViewList.Add(HiddenView)

            'change the saved features coordinates 
            For Each HideFeature As OutputFormat In HiddenFeatureList
                If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Circle Then
                    HideFeature.OriginAndAddition(0) = SetXHiddenFeaturePosition(HideFeature.OriginAndAddition(0), HiddenView.BoundProp(4), HiddenView.ViewType)
                    HideFeature.OriginAndAddition(1) = SetYHiddenFeaturePosition(HideFeature.OriginAndAddition(1), HiddenView.BoundProp(5), HiddenView.ViewType)
                End If

                If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Polyline Then

                End If

                HideFeature.MiscProp(1) = HiddenView.ViewType.ToUpper
                UnIdentifiedFeature.Add(HideFeature)
                UnIdentifiedCounter = UnIdentifiedCounter + 1
                AddToTable(HideFeature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)

                RegisterToView(HideFeature)
            Next
        End If
    End Sub

    'checking if current surface exist in the surface collection
    Private Function CheckIfSurfaceExists(ByVal ProjectionViewCollection As List(Of ViewProp), _
                                          ByVal View2Check As String) As Boolean
        For Each ProjectView As ViewProp In ProjectionViewCollection
            If ProjectView.ViewType.ToLower.Equals(View2Check.ToLower) Then
                Return True
            End If
        Next
    End Function

    Private TempCoordinate As Double

    'setting x coordinate for hole feature
    Private Function SetXPosition(ByVal a As Double, ByVal view As String) As Double
        TempCoordinate = New Double

        TempCoordinate = Round(a - RefPoint.X, 3)

        'convert coordinate only for bottom view
        If view.ToLower.Equals("bottom") Then
            TempCoordinate = -1 * TempCoordinate
        End If

        Return TempCoordinate

    End Function

    'setting y coordinate for hole feature
    Private Function SetYPosition(ByVal a As Double, ByVal view As String) As Double
        TempCoordinate = New Double

        TempCoordinate = Round(a - RefPoint.Y, 3)

        'convert coordinate only for bottom view
        If view.ToLower.Equals("bottom") Then
            TempCoordinate = -1 * TempCoordinate
        End If

        Return TempCoordinate

    End Function

    'setting x coordinate for hidden feature
    Private Function SetXHiddenFeaturePosition(ByVal a As Double, ByVal b As Double, ByVal view As String) As Double

        'initiate the x coordinate variable
        TempCoordinate = New Double

        If a > 0 Then
            If isequal(RefPoint.X, BBMinX) Then
                TempCoordinate = b - a

            ElseIf isequal(RefPoint.X, BBMaxX) Then
                TempCoordinate = a - b
            End If
        ElseIf a < 0 Then
            TempCoordinate = Abs(a) - b

            If view.ToLower.Equals("top") And isequal(RefPoint.X, BBMinX) Then
                TempCoordinate = -1 * TempCoordinate
            End If
        End If

        'convert coordinate only for bottom view
        If view.ToLower.Equals("bottom") Then
            TempCoordinate = -1 * TempCoordinate
        End If

        'output the result
        Return TempCoordinate

    End Function

    'setting y position for hidden feature
    Private Function SetYHiddenFeaturePosition(ByVal a As Double, ByVal b As Double, ByVal view As String) As Double

        'initiate the x coordinate variable
        TempCoordinate = New Double

        If a > 0 Then
            TempCoordinate = a - b
            If isequal(RefPoint.Y, BBMinY) And view.ToLower.Equals("top") Then
                TempCoordinate = -1 * TempCoordinate
            End If
        ElseIf a < 0 Then
            If isequal(RefPoint.Y, BBMinY) Then
                TempCoordinate = b - Abs(a)

            ElseIf isequal(RefPoint.Y, BBMaxY) Then
                TempCoordinate = Abs(a) - b
            End If
        End If

        'convert coordinate only for bottom view
        If view.ToLower.Equals("bottom") Then
            TempCoordinate = -1 * TempCoordinate
        End If

        'output the result
        Return TempCoordinate

    End Function

    Private ChangedReferencePoint(3) As Double

    'changing the reference point for virtual surface
    Private Sub ChangeTheRefPoint(ByRef ActualReferencePoint As Point3d)
        'If isequal(ActualReferencePoint.X, BBMinX) Then
        '    ChangedReferencePoint(0) = BBMaxX
        'ElseIf isequal(ActualReferencePoint.X, BBMaxX) Then
        '    ChangedReferencePoint(0) = BBMinX
        'End If

        ChangedReferencePoint(0) = ActualReferencePoint.X

        If isequal(ActualReferencePoint.Y, BBMinY) Then
            ChangedReferencePoint(1) = BBMaxY
        ElseIf isequal(ActualReferencePoint.Y, BBMaxY) Then
            ChangedReferencePoint(1) = BBMinY
        End If
        ChangedReferencePoint(2) = ActualReferencePoint.Z

        ActualReferencePoint = New Point3d(ChangedReferencePoint(0), ChangedReferencePoint(1), ChangedReferencePoint(2))

    End Sub

    'setting up the Bounding box Properties values for each view
    Private Sub SetTheBoundaryParameter(ByRef ProjectionView As ViewProp, _
                                        ByVal ViewType As String)
        ProjectionView.ViewType = ViewType
        ProjectionView.BoundProp(0) = BBMinX
        ProjectionView.BoundProp(1) = BBMinY
        ProjectionView.BoundProp(2) = BBMaxX
        ProjectionView.BoundProp(3) = BBMaxY
        ProjectionView.BoundProp(4) = Round(BBMaxX - BBMinX, 3)
        ProjectionView.BoundProp(5) = Round(BBMaxY - BBMinY, 3)
        ProjectionView.BoundingBox = BoundingBoxLine
        ProjectionView.CircleEntities = CircEntities
        ProjectionView.LineEntities = LineEntities
        ProjectionView.ArcEntities = ArcEntities
        ProjectionView.Entities = AllEntities

    End Sub

    'setting up the entities values for each view
    Private Sub SetCurrentViewEntities(ByRef ProjectionView As ViewProp)

    End Sub

    'setting up the reference point value
    Private Sub SetTheReferencePoint(ByRef ProjectionView As ViewProp, _
                                    ByVal ActualReferencePoint As Point3d, _
                                    ByVal ViewType As String)

        If String.Equals(ViewType.ToLower, "bottom") Then
            If isequal(ActualReferencePoint.X, BBMinX) Then
                ProjectionView.RefProp(0) = Round(BBMaxX - BBMinX, 3)
            ElseIf isequal(ActualReferencePoint.X, BBMaxX) Then
                ProjectionView.RefProp(0) = 0
            End If

            If isequal(ActualReferencePoint.Y, BBMinY) Then
                ProjectionView.RefProp(1) = Round(BBMaxY - BBMinY, 3)
            ElseIf isequal(ActualReferencePoint.Y, BBMaxY) Then
                ProjectionView.RefProp(1) = 0
            End If
        Else
            If isequal(ActualReferencePoint.X, BBMinX) Then
                ProjectionView.RefProp(0) = 0
            ElseIf isequal(ActualReferencePoint.X, BBMaxX) Then
                ProjectionView.RefProp(0) = Round(BBMaxX - BBMinX, 3)
            End If

            If isequal(ActualReferencePoint.Y, BBMinY) Then
                ProjectionView.RefProp(1) = 0
            ElseIf isequal(ActualReferencePoint.Y, BBMaxY) Then
                ProjectionView.RefProp(1) = Round(BBMaxY - BBMinY, 3)
            End If
        End If

        ProjectionView.RefProp(2) = Round(ActualReferencePoint.Z, 3)
    End Sub

    'assign the feature according to their location
    Private Sub RegisterToView(ByVal FeatureTmp As OutputFormat)

        For Each view As ViewProp In ProjectionView
            If view.ViewType.Equals(FeatureTmp.MiscProp(1)) Then
                view.SetFeature(FeatureTmp)
            End If
        Next
    End Sub

    'Get the actual reference point for current view
    Private Function GetActualRefPoint(ByVal ViewRefProp As Double()) As Point3d
        If ViewRefProp(0) = 0 And ViewRefProp(1) = 0 Then
            Return New Point3d(BBMinX, BBMinY, 0)
        End If

        If ViewRefProp(0) <> 0 And ViewRefProp(1) = 0 Then
            Return New Point3d(BBMaxX, BBMinY, 0)
        End If

        If ViewRefProp(0) = 0 And ViewRefProp(1) <> 0 Then
            Return New Point3d(BBMinX, BBMaxY, 0)
        End If

        If ViewRefProp(0) <> 0 And ViewRefProp(1) <> 0 Then
            Return New Point3d(BBMaxX, BBMaxY, 0)
        End If
    End Function

    'check the surface name
    Private Function CheckTheSurface(ByVal SurfaceName As String, ByVal SurfaceIndex As Integer) As String
        If SurfaceIndex = 2 Then
            Return SearchOppositeSurf(SurfaceName)
        Else
            Return SurfaceName
        End If
    End Function

    'search the surface if the linetype is define as a hidden line type
    Private Function SearchOppositeSurf(ByVal Surface As String) As String
        Select Case Surface.ToLower
            Case "front"
                Return "BACK"
            Case "back"
                Return "FRONT"
            Case "top"
                Return "BOTTOM"
            Case "bottom"
                Return "TOP"
            Case "left"
                Return "RIGHT"
            Case "right"
                Return "LEFT"
            Case Else
                Return Nothing
        End Select
    End Function

    'add the recognized feature into the machining feature table
    Private Sub AddToTable(ByVal FeatureTmp As OutputFormat, ByVal List As System.Data.DataTable, _
                           ByVal Table As System.Windows.Forms.DataGridView)

        Dim NewRow As DataRow = List.NewRow()
        NewRow("State") = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\exclamation.png")
        NewRow("Name") = FeatureTmp.MiscProp(0)
        NewRow("Surface") = FeatureTmp.MiscProp(1)
        NewRow("Biner") = "0"
        NewRow("Object") = FeatureTmp

        List.Rows.Add(NewRow)

    End Sub

    'looking the feature name in japanese
    Private Function FindTheirJapsName(ByVal FeatureName As String) As String

        If FeatureName.ToLower.Contains("ream") Then
            Return "リーマ穴"
        End If

        If FeatureName.ToLower.Contains("sunkbolt") Then
            Return "段付きボルト穴" + RTrim(FeatureName).Substring(8)
        End If

        If FeatureName.ToLower.Contains("drill") Then
            Return "ドリル穴"
        End If

        If FeatureName.ToLower.Contains("tap, pt") Then
            Return "ＰＴタップ穴" + RTrim(FeatureName).Substring(3)
        End If

        If FeatureName.ToLower.Contains("tap, m") Then
            Return "タップ穴" + RTrim(FeatureName).Substring(3)
        End If

        Return ""
    End Function

    Private MillFeatureList As List(Of OutputFormat)
    Private MillingProcessor As MillingProcessor

    Private IdentifiedCounter, UnIdentifiedCounter As Integer

    Private GetPoints As GetPoints
    Private AllPoints As List(Of Point3d)
    Private GroupOfEntity As List(Of AllPoints)
    Private UnAdjacentPoints As List(Of Point3d)
    Private GeometryProcessor As GeometryProcessor

    Private MillingObjectId As List(Of ObjectId)
    Private ListLoopTemp As List(Of List(Of Entity))
    Private LoopTemp As List(Of Entity)

    Private MainLoop As List(Of Entity)
    Private GroupLoop As List(Of List(Of Entity))
    Private GroupLoopPoints As List(Of List(Of Point3d))

    Private PolygonProcessor As PolygonProcessor
    Private ViewProcessor As ViewProcessor

    Private Function isequal(ByVal x As Double, ByVal y As Double) As Boolean
        If Math.Abs(x - y) > 0.1 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function isequalpoint(ByVal point1 As Point3d, ByVal point2 As Point3d) As Boolean
        If Math.Abs(point1.X - point2.X) < 0.1 And Math.Abs(point1.Y - point2.Y) < 0.1 And Math.Abs(point1.Z - point2.Z) < 0.1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function LineBreak(ByVal LineTemp As Line, ByVal PointTemp As Point3d)
        Dim SplittedLines As New List(Of Line)
        SplittedLines.Add(New Line(LineTemp.StartPoint, PointTemp))
        SplittedLines.Add(New Line(LineTemp.EndPoint, PointTemp))
        Return SplittedLines
    End Function

    Private Feature As OutputFormat
    Private ProjectView As ViewProp

    Private index As Integer = 0

    Public Shared IdentifiedFeature As New List(Of OutputFormat)
    Public Shared UnIdentifiedFeature As New List(Of OutputFormat)

    Public Shared ReadOnly TmpIdentifiedFeature As New List(Of OutputFormat)
    Public Shared ReadOnly TmpUnidentifiedFeature As New List(Of OutputFormat)

    Public Shared HiddenFeature As List(Of OutputFormat)

    Public Shared ProjectionView As New List(Of ViewProp)

End Class 'AcEdSSGetCommand
'End Namespace 'Selection