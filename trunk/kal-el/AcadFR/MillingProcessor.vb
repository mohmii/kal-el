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

Imports System
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.ComponentModel


Public Class MillingProcessor
    Private GetPoints As GetPoints
    Private AllPoints As List(Of Point3d)
    Private GroupOfEntity As List(Of AllPoints)
    Private UnAdjacentPoints As List(Of Point3d)
    Private LowerLeftCorner As Point3d

    Public Function SearchLeftCornerPoint(ByVal PointCollection As List(Of Point3d))
        Dim PointComparerX, PointComparerY As New Double
        PointComparerX = PointCollection(0).X
        PointComparerY = PointCollection(0).Y
        LowerLeftCorner = New Point3d
        For Each PointTmp As Point3d In PointCollection
            If (Not (PointTmp.X > PointComparerX) And Not (PointTmp.Y > PointComparerY)) Then
                PointComparerX = PointTmp.X
                PointComparerY = PointTmp.Y
                LowerLeftCorner = PointTmp
            End If
        Next
        Return LowerLeftCorner
    End Function

    Private PreviousEntity As Entity
    Private FirstTimeEnter, EndPointHasBeenReach, AngleInitiateStatus, GetLinePathStatus As Boolean
    Private EntityPathIndex, PointPathIndex As Integer
    Private AngleTmp As Double
    Private MainLoopPoint As List(Of Point3d)

    Public Sub CheckMainLoop(ByVal PointTmp As Point3d, ByRef MainLoop As List(Of Entity))
        'check the point status for the stopping rule
        While EndPoint <> PointTmp
            If EndPointHasBeenReach = False Then

                'only set for the first time enter
                If EndPoint.X = 0 And EndPoint.Y = 0 Then
                    EndPoint = PointTmp
                    FirstTimeEnter = True
                Else
                    FirstTimeEnter = False
                    For Each EntityTmp As EntityProp In GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList
                        If (EntityTmp.Line.ObjectId = PreviousEntity.ObjectId) Or _
                        (EntityTmp.Arc.ObjectId = PreviousEntity.ObjectId) Then
                            AngleConverter = EntityTmp.Angle
                        End If
                    Next
                End If

                'make several local temporary place for angle and line-point index for define the next path
                AngleInitiateStatus = False
                GetLinePathStatus = False

                'check for each line contain the point
                For Each EntityTmp As EntityProp In GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList
                    If FirstTimeEnter = True Then
                        If AngleInitiateStatus = False Then
                            AngleTmp = EntityTmp.Angle
                            AngleInitiateStatus = True
                        End If
                        'selecting the smalles line angle for the next path
                        If EntityTmp.Angle <= AngleTmp Then
                            AngleTmp = EntityTmp.Angle
                            EntityPathIndex = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.IndexOf(EntityTmp)
                            GetLinePathStatus = True
                        End If
                    ElseIf FirstTimeEnter = False And GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.Count = 2 Then
                        If (PreviousEntity.ObjectId <> EntityTmp.Line.ObjectId) _
                        And (PreviousEntity.ObjectId <> EntityTmp.Arc.ObjectId) Then
                            EntityPathIndex = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.IndexOf(EntityTmp)
                            GetLinePathStatus = True
                        End If
                    Else
                        If (PreviousEntity.ObjectId <> EntityTmp.Line.ObjectId) _
                        And (PreviousEntity.ObjectId <> EntityTmp.Arc.ObjectId) Then
                            AngleTmpConversion = EntityTmp.Angle - AngleConverter
                            If AngleTmpConversion < 0 Then
                                AngleTmpConversion = AngleTmpConversion + 360
                            End If
                            If AngleInitiateStatus = False Then
                                AngleTmp = AngleTmpConversion
                                AngleInitiateStatus = True
                            End If
                            'selecting the smallest line magnitude for the next path
                            If AngleTmpConversion <= AngleTmp Then
                                AngleTmp = AngleTmpConversion
                                EntityPathIndex = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.IndexOf(EntityTmp)
                                GetLinePathStatus = True
                            End If
                        End If
                    End If
                Next

                If GetLinePathStatus = True Then
                    'add the line path to the group

                    'for line type
                    If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc.ObjectId.IsNull = True Then
                        MainLoop.Add(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Line)
                        MainLoopPoint.Add(PointTmp)
                        PreviousEntity = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Line
                        'select the opposite point from the current point in the selected line path
                        If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint = PointTmp Then
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).EndPoint)
                        Else
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint)
                        End If

                        'for arc type
                    ElseIf GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc.ObjectId.IsNull = False Then
                        MainLoop.Add(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc)
                        MainLoopPoint.Add(PointTmp)
                        PreviousEntity = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc
                        'select the opposite point from the current point in the selected line path
                        If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint = PointTmp Then
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).EndPoint)
                        Else
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint)
                        End If
                    End If
                    PointTmp = AllPoints(PointPathIndex)
                    'iterate path for the next point recursively
                    CheckMainLoop(PointTmp, MainLoop)
                Else
                    Exit While
                End If
            Else
                Exit Sub
            End If
        End While

        EndPointHasBeenReach = True

    End Sub

    Public Sub LoopFinder(ByVal Entities As List(Of Entity), ByRef GroupLoop As List(Of List(Of Entity)), _
                          ByRef GroupLoopPoints As List(Of List(Of Point3d)), ByRef MainLoop As List(Of Entity))

        GetPoints = New GetPoints
        AllPoints = New List(Of Point3d)
        GroupOfEntity = New List(Of AllPoints)
        UnAdjacentPoints = New List(Of Point3d)

        'filtering the unadjacents point
        GetPoints.UnAdjacentPointExtractor(Entities, AllPoints, GroupOfEntity, UnAdjacentPoints)

        'define a new variables for the end point
        EndPoint = New Point3d 'used for stopping rule
        PreviousEntity = Nothing

        AngleTmp = New Double
        EntityPathIndex = New Integer
        PointPathIndex = New Integer
        AngleInitiateStatus = New Boolean
        GetLinePathStatus = New Boolean
        AngleConverter = New Double
        AngleTmpConversion = New Double
        EndPointHasBeenReach = False
        MainLoopPoint = New List(Of Point3d)

        'search main loop
        CheckMainLoop(SearchLeftCornerPoint(AllPoints), MainLoop)

        'create a new group of loop for this current view

        RootPathPoint = New Point3d

        'initiate the progress bar
        UserControl3.acedSetStatusBarProgressMeter("Finding Loop", 0, UnAdjacentPoints.Count)
        Dim i As Integer

        'searching loop from each unadjacent points
        For Each PointTmp As Point3d In UnAdjacentPoints

            'define a new variables for the selected lines path and the end point
            EndPoint = New Point3d 'used for stopping rule
            PreviousEntity = Nothing
            RootPathPoint = PointTmp

            'start iterate
            CheckInnerLoop(RootPathPoint, MainLoop, GroupLoop, GroupLoopPoints)

            'add the progress bar
            i = i + 1
            'System.Threading.Thread.Sleep(1)
            UserControl3.acedSetStatusBarProgressMeterPos(i)
            System.Windows.Forms.Application.DoEvents()
        Next

        UserControl3.acedRestoreStatusBar()
    End Sub

    Private EndPoint, RootPathPoint As Point3d
    Private AngleConverter, AngleTmpConversion As Double
    Private GroupEntity As List(Of Entity)
    Private GroupPoints As List(Of Point3d)

    Private Sub CheckInnerLoop(ByVal PointTmp As Point3d, ByVal MainLoop As List(Of Entity), _
                              ByRef GroupLoop As List(Of List(Of Entity)), ByRef GroupLoopPoints As List(Of List(Of Point3d)))
        'check the point status for the stopping rule

        Dim IndexInMainLoop, IndexB4InMainLoop As New Integer

        IndexInMainLoop = MainLoopPoint.IndexOf(PointTmp)
        IndexB4InMainLoop = IndexInMainLoop - 1
        If IndexB4InMainLoop < 0 Then
            IndexB4InMainLoop = MainLoopPoint.Count - 1
        End If

        Dim status As Boolean = False

        'check for each line contain the point
        For Each EntityTmp As EntityProp In GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList
            If IndexInMainLoop >= 0 Then
                If EntityTmp.Arc.ObjectId.IsNull = True Then
                    If Not ((EntityTmp.StartPoint = MainLoopPoint(IndexInMainLoop) And EntityTmp.EndPoint = MainLoopPoint(IndexB4InMainLoop)) Or _
                            (EntityTmp.EndPoint = MainLoopPoint(IndexInMainLoop) And EntityTmp.StartPoint = MainLoopPoint(IndexB4InMainLoop))) Then
                        status = True
                    End If
                ElseIf EntityTmp.Arc.ObjectId.IsNull = False Then
                    If Not ((EntityTmp.StartPoint = MainLoopPoint(IndexInMainLoop) And EntityTmp.EndPoint = MainLoopPoint(IndexB4InMainLoop)) Or _
                            (EntityTmp.EndPoint = MainLoopPoint(IndexInMainLoop) And EntityTmp.StartPoint = MainLoopPoint(IndexB4InMainLoop))) Then
                        status = True
                    End If
                End If
            Else
                status = True
            End If

            If status = True Then
                'for line type
                If EntityTmp.Arc.ObjectId.IsNull = True Then
                    GroupEntity = New List(Of Entity)
                    GroupPoints = New List(Of Point3d)
                    GroupEntity.Add(EntityTmp.Line)
                    GroupPoints.Add(PointTmp)
                    PreviousEntity = EntityTmp.Line
                    EndPoint = PointTmp

                    If PointTmp = EntityTmp.StartPoint Then
                        PointTmp = EntityTmp.EndPoint
                    Else
                        PointTmp = EntityTmp.StartPoint
                    End If

                    EndPointHasBeenReach = False

                    CheckEachLinePath(PointTmp)
                    PointTmp = EndPoint

                    If GroupLoop.Count = 0 And IsLoopEqual(MainLoop, GroupEntity) = False Then
                        GroupLoop.Add(GroupEntity)
                        GroupLoopPoints.Add(GroupPoints)
                    ElseIf GroupLoop.Count > 0 And IsLoopEqual(MainLoop, GroupEntity) = False Then
                        Dim IsLoopEqualStatus As Boolean
                        For Each GroupEntityTmp As List(Of Entity) In GroupLoop
                            IsLoopEqualStatus = IsLoopEqual(GroupEntityTmp, GroupEntity)
                            If IsLoopEqualStatus = True Then
                                Exit For
                            End If
                        Next
                        If IsLoopEqualStatus = False Then
                            GroupLoop.Add(GroupEntity)
                            GroupLoopPoints.Add(GroupPoints)
                        End If

                    End If

                    'For arc type
                ElseIf EntityTmp.Arc.ObjectId.IsNull = False Then
                    GroupEntity = New List(Of Entity)
                    GroupPoints = New List(Of Point3d)
                    GroupEntity.Add(EntityTmp.Arc)
                    GroupPoints.Add(PointTmp)
                    PreviousEntity = EntityTmp.Arc
                    EndPoint = PointTmp

                    If PointTmp = EntityTmp.StartPoint Then
                        PointTmp = EntityTmp.EndPoint
                    Else
                        PointTmp = EntityTmp.StartPoint
                    End If

                    EndPointHasBeenReach = False

                    CheckEachLinePath(PointTmp)
                    PointTmp = EndPoint

                    If GroupLoop.Count = 0 And IsLoopEqual(MainLoop, GroupEntity) = False Then
                        GroupLoop.Add(GroupEntity)
                        GroupLoopPoints.Add(GroupPoints)
                    ElseIf GroupLoop.Count > 0 And IsLoopEqual(MainLoop, GroupEntity) = False Then
                        Dim IsLoopEqualStatus As Boolean
                        For Each GroupEntityTmp As List(Of Entity) In GroupLoop
                            IsLoopEqualStatus = IsLoopEqual(GroupEntityTmp, GroupEntity)
                            If IsLoopEqualStatus = True Then
                                Exit For
                            End If
                        Next
                        If IsLoopEqualStatus = False Then
                            GroupLoop.Add(GroupEntity)
                            GroupLoopPoints.Add(GroupPoints)
                        End If
                    End If
                End If
            End If
            status = False
        Next
    End Sub

    Private Sub CheckEachLinePath(ByVal PointTmp As Point3d)
        While EndPoint <> PointTmp
            If EndPointHasBeenReach = False Then
                'only set for the first time enter
                For Each EntityTmp As EntityProp In GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList
                    If (EntityTmp.Line.ObjectId = PreviousEntity.ObjectId) Or (EntityTmp.Arc.ObjectId = PreviousEntity.ObjectId) Then
                        AngleConverter = EntityTmp.Angle
                    End If
                Next

                'make several local temporary place for angle and line index for define the next path
                AngleInitiateStatus = False
                GetLinePathStatus = False

                For Each EntityTmp As EntityProp In GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList
                    If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.Count = 2 Then
                        If (PreviousEntity.ObjectId <> EntityTmp.Line.ObjectId) And (PreviousEntity.ObjectId <> EntityTmp.Arc.ObjectId) Then
                            EntityPathIndex = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.IndexOf(EntityTmp)
                            GetLinePathStatus = True
                        End If
                    Else
                        AngleTmpConversion = New Double

                        If (EntityTmp.Line.ObjectId = PreviousEntity.ObjectId) Or (EntityTmp.Arc.ObjectId = PreviousEntity.ObjectId) Then
                            AngleTmpConversion = EntityTmp.Angle - AngleConverter
                        Else
                            AngleTmpConversion = EntityTmp.Angle - AngleConverter
                            If AngleTmpConversion < 0 Then
                                AngleTmpConversion = AngleTmpConversion + 360
                            End If
                        End If

                        If AngleInitiateStatus = False Then
                            AngleTmp = AngleTmpConversion
                            AngleInitiateStatus = True
                        End If

                        'selecting the biggest line angle for the next path
                        If AngleTmpConversion >= AngleTmp And (PreviousEntity.ObjectId <> EntityTmp.Line.ObjectId) _
                        And (PreviousEntity.ObjectId <> EntityTmp.Line.ObjectId) Then
                            AngleTmp = AngleTmpConversion
                            EntityPathIndex = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.IndexOf(EntityTmp)
                            GetLinePathStatus = True
                        End If
                    End If
                Next

                If GetLinePathStatus = True Then

                    GroupPoints.Add(PointTmp)

                    'add the line path to the group
                    If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc.ObjectId.IsNull = True Then
                        GroupEntity.Add(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Line)
                        PreviousEntity = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Line

                        'select the opposite point from the current point in the selected line path
                        If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint = PointTmp Then
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).EndPoint)
                        Else
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint)
                        End If
                    ElseIf GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc.ObjectId.IsNull = False Then
                        GroupEntity.Add(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc)
                        PreviousEntity = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).Arc

                        'select the opposite point from the current point in the selected line path
                        If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint = PointTmp Then
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).EndPoint)
                        Else
                            PointPathIndex = AllPoints.IndexOf(GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(EntityPathIndex).StartPoint)
                        End If
                    End If

                    PointTmp = AllPoints(PointPathIndex)

                    'iterate path for the next point recursively
                    CheckEachLinePath(PointTmp)
                Else
                    Exit While
                End If
            Else
                Exit Sub
            End If
        End While

        EndPointHasBeenReach = True
    End Sub

    Private Function IsLoopEqual(ByVal LoopA As List(Of Entity), ByVal LoopB As List(Of Entity)) As Boolean

        If LoopA.Count <> LoopB.Count Then Return False

        Dim incr, length As New Integer
        length = LoopA.Count

        While incr < length
            If LoopA(0) = LoopB(incr) Then Exit While
            incr = incr + 1
        End While

        If incr = length Then Return False

        'for 2 loops in one direction and different direction
        For i As Integer = 0 To LoopA.Count - 1
            If (LoopA(i) <> LoopB((incr + i) Mod length)) And (LoopA(i) <> LoopB((incr + length - i) Mod length)) Then Return False
        Next

        Return True

    End Function

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

End Class

Public Class AllPoints
    Public EntityList As New List(Of EntityProp)
End Class

Public Class EntityProp
    Public Line As New Line
    Public Arc As New Arc
    Public StartPoint As Point3d
    Public EndPoint As Point3d
    'Angle between start point and end point of entity
    Public Angle As New Double
End Class

Public Class AllLoop
    Public LoopList As New List(Of Line)
End Class