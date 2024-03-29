Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Interop
Imports System.Linq
Imports System.Math
Imports System.IO
Imports System.Threading
Imports FR

'Namespace Selection
Public Class SelectionCommand

    'save the idarray as object id
    Public Shared IdArray As ObjectId()
    Private RefPoint As Point3d
    Public Shared LastRefPoint As Point3d

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
    Private ArcEntities As List(Of Arc)

    Public Shared LineEntities As List(Of Line)
    Public Shared AllEntities As List(Of Entity)

    'List of unidentified entities
    Private UIEntities As List(Of Entity)
    Public Shared UIEntitiesAll As List(Of Entity)
    Private UI2CircList As List(Of IEnumerable(Of Circle))
    Public Shared UI2CircListAll As List(Of IEnumerable(Of Circle))

    'Variabel untuk isi kosongnya LineTypes and SchematicPresetting
    Private LineTypesStat As Boolean

    Private PLEntities As List(Of Polyline)
    Private MaxPoint, MinPoint As List(Of Point3d)
    Private obj As DBObject
    Private entity As Entity

    Private ListOfDiameter As List(Of Double)
    Private ListOfCenter As List(Of Point3d)

    Private Linetypes As LinetypesPresetting
    Private Schematic As SchematicPresetting
    Private ProgBar As ProgressForm

    Private Sub ClasifyEntities(ByVal AcadConnection As AcadConn, _
                                ByRef ObjIDsClassify As List(Of ObjectId))

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
        Dim ObjIDAdd As New List(Of ObjectId)
        Dim ObjIDRemoveIdx As New List(Of Integer)
        LineTypesStat = New Boolean

        MaxPoint = New List(Of Point3d)
        MinPoint = New List(Of Point3d)

        ListOfCenter = New List(Of Point3d)
        ListOfDiameter = New List(Of Double)

        Dim AcadConn2 As New AcadConn

        AcadConn2.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)

        Using AcadConn2.myT
            AcadConn2.OpenBlockTableRec()
            AcadConn2.btr.UpgradeOpen()

            'classify the entities into three categories (circle, line, and arc)
            For Each id In ObjIDsClassify
                entity = AcadConn2.myT.GetObject(id, OpenMode.ForWrite, True)
                'add circle, line and arc entities
                If Check2Database.CheckIfEntity(entity) = True Then 'And (TypeOf (entity) Is DBPoint) And Not (TypeOf (entity) Is DBText)
                    If TypeOf (entity) Is Circle Then
                        CircEntities.Add(entity)
                        MaxPoint.Add(entity.GeometricExtents.MaxPoint)
                        MinPoint.Add(entity.GeometricExtents.MinPoint)
                        AllEntities.Add(entity)
                    ElseIf TypeOf (entity) Is Line Then
                        LineEntities.Add(entity)
                        MaxPoint.Add(entity.GeometricExtents.MaxPoint)
                        MinPoint.Add(entity.GeometricExtents.MinPoint)
                        AllEntities.Add(entity)
                    ElseIf TypeOf (entity) Is Arc Then
                        ArcEntities.Add(entity)
                        MaxPoint.Add(entity.GeometricExtents.MaxPoint)
                        MinPoint.Add(entity.GeometricExtents.MinPoint)
                        AllEntities.Add(entity)
                    ElseIf TypeOf (entity) Is Polyline Then
                        PLEntities.Add(entity)
                        MaxPoint.Add(entity.GeometricExtents.MaxPoint)
                        MinPoint.Add(entity.GeometricExtents.MinPoint)
                        AllEntities.Add(entity)
                    End If
                End If
            Next id
            AcadConn2.myT.Commit()
        End Using
        AcadConn2.myT.Dispose()
    End Sub

    Public Shared BBMaxX, BBMaxY, BBMinX, BBMinY As Double
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
    Public Shared ObjIDsClassify As List(Of ObjectId)

    Private Check2Database As DatabaseConn
    Private CheckResult() As String

    Private SplitResult As DBObjectCollection
    Public Shared HiddenEntity As List(Of Entity)
    Private DwgPreprocessor As DwgProcessor
    Private PointStatus As Boolean
    Private VLine As VirtualLine
    Private VirtualLines As List(Of Line)

    Private Sub IntializeDb()
        Check2Database = New DatabaseConn
        Check2Database.InitLinesDb()
        Check2Database.InitHoleDb()
    End Sub

    Private CircGroupMember As Integer
    Private CircProcessor As CircleProcessor

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
        Opts.MessageForAdding = "Select " + setView.viewis.ToUpper + " view:"

        'get the result of selection
        res = ed.GetSelection(Opts)
        If res.Status = PromptStatus.OK Then

            'add the view to the view combobox
            adskClass.myPalette.ComboBox2.Items.Add(setView.viewis)

            SS = res.Value
            IdArray = SS.GetObjectIds
            ObjIDsClassify = New List(Of ObjectId)

            For Each id As ObjectId In IdArray
                ObjIDsClassify.Add(id)
            Next

            AcadConn = New AcadConn

            'StartTransaction
            AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)

            Try
                Using AcadConn.myT

                    AcadConn.OpenBlockTableRec()
                    AcadConn.btr.UpgradeOpen()

                    'initialize database
                    IntializeDb()
                    ClasifyEntities(AcadConn, ObjIDsClassify)

                    If MaxPoint.Count = 0 Then
                        'The recognizing process could not be continued. , There were problems in finding the appropriate outer vertices for creating the bounding box for this
                        ' view. , Hint: Please check the database for solid and hidden line definition.
                        MsgBox("[ERROR001] 認識操作を継続できません." + vbCrLf + _
                               "外枠を構成する角が見つかりません " + _
                               setView.viewis + " 設計面." + vbCrLf + vbCrLf + _
                               "物体線と隠れ線のデータベースを確認してください.", MsgBoxStyle.Critical)
                        Exit Try
                    Else
                        DefineBoundBox(MaxPoint, MinPoint)
                    End If

                    If (BBMaxX = -10000) Or (BBMaxY = -10000) Or (BBMinX = 10000) Or (BBMinY = 10000) Then
                        MsgBox("指定された投影図中の形状認識に問題が起こりました。" + vbCrLf + _
                               "FRデータベースを調べてください。" + vbCrLf + "登録されていない線種のプロパティがあるようです。" _
                               , MsgBoxStyle.Exclamation)
                    Else

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
                        ProjectView.ActRefPoint = RefPoint

                        'set the view generation status to be true
                        ProjectView.GenerationStat = True

                        'set status for double mirror
                        ProjectView.DoubleMirrorStat = False
                        If setView.CBDoubleMirror = True Then
                            ProjectView.DoubleMirrorStat = True
                        End If

                        'register the new view to projection view collection
                        RegisterToViewCollection(ProjectionView, ProjectView)

                        'set last ref point that needed for add feature manually
                        LastRefPoint = New Point3d
                        LastRefPoint = RefPoint

                        'list project view as last view selected for add feature manually
                        LastViewSelected = New ViewProp
                        LastViewSelected = ProjectView

                        'initiate a variable for placing the hidden feature
                        HiddenFeature = New List(Of OutputFormat)
                        HiddenEntity = New List(Of Entity)

                        'initiate the feature counter
                        IdentifiedCounter = Nothing
                        UnIdentifiedCounter = Nothing
                        adskClass.myPalette.AddManual.Enabled = True

                        'start processing for circle base entities
                        If CircEntities.Count <> 0 And setView.CBHole = True Then
                            Dim getGroup = From item In CircEntities _
                                           Group item By CircXCenter = item.Center Into GroupMember = Group _
                                           Select Honor = GroupMember

                            'get the number of group
                            CircGroupMember = New Integer
                            For Each result As IEnumerable(Of Circle) In getGroup
                                CircGroupMember = CircGroupMember + 1
                            Next

                            'initiate the supporting variable
                            CheckResult = Nothing

                            'initiate the progress bar
                            'UserControl3.'acedSetStatusBarProgressMeter("Circle Features", 0, CircGroupMember)
                            ProgBar = New ProgressForm
                            ProgBar.Text = "穴形状処理中" 'Processing Circle Features
                            ProgBar.ProgressBar1.Maximum = CircGroupMember
                            ProgBar.Show()
                            ProgBar.ProgressBar1.Value = 0
                            System.Windows.Forms.Application.DoEvents()

                            Dim i As Integer

                            CircProcessor = New CircleProcessor

                            'create condition if the group consist of two circle, cause this group _
                            ' will be considered as tap c'bore
                            For Each result As IEnumerable(Of Circle) In getGroup

                                CircMember = result.Count()
                                Dim Surface As Integer = 3

                                CircProcessor.ClassifyCircles(CircMember, Check2Database, result, Surface, _
                                                              Feature, RefPoint, ProjectView)

                                'set the current feature to current view
                                RegisterToView(Feature)

                                'add the progress bar
                                i = i + 1
                                ProgBar.ProgressBar1.Value = i
                                ProgBar.Label1.Text = Round(((i / CircGroupMember) * 100), 0).ToString

                                'System.Threading.Thread.Sleep(1)
                                'UserControl3.''acedSetStatusBarProgressMeterPos(i)
                                System.Windows.Forms.Application.DoEvents()
                            Next

                            'UserControl3.acedRestoreStatusBar()
                            ProgBar.Close()
                            ProgBar.Dispose()
                        End If


                        'check the polyline features
                        If PLEntities.Count <> 0 And setView.CBPoly = True Then
                            'initiate the progress bar
                            'UserControl3.'acedSetStatusBarProgressMeter("Polyline Features", 0, PLEntities.Count)
                            ProgBar = New ProgressForm
                            ProgBar.Text = "ポリライン処理中" 'Processing Polyline Features
                            ProgBar.ProgressBar1.Maximum = PLEntities.Count
                            ProgBar.Show()
                            ProgBar.ProgressBar1.Value = 0
                            System.Windows.Forms.Application.DoEvents()

                            Dim i As Integer
                            Dim ViewPolyTemp As New ViewProp
                            ViewPolyTemp = ProjectView

                            If ProjectView.DoubleMirrorStat = True Then
                                SetRefPointDoubleMirror(ViewPolyTemp)
                            End If

                            For Each PolyTemp As Polyline In PLEntities
                                Feature = New OutputFormat

                                Feature.FeatureName = "POLYLINE"
                                Feature.ObjectId.Add(PolyTemp.ObjectId)
                                Feature.MiscProp(0) = "ポリライン"
                                Feature.MiscProp(1) = setView.viewis
                                Feature.SurfaceName = setView.viewis
                                Feature.Pline = PolyTemp
                                Feature.Planelocation = ViewPolyTemp

                                If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(PolyTemp) Then
                                    HiddenFeature.Add(Feature)
                                    HiddenEntity.Add(PolyTemp)
                                Else
                                    'add to the unidentified feature list
                                    UnIdentifiedFeature.Add(Feature)
                                    TmpUnidentifiedFeature.Add(Feature)
                                    UnIdentifiedCounter = UnIdentifiedCounter + 1
                                    AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                                End If

                                'set the current feature to current view
                                RegisterToView(Feature)

                                'add the progress bar
                                i = i + 1
                                ProgBar.ProgressBar1.Value = i
                                ProgBar.Label1.Text = Round(((i / PLEntities.Count) * 100), 0).ToString

                                'System.Threading.Thread.Sleep(1)
                                'UserControl3.''acedSetStatusBarProgressMeterPos(i)
                                System.Windows.Forms.Application.DoEvents()
                            Next
                            'UserControl3.acedRestoreStatusBar()
                            ProgBar.Close()
                            ProgBar.Dispose()
                        End If

                        'start processing for milling features
                        If setView.CBMill = True And LineEntities.Count <> 0 Then

                            AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
                            AcadConn.OpenBlockTableRec()
                            AcadConn.btr.UpgradeOpen()

                            If adskClass.AppPreferences.DrawPP = True Then
                                DwgPreprocessor = New DwgProcessor
                                DwgPreprocessor.StartPreProcessing(AllEntities, LineEntities, AcadConn.myT)
                            End If

                            If adskClass.AppPreferences.MultiAnalysis = True Then
                                VLine = New VirtualLine
                                VirtualLines = New List(Of Line)

                                Dim BoundBoxLineList As New List(Of Line)
                                BoundBoxLineList.Add(BoundingBoxLine(0))
                                BoundBoxLineList.Add(BoundingBoxLine(1))
                                BoundBoxLineList.Add(BoundingBoxLine(2))
                                BoundBoxLineList.Add(BoundingBoxLine(3))

                                'variables prepared for generate virtual lines
                                VLine.GenerateVL(BBMinX, BBMinY, BBMaxX, BBMaxY, BoundBoxLineList, _
                                                 VirtualLines, LineEntities, AllEntities, AcadConn)

                            End If

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
                                AcadConn.btr.AppendEntity(lineTmp)
                                AcadConn.myT.AddNewlyCreatedDBObject(lineTmp, True)
                            Next

                            AcadConn.myT.Commit()

                            'start get entities in selected view
                            'ProjectionView(ProjectionView.IndexOf(ProjectView)).LineEntities = LineEntities
                            'ProjectionView(ProjectionView.IndexOf(ProjectView)).Entities = AllEntities
                            ProjectionView(ProjectionView.Count - 1).ArcEntities = ArcEntities
                            ProjectionView(ProjectionView.Count - 1).LineEntities = LineEntities
                            ProjectionView(ProjectionView.Count - 1).Entities = AllEntities

                            If ProjectionView.Count = 1 Then
                                'Counter = New Integer
                                MillingProcessor = New MillingProcessor

                                MainLoop = New List(Of Entity)
                                MainLoopPts = New List(Of Point3d)
                                GroupLoop = New List(Of List(Of Entity))
                                GroupLoopPoints = New List(Of List(Of Point3d))

                                MillingProcessor.LoopFinder(ProjectionView(0).Entities, GroupLoop, GroupLoopPoints, MainLoop, MainLoopPts)
                                ProjectionView(0).MainLoop = MainLoop
                                ProjectionView(0).GroupLoop = GroupLoop
                                ProjectionView(0).GroupLoopPoints = GroupLoopPoints

                                ViewProcessor = New ViewProcessor
                                ViewProcessor.SingleViewProcessor(ProjectionView(0), UnIdentifiedFeature, TmpUnidentifiedFeature, UnIdentifiedCounter)

                            ElseIf ProjectionView.Count > 1 Then
                                'Loop Finder for new view added
                                MillingProcessor = New MillingProcessor

                                MainLoop = New List(Of Entity)
                                MainLoopPts = New List(Of Point3d)
                                GroupLoop = New List(Of List(Of Entity))
                                GroupLoopPoints = New List(Of List(Of Point3d))

                                MillingProcessor.LoopFinder(ProjectionView(ProjectionView.Count - 1).Entities, GroupLoop, GroupLoopPoints, MainLoop, MainLoopPts)
                                ProjectionView(ProjectionView.Count - 1).MainLoop = MainLoop
                                ProjectionView(ProjectionView.Count - 1).GroupLoop = GroupLoop
                                ProjectionView(ProjectionView.Count - 1).GroupLoopPoints = GroupLoopPoints

                                'FR multiple view milling
                                ViewProcessor = New ViewProcessor

                                'check multiview analysis selection in preferences setting
                                If adskClass.AppPreferences.MultiAnalysis = True Then
                                    ViewProcessor.MultipleViewProcessor(ProjectionView, ProjectionView.Count - 1, UnIdentifiedFeature, TmpUnidentifiedFeature, _
                                                                        UnIdentifiedCounter, IdentifiedFeature, TmpIdentifiedFeature, IdentifiedCounter)
                                Else
                                    ViewProcessor.SingleViewProcessor(ProjectionView(ProjectionView.Count - 1), UnIdentifiedFeature, TmpUnidentifiedFeature, UnIdentifiedCounter)
                                End If

                            End If
                        End If

                        'change coordinate for double mirror
                        If ProjectView.DoubleMirrorStat = True Then
                            SetRefPointDoubleMirror(ProjectView)
                        End If

                        'check the hidden feature
                        If HiddenFeature.Count <> 0 And setView.CBHidden = True Then
                            HiddenInitiate(HiddenFeature, ProjectView, setView.viewis, ProjectionView)
                        End If

                        'report the recognition results
                        If IdentifiedFeature.Count <> 0 Or UnIdentifiedFeature.Count <> 0 Then
                            ReportTheFindings(IdentifiedCounter, UnIdentifiedCounter)
                        Else
                            MsgBox("この投影図には、円が見当たりませんでした。" + setView.viewis + " 設計面")
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
        Else
            'view selection was canceled
            MsgBox("設計面の選択をキャンセルしました。")
        End If
    End Sub

    Private SurfaceIndex As Integer
    'hidden view automatic
    Public Overloads Sub HiddenInitiate(ByRef HiddenFeature As List(Of OutputFormat), ByRef ProjectView As ViewProp, ByVal ViewName As String, _
                              ByRef ProjectionView As List(Of ViewProp))
        If MsgBox(HiddenFeature.Count.ToString + " 個の隠れ線形状が見つかりました。" _
                                   + vbCrLf + vbCrLf _
                                   + "これらを無視しても良いですか？", MsgBoxStyle.YesNo, "AcadFR - 隠れ形状") = MsgBoxResult.No Then '隠れ形状 --> Hidden Feature

            If MsgBox("これらの形状を反対側の面に設定しますがよいですか？", MsgBoxStyle.YesNo, _
                                          "AcadFR - 隠れ形状") = MsgBoxResult.Yes Then

                ProjectView = New ViewProp

                If UserControl3.ManualStat = False Then
                    SetTheBoundaryParameter(ProjectView, SearchOppositeSurf(setView.viewis))
                    SetTheReferencePoint(ProjectView, RefPoint)

                    'find the selected projection view
                    SurfaceIndex = New Integer
                    Dim i As New Integer
                    For Each Surface As ViewProp In ProjectionView
                        If Surface.ViewType.ToLower = SearchOppositeSurf(ProjectView.ViewType.ToLower).ToLower Then
                            SurfaceIndex = i
                            Exit For
                        End If
                        i = i + 1
                    Next

                    'change coordinate for double mirror
                    If ProjectionView(SurfaceIndex).DoubleMirrorStat = True Then
                        SetRefPointDoubleMirror(ProjectView)
                    End If

                    ProjectView.ActRefPoint = RefPoint
                Else ' for milling manual hidden
                    'find the selected projection view
                    SurfaceIndex = New Integer
                    Dim i As New Integer
                    For Each Surface As ViewProp In ProjectionView
                        If Surface.ViewType.ToLower = AddManualSurface.SelSurfMan.ToLower Then
                            SurfaceIndex = i
                            Exit For
                        End If
                        i = i + 1
                    Next
                    SetTheBoundaryParameter(ProjectView, ProjectionView(SurfaceIndex))
                    SetTheReferencePoint(ProjectView, ProjectionView(SurfaceIndex).ActRefPoint)
                    'change coordinate for double mirror
                    If ProjectionView(SurfaceIndex).DoubleMirrorStat = True Then
                        SetRefPointDoubleMirror(ProjectView)
                    End If
                    ProjectView.ActRefPoint = ProjectionView(SurfaceIndex).ActRefPoint
                End If

                ProjectView.GenerationStat = False

                RegisterToViewCollection(ProjectionView, ProjectView, HiddenFeature)

                Dim ViewStat As New Boolean
                For Each SurfAdded As String In adskClass.myPalette.ComboBox2.Items
                    If SurfAdded.ToLower = ProjectView.ViewType.ToLower Then
                        ViewStat = True
                        Exit For
                    End If
                Next
                If ViewStat = False Then
                    adskClass.myPalette.ComboBox2.Items.Add(ProjectView.ViewType.ToUpper)
                End If

            Else
                For Each HideFeature As OutputFormat In HiddenFeature
                    UnIdentifiedFeature.Add(HideFeature)
                    TmpUnidentifiedFeature.Add(HideFeature)
                    UnIdentifiedCounter = UnIdentifiedCounter + 1
                    AddToTable(HideFeature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                Next
            End If
            End If
    End Sub

    'hidden view manual for circle
    Public Overloads Sub HiddenInitiate(ByRef HiddenFeature As List(Of OutputFormat), ByRef ProjectView As ViewProp, ByVal ViewName As String, _
                              ByRef ProjectionView As List(Of ViewProp), ByRef FeatCount As Integer)
        If MsgBox(HiddenFeature.Count.ToString + " 個の隠れ線形状が見つかりました。" _
                                   + vbCrLf + vbCrLf _
                                   + "これらを無視しても良いですか？", MsgBoxStyle.YesNo, "AcadFR - 隠れ形状") = MsgBoxResult.No Then '隠れ形状 --> Hidden Feature

            If MsgBox("これらの形状を反対側の面に設定しますがよいですか？", MsgBoxStyle.YesNo, _
                                          "AcadFR - 隠れ形状") = MsgBoxResult.Yes Then

                ProjectView = New ViewProp

                If UserControl3.ManualStat = False Then
                    SetTheBoundaryParameter(ProjectView, SearchOppositeSurf(setView.viewis))
                    SetTheReferencePoint(ProjectView, RefPoint)
                    'find the selected projection view
                    SurfaceIndex = New Integer
                    Dim i As New Integer
                    For Each Surface As ViewProp In ProjectionView
                        If Surface.ViewType.ToLower = SearchOppositeSurf(ProjectView.ViewType.ToLower).ToLower Then
                            SurfaceIndex = i
                            Exit For
                        End If
                        i = i + 1
                    Next

                    'change coordinate for double mirror
                    If ProjectionView(SurfaceIndex).DoubleMirrorStat = True Then
                        SetRefPointDoubleMirror(ProjectView)
                    End If
                    ProjectView.ActRefPoint = RefPoint
                Else
                    'find the selected projection view
                    SurfaceIndex = New Integer
                    Dim i As New Integer
                    For Each Surface As ViewProp In ProjectionView
                        If Surface.ViewType.ToLower = AddManualSurface.SelSurfMan.ToLower Then
                            SurfaceIndex = i
                            Exit For
                        End If
                        i = i + 1
                    Next
                    SetTheBoundaryParameter(ProjectView, ProjectionView(SurfaceIndex))
                    SetTheReferencePoint(ProjectView, ProjectionView(SurfaceIndex).ActRefPoint)
                    'change coordinate for double mirror
                    If ProjectionView(SurfaceIndex).DoubleMirrorStat = True Then
                        SetRefPointDoubleMirror(ProjectView)
                    End If
                    ProjectView.ActRefPoint = ProjectionView(SurfaceIndex).ActRefPoint
                End If

                ProjectView.GenerationStat = False

                RegisterToViewCollection(ProjectionView, ProjectView, HiddenFeature)

                Dim ViewStat As New Boolean
                For Each SurfAdded As String In adskClass.myPalette.ComboBox2.Items
                    If SurfAdded.ToLower = ProjectView.ViewType.ToLower Then
                        ViewStat = True
                        Exit For
                    End If
                Next
                If ViewStat = False Then
                    adskClass.myPalette.ComboBox2.Items.Add(ProjectView.ViewType.ToUpper)
                End If
            Else
                For Each HideFeature As OutputFormat In HiddenFeature
                    UnIdentifiedFeature.Add(HideFeature)
                    TmpUnidentifiedFeature.Add(HideFeature)
                    'UnIdentifiedCounter = UnIdentifiedCounter + 1
                    FeatCount = FeatCount + 1
                    AddToTable(HideFeature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                Next
            End If
        End If
    End Sub

    Private Function FindOrthoView(ByVal ViewName As String) As OrthographicView
        Select Case ViewName.ToLower
            Case "top"
                Return OrthographicView.TopView
            Case "bottom"
                Return OrthographicView.BottomView
            Case "right"
                Return OrthographicView.RightView
            Case "left"
                Return OrthographicView.LeftView
            Case "front"
                Return OrthographicView.FrontView
            Case "back"
                Return OrthographicView.BackView
            Case Else
                Return OrthographicView.NonOrthoView
        End Select
    End Function

    'report how many features that was add to the table
    Private Sub ReportTheFindings(ByVal Identified As String, ByVal Unidentified As String)
        adskClass.myPalette.IdentifiedFeature.ClearSelection()
        adskClass.myPalette.UnidentifiedFeature.ClearSelection()
        adskClass.myPalette.MakeItBlank()

        MsgBox(Identified.ToString + "  認識できた形状のリストに追加される形状" + vbCrLf _
                               + Unidentified.ToString + "  その他の形状のリストに追加される形状")
    End Sub

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

    Private TmpValue As Double
    Private RowIndex As List(Of Integer)
    Private Feature2Check As OutputFormat

    'Change the features coordinates inside the saved view
    Public Overloads Sub ChangeFeatCoordinate(ByRef ListedView As ViewProp, ByVal NewView As ViewProp)
        'get hidden feature from identified feature list
        RowIndex = New List(Of Integer)
        For Each IFList As System.Windows.Forms.DataGridViewRow In adskClass.myPalette.IdentifiedFeature.Rows
            Feature2Check = New OutputFormat
            Feature2Check = IFList.Cells("Object").Value
            If Feature2Check.HiddenStatus = True And ListedView.ViewType.ToLower.Equals(Feature2Check.MiscProp(1).ToLower) Then
                RowIndex.Add(IFList.Index)
            End If
        Next

        'change the U dan V coordinate from identified feature list
        For i As Integer = 0 To RowIndex.Count - 1
            Feature2Check = New OutputFormat
            Feature2Check = adskClass.myPalette.IdentifiedFeature.Rows(RowIndex(i)).Cells("Object").Value
            'change the X coordinate by the current reference point position
            TmpValue = New Double
            TmpValue = Feature2Check.OriginAndAddition(0) 'get the X coordinate

            Feature2Check.OriginAndAddition(0) = TmpValue - (NewView.RefProp(0) - ListedView.RefProp(0))

            If NewView.DoubleMirrorStat = True Then
                Feature2Check.OriginAndAddition(0) = NewView.RefProp(0) - (NewView.BoundProp(4) - (NewView.RefProp(0) + Feature2Check.OriginAndAddition(0)))
            End If
            'change the Y coordinate by the current reference point position
            TmpValue = New Double
            TmpValue = Feature2Check.OriginAndAddition(1) 'get the Y coordinate
            Feature2Check.OriginAndAddition(1) = TmpValue - (NewView.RefProp(1) - ListedView.RefProp(1))
            If NewView.DoubleMirrorStat = True Then
                Feature2Check.OriginAndAddition(1) = NewView.RefProp(1) - (NewView.BoundProp(5) - (NewView.RefProp(1) + Feature2Check.OriginAndAddition(1)))
            End If
            adskClass.myPalette.IdentifiedFeature.Rows(RowIndex(i)).Cells("Object").Value = Feature2Check
        Next

        'get hidden feature from UnIdentified feature list
        RowIndex = New List(Of Integer)
        For Each UFList As System.Windows.Forms.DataGridViewRow In adskClass.myPalette.UnidentifiedFeature.Rows
            Feature2Check = New OutputFormat
            Feature2Check = UFList.Cells("Object").Value
            If Feature2Check.HiddenStatus = True And ListedView.ViewType.ToLower.Equals(Feature2Check.MiscProp(1).ToLower) Then
                RowIndex.Add(UFList.Index)
            End If
        Next

        'change the U dan V coordinate from unidentified feature list
        For i As Integer = 0 To RowIndex.Count - 1
            Feature2Check = New OutputFormat
            Feature2Check = adskClass.myPalette.UnidentifiedFeature.Rows(RowIndex(i)).Cells("Object").Value
            'change the X coordinate by the current reference point position
            TmpValue = New Double
            TmpValue = Feature2Check.OriginAndAddition(0) 'get the X coordinate

            Feature2Check.OriginAndAddition(0) = TmpValue - (NewView.RefProp(0) - ListedView.RefProp(0))
            If NewView.DoubleMirrorStat = True Then
                Feature2Check.OriginAndAddition(0) = NewView.RefProp(0) - (NewView.BoundProp(4) - (NewView.RefProp(0) + Feature2Check.OriginAndAddition(0)))
            End If
            'change the Y coordinate by the current reference point position
            TmpValue = New Double
            TmpValue = Feature2Check.OriginAndAddition(1) 'get the Y coordinate
            Feature2Check.OriginAndAddition(1) = TmpValue - (NewView.RefProp(1) - ListedView.RefProp(1))
            If NewView.DoubleMirrorStat = True Then
                Feature2Check.OriginAndAddition(1) = NewView.RefProp(1) - (NewView.BoundProp(5) - (NewView.RefProp(1) + Feature2Check.OriginAndAddition(1)))
            End If
            adskClass.myPalette.UnidentifiedFeature.Rows(RowIndex(i)).Cells("Object").Value = Feature2Check
        Next

    End Sub

    'change the feature coordinates inside the hidden view
    Public Overloads Sub ChangeFeatCoordinate(ByVal ListedView As ViewProp, ByVal HiddenView As ViewProp, _
                                               ByRef HiddenFeature As OutputFormat)


        'change the X coordinate by the current reference point position
        TmpValue = New Double
        TmpValue = HiddenFeature.OriginAndAddition(0) 'get the X coordinate

        HiddenFeature.OriginAndAddition(0) = TmpValue - (ListedView.RefProp(0) - HiddenView.RefProp(0))

        'change the Y coordinate by the current reference point position
        TmpValue = New Double
        TmpValue = HiddenFeature.OriginAndAddition(1) 'get the Y coordinate
        HiddenFeature.OriginAndAddition(1) = TmpValue - (ListedView.RefProp(1) - HiddenView.RefProp(1))

    End Sub

    'change the feature coordinates inside the hidden view for add coordinate manual
    Public Overloads Sub ChangeFeatCoordinate(ByVal ListedView As ViewProp, ByVal HiddenView As ViewProp, _
                                               ByRef TempX As Double, ByRef TempY As Double)


        'change the X coordinate by the current reference point position
        TmpValue = New Double
        TmpValue = TempX 'get the X coordinate

        TempX = TmpValue - (ListedView.RefProp(0) - HiddenView.RefProp(0))

        'change the Y coordinate by the current reference point position
        TmpValue = New Double
        TmpValue = TempY 'get the Y coordinate
        TempY = TmpValue - (ListedView.RefProp(1) - HiddenView.RefProp(1))

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
                        ProjectViewTmp.DoubleMirrorStat = NewView.DoubleMirrorStat
                    End If

                End If
            Next

        Else
            'add the to the list of projection view
            ProjectionViewList.Add(NewView)
        End If
    End Sub

    'Register the new hidden view to the projection view collection
    Public Overloads Sub RegisterToViewCollection(ByRef ProjectionViewList As List(Of ViewProp), ByVal HiddenView As ViewProp, _
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
                            If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Circle Or _
                            TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Line Or _
                            TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Arc Then

                                'rotate coordinates in Y axis
                                HideFeature.OriginAndAddition(0) = SetXHiddenFeaturePosition(HideFeature, HiddenView)
                                HideFeature.OriginAndAddition(1) = SetYHiddenFeaturePosition(HideFeature, HiddenView)

                                'change the hidden features coordinates
                                ChangeFeatCoordinate(ProjectViewTmp, HiddenView, HideFeature)

                            End If

                            If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Polyline Then
                                'HideFeature.Planelocation = HiddenView
                            End If

                            HideFeature.MiscProp(1) = HiddenView.ViewType.ToUpper
                            HideFeature.SurfaceName = HideFeature.MiscProp(1)
                            UnIdentifiedFeature.Add(HideFeature)
                            UnIdentifiedCounter = UnIdentifiedCounter + 1
                            AddToTable(HideFeature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)

                            RegisterToView(HideFeature)
                        Next
                    Else
                        'change the saved features coordinates 
                        For Each HideFeature As OutputFormat In HiddenFeatureList
                            If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Circle Or _
                                        TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Line Or _
                                        TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Arc Then
                                HideFeature.OriginAndAddition(0) = SetXHiddenFeaturePosition(HideFeature, HiddenView)
                                HideFeature.OriginAndAddition(1) = SetYHiddenFeaturePosition(HideFeature, HiddenView)
                            End If

                            If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Polyline Then
                                'HideFeature.Planelocation = HiddenView
                            End If

                            HideFeature.MiscProp(1) = HiddenView.ViewType.ToUpper
                            HideFeature.SurfaceName = HideFeature.MiscProp(1)
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
                If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Circle Or _
                            TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Line Or _
                            TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Arc Then
                    HideFeature.OriginAndAddition(0) = SetXHiddenFeaturePosition(HideFeature, HiddenView)
                    HideFeature.OriginAndAddition(1) = SetYHiddenFeaturePosition(HideFeature, HiddenView)
                End If

                If TypeOf HiddenEntity(HiddenFeature.IndexOf(HideFeature)) Is Polyline Then
                    'HideFeature.Planelocation = HiddenView
                End If

                HideFeature.MiscProp(1) = HiddenView.ViewType.ToUpper
                HideFeature.SurfaceName = HideFeature.MiscProp(1)
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

    'setting x coordinate for hidden feature
    Public Function SetXHiddenFeaturePosition(ByRef HideFeature As OutputFormat, ByVal HiddenView As ViewProp) As Double

        'initiate the x coordinate variable
        Dim XCoordinate As New Double
        XCoordinate = HideFeature.OriginAndAddition(0)
        TempCoordinate = New Double

        TempCoordinate = -1 * XCoordinate
        'case for milling
        If HideFeature.SolidArcCount > 0 Or HideFeature.HiddenLineCount > 0 Then
            If HideFeature.FeatureName = "Square Slot" And HideFeature.MiscProp(2) = "0" Then
                'TempCoordinate = TempCoordinate - HiddenView.BoundProp(4)
                TempCoordinate = TempCoordinate - HideFeature.OriginAndAddition(4)
            ElseIf HideFeature.FeatureName = "Square Step" And HideFeature.MiscProp(2) = "0" Then
                'TempCoordinate = TempCoordinate - HiddenView.BoundProp(4)
                TempCoordinate = TempCoordinate - HideFeature.OriginAndAddition(4)
            ElseIf HideFeature.FeatureName = "Square Step" And HideFeature.MiscProp(2) = "1" Then
                'TempCoordinate = TempCoordinate + HiddenView.BoundProp(4)
                TempCoordinate = TempCoordinate + HideFeature.OriginAndAddition(4)
            ElseIf HideFeature.FeatureName = "Blind Slot" And HideFeature.MiscProp(2) = "2" Then
                HideFeature.MiscProp(2) = "3"
            ElseIf HideFeature.FeatureName = "Blind Slot" And HideFeature.MiscProp(2) = "3" Then
                HideFeature.MiscProp(2) = "2"
            ElseIf HideFeature.FeatureName = "3-side Pocket" And HideFeature.MiscProp(2) = "2" Then
                HideFeature.MiscProp(2) = "3"
            ElseIf HideFeature.FeatureName = "3-side Pocket" And HideFeature.MiscProp(2) = "3" Then
                HideFeature.MiscProp(2) = "2"
            ElseIf HideFeature.FeatureName = "2-side Pocket" Then
                Dim D1Temp, D2Temp As New Integer
                D1Temp = HideFeature.OriginAndAddition(3)
                D2Temp = HideFeature.OriginAndAddition(4)
                HideFeature.OriginAndAddition(3) = D2Temp
                HideFeature.OriginAndAddition(4) = D1Temp
                If HideFeature.MiscProp(2) = "0" Then
                    HideFeature.MiscProp(2) = "1"
                ElseIf HideFeature.MiscProp(2) = "1" Then
                    HideFeature.MiscProp(2) = "0"
                ElseIf HideFeature.MiscProp(2) = "2" Then
                    HideFeature.MiscProp(2) = "3"
                ElseIf HideFeature.MiscProp(2) = "3" Then
                    HideFeature.MiscProp(2) = "2"
                End If
            End If
        End If
        'End If

        'output the result
        Return TempCoordinate

    End Function

    'setting y position for hidden feature
    Public Function SetYHiddenFeaturePosition(ByRef HideFeature As OutputFormat, ByVal HiddenView As ViewProp) As Double

        'initiate the y coordinate variable
        Dim YCoordinate As New Double
        YCoordinate = HideFeature.OriginAndAddition(1)
        TempCoordinate = New Double

        TempCoordinate = YCoordinate

        'case for milling
        If HideFeature.SolidArcCount > 0 Or HideFeature.HiddenLineCount > 0 Then
            If HideFeature.FeatureName = "Square Step" And HideFeature.MiscProp(2) = "2" Then
                'TempCoordinate = TempCoordinate - HiddenView.BoundProp(5)
                TempCoordinate = TempCoordinate - HideFeature.OriginAndAddition(4)
                HideFeature.MiscProp(2) = "3"
            ElseIf HideFeature.FeatureName = "Square Step" And HideFeature.MiscProp(2) = "3" Then
                'TempCoordinate = TempCoordinate + HiddenView.BoundProp(5)
                TempCoordinate = TempCoordinate + HideFeature.OriginAndAddition(4)
                HideFeature.MiscProp(2) = "2"
            End If
        End If
        'End If

        'output the result
        Return TempCoordinate

    End Function

    'setting up the Bounding box Properties values for each view
    Public Overloads Sub SetTheBoundaryParameter(ByRef ProjectionView As ViewProp, _
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

    'setting up the Bounding box Properties for add manual hidden
    Public Overloads Sub SetTheBoundaryParameter(ByRef ProjectionView As ViewProp, _
                                        ByVal RefProjView As ViewProp)
        ProjectionView.ViewType = SearchOppositeSurf(RefProjView.ViewType)
        ProjectionView.BoundProp(0) = RefProjView.BoundProp(0)
        ProjectionView.BoundProp(1) = RefProjView.BoundProp(1)
        ProjectionView.BoundProp(2) = RefProjView.BoundProp(2)
        ProjectionView.BoundProp(3) = RefProjView.BoundProp(3)
        ProjectionView.BoundProp(4) = RefProjView.BoundProp(4)
        ProjectionView.BoundProp(5) = RefProjView.BoundProp(5)
        ProjectionView.BoundingBox = RefProjView.BoundingBox
        ProjectionView.CircleEntities = RefProjView.CircleEntities
        ProjectionView.LineEntities = RefProjView.LineEntities
        ProjectionView.ArcEntities = RefProjView.ArcEntities
        ProjectionView.Entities = RefProjView.Entities
    End Sub

    'setting up the reference point value for visible surface
    Public Overloads Sub SetTheReferencePoint(ByRef ProjectionView As ViewProp, _
                                    ByVal ActualReferencePoint As Point3d, _
                                    ByVal ViewType As String)

        If isequal(ActualReferencePoint.X, BBMinX) Then
            ProjectionView.RefProp(0) = 0
        ElseIf isequal(ActualReferencePoint.X, BBMaxX) Then
            ProjectionView.RefProp(0) = Round(BBMaxX - BBMinX, 3)
        Else
            ProjectionView.RefProp(0) = Round(ActualReferencePoint.X - BBMinX, 3)
        End If

        If isequal(ActualReferencePoint.Y, BBMinY) Then
            ProjectionView.RefProp(1) = 0
        ElseIf isequal(ActualReferencePoint.Y, BBMaxY) Then
            ProjectionView.RefProp(1) = Round(BBMaxY - BBMinY, 3)
        Else
            ProjectionView.RefProp(1) = Round(ActualReferencePoint.Y - BBMinY, 3)
        End If

        ProjectionView.RefProp(2) = Round(ActualReferencePoint.Z, 3)
    End Sub

    'setting up the reference point value for hidden surface
    Public Overloads Sub SetTheReferencePoint(ByRef ProjectionView As ViewProp, _
                                    ByVal ActualReferencePoint As Point3d)

        If isequal(ActualReferencePoint.X, BBMinX) Then
            ProjectionView.RefProp(0) = Round(BBMaxX - BBMinX, 3)
        ElseIf isequal(ActualReferencePoint.X, BBMaxX) Then
            ProjectionView.RefProp(0) = 0
        Else
            ProjectionView.RefProp(0) = Round(BBMaxX - ActualReferencePoint.X, 3)
        End If

        If isequal(ActualReferencePoint.Y, BBMinY) Then
            ProjectionView.RefProp(1) = 0
        ElseIf isequal(ActualReferencePoint.Y, BBMaxY) Then
            ProjectionView.RefProp(1) = Round(BBMaxY - BBMinY, 3)
        Else
            ProjectionView.RefProp(1) = Round(ActualReferencePoint.Y - BBMinY, 3)
        End If

        ProjectionView.RefProp(2) = Round(ActualReferencePoint.Z, 3)
    End Sub

    'setting up the reference point value for visible surface
    Public Sub SetRefPointDoubleMirror(ByRef ProjectionView As ViewProp)
        ProjectionView.RefProp(0) = ProjectionView.BoundProp(4) - ProjectionView.RefProp(0)
        ProjectionView.RefProp(1) = ProjectionView.BoundProp(5) - ProjectionView.RefProp(1)
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
    Private Sub AddToTable(ByRef FeatureTmp As OutputFormat, ByVal List As System.Data.DataTable, _
                           ByVal Table As System.Windows.Forms.DataGridView)

        Dim NewRow As DataRow = List.NewRow()
        NewRow("State") = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\exclamation.png")
        NewRow("Name") = FeatureTmp.MiscProp(0)
        NewRow("Surface") = FeatureTmp.MiscProp(1)
        NewRow("Biner") = "0"
        NewRow("Object") = FeatureTmp

        List.Rows.Add(NewRow)

    End Sub

    Private MillFeatureList As List(Of OutputFormat)
    Private MillingProcessor As MillingProcessor

    Public Shared IdentifiedCounter, UnIdentifiedCounter As Integer

    Private GetPoints As GetPoints
    Private AllPoints As List(Of Point3d)
    Private GroupOfEntity As List(Of AllPoints)
    Private UnAdjacentPoints As List(Of Point3d)
    Private GeometryProcessor As GeometryProcessor

    Private MillingObjectId As List(Of ObjectId)

    Private MainLoop As List(Of Entity)
    Private MainLoopPts As List(Of Point3d)
    Private GroupLoop As List(Of List(Of Entity))
    Private GroupLoopPoints As List(Of List(Of Point3d))

    Private PolygonProcessor As PolygonProcessor
    Private ViewProcessor As ViewProcessor

    Private Function isequal(ByVal x As Double, ByVal y As Double) As Boolean
        If Math.Abs(x - y) > adskClass.AppPreferences.ToleranceValues Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function isequalpoint(ByVal point1 As Point3d, ByVal point2 As Point3d) As Boolean
        If Math.Abs(point1.X - point2.X) <= adskClass.AppPreferences.ToleranceValues _
        And Math.Abs(point1.Y - point2.Y) <= adskClass.AppPreferences.ToleranceValues _
        And Math.Abs(point1.Z - point2.Z) <= adskClass.AppPreferences.ToleranceValues Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Feature As OutputFormat
    Public Shared ProjectView As ViewProp
    Public Shared LastViewSelected As ViewProp

    Private index As Integer = 0

    Public Shared IdentifiedFeature As New List(Of OutputFormat)
    Public Shared UnIdentifiedFeature As New List(Of OutputFormat)

    Public Shared ReadOnly TmpIdentifiedFeature As New List(Of OutputFormat)
    Public Shared ReadOnly TmpUnidentifiedFeature As New List(Of OutputFormat)

    Public Shared HiddenFeature As List(Of OutputFormat)

    Public Shared ProjectionView As New List(Of ViewProp)

End Class 'AcEdSSGetCommand
'End Namespace 'Selection