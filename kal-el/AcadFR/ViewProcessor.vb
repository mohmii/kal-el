Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.Math
Imports FR

Public Class ViewProcessor
    Private Check2Database As DatabaseConn
    Private MillingObjectId As List(Of ObjectId)
    Private ListLoopTemp As List(Of List(Of Entity))
    Private PolygonProcessor As PolygonProcessor
    Private Feature As OutputFormat
    Private SeqBound, SeqHid As Boolean
    Private SolLine, SolLineBound, VirtuLine, HidLine, SolArc, HidArc As Integer

    'regular method
    Public Overloads Sub SingleViewProcessor(ByVal View As ViewProp, ByRef UnIdentifiedFeature As List(Of OutputFormat), ByRef TmpUnidentifiedFeature As List(Of OutputFormat), ByRef UnIdentifiedCounter As Integer)

        'initiate the progress bar
        UserControl3.acedSetStatusBarProgressMeter("Milling Features", 0, View.GroupLoop.Count)
        Dim i As Integer

        For Each GroupEntity As List(Of Entity) In View.GroupLoop
            If GroupEntity.Count <= 8 Then
                MillingObjectId = New List(Of ObjectId)
                Feature = New OutputFormat
                ListLoopTemp = New List(Of List(Of Entity))

                'add to the unidentified feature list
                For Each EntityTmp As Entity In GroupEntity
                    MillingObjectId.Add(EntityTmp.ObjectId)
                Next

                CountEntity(GroupEntity, View, SolLine, SolLineBound, _
                            VirtuLine, HidLine, SolArc, HidArc, SeqBound, SeqHid)

                ListLoopTemp.Add(GroupEntity)

                Feature.EntityMember = MillingObjectId.Count
                Feature.ObjectId = MillingObjectId
                Feature.ListLoop = ListLoopTemp
                Feature.SolidLineCount = SolLine
                Feature.SolidLineInBoundCount = SolLineBound
                Feature.VirtualLineCount = VirtuLine
                Feature.HiddenLineCount = HidLine
                Feature.SolidArcCount = SolArc
                Feature.HiddenArcCount = HidArc
                Feature.SequenceSolidBound = SeqBound
                Feature.SequenceSolidHidden = SeqHid

                SingleViewProp(Feature, GroupEntity, View)

                If adskClass.AppPreferences.MultiAnalysis = True Then
                    'set the feature property

                    Feature.FeatureName = "Mill Candidate"
                    Feature.MiscProp(0) = "Mill Candidate"
                    Feature.MiscProp(1) = View.ViewType
                    If Feature.HiddenLineCount > 0 Then
                        'Feature.MiscProp(1) = SearchOppositeSurf(View.ViewType)
                        'adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                        SelectionCommand.HiddenFeature.Add(Feature)
                        SelectionCommand.HiddenEntity.Add(GroupEntity(0))
                    Else
                        UnIdentifiedCounter = UnIdentifiedCounter + 1
                        UnIdentifiedFeature.Add(Feature)
                        TmpUnidentifiedFeature.Add(Feature)
                        AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                    End If

                    
                Else
                    If Not Feature.FeatureName = "" Then
                        'set the feature property
                        Feature.MiscProp(1) = View.ViewType
                        If Feature.HiddenLineCount > 0 Then
                            'Feature.MiscProp(1) = SearchOppositeSurf(View.ViewType)
                            'adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                            SelectionCommand.HiddenFeature.Add(Feature)
                            SelectionCommand.HiddenEntity.Add(GroupEntity(0))
                        Else
                            UnIdentifiedCounter = UnIdentifiedCounter + 1
                            UnIdentifiedFeature.Add(Feature)
                            TmpUnidentifiedFeature.Add(Feature)
                            AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                        End If


                    End If
                End If
            End If
            'add the progress bar
            i = i + 1
            'System.Threading.Thread.Sleep(1)
            UserControl3.acedSetStatusBarProgressMeterPos(i)
            System.Windows.Forms.Application.DoEvents()
        Next

        UserControl3.acedRestoreStatusBar()
    End Sub

    'add feature manual method
    Public Overloads Sub SingleViewProcessor(ByVal GLoop As List(Of List(Of Entity)), ByVal View As ViewProp, _
                                             ByRef UnIdentifiedFeature As List(Of OutputFormat), ByRef TmpUnidentifiedFeature As List(Of OutputFormat), _
                                             ByVal GLoopPts As List(Of List(Of Point3d)), ByRef UnIdentifiedCounter As Integer)
        Dim Selcom As New SelectionCommand
        Dim FeatureHidList As New List(Of OutputFormat)
        For Each GroupEntity As List(Of Entity) In GLoop
            MillingObjectId = New List(Of ObjectId)
            Feature = New OutputFormat
            ListLoopTemp = New List(Of List(Of Entity))

            'add to the unidentified feature list
            For Each EntityTmp As Entity In GroupEntity
                MillingObjectId.Add(EntityTmp.ObjectId)
            Next

            CountEntity(GroupEntity, View, SolLine, SolLineBound, VirtuLine, HidLine, SolArc, HidArc, SeqBound, SeqHid)

            ListLoopTemp.Add(GroupEntity)
            Feature.EntityMember = MillingObjectId.Count
            Feature.ObjectId = MillingObjectId
            Feature.ListLoop = ListLoopTemp
            Feature.SolidLineCount = SolLine
            Feature.SolidLineInBoundCount = SolLineBound
            Feature.VirtualLineCount = VirtuLine
            Feature.HiddenLineCount = HidLine
            Feature.SolidArcCount = SolArc
            Feature.HiddenArcCount = HidArc
            Feature.SequenceSolidBound = SeqBound
            Feature.SequenceSolidHidden = SeqHid
            SingleViewProp(Feature, GroupEntity, View, GLoopPts, GLoop)


            If Not Feature.FeatureName = "" Then
                'set the feature property
                Feature.MiscProp(1) = View.ViewType

                If Feature.HiddenLineCount > 0 Then
                    'Feature.MiscProp(1) = SearchOppositeSurf(View.ViewType)
                    'adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                    SelectionCommand.HiddenFeature.Clear()
                    SelectionCommand.HiddenFeature.Add(Feature)
                    SelectionCommand.HiddenEntity.Add(GroupEntity(0))
                    Selcom.HiddenInitiate(SelectionCommand.HiddenFeature, SelectionCommand.ProjectView, View.ViewType, SelectionCommand.ProjectionView)
                Else
                    UnIdentifiedCounter = UnIdentifiedCounter + 1
                    UnIdentifiedFeature.Add(Feature)
                    TmpUnidentifiedFeature.Add(Feature)
                    AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                End If

            Else
                'set the feature property
                Feature.FeatureName = "Mill Candidate"
                Feature.MiscProp(0) = "Mill Candidate"
                Feature.MiscProp(1) = View.ViewType
                If Feature.HiddenLineCount > 0 Then
                    'Feature.MiscProp(1) = SearchOppositeSurf(View.ViewType)
                    'adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                    SelectionCommand.HiddenFeature.Clear()
                    SelectionCommand.HiddenFeature.Add(Feature)
                    SelectionCommand.HiddenEntity.Add(GroupEntity(0))
                    Selcom.HiddenInitiate(SelectionCommand.HiddenFeature, SelectionCommand.ProjectView, View.ViewType, SelectionCommand.ProjectionView)
                Else
                    UnIdentifiedCounter = UnIdentifiedCounter + 1
                    UnIdentifiedFeature.Add(Feature)
                    TmpUnidentifiedFeature.Add(Feature)
                    AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                End If
            End If
        Next
    End Sub

    'method for regular single view
    Public Overloads Sub SingleViewProp(ByRef Feature As OutputFormat, ByVal GEntity As List(Of Entity), ByVal View As ViewProp)
        Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
        Dim Orientation As String = 0
        Dim StatOnBound, StatOnOrigin As Boolean
        Dim TmpLine As Line
        Dim TmpArc As Arc

        'Slot with D1, D2
        If (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0 And Feature.SequenceSolidBound = True) Or _
        (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0 And Feature.SequenceSolidBound = True) Then
            Dim D2Stat As Boolean = False
            For Each EntityTmp As Entity In GEntity
                TmpLine = New Line
                TmpLine = EntityTmp
                StatOnBound = New Boolean
                StatOnOrigin = New Boolean
                If D2Stat = False Then
                    D2 = Round(LineLength(TmpLine), 3)
                    D2Stat = True
                Else
                    D2 = Max(D2, Round(LineLength(TmpLine), 3))
                End If

                If PointOnline(TmpLine.StartPoint, View.BoundingBox(3).StartPoint, View.BoundingBox(3).EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, View.BoundingBox(3).StartPoint, View.BoundingBox(3).EndPoint) = 2 Then
                    Orientation = "0" 'Horizontal
                    D1 = Round(LineLength(TmpLine), 3)
                    OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                    OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                ElseIf PointOnline(TmpLine.StartPoint, View.BoundingBox(0).StartPoint, View.BoundingBox(0).EndPoint) = 2 And _
                    PointOnline(TmpLine.EndPoint, View.BoundingBox(0).StartPoint, View.BoundingBox(0).EndPoint) = 2 Then
                    Orientation = "1" 'Vertical
                    D1 = Round(LineLength(TmpLine), 3)
                    OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                    OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                End If
            Next
            Feature.FeatureName = "Square Slot"
            Feature.MiscProp(0) = "Square Slot"

            'Step with D1, D2
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 3 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0) _
        Or (Feature.SolidLineCount = 3 And Feature.SolidLineInBoundCount = 3 And Feature.HiddenLineCount = 1 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0) Then
            For Each EntityTmp As Entity In GEntity
                TmpLine = New Line
                TmpLine = EntityTmp
                For Each LineBB As Line In View.BoundingBox
                    If (isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint)) = True _
                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = True Then
                        D2 = Round(LineLength(TmpLine), 3)
                        If LineBB = View.BoundingBox(0) Then
                            Orientation = "0" 'Lower Side
                            OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                        ElseIf LineBB = View.BoundingBox(1) Then
                            Orientation = "3" 'Right Side
                            OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                        ElseIf LineBB = View.BoundingBox(2) Then
                            Orientation = "1" 'Upper Side
                            OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                        ElseIf LineBB = View.BoundingBox(3) Then
                            Orientation = "2" 'Left Side
                            OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                        End If
                        Exit For
                    ElseIf ((isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = False) _
                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = False _
                    Or (isequalpoint(TmpLine.EndPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.EndPoint)) = False _
                    Or (isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = False)) And D1 = 0 Then
                        D1 = Round(LineLength(TmpLine), 3)
                        Exit For
                    End If
                Next
            Next
            Feature.FeatureName = "Square Step"
            Feature.MiscProp(0) = "Square Step"

            '2-side pocket with D1, D2, D4
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 1 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 1) Then
            Dim Position1 As String = Nothing
            Dim Position2 As String = Nothing
            Dim DimPos1, DimPos2 As New Double
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    For Each LineBB As Line In View.BoundingBox
                        If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2) Then
                            If LineBB = View.BoundingBox(0) Then
                                If IsNothing(Position1) Then
                                    Position1 = "lower"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "lower"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            ElseIf LineBB = View.BoundingBox(1) Then
                                If IsNothing(Position1) Then
                                    Position1 = "right"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "right"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            ElseIf LineBB = View.BoundingBox(2) Then
                                If IsNothing(Position1) Then
                                    Position1 = "upper"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "upper"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            ElseIf LineBB = View.BoundingBox(3) Then
                                If IsNothing(Position1) Then
                                    Position1 = "left"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "left"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            End If
                            Exit For
                        End If
                    Next
                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    D4 = Round(TmpArc.Radius, 3)
                End If
            Next

            'searching the orientation and origin
            If (Position1 = "lower" And Position2 = "left") Or (Position1 = "left" And Position2 = "lower") Then
                Orientation = "0" 'Lower Left
                OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "lower" And Position2 = "left") Then
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                Else
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                End If
            ElseIf (Position1 = "lower" And Position2 = "right") Or (Position1 = "right" And Position2 = "lower") Then
                Orientation = "1" 'Lower Right
                OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "lower" And Position2 = "right") Then
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                Else
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                End If
            ElseIf (Position1 = "upper" And Position2 = "right") Or (Position1 = "right" And Position2 = "upper") Then
                Orientation = "3" 'Upper Right
                OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "upper" And Position2 = "right") Then
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                Else
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                End If
            ElseIf (Position1 = "upper" And Position2 = "left") Or (Position1 = "left" And Position2 = "upper") Then
                Orientation = "2" 'Upper Left
                OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "upper" And Position2 = "left") Then
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                Else
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                End If
            End If
            Feature.FeatureName = "2-side Pocket"
            Feature.MiscProp(0) = "2-side Pocket"

            '3-side pocket with D1, D2, D4
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 2 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 1 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 3 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 2) Then
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    For Each LineBB As Line In View.BoundingBox
                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                            D1 = Round(LineLength(TmpLine), 3)
                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                            If LineBB = View.BoundingBox(0) Then
                                Orientation = "0" 'Lower Side
                            ElseIf LineBB = View.BoundingBox(1) Then
                                Orientation = "3" 'Right Side
                            ElseIf LineBB = View.BoundingBox(2) Then
                                Orientation = "1" 'Upper Side
                            ElseIf LineBB = View.BoundingBox(3) Then
                                Orientation = "2" 'Left Side
                            End If
                            Exit For
                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                            (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2 And _
                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2)) Then
                            D2 = Round(LineLength(TmpLine), 3)
                            Exit For
                        End If
                    Next
                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    If D4 = 0 Then
                        D4 = Round(TmpArc.Radius, 3)
                    End If
                End If
            Next
            D2 = D2 + D4
            Feature.FeatureName = "3-side Pocket"
            Feature.MiscProp(0) = "3-side Pocket"

            '4-side pocket with D1, D2, D4, angle
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 4 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 0 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 4 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 4) Then
            Dim AngleTmp As New Double
            Dim InitialStat As Boolean = True
            Dim Origin As Point3d
            PolygonProcessor = New PolygonProcessor
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    AngleTmp = Atan2(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y, TmpLine.EndPoint.X - TmpLine.StartPoint.X) * (180 / PI)
                    If AngleTmp < 0 Then
                        AngleTmp = AngleTmp + 360
                    End If
                    If (AngleTmp >= 0 And AngleTmp < 90) Or (AngleTmp >= 180 And AngleTmp < 270) Then
                        D1 = Round(LineLength(TmpLine), 3)
                        If (AngleTmp >= 180 And AngleTmp < 270) Then
                            AngleTmp = AngleTmp - 180
                        End If
                    ElseIf (AngleTmp >= 90 And AngleTmp < 180) Or (AngleTmp >= 270 And AngleTmp < 360) Then
                        D2 = Round(LineLength(TmpLine), 3)
                        If (AngleTmp >= 270 And AngleTmp < 360) Then
                            AngleTmp = AngleTmp - 180
                        End If
                    End If

                    If InitialStat = True Then
                        Angle = AngleTmp
                        InitialStat = False
                    Else
                        Angle = Min(Angle, AngleTmp)
                    End If

                ElseIf (TypeOf EntityTmp Is Arc) And (D4 = 0) Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    D4 = Round(TmpArc.Radius, 3)
                End If
            Next
            D1 = D1 + (2 * D4)
            D2 = D2 + (2 * D4)
            PolygonProcessor.GetArea(View.GroupLoopPoints(View.GroupLoop.IndexOf(GEntity)))
            Origin = PolygonProcessor.GetCentroid(View.GroupLoopPoints(View.GroupLoop.IndexOf(GEntity)))
            OriU = Origin.X - View.BoundProp(0) - View.RefProp(0)
            OriV = Origin.Y - View.BoundProp(1) - View.RefProp(1)
            Feature.FeatureName = "4-side Pocket"
            Feature.MiscProp(0) = "4-side Pocket"

            'long hole with D1, D2, D4, angle
        ElseIf (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 2 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 0 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 2) Then
            Dim D2Temp As New Double
            Dim Origin As Point3d
            PolygonProcessor = New PolygonProcessor
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    If D1 = 0 Then
                        D1 = Round(LineLength(TmpLine), 3)
                    End If
                    Angle = Atan2(Abs(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y), Abs(TmpLine.EndPoint.X - TmpLine.StartPoint.X)) * (180 / PI)

                    If Angle < 0 Then
                        Angle = Angle + 360
                    End If

                    If Angle >= 180 Then
                        Angle = Angle - 180
                    End If

                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    If D2 = 0 Then
                        D2 = 2 * Round(TmpArc.Radius, 3)
                    End If
                End If
            Next
            D1 = D1 + D2
            PolygonProcessor.GetArea(View.GroupLoopPoints(View.GroupLoop.IndexOf(GEntity)))
            Origin = PolygonProcessor.GetCentroid(View.GroupLoopPoints(View.GroupLoop.IndexOf(GEntity)))
            OriU = Origin.X - View.BoundProp(0) - View.RefProp(0)
            OriV = Origin.Y - View.BoundProp(1) - View.RefProp(1)
            Feature.FeatureName = "Long Hole"
            Feature.MiscProp(0) = "Long Hole"

            'Blind Slot with D1, D2
        ElseIf (Feature.SolidLineCount = 3 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 1 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 1 And Feature.SolidArcCount = 1 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 1 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 1) Then
            Dim TempRad As New Double
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    For Each LineBB As Line In View.BoundingBox
                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                            D1 = Round(LineLength(TmpLine), 3)
                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                            If LineBB = View.BoundingBox(0) Then
                                Orientation = "0" 'Lower Side
                            ElseIf LineBB = View.BoundingBox(1) Then
                                Orientation = "3" 'Right Side
                            ElseIf LineBB = View.BoundingBox(2) Then
                                Orientation = "1" 'Upper Side
                            ElseIf LineBB = View.BoundingBox(3) Then
                                Orientation = "2" 'Left Side
                            End If
                            Exit For
                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                            (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2 And _
                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2)) And D2 = 0 Then
                            D2 = Round(LineLength(TmpLine), 3)
                            Exit For
                        End If
                    Next
                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    If TempRad = 0 Then
                        TempRad = Round(TmpArc.Radius, 3)
                    End If
                End If
            Next
            D2 = D2 + TempRad
            Feature.FeatureName = "Blind Slot"
            Feature.MiscProp(0) = "Blind Slot"

        ElseIf adskClass.AppPreferences.MultiAnalysis = True Then
            'not main loop of a feature
            'Slot with D1 dan D3
            If Feature.SolidLineCount = 3 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 0 _
            And Feature.VirtualLineCount = 1 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0 Then
                For Each EntityTmp As Entity In GEntity
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    For Each LineBB As Line In View.BoundingBox
                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                            D1 = Round(LineLength(TmpLine), 3)
                            Exit For
                        ElseIf (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2 And _
                                PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2) Then
                            D3 = Round(LineLength(TmpLine), 3)
                            Exit For
                        End If
                    Next
                Next

                '3-side pocket or blind slot with D1, D3
            ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 0 _
            And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0) Then
                For Each EntityTmp As Entity In GEntity
                    If TypeOf EntityTmp Is Line Then
                        TmpLine = New Line
                        TmpLine = EntityTmp
                        For Each LineBB As Line In View.BoundingBox
                            If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                                D1 = Round(LineLength(TmpLine), 3)
                                If LineBB = View.BoundingBox(0) Then
                                    Orientation = "1" 'Upper Side
                                ElseIf LineBB = View.BoundingBox(1) Then
                                    Orientation = "2" 'Left Side
                                ElseIf LineBB = View.BoundingBox(2) Then
                                    Orientation = "0" 'Lower Side
                                ElseIf LineBB = View.BoundingBox(3) Then
                                    Orientation = "3" 'Right Side
                                End If
                                Exit For
                            ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2 And _
                                PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2)) Then
                                D3 = Round(LineLength(TmpLine), 3)
                                Exit For
                            End If
                        Next
                    End If
                Next
                D2 = D2 + D4
            End If
        End If

        Feature.MiscProp(2) = Orientation
        Feature.OriginAndAddition(0) = Round(OriU, 3)
        Feature.OriginAndAddition(1) = Round(OriV, 3)
        Feature.OriginAndAddition(2) = Round(OriW, 3)
        Feature.OriginAndAddition(3) = D1
        Feature.OriginAndAddition(4) = D2
        Feature.OriginAndAddition(5) = D3
        Feature.OriginAndAddition(6) = D4
        Feature.OriginAndAddition(7) = Angle
    End Sub

    'method for added manual single view
    Public Overloads Sub SingleViewProp(ByRef Feature As OutputFormat, ByVal GEntity As List(Of Entity), ByVal View As ViewProp, _
                                        ByVal GLoopPts As List(Of List(Of Point3d)), ByVal GLoop As List(Of List(Of Entity)))
        Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
        Dim Orientation As String = 0
        Dim StatOnBound, StatOnOrigin As Boolean
        Dim TmpLine As Line
        Dim TmpArc As Arc

        'Slot with D1, D2
        If (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0 And Feature.SequenceSolidBound = True) Or _
        (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0 And Feature.SequenceSolidBound = True) Then
            Dim D2Stat As Boolean = False
            For Each EntityTmp As Entity In GEntity
                TmpLine = New Line
                TmpLine = EntityTmp
                StatOnBound = New Boolean
                StatOnOrigin = New Boolean
                If D2Stat = False Then
                    D2 = Round(LineLength(TmpLine), 3)
                    D2Stat = True
                Else
                    D2 = Max(D2, Round(LineLength(TmpLine), 3))
                End If

                If PointOnline(TmpLine.StartPoint, View.BoundingBox(3).StartPoint, View.BoundingBox(3).EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, View.BoundingBox(3).StartPoint, View.BoundingBox(3).EndPoint) = 2 Then
                    Orientation = "0" 'Horizontal
                    D1 = Round(LineLength(TmpLine), 3)
                    OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                    OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                ElseIf PointOnline(TmpLine.StartPoint, View.BoundingBox(0).StartPoint, View.BoundingBox(0).EndPoint) = 2 And _
                    PointOnline(TmpLine.EndPoint, View.BoundingBox(0).StartPoint, View.BoundingBox(0).EndPoint) = 2 Then
                    Orientation = "1" 'Vertical
                    D1 = Round(LineLength(TmpLine), 3)
                    OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                    OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                End If
            Next
            Feature.FeatureName = "Square Slot"
            Feature.MiscProp(0) = "Square Slot"

            'Step with D1, D2
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 3 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0) _
        Or (Feature.SolidLineCount = 3 And Feature.SolidLineInBoundCount = 3 And Feature.HiddenLineCount = 1 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 0) Then
            For Each EntityTmp As Entity In GEntity
                TmpLine = New Line
                TmpLine = EntityTmp
                For Each LineBB As Line In View.BoundingBox
                    If (isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint)) = True _
                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = True Then
                        D2 = Round(LineLength(TmpLine), 3)
                        If LineBB = View.BoundingBox(0) Then
                            Orientation = "0" 'Lower Side
                            OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                        ElseIf LineBB = View.BoundingBox(1) Then
                            Orientation = "3" 'Right Side
                            OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                        ElseIf LineBB = View.BoundingBox(2) Then
                            Orientation = "1" 'Upper Side
                            OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                        ElseIf LineBB = View.BoundingBox(3) Then
                            Orientation = "2" 'Left Side
                            OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                            OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                        End If
                        Exit For
                    ElseIf ((isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = False) _
                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = False _
                    Or (isequalpoint(TmpLine.EndPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.EndPoint)) = False _
                    Or (isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = False)) And D1 = 0 Then
                        D1 = Round(LineLength(TmpLine), 3)
                        Exit For
                    End If
                Next
            Next
            Feature.FeatureName = "Square Step"
            Feature.MiscProp(0) = "Square Step"

            '2-side pocket with D1, D2, D4
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 1 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 2 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 1) Then
            Dim Position1 As String = Nothing
            Dim Position2 As String = Nothing
            Dim DimPos1, DimPos2 As New Double
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    For Each LineBB As Line In View.BoundingBox
                        If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2) Then
                            If LineBB = View.BoundingBox(0) Then
                                If IsNothing(Position1) Then
                                    Position1 = "lower"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "lower"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            ElseIf LineBB = View.BoundingBox(1) Then
                                If IsNothing(Position1) Then
                                    Position1 = "right"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "right"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            ElseIf LineBB = View.BoundingBox(2) Then
                                If IsNothing(Position1) Then
                                    Position1 = "upper"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "upper"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            ElseIf LineBB = View.BoundingBox(3) Then
                                If IsNothing(Position1) Then
                                    Position1 = "left"
                                    DimPos1 = LineLength(TmpLine)
                                Else
                                    Position2 = "left"
                                    DimPos2 = LineLength(TmpLine)
                                End If
                            End If
                            Exit For
                        End If
                    Next
                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    D4 = Round(TmpArc.Radius, 3)
                End If
            Next

            'searching the orientation and origin
            If (Position1 = "lower" And Position2 = "left") Or (Position1 = "left" And Position2 = "lower") Then
                Orientation = "0" 'Lower Left
                OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "lower" And Position2 = "left") Then
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                Else
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                End If
            ElseIf (Position1 = "lower" And Position2 = "right") Or (Position1 = "right" And Position2 = "lower") Then
                Orientation = "1" 'Lower Right
                OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(1) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "lower" And Position2 = "right") Then
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                Else
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                End If
            ElseIf (Position1 = "upper" And Position2 = "right") Or (Position1 = "right" And Position2 = "upper") Then
                Orientation = "3" 'Upper Right
                OriU = View.BoundProp(2) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "upper" And Position2 = "right") Then
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                Else
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                End If
            ElseIf (Position1 = "upper" And Position2 = "left") Or (Position1 = "left" And Position2 = "upper") Then
                Orientation = "2" 'Upper Left
                OriU = View.BoundProp(0) - View.BoundProp(0) - View.RefProp(0)
                OriV = View.BoundProp(3) - View.BoundProp(1) - View.RefProp(1)
                If (Position1 = "upper" And Position2 = "left") Then
                    D2 = Round(DimPos1, 3)
                    D1 = Round(DimPos2, 3)
                Else
                    D1 = Round(DimPos1, 3)
                    D2 = Round(DimPos2, 3)
                End If
            End If
            Feature.FeatureName = "2-side Pocket"
            Feature.MiscProp(0) = "2-side Pocket"

            '3-side pocket with D1, D2, D4
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 2 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 1 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 3 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 2) Then
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    For Each LineBB As Line In View.BoundingBox
                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                            D1 = Round(LineLength(TmpLine), 3)
                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                            If LineBB = View.BoundingBox(0) Then
                                Orientation = "0" 'Lower Side
                            ElseIf LineBB = View.BoundingBox(1) Then
                                Orientation = "3" 'Right Side
                            ElseIf LineBB = View.BoundingBox(2) Then
                                Orientation = "1" 'Upper Side
                            ElseIf LineBB = View.BoundingBox(3) Then
                                Orientation = "2" 'Left Side
                            End If
                            Exit For
                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                            (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2 And _
                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2)) Then
                            D2 = Round(LineLength(TmpLine), 3)
                            Exit For
                        End If
                    Next
                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    If D4 = 0 Then
                        D4 = Round(TmpArc.Radius, 3)
                    End If
                End If
            Next
            D2 = D2 + D4
            Feature.FeatureName = "3-side Pocket"
            Feature.MiscProp(0) = "3-side Pocket"

            '4-side pocket with D1, D2, D4, angle
        ElseIf (Feature.SolidLineCount = 4 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 4 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 0 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 4 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 4) Then
            Dim AngleTmp As New Double
            Dim InitialStat As Boolean = True
            Dim Origin As Point3d
            PolygonProcessor = New PolygonProcessor
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    AngleTmp = Atan2(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y, TmpLine.EndPoint.X - TmpLine.StartPoint.X) * (180 / PI)
                    If AngleTmp < 0 Then
                        AngleTmp = AngleTmp + 360
                    End If
                    If (AngleTmp >= 0 And AngleTmp < 90) Or (AngleTmp >= 180 And AngleTmp < 270) Then
                        D1 = Round(LineLength(TmpLine), 3)
                        If (AngleTmp >= 180 And AngleTmp < 270) Then
                            AngleTmp = AngleTmp - 180
                        End If
                    ElseIf (AngleTmp >= 90 And AngleTmp < 180) Or (AngleTmp >= 270 And AngleTmp < 360) Then
                        D2 = Round(LineLength(TmpLine), 3)
                        If (AngleTmp >= 270 And AngleTmp < 360) Then
                            AngleTmp = AngleTmp - 180
                        End If
                    End If

                    If InitialStat = True Then
                        Angle = AngleTmp
                        InitialStat = False
                    Else
                        Angle = Min(Angle, AngleTmp)
                    End If

                ElseIf (TypeOf EntityTmp Is Arc) And (D4 = 0) Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    D4 = Round(TmpArc.Radius, 3)
                End If
            Next
            D1 = D1 + (2 * D4)
            D2 = D2 + (2 * D4)
            PolygonProcessor.GetArea(GLoopPts(GLoop.IndexOf(GEntity)))
            Origin = PolygonProcessor.GetCentroid(GLoopPts(GLoop.IndexOf(GEntity)))
            OriU = Origin.X - View.BoundProp(0) - View.RefProp(0)
            OriV = Origin.Y - View.BoundProp(1) - View.RefProp(1)
            Feature.FeatureName = "4-side Pocket"
            Feature.MiscProp(0) = "4-side Pocket"

            'long hole with D1, D2, D4, angle
        ElseIf (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 2 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 0 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 2) Then
            Dim D2Temp As New Double
            Dim Origin As Point3d
            PolygonProcessor = New PolygonProcessor
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    If D1 = 0 Then
                        D1 = Round(LineLength(TmpLine), 3)
                    End If
                    Angle = Atan2(Abs(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y), Abs(TmpLine.EndPoint.X - TmpLine.StartPoint.X)) * (180 / PI)

                    If Angle < 0 Then
                        Angle = Angle + 360
                    End If

                    If Angle >= 180 Then
                        Angle = Angle - 180
                    End If

                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    If D2 = 0 Then
                        D2 = 2 * Round(TmpArc.Radius, 3)
                    End If
                End If
            Next
            D1 = D1 + D2
            PolygonProcessor.GetArea(GLoopPts(GLoop.IndexOf(GEntity)))
            Origin = PolygonProcessor.GetCentroid(GLoopPts(GLoop.IndexOf(GEntity)))
            OriU = Origin.X - View.BoundProp(0) - View.RefProp(0)
            OriV = Origin.Y - View.BoundProp(1) - View.RefProp(1)
            Feature.FeatureName = "Long Hole"
            Feature.MiscProp(0) = "Long Hole"

            'Blind Slot with D1, D2
        ElseIf (Feature.SolidLineCount = 3 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 1 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 2 And Feature.SolidLineInBoundCount = 0 And Feature.HiddenLineCount = 0 _
        And Feature.VirtualLineCount = 1 And Feature.SolidArcCount = 1 And Feature.HiddenArcCount = 0) Or _
        (Feature.SolidLineCount = 1 And Feature.SolidLineInBoundCount = 1 And Feature.HiddenLineCount = 2 _
        And Feature.VirtualLineCount = 0 And Feature.SolidArcCount = 0 And Feature.HiddenArcCount = 1) Then
            Dim TempRad As New Double
            For Each EntityTmp As Entity In GEntity
                If TypeOf EntityTmp Is Line Then
                    TmpLine = New Line
                    TmpLine = EntityTmp
                    For Each LineBB As Line In View.BoundingBox
                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                            D1 = Round(LineLength(TmpLine), 3)
                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - View.BoundProp(0) - View.RefProp(0)
                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - View.BoundProp(1) - View.RefProp(1)
                            If LineBB = View.BoundingBox(0) Then
                                Orientation = "0" 'Lower Side
                            ElseIf LineBB = View.BoundingBox(1) Then
                                Orientation = "3" 'Right Side
                            ElseIf LineBB = View.BoundingBox(2) Then
                                Orientation = "1" 'Upper Side
                            ElseIf LineBB = View.BoundingBox(3) Then
                                Orientation = "2" 'Left Side
                            End If
                            Exit For
                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                            (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2 And _
                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2)) And D2 = 0 Then
                            D2 = Round(LineLength(TmpLine), 3)
                            Exit For
                        End If
                    Next
                ElseIf TypeOf EntityTmp Is Arc Then
                    TmpArc = New Arc
                    TmpArc = EntityTmp
                    If TempRad = 0 Then
                        TempRad = Round(TmpArc.Radius, 3)
                    End If
                End If
            Next
            D2 = D2 + TempRad
            Feature.FeatureName = "Blind Slot"
            Feature.MiscProp(0) = "Blind Slot"
        End If

        Feature.MiscProp(2) = Orientation
        Feature.OriginAndAddition(0) = Round(OriU, 3)
        Feature.OriginAndAddition(1) = Round(OriV, 3)
        Feature.OriginAndAddition(2) = Round(OriW, 3)
        Feature.OriginAndAddition(3) = D1
        Feature.OriginAndAddition(4) = D2
        Feature.OriginAndAddition(5) = D3
        Feature.OriginAndAddition(6) = D4
        Feature.OriginAndAddition(7) = Angle
    End Sub


    Public Sub MultipleViewProcessor(ByVal ListView As List(Of ViewProp), ByVal ViewNum As Integer, ByRef UnIdentifiedFeature As List(Of OutputFormat), _
                                     ByRef TmpUnidentifiedFeature As List(Of OutputFormat), ByRef UnIdentifiedCounter As Integer, ByRef IdentifiedFeature As List(Of OutputFormat), _
                                     ByRef TmpIdentifiedFeature As List(Of OutputFormat), ByRef IdentifiedCounter As Integer)

        'variables needed for multiview rule-based
        Dim VLReference, SLReference, SLBReference, HLReference, SAReference, HAReference As Integer
        Dim VLCorresponding, SLCorresponding, SLBCorresponding, HLCorresponding, SACorresponding, HACorresponding As Integer
        Dim IdentificationStatus, InputStatus As Boolean
        Dim count2loop As Single
        Dim LineTmp, LineTmp2 As Line
        Dim ArcTmp, ArcTmp2 As Arc

        'initiate the progress bar
        UserControl3.acedSetStatusBarProgressMeter("Get Features", 0, ListView(ViewNum).GroupLoop.Count)
        Dim i As Integer

        For Each GroupEntity As List(Of Entity) In ListView(ViewNum).GroupLoop
            MillingObjectId = New List(Of ObjectId)
            IdentificationStatus = New Boolean

            CountEntity(GroupEntity, ListView(ViewNum), SLReference, SLBReference, VLReference, _
                        HLReference, SAReference, HAReference, SeqBound, SeqHid)

            'For Each ViewTmp As ViewProp In ListView
            'For j As Integer = ViewNum + 1 To ListView.Count - 1
            For j As Integer = 0 To ListView.Count - 2
                'If IsNothing(ViewTmp.ViewTag) Then
                For Each GroupEntity2 As List(Of Entity) In ListView(j).GroupLoop
                    count2loop = New Single
                    For Each EntityTmp As Entity In GroupEntity
                        For Each EntityTmp2 As Entity In GroupEntity2
                            If TypeOf EntityTmp2 Is Line Then
                                LineTmp2 = New Line
                                LineTmp2 = EntityTmp2
                                If TypeOf EntityTmp Is Line Then
                                    LineTmp = New Line
                                    LineTmp = EntityTmp
                                    If (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "right") Or (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "left") Or (ListView(ViewNum).ViewType.ToLower = "left" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "back") Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "right") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "left") Or (ListView(ViewNum).ViewType.ToLower = "left" And ListView(j).ViewType.ToLower = "back") Then
                                        If GPCFrontRight(LineTmp.StartPoint, LineTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                         ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                         ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "top") Or (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "bottom") Or (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "front") Then
                                        If GPCFrontTop(LineTmp.StartPoint, LineTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "top") Or (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "back") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "bottom") Or (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "back") Then
                                        If GPCFrontTop(LineTmp.StartPoint, LineTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(3)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "right") Then
                                        If GPCTopRight(LineTmp.StartPoint, LineTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "top") Then
                                        If GPCTopRight(LineTmp2.StartPoint, LineTmp2.EndPoint, LineTmp.StartPoint, LineTmp.EndPoint, _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1), _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "right") Then
                                        If GPCTopRight(LineTmp.StartPoint, LineTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(3), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "bottom") Then
                                        If GPCTopRight(LineTmp2.StartPoint, LineTmp2.EndPoint, LineTmp.StartPoint, LineTmp.EndPoint, _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1), _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(3)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If
                                    End If

                                ElseIf TypeOf EntityTmp Is Arc Then
                                    ArcTmp = New Arc
                                    ArcTmp = EntityTmp

                                    If (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "right") Or (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "left") Or (ListView(ViewNum).ViewType.ToLower = "left" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "back") Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "right") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "left") Or (ListView(ViewNum).ViewType.ToLower = "left" And ListView(j).ViewType.ToLower = "back") Then
                                        If GPCFrontRight(ArcTmp.StartPoint, ArcTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                         ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                         ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 2 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "top") Or (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "bottom") Or (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "front") Then
                                        If GPCFrontTop(ArcTmp.StartPoint, ArcTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 2 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "top") Or (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "back") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "bottom") Or (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "back") Then
                                        If GPCFrontTop(ArcTmp.StartPoint, ArcTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(3)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "right") Then
                                        If GPCTopRight(ArcTmp.StartPoint, ArcTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 2 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "top") Then
                                        If GPCTopRight(LineTmp2.StartPoint, LineTmp2.EndPoint, ArcTmp.StartPoint, ArcTmp.EndPoint, _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(2), _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1)) = 2 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "right") Then
                                        If GPCTopRight(ArcTmp.StartPoint, ArcTmp.EndPoint, LineTmp2.StartPoint, LineTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(3), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "bottom") Then
                                        If GPCTopRight(LineTmp2.StartPoint, LineTmp2.EndPoint, ArcTmp.StartPoint, ArcTmp.EndPoint, _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1), _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(3)) = 1 Then
                                            count2loop = count2loop + 1
                                        End If
                                    End If
                                End If

                            ElseIf TypeOf EntityTmp2 Is Arc Then
                                ArcTmp2 = New Arc
                                ArcTmp2 = EntityTmp2

                                If TypeOf EntityTmp Is Line Then
                                    LineTmp = New Line
                                    LineTmp = EntityTmp
                                    If (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "right") Or (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "left") Or (ListView(ViewNum).ViewType.ToLower = "left" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "back") Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "right") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "left") Or (ListView(ViewNum).ViewType.ToLower = "left" And ListView(j).ViewType.ToLower = "back") Then
                                        If GPCFrontRight(LineTmp.StartPoint, LineTmp.EndPoint, ArcTmp2.StartPoint, ArcTmp2.EndPoint, _
                                                         ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                         ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "top") Or (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "front") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "front" And ListView(j).ViewType.ToLower = "bottom") Or (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "front") Then
                                        If GPCFrontTop(LineTmp.StartPoint, LineTmp.EndPoint, ArcTmp2.StartPoint, ArcTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "top") Or (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "back") _
                                     Or (ListView(ViewNum).ViewType.ToLower = "back" And ListView(j).ViewType.ToLower = "bottom") Or (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "back") Then
                                        If GPCFrontTop(LineTmp.StartPoint, LineTmp.EndPoint, ArcTmp2.StartPoint, ArcTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(3)) = 1 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "top" And ListView(j).ViewType.ToLower = "right") Then
                                        If GPCTopRight(LineTmp.StartPoint, LineTmp.EndPoint, ArcTmp2.StartPoint, ArcTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "top") Then
                                        If GPCTopRight(ArcTmp2.StartPoint, ArcTmp2.EndPoint, LineTmp.StartPoint, LineTmp.EndPoint, _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1), _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "bottom" And ListView(j).ViewType.ToLower = "right") Then
                                        If GPCTopRight(LineTmp.StartPoint, LineTmp.EndPoint, ArcTmp2.StartPoint, ArcTmp2.EndPoint, _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(3), _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1)) = 1 Then
                                            count2loop = count2loop + 0.5
                                        End If

                                    ElseIf (ListView(ViewNum).ViewType.ToLower = "right" And ListView(j).ViewType.ToLower = "bottom") Then
                                        If GPCTopRight(ArcTmp2.StartPoint, ArcTmp2.EndPoint, LineTmp.StartPoint, LineTmp.EndPoint, _
                                                       ListView(j).BoundProp(0), ListView(j).BoundProp(1), _
                                                       ListView(ViewNum).BoundProp(0), ListView(ViewNum).BoundProp(3)) = 1 Then
                                            count2loop = count2loop + 0.5
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    Next

                    'empirical number > 4
                    If count2loop > 4 Then
                        CountEntity(GroupEntity2, ListView(j), SLCorresponding, SLBCorresponding, VLCorresponding, _
                                    HLCorresponding, SACorresponding, HACorresponding, SeqBound, SeqHid)

                        'rule-based for square slot features main ref
                        If (SLReference = 4 And SLBReference = 2 And SAReference = 0 And SLCorresponding = 3 _
                           And SLBCorresponding = 0 And HLCorresponding = 0 And VLCorresponding = 1) _
                           Or (SLReference = 2 And SLBReference = 2 And SAReference = 0 And HLReference = 2 And SLCorresponding = 3 _
                           And SLBCorresponding = 0 And HLCorresponding = 0 And VLCorresponding = 1) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim StatOnBound, StatOnOrigin As Boolean
                            Dim TmpLine As Line

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2
                            For Each EntityTmp As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp
                                StatOnBound = New Boolean
                                StatOnOrigin = New Boolean
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                                        StatOnBound = True
                                        If isequal(TmpLine.StartPoint.X, ListView(ViewNum).BoundProp(0)) = True And isequal(TmpLine.EndPoint.X, ListView(ViewNum).BoundProp(0)) = True Then
                                            StatOnOrigin = True
                                            Orientation = "0" 'Horizontal
                                        ElseIf isequal(TmpLine.StartPoint.Y, ListView(ViewNum).BoundProp(1)) = True And isequal(TmpLine.EndPoint.Y, ListView(ViewNum).BoundProp(1)) = True Then
                                            StatOnOrigin = True
                                            Orientation = "1" 'Vertical
                                        End If
                                        Exit For
                                    End If
                                Next
                                If StatOnBound = True And StatOnOrigin = True And D1 = 0 Then
                                    D1 = Round(LineLength(TmpLine), 3)
                                    OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                    OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                ElseIf StatOnBound = False And D2 = 0 Then
                                    D2 = Round(LineLength(TmpLine), 3)
                                End If
                            Next

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Square Slot"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Square Slot"
                            Feature.ListLoop = ListLoopTemp
                            If HLReference = 2 Then
                                If Orientation = "0" Then
                                    OriU = 0
                                Else
                                    OriU = -OriU
                                End If
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(ViewNum).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For


                            'rule-based for square slot features alt ref
                        ElseIf (SLReference = 3 And SLBReference = 0 And SAReference = 0 And HLReference = 0 And VLReference = 1 _
                                And SLCorresponding = 4 And SLBCorresponding = 2 And HLCorresponding = 0 And VLCorresponding = 0) _
                           Or (SLReference = 3 And SLBReference = 0 And SAReference = 0 And HLReference = 0 And VLReference = 1 _
                               And SLCorresponding = 2 And SLBCorresponding = 2 And HLCorresponding = 2 And VLCorresponding = 0) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim StatOnBound, StatOnOrigin As Boolean
                            Dim TmpLine As Line

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2
                            For Each EntityTmp As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp
                                StatOnBound = New Boolean
                                StatOnOrigin = New Boolean
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                                        StatOnBound = True
                                        If isequal(TmpLine.StartPoint.X, ListView(j).BoundProp(0)) = True And isequal(TmpLine.EndPoint.X, ListView(j).BoundProp(0)) = True Then
                                            StatOnOrigin = True
                                            Orientation = "0" 'Horizontal
                                        ElseIf isequal(TmpLine.StartPoint.Y, ListView(j).BoundProp(1)) = True And isequal(TmpLine.EndPoint.Y, ListView(j).BoundProp(1)) = True Then
                                            StatOnOrigin = True
                                            Orientation = "1" 'Vertical
                                        End If
                                        Exit For
                                    End If
                                Next
                                If StatOnBound = True And StatOnOrigin = True And D1 = 0 Then
                                    D1 = Round(LineLength(TmpLine), 3)
                                    OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                    OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                ElseIf StatOnBound = False And D2 = 0 Then
                                    D2 = Round(LineLength(TmpLine), 3)
                                End If
                            Next

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Square Slot"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Square Slot"
                            Feature.ListLoop = ListLoopTemp
                            If HLCorresponding = 2 Then
                                If Orientation = "0" Then
                                    OriU = 0
                                Else
                                    OriU = -OriU
                                End If
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(j).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(j).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For


                            'rule-based for square step features main
                        ElseIf (SLReference = 4 And SLBReference = 3 And HLReference = 0 And VLReference = 0 _
                                And SLCorresponding = 2 And SLBCorresponding = 0 And HLCorresponding = 0 And VLCorresponding = 2) _
                               Or (SLReference = 3 And SLBReference = 3 And HLReference = 1 And VLReference = 0 _
                                   And SLCorresponding = 2 And SLBCorresponding = 0 And HLCorresponding = 0 And VLCorresponding = 2) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim StatD3Step As Boolean
                            Dim TmpLine As Line

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2
                            For Each EntityTmp As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint)) = True _
                                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = True Then
                                        D2 = Round(LineLength(TmpLine), 3)
                                        If LineBB = ListView(ViewNum).BoundingBox(0) Then
                                            Orientation = "0" 'Lower Side
                                            OriU = ListView(ViewNum).BoundProp(0) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                            OriV = ListView(ViewNum).BoundProp(1) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                        ElseIf LineBB = ListView(ViewNum).BoundingBox(1) Then
                                            Orientation = "3" 'Right Side
                                            OriU = ListView(ViewNum).BoundProp(2) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                            OriV = ListView(ViewNum).BoundProp(1) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                        ElseIf LineBB = ListView(ViewNum).BoundingBox(2) Then
                                            Orientation = "1" 'Upper Side
                                            OriU = ListView(ViewNum).BoundProp(2) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                            OriV = ListView(ViewNum).BoundProp(3) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                        ElseIf LineBB = ListView(ViewNum).BoundingBox(3) Then
                                            Orientation = "2" 'Left Side
                                            OriU = ListView(ViewNum).BoundProp(0) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                            OriV = ListView(ViewNum).BoundProp(3) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                        End If
                                        Exit For
                                    ElseIf ((isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = False) _
                                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = False _
                                    Or (isequalpoint(TmpLine.EndPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.EndPoint)) = False _
                                    Or (isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = False)) And D1 = 0 Then
                                        D1 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                StatD3Step = New Boolean
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        If StatD3Step = False Then
                                            D3 = Round(LineLength(TmpLine), 3)
                                            StatD3Step = True
                                        ElseIf StatD3Step = True And isequal(D1, D3) = False Then
                                            D3 = Round(LineLength(TmpLine), 3)
                                            Exit For
                                        End If
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Square Step"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Square Step"
                            Feature.ListLoop = ListLoopTemp
                            If HLReference = 1 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "3" Then
                                    Orientation = "2"
                                ElseIf Orientation = "2" Then
                                    Orientation = "3"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(ViewNum).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For


                            'rule-based for square step features alt 
                        ElseIf (SLReference = 2 And SLBReference = 0 And HLReference = 0 And VLReference = 2 _
                                And SLCorresponding = 4 And SLBCorresponding = 3 And HLCorresponding = 0 And VLCorresponding = 0) _
                               Or (SLReference = 2 And SLBReference = 0 And HLReference = 0 And VLReference = 2 _
                                   And SLCorresponding = 3 And SLBCorresponding = 3 And HLCorresponding = 1 And VLCorresponding = 0) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim StatD3Step As Boolean
                            Dim TmpLine As Line

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2
                            For Each EntityTmp As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint)) = True _
                                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = True Then
                                        D2 = Round(LineLength(TmpLine), 3)
                                        If LineBB = ListView(j).BoundingBox(0) Then
                                            Orientation = "0" 'Lower Side
                                            OriU = ListView(j).BoundProp(0) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                            OriV = ListView(j).BoundProp(1) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                        ElseIf LineBB = ListView(j).BoundingBox(1) Then
                                            Orientation = "3" 'Right Side
                                            OriU = ListView(j).BoundProp(2) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                            OriV = ListView(j).BoundProp(1) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                        ElseIf LineBB = ListView(j).BoundingBox(2) Then
                                            Orientation = "1" 'Upper Side
                                            OriU = ListView(j).BoundProp(2) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                            OriV = ListView(j).BoundProp(3) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                        ElseIf LineBB = ListView(j).BoundingBox(3) Then
                                            Orientation = "2" 'Left Side
                                            OriU = ListView(j).BoundProp(0) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                            OriV = ListView(j).BoundProp(3) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                        End If
                                        Exit For
                                    ElseIf ((isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = False) _
                                    Or (isequalpoint(TmpLine.StartPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.EndPoint, LineBB.StartPoint)) = False _
                                    Or (isequalpoint(TmpLine.EndPoint, LineBB.StartPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.EndPoint)) = False _
                                    Or (isequalpoint(TmpLine.EndPoint, LineBB.EndPoint) = True And isequalpoint(TmpLine.StartPoint, LineBB.StartPoint) = False)) And D1 = 0 Then
                                        D1 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                StatD3Step = New Boolean
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        If StatD3Step = False Then
                                            D3 = Round(LineLength(TmpLine), 3)
                                            StatD3Step = True
                                        ElseIf StatD3Step = True And isequal(D1, D3) = False Then
                                            D3 = Round(LineLength(TmpLine), 3)
                                            Exit For
                                        End If
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Square Step"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Square Step"
                            Feature.ListLoop = ListLoopTemp
                            If HLCorresponding = 1 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(j).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "3" Then
                                    Orientation = "2"
                                ElseIf Orientation = "2" Then
                                    Orientation = "3"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(j).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle


                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for 4-side pocket features (blind or through) main
                        ElseIf (SLReference = 4 And SLBReference = 0 And SAReference = 4 And SLCorresponding = 1 _
                               And SLBCorresponding = 1 And HLCorresponding = 3 And VLCorresponding = 0) Or _
                               (SLReference = 4 And SLBReference = 0 And SAReference = 4 And SLCorresponding = 2 _
                               And SLBCorresponding = 2 And HLCorresponding = 2 And VLCorresponding = 0) Or _
                               (SLReference = 0 And SLBReference = 0 And HAReference = 4 And HLReference = 4 _
                               And SLCorresponding = 1 And SLBCorresponding = 1 And HACorresponding = 0 And HLCorresponding = 3) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle, AngleTmp As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc
                            Dim InitialStat As Boolean = True
                            Dim Origin As Point3d

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, D4, Angle
                            PolygonProcessor = New PolygonProcessor
                            For Each EntityTmp As Entity In GroupEntity
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    AngleTmp = Atan2(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y, TmpLine.EndPoint.X - TmpLine.StartPoint.X) * (180 / PI)
                                    If AngleTmp < 90 Or (AngleTmp >= 180 And AngleTmp < 270) Then
                                        D1 = Round(LineLength(TmpLine), 3)
                                        If (AngleTmp >= 180 And AngleTmp < 270) Then
                                            AngleTmp = AngleTmp - 180
                                        End If
                                    ElseIf AngleTmp >= 90 Or (AngleTmp >= 270 And AngleTmp < 360) Then
                                        D2 = Round(LineLength(TmpLine), 3)
                                        If (AngleTmp >= 270 And AngleTmp < 360) Then
                                            AngleTmp = AngleTmp - 180
                                        End If
                                    End If

                                    If InitialStat = True Then
                                        Angle = AngleTmp
                                        InitialStat = False
                                    Else
                                        Angle = Min(Angle, AngleTmp)
                                    End If

                                ElseIf (TypeOf EntityTmp Is Arc) And (D4 = 0) Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    D4 = Round(TmpArc.Radius, 3)
                                End If
                            Next
                            D1 = D1 + (2 * D4)
                            D2 = D2 + (2 * D4)
                            PolygonProcessor.GetArea(ListView(ViewNum).GroupLoopPoints(ListView(ViewNum).GroupLoop.IndexOf(GroupEntity)))
                            Origin = PolygonProcessor.GetCentroid(ListView(ViewNum).GroupLoopPoints(ListView(ViewNum).GroupLoop.IndexOf(GroupEntity)))
                            OriU = Origin.X - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                            OriV = Origin.Y - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                                If D3 > 0 Then
                                    Exit For
                                End If
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "4-side Pocket"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "4-side Pocket"
                            Feature.ListLoop = ListLoopTemp
                            If HAReference = 4 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(ViewNum).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for 4-side pocket features (blind or through) alt
                        ElseIf (SLReference = 1 And SLBReference = 1 And SAReference = 0 And HLReference = 3 _
                                And SLCorresponding = 4 And SLBCorresponding = 0 And SACorresponding = 4 And HLCorresponding = 0) Or _
                               (SLReference = 2 And SLBReference = 2 And SAReference = 0 And HLReference = 2 _
                                And SLCorresponding = 4 And SLBCorresponding = 0 And SACorresponding = 4 And HLCorresponding = 0) Or _
                                (SLReference = 1 And SLBReference = 1 And HAReference = 0 And HLReference = 3 _
                               And SLCorresponding = 0 And SLBCorresponding = 0 And HACorresponding = 4 And HLCorresponding = 4) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle, AngleTmp As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc
                            Dim InitialStat As Boolean = True
                            Dim Origin As Point3d

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, D4, Angle
                            PolygonProcessor = New PolygonProcessor
                            For Each EntityTmp As Entity In GroupEntity2
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    AngleTmp = Atan2(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y, TmpLine.EndPoint.X - TmpLine.StartPoint.X) * (180 / PI)
                                    If AngleTmp < 90 Or (AngleTmp >= 180 And AngleTmp < 270) Then
                                        D1 = Round(LineLength(TmpLine), 3)
                                        If (AngleTmp >= 180 And AngleTmp < 270) Then
                                            AngleTmp = AngleTmp - 180
                                        End If
                                    ElseIf AngleTmp >= 90 Or (AngleTmp >= 270 And AngleTmp < 360) Then
                                        D2 = Round(LineLength(TmpLine), 3)
                                        If (AngleTmp >= 270 And AngleTmp < 360) Then
                                            AngleTmp = AngleTmp - 180
                                        End If
                                    End If

                                    If InitialStat = True Then
                                        Angle = AngleTmp
                                        InitialStat = False
                                    Else
                                        Angle = Min(Angle, AngleTmp)
                                    End If

                                ElseIf (TypeOf EntityTmp Is Arc) And (D4 = 0) Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    D4 = Round(TmpArc.Radius, 3)
                                End If
                            Next
                            D1 = D1 + (2 * D4)
                            D2 = D2 + (2 * D4)
                            PolygonProcessor.GetArea(ListView(j).GroupLoopPoints(ListView(j).GroupLoop.IndexOf(GroupEntity)))
                            Origin = PolygonProcessor.GetCentroid(ListView(j).GroupLoopPoints(ListView(j).GroupLoop.IndexOf(GroupEntity)))
                            OriU = Origin.X - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                            OriV = Origin.Y - ListView(j).BoundProp(1) - ListView(j).RefProp(1)

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "4-side Pocket"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "4-side Pocket"
                            Feature.ListLoop = ListLoopTemp
                            If HACorresponding = 4 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(j).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(j).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for 3-side pocket features main
                        ElseIf (SLReference = 4 And SLBReference = 1 And SAReference = 2 _
                        And SLCorresponding = 4 And SLBCorresponding = 1 And SACorresponding = 0) Or _
                        (SLReference = 4 And SLBReference = 1 And SAReference = 2 And HLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And SACorresponding = 0 And HLCorresponding = 2) Or _
                        (SLReference = 1 And SLBReference = 1 And HAReference = 2 And HLReference = 3 _
                        And SLCorresponding = 4 And SLBCorresponding = 1 And HACorresponding = 0 And HLCorresponding = 0) Or _
                        (SLReference = 1 And SLBReference = 1 And HAReference = 2 And HLReference = 3 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And HACorresponding = 0 And HLCorresponding = 2) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, D4
                            For Each EntityTmp1 As Entity In GroupEntity
                                If TypeOf EntityTmp1 Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp1
                                    For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                                            D1 = Round(LineLength(TmpLine), 3)
                                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                            If LineBB = ListView(ViewNum).BoundingBox(0) Then
                                                Orientation = "0" 'Lower Side
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(1) Then
                                                Orientation = "3" 'Right Side
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(2) Then
                                                Orientation = "1" 'Upper Side
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(3) Then
                                                Orientation = "2" 'Left Side
                                            End If
                                            Exit For
                                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                            (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2)) And D2 = 0 Then
                                            D2 = Round(LineLength(TmpLine), 3)
                                            Exit For
                                        End If
                                    Next
                                ElseIf TypeOf EntityTmp1 Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp1
                                    If D4 = 0 Then
                                        D4 = Round(TmpArc.Radius, 3)
                                    End If
                                End If
                            Next
                            D2 = D2 + D4

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "3-side Pocket"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "3-side Pocket"
                            Feature.ListLoop = ListLoopTemp
                            If HAReference = 2 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "2" Then
                                    Orientation = "3"
                                ElseIf Orientation = "3" Then
                                    Orientation = "2"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(ViewNum).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(1) = OriV
                            Feature.OriginAndAddition(2) = OriW
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for 3-side pocket features alt
                        ElseIf (SLReference = 4 And SLBReference = 1 And SAReference = 0 _
                        And SLCorresponding = 4 And SLBCorresponding = 1 And SACorresponding = 2) Or _
                        (SLReference = 2 And SLBReference = 2 And SAReference = 0 And HLReference = 2 _
                        And SLCorresponding = 4 And SLBCorresponding = 1 And SACorresponding = 2 And HLCorresponding = 0) Or _
                        (SLReference = 4 And SLBReference = 1 And HAReference = 0 And HLReference = 0 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And HACorresponding = 2 And HLCorresponding = 3) Or _
                        (SLReference = 2 And SLBReference = 2 And HAReference = 0 And HLReference = 2 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And HACorresponding = 2 And HLCorresponding = 3) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, D4
                            For Each EntityTmp1 As Entity In GroupEntity2
                                If TypeOf EntityTmp1 Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp1
                                    For Each LineBB As Line In ListView(j).BoundingBox
                                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                                            D1 = Round(LineLength(TmpLine), 3)
                                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                            If LineBB = ListView(j).BoundingBox(0) Then
                                                Orientation = "0" 'Lower Side
                                            ElseIf LineBB = ListView(j).BoundingBox(1) Then
                                                Orientation = "3" 'Right Side
                                            ElseIf LineBB = ListView(j).BoundingBox(2) Then
                                                Orientation = "1" 'Upper Side
                                            ElseIf LineBB = ListView(j).BoundingBox(3) Then
                                                Orientation = "2" 'Left Side
                                            End If
                                            Exit For
                                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                            (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2)) And D2 = 0 Then
                                            D2 = Round(LineLength(TmpLine), 3)
                                            Exit For
                                        End If
                                    Next
                                ElseIf TypeOf EntityTmp1 Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp1
                                    If D4 = 0 Then
                                        D4 = Round(TmpArc.Radius, 3)
                                    End If
                                End If
                            Next
                            D2 = D2 + D4

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "3-side Pocket"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "3-side Pocket"
                            Feature.ListLoop = ListLoopTemp
                            If HACorresponding = 2 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(j).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "2" Then
                                    Orientation = "3"
                                ElseIf Orientation = "3" Then
                                    Orientation = "2"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(j).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for 2-side pocket features main
                        ElseIf (SLReference = 4 And SLBReference = 2 And SAReference = 1 _
                        And SLCorresponding = 4 And SLBCorresponding = 2 And SACorresponding = 0) Or _
                        (SLReference = 4 And SLBReference = 2 And SAReference = 1 And HLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And SACorresponding = 0 And HLCorresponding = 2) Or _
                        (SLReference = 2 And SLBReference = 2 And HAReference = 1 And HLReference = 2 _
                        And SLCorresponding = 4 And SLBCorresponding = 2 And HACorresponding = 0 And HLCorresponding = 0) Or _
                        (SLReference = 2 And SLBReference = 2 And HAReference = 1 And HLReference = 2 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And HACorresponding = 0 And HLCorresponding = 2) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle, DimPos1, DimPos2, D3H, D3V As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc
                            Dim Position1 As String = Nothing
                            Dim Position2 As String = Nothing

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, D4
                            For Each EntityTmp As Entity In GroupEntity
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                        If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2) Then
                                            If LineBB = ListView(ViewNum).BoundingBox(0) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "lower"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "lower"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(1) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "right"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "right"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(2) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "upper"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "upper"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(3) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "left"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "left"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            End If
                                            Exit For
                                        End If
                                    Next
                                ElseIf TypeOf EntityTmp Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    D4 = Round(TmpArc.Radius, 3)
                                End If
                            Next

                            If (Position1 = "lower" And Position2 = "left") Or (Position1 = "left" And Position2 = "lower") Then
                                Orientation = "0" 'Lower Left
                                OriU = ListView(ViewNum).BoundProp(0) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                OriV = ListView(ViewNum).BoundProp(1) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                If (Position1 = "lower" And Position2 = "left") Then
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                Else
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                End If
                            ElseIf (Position1 = "lower" And Position2 = "right") Or (Position1 = "right" And Position2 = "lower") Then
                                Orientation = "1" 'Lower Right
                                OriU = ListView(ViewNum).BoundProp(2) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                OriV = ListView(ViewNum).BoundProp(1) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                If (Position1 = "lower" And Position2 = "right") Then
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                Else
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                End If
                            ElseIf (Position1 = "upper" And Position2 = "right") Or (Position1 = "right" And Position2 = "upper") Then
                                Orientation = "3" 'Upper Right
                                OriU = ListView(ViewNum).BoundProp(2) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                OriV = ListView(ViewNum).BoundProp(3) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                If (Position1 = "upper" And Position2 = "right") Then
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                Else
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                End If
                            ElseIf (Position1 = "upper" And Position2 = "left") Or (Position1 = "left" And Position2 = "upper") Then
                                Orientation = "2" 'Upper Left
                                OriU = ListView(ViewNum).BoundProp(0) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                OriV = ListView(ViewNum).BoundProp(3) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                If (Position1 = "upper" And Position2 = "left") Then
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                Else
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                End If
                            End If

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2) Then
                                        If isequal(Abs(TmpLine.StartPoint.X - TmpLine.EndPoint.X), 0) = True _
                                        And isequal(Abs(TmpLine.StartPoint.Y - TmpLine.EndPoint.Y), 0) = False Then
                                            D3V = Round(LineLength(TmpLine), 3)
                                        ElseIf isequal(Abs(TmpLine.StartPoint.X - TmpLine.EndPoint.X), 0) = False _
                                        And isequal(Abs(TmpLine.StartPoint.Y - TmpLine.EndPoint.Y), 0) = True Then
                                            D3H = Round(LineLength(TmpLine), 3)
                                        End If
                                        Exit For
                                    End If
                                Next
                            Next

                            Select Case ListView(ViewNum).ViewType.ToLower
                                Case "top", "bottom"
                                    If ListView(j).ViewType.ToLower = "left" Or ListView(j).ViewType.ToLower = "right" Then
                                        D3 = D3H
                                    ElseIf ListView(j).ViewType.ToLower = "front" Or ListView(j).ViewType.ToLower = "back" Then
                                        D3 = D3V
                                    End If
                                Case "front", "back"
                                    If ListView(j).ViewType.ToLower = "left" Or ListView(j).ViewType.ToLower = "right" Then
                                        D3 = D3H
                                    ElseIf ListView(j).ViewType.ToLower = "top" Or ListView(j).ViewType.ToLower = "bottom" Then
                                        D3 = D3V
                                    End If
                                Case "right", "left"
                                    If ListView(j).ViewType.ToLower = "front" Or ListView(j).ViewType.ToLower = "back" Then
                                        D3 = D3H
                                    ElseIf ListView(j).ViewType.ToLower = "top" Or ListView(j).ViewType.ToLower = "bottom" Then
                                        D3 = D3V
                                    End If
                            End Select

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "2-side Pocket"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "2-side Pocket"
                            Feature.ListLoop = ListLoopTemp
                            If HAReference = 1 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "0" Then
                                    Orientation = "1"
                                ElseIf Orientation = "1" Then
                                    Orientation = "0"
                                ElseIf Orientation = "2" Then
                                    Orientation = "3"
                                ElseIf Orientation = "3" Then
                                    Orientation = "2"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(ViewNum).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for 2-side pocket features alt
                        ElseIf (SLReference = 4 And SLBReference = 2 And SAReference = 0 _
                        And SLCorresponding = 4 And SLBCorresponding = 2 And SACorresponding = 1) Or _
                        (SLReference = 2 And SLBReference = 2 And SAReference = 0 And HLReference = 2 _
                        And SLCorresponding = 4 And SLBCorresponding = 2 And SACorresponding = 1 And HLCorresponding = 0) Or _
                        (SLReference = 4 And SLBReference = 2 And HAReference = 0 And HLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And HACorresponding = 1 And HLCorresponding = 2) Or _
                        (SLReference = 2 And SLBReference = 2 And HAReference = 0 And HLReference = 2 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And HACorresponding = 1 And HLCorresponding = 2) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle, DimPos1, DimPos2, D3H, D3V As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc
                            Dim Position1 As String = Nothing
                            Dim Position2 As String = Nothing

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, D4
                            For Each EntityTmp As Entity In GroupEntity2
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    For Each LineBB As Line In ListView(j).BoundingBox
                                        If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2) Then
                                            If LineBB = ListView(j).BoundingBox(0) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "lower"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "lower"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            ElseIf LineBB = ListView(j).BoundingBox(1) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "right"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "right"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            ElseIf LineBB = ListView(j).BoundingBox(2) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "upper"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "upper"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            ElseIf LineBB = ListView(j).BoundingBox(3) Then
                                                If IsNothing(Position1) Then
                                                    Position1 = "left"
                                                    DimPos1 = LineLength(TmpLine)
                                                Else
                                                    Position2 = "left"
                                                    DimPos2 = LineLength(TmpLine)
                                                End If
                                            End If
                                            Exit For
                                        End If
                                    Next
                                ElseIf TypeOf EntityTmp Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    D4 = Round(TmpArc.Radius, 3)
                                End If
                            Next

                            If (Position1 = "lower" And Position2 = "left") Or (Position1 = "left" And Position2 = "lower") Then
                                Orientation = "0" 'Lower Left
                                OriU = ListView(j).BoundProp(0) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                OriV = ListView(j).BoundProp(1) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                If (Position1 = "lower" And Position2 = "left") Then
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                Else
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                End If
                            ElseIf (Position1 = "lower" And Position2 = "right") Or (Position1 = "right" And Position2 = "lower") Then
                                Orientation = "1" 'Lower Right
                                OriU = ListView(j).BoundProp(2) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                OriV = ListView(j).BoundProp(1) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                If (Position1 = "lower" And Position2 = "right") Then
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                Else
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                End If
                            ElseIf (Position1 = "upper" And Position2 = "right") Or (Position1 = "right" And Position2 = "upper") Then
                                Orientation = "3" 'Upper Right
                                OriU = ListView(j).BoundProp(2) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                OriV = ListView(j).BoundProp(3) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                If (Position1 = "upper" And Position2 = "right") Then
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                Else
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                End If
                            ElseIf (Position1 = "upper" And Position2 = "left") Or (Position1 = "left" And Position2 = "upper") Then
                                Orientation = "2" 'Upper Left
                                OriU = ListView(j).BoundProp(0) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                OriV = ListView(j).BoundProp(3) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                If (Position1 = "upper" And Position2 = "left") Then
                                    D2 = Round(DimPos1, 3)
                                    D1 = Round(DimPos2, 3)
                                Else
                                    D1 = Round(DimPos1, 3)
                                    D2 = Round(DimPos2, 3)
                                End If
                            End If

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2) Then
                                        If isequal(Abs(TmpLine.StartPoint.X - TmpLine.EndPoint.X), 0) = True _
                                        And isequal(Abs(TmpLine.StartPoint.Y - TmpLine.EndPoint.Y), 0) = False Then
                                            D3V = Round(LineLength(TmpLine), 3)
                                        ElseIf isequal(Abs(TmpLine.StartPoint.X - TmpLine.EndPoint.X), 0) = False _
                                        And isequal(Abs(TmpLine.StartPoint.Y - TmpLine.EndPoint.Y), 0) = True Then
                                            D3H = Round(LineLength(TmpLine), 3)
                                        End If
                                        Exit For
                                    End If
                                Next
                            Next

                            Select Case ListView(j).ViewType.ToLower
                                Case "top", "bottom"
                                    If ListView(ViewNum).ViewType.ToLower = "left" Or ListView(ViewNum).ViewType.ToLower = "right" Then
                                        D3 = D3H
                                    ElseIf ListView(ViewNum).ViewType.ToLower = "front" Or ListView(ViewNum).ViewType.ToLower = "back" Then
                                        D3 = D3V
                                    End If
                                Case "front", "back"
                                    If ListView(ViewNum).ViewType.ToLower = "left" Or ListView(ViewNum).ViewType.ToLower = "right" Then
                                        D3 = D3H
                                    ElseIf ListView(ViewNum).ViewType.ToLower = "top" Or ListView(ViewNum).ViewType.ToLower = "bottom" Then
                                        D3 = D3V
                                    End If
                                Case "right", "left"
                                    If ListView(ViewNum).ViewType.ToLower = "front" Or ListView(ViewNum).ViewType.ToLower = "back" Then
                                        D3 = D3H
                                    ElseIf ListView(ViewNum).ViewType.ToLower = "top" Or ListView(ViewNum).ViewType.ToLower = "bottom" Then
                                        D3 = D3V
                                    End If
                            End Select

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "2-side Pocket"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "2-side Pocket"
                            Feature.ListLoop = ListLoopTemp
                            If HACorresponding = 1 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(j).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "0" Then
                                    Orientation = "1"
                                ElseIf Orientation = "1" Then
                                    Orientation = "0"
                                ElseIf Orientation = "2" Then
                                    Orientation = "3"
                                ElseIf Orientation = "3" Then
                                    Orientation = "2"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(j).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for long hole feature main
                        ElseIf (SLReference = 2 And SLBReference = 0 And SAReference = 4 And HLReference = 0 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And SACorresponding = 0 And HLCorresponding = 3) Or _
                        (SLReference = 2 And SLBReference = 0 And SAReference = 4 And HLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And SACorresponding = 0 And HLCorresponding = 2) Or _
                        (SLReference = 0 And SLBReference = 0 And HAReference = 2 And HLReference = 2 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And HACorresponding = 0 And HLCorresponding = 3) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc
                            Dim Origin As Point3d

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, Angle
                            PolygonProcessor = New PolygonProcessor
                            For Each EntityTmp As Entity In GroupEntity
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    If D1 = 0 Then
                                        D1 = Round(LineLength(TmpLine), 3)
                                    End If
                                    Angle = Atan2(Abs(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y), Abs(TmpLine.EndPoint.X - TmpLine.StartPoint.X)) * (180 / PI)
                                ElseIf TypeOf EntityTmp Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    If D2 = 0 Then
                                        D2 = 2 * Round(TmpArc.Radius, 3)
                                    End If
                                End If
                            Next
                            D1 = D1 + D2
                            PolygonProcessor.GetArea(ListView(ViewNum).GroupLoopPoints(ListView(ViewNum).GroupLoop.IndexOf(GroupEntity)))
                            Origin = PolygonProcessor.GetCentroid(ListView(ViewNum).GroupLoopPoints(ListView(ViewNum).GroupLoop.IndexOf(GroupEntity)))
                            OriU = Origin.X - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                            OriV = Origin.Y - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Long Hole"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Long Hole"
                            Feature.ListLoop = ListLoopTemp
                            If HLReference = 2 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(ViewNum).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for long hole feature alt
                        ElseIf (SLReference = 1 And SLBReference = 1 And SAReference = 0 And HLReference = 3 _
                        And SLCorresponding = 2 And SLBCorresponding = 0 And SACorresponding = 4 And HLCorresponding = 0) Or _
                        (SLReference = 2 And SLBReference = 2 And SAReference = 0 And HLReference = 2 _
                        And SLCorresponding = 2 And SLBCorresponding = 0 And SACorresponding = 4 And HLCorresponding = 0) Or _
                        (SLReference = 1 And SLBReference = 1 And HAReference = 0 And HLReference = 3 _
                        And SLCorresponding = 0 And SLBCorresponding = 0 And HACorresponding = 2 And HLCorresponding = 2) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc
                            Dim Origin As Point3d

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2, Angle
                            PolygonProcessor = New PolygonProcessor
                            For Each EntityTmp As Entity In GroupEntity2
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    If D1 = 0 Then
                                        D1 = Round(LineLength(TmpLine), 3)
                                    End If
                                    Angle = Atan2(Abs(TmpLine.EndPoint.Y - TmpLine.StartPoint.Y), Abs(TmpLine.EndPoint.X - TmpLine.StartPoint.X)) * (180 / PI)
                                ElseIf TypeOf EntityTmp Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    If D2 = 0 Then
                                        D2 = 2 * Round(TmpArc.Radius, 3)
                                    End If
                                End If
                            Next
                            D1 = D1 + D2
                            PolygonProcessor.GetArea(ListView(j).GroupLoopPoints(ListView(j).GroupLoop.IndexOf(GroupEntity)))
                            Origin = PolygonProcessor.GetCentroid(ListView(j).GroupLoopPoints(ListView(j).GroupLoop.IndexOf(GroupEntity)))
                            OriU = Origin.X - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                            OriV = Origin.Y - ListView(j).BoundProp(1) - ListView(j).RefProp(1)

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next


                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Long Hole"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Long Hole"
                            Feature.ListLoop = ListLoopTemp
                            If HLCorresponding = 2 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(j).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(j).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for blind slot features main
                        ElseIf (SLReference = 3 And SLBReference = 1 And SAReference = 1 _
                        And SLCorresponding = 4 And SLBCorresponding = 1 And SACorresponding = 0) Or _
                        (SLReference = 3 And SLBReference = 1 And SAReference = 1 And HLReference = 0 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And SACorresponding = 0 And HLCorresponding = 3) Or _
                        (SLReference = 3 And SLBReference = 1 And SAReference = 1 And HLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And SACorresponding = 0 And HLCorresponding = 2) Or _
                        (SLReference = 1 And SLBReference = 1 And HLReference = 2 And HAReference = 1 _
                        And SLCorresponding = 4 And SLBCorresponding = 1 And HLCorresponding = 0 And HACorresponding = 0) Or _
                        (SLReference = 1 And SLBReference = 1 And HLReference = 2 And HAReference = 1 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And HLCorresponding = 3 And HACorresponding = 0) Or _
                        (SLReference = 1 And SLBReference = 1 And HLReference = 2 And HAReference = 1 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And HLCorresponding = 2 And HACorresponding = 0) Or _
                        (SLReference = 2 And SLBReference = 0 And SAReference = 1 And VLReference = 1 _
                        And SLCorresponding = 4 And SLBCorresponding = 2 And SACorresponding = 0 And VLCorresponding = 0) Or _
                        (SLReference = 2 And SLBReference = 0 And SAReference = 1 And HLReference = 0 And VLReference = 1 _
                        And SLCorresponding = 2 And SLBCorresponding = 2 And SACorresponding = 0 And HLCorresponding = 2 And VLCorresponding = 0) Or _
                        (SLReference = 2 And SLBReference = 0 And SAReference = 1 And HLReference = 0 And VLReference = 1 _
                        And SLCorresponding = 3 And SLBCorresponding = 3 And SACorresponding = 0 And HLCorresponding = 1 And VLCorresponding = 0) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle, TempRad As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2
                            For Each EntityTmp As Entity In GroupEntity
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                                            D1 = Round(LineLength(TmpLine), 3)
                                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - ListView(ViewNum).BoundProp(0) - ListView(ViewNum).RefProp(0)
                                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - ListView(ViewNum).BoundProp(1) - ListView(ViewNum).RefProp(1)
                                            If LineBB = ListView(ViewNum).BoundingBox(0) Then
                                                Orientation = "0" 'Lower Side
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(1) Then
                                                Orientation = "3" 'Right Side
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(2) Then
                                                Orientation = "1" 'Upper Side
                                            ElseIf LineBB = ListView(ViewNum).BoundingBox(3) Then
                                                Orientation = "2" 'Left Side
                                            End If
                                            Exit For
                                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                            (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2)) And D2 = 0 Then
                                            D2 = Round(LineLength(TmpLine), 3)
                                            Exit For
                                        End If
                                    Next
                                ElseIf TypeOf EntityTmp Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    If TempRad = 0 Then
                                        TempRad = Round(TmpArc.Radius, 3)
                                    End If
                                End If
                            Next
                            D2 = D2 + TempRad

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity2
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(j).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Blind Slot"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Blind Slot"
                            Feature.ListLoop = ListLoopTemp
                            If HLReference = 2 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "2" Then
                                    Orientation = "3"
                                ElseIf Orientation = "3" Then
                                    Orientation = "2"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(ViewNum).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle

                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                            'rule-based for blind slot features alt
                        ElseIf (SLReference = 4 And SLBReference = 1 And SAReference = 0 _
                        And SLCorresponding = 3 And SLBCorresponding = 1 And SACorresponding = 1) Or _
                        (SLReference = 1 And SLBReference = 1 And SAReference = 0 And HLReference = 3 _
                        And SLCorresponding = 3 And SLBCorresponding = 1 And SACorresponding = 1 And HLCorresponding = 0) Or _
                        (SLReference = 2 And SLBReference = 2 And SAReference = 0 And HLReference = 2 _
                        And SLCorresponding = 3 And SLBCorresponding = 1 And SACorresponding = 1 And HLCorresponding = 0) Or _
                        (SLReference = 4 And SLBReference = 1 And HLReference = 0 And HAReference = 0 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And HLCorresponding = 2 And HACorresponding = 1) Or _
                        (SLReference = 1 And SLBReference = 1 And HLReference = 3 And HAReference = 0 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And HLCorresponding = 2 And HACorresponding = 1) Or _
                        (SLReference = 2 And SLBReference = 2 And HLReference = 2 And HAReference = 0 _
                        And SLCorresponding = 1 And SLBCorresponding = 1 And HLCorresponding = 2 And HACorresponding = 1) Or _
                        (SLReference = 4 And SLBReference = 2 And SAReference = 0 And VLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 0 And SACorresponding = 1 And VLCorresponding = 1) Or _
                        (SLReference = 2 And SLBReference = 2 And SAReference = 0 And HLReference = 2 And VLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 0 And SACorresponding = 1 And HLCorresponding = 0 And VLCorresponding = 1) Or _
                        (SLReference = 3 And SLBReference = 3 And SAReference = 0 And HLReference = 1 And VLReference = 0 _
                        And SLCorresponding = 2 And SLBCorresponding = 0 And SACorresponding = 1 And HLCorresponding = 0 And VLCorresponding = 1) Then
                            Dim D1, D2, D3, D4, OriU, OriV, OriW, Angle, TempRad As New Double
                            Dim Orientation As String = Nothing
                            Dim TmpLine As Line
                            Dim TmpArc As Arc

                            For Each EntityTmp As Entity In GroupEntity
                                MillingObjectId.Add(EntityTmp.ObjectId)
                            Next
                            For Each EntityTmp2 As Entity In GroupEntity2
                                MillingObjectId.Add(EntityTmp2.ObjectId)
                            Next
                            Feature = New OutputFormat
                            ListLoopTemp = New List(Of List(Of Entity))
                            ListLoopTemp.Add(GroupEntity)
                            ListLoopTemp.Add(GroupEntity2)

                            'add Orientation, Origin, D1, D2
                            For Each EntityTmp As Entity In GroupEntity2
                                If TypeOf EntityTmp Is Line Then
                                    TmpLine = New Line
                                    TmpLine = EntityTmp
                                    For Each LineBB As Line In ListView(j).BoundingBox
                                        If PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                        PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                                            D1 = Round(LineLength(TmpLine), 3)
                                            OriU = ((TmpLine.StartPoint.X + TmpLine.EndPoint.X) / 2) - ListView(j).BoundProp(0) - ListView(j).RefProp(0)
                                            OriV = ((TmpLine.StartPoint.Y + TmpLine.EndPoint.Y) / 2) - ListView(j).BoundProp(1) - ListView(j).RefProp(1)
                                            If LineBB = ListView(j).BoundingBox(0) Then
                                                Orientation = "0" 'Lower Side
                                            ElseIf LineBB = ListView(j).BoundingBox(1) Then
                                                Orientation = "3" 'Right Side
                                            ElseIf LineBB = ListView(j).BoundingBox(2) Then
                                                Orientation = "1" 'Upper Side
                                            ElseIf LineBB = ListView(j).BoundingBox(3) Then
                                                Orientation = "2" 'Left Side
                                            End If
                                            Exit For
                                        ElseIf ((PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                            (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                            PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2)) And D2 = 0 Then
                                            D2 = Round(LineLength(TmpLine), 3)
                                            Exit For
                                        End If
                                    Next
                                ElseIf TypeOf EntityTmp Is Arc Then
                                    TmpArc = New Arc
                                    TmpArc = EntityTmp
                                    If TempRad = 0 Then
                                        TempRad = Round(TmpArc.Radius, 3)
                                    End If
                                End If
                            Next
                            D2 = D2 + TempRad

                            'add D3
                            For Each EntityTmp2 As Entity In GroupEntity
                                TmpLine = New Line
                                TmpLine = EntityTmp2
                                For Each LineBB As Line In ListView(ViewNum).BoundingBox
                                    If (PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Or _
                                    (PointOnline(TmpLine.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                                    PointOnline(TmpLine.StartPoint, LineBB.StartPoint, LineBB.EndPoint) <> 2) Then
                                        D3 = Round(LineLength(TmpLine), 3)
                                        Exit For
                                    End If
                                Next
                            Next

                            'set the feature property
                            Feature.EntityMember = MillingObjectId.Count
                            Feature.FeatureName = "Blind Slot"
                            Feature.ObjectId = MillingObjectId
                            Feature.MiscProp(0) = "Blind Slot"
                            Feature.ListLoop = ListLoopTemp
                            If HLCorresponding = 2 Then
                                OriU = -OriU
                                Feature.MiscProp(1) = SearchOppositeSurf(ListView(j).ViewType)
                                adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                                If Orientation = "2" Then
                                    Orientation = "3"
                                ElseIf Orientation = "3" Then
                                    Orientation = "2"
                                End If
                            Else
                                OriU = OriU
                                Feature.MiscProp(1) = ListView(j).ViewType
                            End If
                            Feature.MiscProp(2) = Orientation
                            Feature.OriginAndAddition(0) = Round(OriU, 3)
                            Feature.OriginAndAddition(1) = Round(OriV, 3)
                            Feature.OriginAndAddition(2) = Round(OriW, 3)
                            Feature.OriginAndAddition(3) = D1
                            Feature.OriginAndAddition(4) = D2
                            Feature.OriginAndAddition(5) = D3
                            Feature.OriginAndAddition(6) = D4
                            Feature.OriginAndAddition(7) = Angle


                            'add to the identified feature list
                            IdentifiedFeature.Add(Feature)
                            TmpIdentifiedFeature.Add(Feature)
                            IdentifiedCounter = IdentifiedCounter + 1

                            AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)

                            'remove from unidentified feature list if the feature is listed as unidentified before
                            adskClass.myPalette.CheckUnidentified(GroupEntity, GroupEntity2, UnIdentifiedFeature, TmpUnidentifiedFeature)

                            IdentificationStatus = True
                            Exit For

                        End If
                    End If
                Next
                'End If
            Next

            'add to unidentified list for loop which do not have corresponding loop
            If IdentificationStatus = False Then
                MillingObjectId = New List(Of ObjectId)
                Feature = New OutputFormat
                InputStatus = New Boolean
                ListLoopTemp = New List(Of List(Of Entity))

                'check if the loop was already part of identified feature
                For Each OutputIdent As OutputFormat In IdentifiedFeature
                    If OutputIdent.ListLoop.Contains(GroupEntity) Then
                        InputStatus = True
                    End If
                Next

                'add to unidentified list for loop which not already in unidentified list
                If InputStatus = False Then
                    'add to the unidentified feature list
                    For Each EntityTmp As Entity In GroupEntity
                        MillingObjectId.Add(EntityTmp.ObjectId)
                    Next

                    ListLoopTemp.Add(GroupEntity)

                    CountEntity(GroupEntity, ListView(ViewNum), SLReference, SLBReference, _
                                VLReference, HLReference, SAReference, HAReference, SeqBound, SeqHid)

                    Feature.EntityMember = MillingObjectId.Count
                    Feature.ObjectId = MillingObjectId
                    Feature.ListLoop = ListLoopTemp
                    Feature.SolidLineCount = SLReference
                    Feature.SolidLineInBoundCount = SLBReference
                    Feature.VirtualLineCount = VLReference
                    Feature.HiddenLineCount = HLReference
                    Feature.SolidArcCount = SAReference
                    Feature.HiddenArcCount = HAReference
                    Feature.SequenceSolidBound = SeqBound
                    Feature.SequenceSolidHidden = SeqHid

                    SingleViewProp(Feature, GroupEntity, ListView(ViewNum))

                    'set the feature property
                    UnIdentifiedCounter = UnIdentifiedCounter + 1
                    Feature.FeatureName = "Mill Candidate"
                    Feature.MiscProp(0) = "Mill Candidate"
                    If Feature.HiddenLineCount > 0 Then
                        Feature.MiscProp(1) = SearchOppositeSurf(ListView(ViewNum).ViewType)
                        adskClass.myPalette.AddHiddenView(Feature.MiscProp(1))
                    Else
                        Feature.MiscProp(1) = ListView(ViewNum).ViewType
                    End If
                    UnIdentifiedFeature.Add(Feature)
                    TmpUnidentifiedFeature.Add(Feature)

                    AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                End If
            End If
            'add the progress bar
            i = i + 1
            'System.Threading.Thread.Sleep(1)
            UserControl3.acedSetStatusBarProgressMeterPos(i)
            System.Windows.Forms.Application.DoEvents()
        Next
        UserControl3.acedRestoreStatusBar()
    End Sub


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

    'line lenght function
    Private Function LineLength(ByVal LineToCheck As Line)
        Return Sqrt(((LineToCheck.StartPoint.X - LineToCheck.EndPoint.X) ^ 2) + ((LineToCheck.StartPoint.Y - LineToCheck.EndPoint.Y) ^ 2))
    End Function

    Public Sub CountEntity(ByVal GEntity As List(Of Entity), ByVal View As ViewProp, ByRef SLCount As Integer, _
                            ByRef SLBCount As Integer, ByRef VLCount As Integer, ByRef HLCount As Integer, _
                            ByRef SACount As Integer, ByRef HACount As Integer, ByRef SequenceBound As Boolean, ByRef SequenceHidden As Boolean)

        Dim LineEnt As Line
        Dim ct As New List(Of Boolean)
        Dim sb, hid As Boolean
        SLCount = New Integer
        SLBCount = New Integer
        VLCount = New Integer
        HLCount = New Integer
        SACount = New Integer
        HACount = New Integer
        SequenceBound = New Boolean
        SequenceHidden = New Boolean
        Check2Database = New DatabaseConn
        Check2Database.InitLinesDb()
        Check2Database.InitHoleDb()

        For Each EntTemp As Entity In GEntity
            If TypeOf EntTemp Is Line Then
                If EntTemp.ColorIndex = 10 Then
                    ' count the virtual lines
                    VLCount = VLCount + 1
                ElseIf Check2Database.CheckIfEntityHidden(EntTemp) = True Then
                    ' count the hidden lines
                    HLCount = HLCount + 1
                Else
                    ' count the solid lines
                    SLCount = SLCount + 1
                    For Each LineBB As Line In View.BoundingBox
                        LineEnt = New Line
                        LineEnt = EntTemp
                        If PointOnline(LineEnt.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                            PointOnline(LineEnt.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                            'count the solid lines in bounding box
                            SLBCount = SLBCount + 1
                        End If
                    Next
                End If
            End If
            If TypeOf EntTemp Is Arc Then
                If Check2Database.CheckIfEntityHidden(EntTemp) = True Then
                    'count the hidden arc
                    HACount = HACount + 1
                Else
                    'count the solid arc
                    SACount = SACount + 1
                End If
            End If
        Next

        'Checking Sequence for redundant component of feature
        If (SLCount = 4 And SLBCount = 2 And HLCount = 0 And VLCount = 0 And SACount = 0 And HACount = 0) Or _
        (SLCount = 2 And SLBCount = 2 And HLCount = 2 And VLCount = 0 And SACount = 0 And HACount = 0) Then
            For Each EntTemp As Entity In GEntity
                sb = New Boolean
                For Each LineBB As Line In View.BoundingBox
                    LineEnt = New Line
                    LineEnt = EntTemp
                    If PointOnline(LineEnt.StartPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 And _
                        PointOnline(LineEnt.EndPoint, LineBB.StartPoint, LineBB.EndPoint) = 2 Then
                        sb = True
                    End If
                Next
                ct.Add(sb)
            Next
            If (ct(0) = True And ct(1) = False And ct(2) = True And ct(3) = False) Or (ct(0) = False And ct(1) = True And ct(2) = False And ct(3) = True) Then
                'Square Slot
                SequenceBound = True
                'if not square slot then 2-side pocket
            End If
        End If

        If SLCount = 2 And SLBCount = 2 And HLCount = 2 And VLCount = 0 And SACount = 0 And HACount = 0 Then
            For Each EntTemp As Entity In GEntity
                hid = New Boolean
                If Check2Database.CheckIfEntityHidden(EntTemp) = True Then
                    hid = True
                End If
                ct.Add(hid)
            Next
            If (ct(0) = True And ct(1) = False And ct(2) = True And ct(3) = False) Or (ct(0) = False And ct(1) = True And ct(2) = False And ct(3) = True) Then
                'Square Slot & 4-side pocket
                SequenceHidden = True
                'if not then 2-side pocket and 3-side pocket
            End If
        End If
    End Sub

    Private Function isequal(ByVal PointA As Double, ByVal PointB As Double) As Boolean
        If Abs(PointA - PointB) <= adskClass.AppPreferences.ToleranceValues Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function isequalpoint(ByVal PointA As Point3d, ByVal PointB As Point3d) As Boolean
        If Abs(PointA.X - PointB.X) <= adskClass.AppPreferences.ToleranceValues _
        And Abs(PointA.Y - PointB.Y) <= adskClass.AppPreferences.ToleranceValues Then
            Return True
        Else
            Return False
        End If
    End Function

    'checking whether if the lines are the same one to know whether the loops are corresponded or not (Front-Right Constraint)
    Private Function GPCFrontRight(ByVal StartPoint1 As Point3d, ByVal EndPoint1 As Point3d, _
                                   ByVal StartPoint2 As Point3d, ByVal EndPoint2 As Point3d, _
                                   ByVal RefPoint1X As Double, ByVal RefPoint1Y As Double, _
                                   ByVal RefPoint2X As Double, ByVal RefPoint2Y As Double) As Integer
        If ((isequal(StartPoint1.Y - RefPoint1Y, StartPoint2.Y - RefPoint2Y) = True) And (isequal(EndPoint1.Y - RefPoint1Y, EndPoint2.Y - RefPoint2Y) = True)) Or _
            ((isequal(StartPoint1.Y - RefPoint1Y, EndPoint2.Y - RefPoint2Y) = True) And (isequal(EndPoint1.Y - RefPoint1Y, StartPoint2.Y - RefPoint2Y) = True)) Then
            Return 1
        ElseIf ((isequal(StartPoint1.Y - RefPoint1Y, StartPoint2.Y - RefPoint2Y) = True) Or (isequal(EndPoint1.Y - RefPoint1Y, EndPoint2.Y - RefPoint2Y) = True)) Or _
               ((isequal(StartPoint1.Y - RefPoint1Y, EndPoint2.Y - RefPoint2Y) = True) Or (isequal(EndPoint1.Y - RefPoint1Y, StartPoint2.Y - RefPoint2Y) = True)) Then
            Return 2
        End If
        Return 0
    End Function

    'checking whether if the lines are the same one to know whether the loops are corresponded or not (Front-Top Constraint)
    Private Function GPCFrontTop(ByVal StartPoint1 As Point3d, ByVal EndPoint1 As Point3d, _
                                 ByVal StartPoint2 As Point3d, ByVal EndPoint2 As Point3d, _
                                 ByVal RefPoint1X As Double, ByVal RefPoint1Y As Double, _
                                 ByVal RefPoint2X As Double, ByVal RefPoint2Y As Double) As Integer
        If ((isequal(StartPoint1.X - RefPoint1X, StartPoint2.X - RefPoint2X) = True) And (isequal(EndPoint1.X - RefPoint1X, EndPoint2.X - RefPoint2X) = True)) Or _
            ((isequal(StartPoint1.X - RefPoint1X, EndPoint2.X - RefPoint2X) = True) And (isequal(EndPoint1.X - RefPoint1X, StartPoint2.X - RefPoint2X) = True)) Then
            Return 1
        ElseIf ((isequal(StartPoint1.X - RefPoint1X, StartPoint2.X - RefPoint2X) = True) Or (isequal(EndPoint1.X - RefPoint1X, EndPoint2.X - RefPoint2X) = True)) Or _
               ((isequal(StartPoint1.X - RefPoint1X, EndPoint2.X - RefPoint2X) = True) Or (isequal(EndPoint1.X - RefPoint1X, StartPoint2.X - RefPoint2X) = True)) Then
            Return 2
        End If

        Return 0
    End Function

    'checking whether if the lines are the same one to know whether the loops are corresponded or not (Top-Right Constraint)
    Private Function GPCTopRight(ByVal StartPoint1 As Point3d, ByVal EndPoint1 As Point3d, _
                                 ByVal StartPoint2 As Point3d, ByVal EndPoint2 As Point3d, _
                                 ByVal RefPoint1X As Double, ByVal RefPoint1Y As Double, _
                                 ByVal RefPoint2X As Double, ByVal RefPoint2Y As Double) As Integer
        If ((isequal(StartPoint1.Y - RefPoint1Y, StartPoint2.X - RefPoint2X) = True) And (isequal(EndPoint1.Y - RefPoint1Y, EndPoint2.X - RefPoint2X) = True)) Or _
           ((isequal(StartPoint1.Y - RefPoint1Y, EndPoint2.X - RefPoint2X) = True) And (isequal(EndPoint1.Y - RefPoint1Y, StartPoint2.X - RefPoint2X) = True)) Then
            Return 1
        ElseIf ((isequal(StartPoint1.Y - RefPoint1Y, StartPoint2.Y - RefPoint2Y) = True) Or (isequal(StartPoint1.Y - RefPoint1Y, EndPoint2.Y - RefPoint2Y) = True)) Or _
               ((isequal(EndPoint1.Y - RefPoint1Y, EndPoint2.Y - RefPoint2Y) = True Or isequal(EndPoint1.Y - RefPoint1Y, StartPoint2.Y - RefPoint2Y) = True)) Then
            Return 2
        End If

        Return 0
    End Function

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

End Class
