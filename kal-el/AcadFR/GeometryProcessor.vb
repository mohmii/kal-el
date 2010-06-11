Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.Math

Public Class GeometryProcessor
    Private xParam, yParam, rParam, sParam, Upper, Lower As Double

    'checking if the two lines were intersecting each other or not by using two points and a line
    Public Overloads Function IsIntersect(ByVal PointA As Point3d, ByVal PointB As Point3d, ByVal LineTmp As Line) As Boolean
        rParam = New Double
        sParam = New Double
        Upper = New Double
        Lower = New Double

        'see the formula in the dr mohsen dissertation
        Upper = ((Math.Round(LineTmp.EndPoint.X, 3) - Math.Round(LineTmp.StartPoint.X, 3)) * (Math.Round(LineTmp.StartPoint.Y, 3) - Math.Round(PointA.Y, 3))) _
        - ((Math.Round(LineTmp.EndPoint.Y, 3) - Math.Round(LineTmp.StartPoint.Y, 3)) * (Math.Round(LineTmp.StartPoint.X, 3) - Math.Round(PointA.X, 3)))

        Lower = ((Math.Round(LineTmp.EndPoint.X, 3) - Math.Round(LineTmp.StartPoint.X, 3)) * (Math.Round(PointB.Y, 3) - Math.Round(PointA.Y, 3))) _
        - ((Math.Round(LineTmp.EndPoint.Y, 3) - Math.Round(LineTmp.StartPoint.Y, 3)) * (Math.Round(PointB.X, 3) - Math.Round(PointA.X, 3)))
        rParam = Upper / Lower

        Upper = ((Math.Round(PointB.X, 3) - Math.Round(PointA.X, 3)) * (Math.Round(LineTmp.StartPoint.Y, 3) - Math.Round(PointA.Y, 3))) _
        - ((Math.Round(PointB.Y, 3) - Math.Round(PointA.Y, 3)) * (Math.Round(LineTmp.StartPoint.X, 3) - Math.Round(PointA.X, 3)))
        sParam = Upper / Lower

        If 0 <= rParam <= 1 And 0 <= sParam <= 1 Then
            Return True 'intersection is exist
        Else
            Return False 'intersection is not exist
        End If
    End Function

    Private Status As Boolean

    'checking if the two lines were intersecting each other or not by using two lines
    Public Overloads Function IsIntersect(ByVal LineA As Line, ByVal LineB As Line) As Boolean
        rParam = New Double
        sParam = New Double
        Upper = New Double
        Lower = New Double

        'see the formula in the dr mohsen dissertation
        'Upper = ((Math.Round(LineB.EndPoint.X, 3) - Math.Round(LineB.StartPoint.X, 3)) * (Math.Round(LineB.StartPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3))) _
        '- ((Math.Round(LineB.EndPoint.Y, 3) - Math.Round(LineB.StartPoint.Y, 3)) * (Math.Round(LineB.StartPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)))
        'Lower = ((Math.Round(LineB.EndPoint.X, 3) - Math.Round(LineB.StartPoint.X, 3)) * (Math.Round(LineA.EndPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3))) _
        '- ((Math.Round(LineB.EndPoint.Y, 3) - Math.Round(LineB.StartPoint.Y, 3)) * (Math.Round(LineA.EndPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)))

        Upper = ((LineB.EndPoint.X - LineB.StartPoint.X) * (LineB.StartPoint.Y - LineA.StartPoint.Y)) _
        - ((LineB.EndPoint.Y - LineB.StartPoint.Y) * (LineB.StartPoint.X - LineA.StartPoint.X))
        Lower = ((LineB.EndPoint.X - LineB.StartPoint.X) * (LineA.EndPoint.Y - LineA.StartPoint.Y)) _
        - ((LineB.EndPoint.Y - LineB.StartPoint.Y) * (LineA.EndPoint.X - LineA.StartPoint.X))

        rParam = Upper / Lower

        'Upper = ((Math.Round(LineA.EndPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)) * (Math.Round(LineB.StartPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3))) _
        '- ((Math.Round(LineA.EndPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3)) * (Math.Round(LineB.StartPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)))

        Upper = ((LineA.EndPoint.X - LineA.StartPoint.X) * (LineB.StartPoint.Y - LineA.StartPoint.Y)) _
- ((LineA.EndPoint.Y - LineA.StartPoint.Y) * (LineB.StartPoint.X - LineA.StartPoint.X))

        sParam = Upper / Lower

        If 0 < rParam And rParam < 1 And 0 < sParam And sParam < 1 Then
            Return True
        Else
            Return False
        End If

    End Function

    'return the intersection point
    Public Function IntersectionPoint(ByVal lineA As Line, ByVal LineB As Line) As Point3d
        xParam = New Double
        yParam = New Double

        'xParam = Math.Round(lineA.StartPoint.X, 3) + ((Math.Round(lineA.EndPoint.X, 3) - Math.Round(lineA.StartPoint.X, 3)) * sParam)
        'yParam = Math.Round(lineA.StartPoint.Y, 3) + ((Math.Round(lineA.EndPoint.Y, 3) - Math.Round(lineA.StartPoint.Y, 3)) * rParam)

        xParam = lineA.StartPoint.X + ((lineA.EndPoint.X - lineA.StartPoint.X) * sParam)
        yParam = lineA.StartPoint.Y + ((lineA.EndPoint.Y - lineA.StartPoint.Y) * rParam)

        Return New Point3d(xParam, yParam, 0)

    End Function

    Private DummyLine1, DummyLine2 As Line
    Private SplitResult As DBObjectCollection
    Private AcadConn As AcadConn

    'checking whether if the point is on the line or not
    Public Function PointOnline(ByVal PointToCheck As Point3d, ByVal StartPoint As Point3d, ByVal EndPoint As Point3d) As Integer

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

    'breaking line
    Private Overloads Function BreakingLine(ByVal LineInput As Line, ByVal BreakPointTmp As Point3dCollection, ByVal LineEdited As Line)

        AcadConn = New AcadConn
        AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        'Try
        Using AcadConn.myT

            If LineInput.GetSplitCurves(BreakPointTmp).Count <> 0 Then
                SplitResult = LineInput.GetSplitCurves(BreakPointTmp)
            End If

            DummyLine1 = SplitResult(0)
            DummyLine2 = SplitResult(1)

            AcadConn.OpenBlockTableRec()

            AcadConn.btr.UpgradeOpen()
            AcadConn.btr.AppendEntity(LineEdited)
            AcadConn.btr.AppendEntity(DummyLine1)
            AcadConn.btr.AppendEntity(DummyLine2)

            AcadConn.myT.AddNewlyCreatedDBObject(LineEdited, True)
            AcadConn.myT.AddNewlyCreatedDBObject(DummyLine1, True)
            AcadConn.myT.AddNewlyCreatedDBObject(DummyLine2, True)
            AcadConn.myT.Commit()

        End Using

        Return SplitResult

        AcadConn.myT.Dispose()

    End Function

    Private Overloads Function BreakingLine(ByVal LineInput As Line, ByVal Points As Point3dCollection)

        AcadConn = New AcadConn
        AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        'Try
        Using AcadConn.myT
            If LineInput.GetSplitCurves(Points).Count <> 0 Then
                SplitResult = LineInput.GetSplitCurves(Points)
            End If

            DummyLine1 = SplitResult(0)
            DummyLine2 = SplitResult(1)

            AcadConn.OpenBlockTableRec()

            AcadConn.btr.UpgradeOpen()
            AcadConn.btr.AppendEntity(DummyLine1)
            AcadConn.btr.AppendEntity(DummyLine2)

            AcadConn.myT.AddNewlyCreatedDBObject(DummyLine1, True)
            AcadConn.myT.AddNewlyCreatedDBObject(DummyLine2, True)
            AcadConn.myT.Commit()

        End Using

        Return SplitResult

        AcadConn.myT.Dispose()

    End Function

    Private BreakPoint As Point3dCollection

    'used in start or end of line
    Public Overloads Sub SplitLine(ByVal LineInput As Line, ByVal Points As Point3d, ByRef EntitiesToAdd As List(Of Entity), _
                              ByRef RemovedEntities As List(Of Entity), ByVal EntityTmp As Entity, _
                              ByRef EntityIndexes As List(Of Integer), ByVal AllEntities As List(Of Entity), _
                              ByRef LineIndexes As List(Of Integer), ByVal LineEntities As List(Of Line), _
                              ByRef PointToEdit As Point3d, ByVal EntityTmp2 As Entity)

        'set the break point
        BreakPoint = New Point3dCollection
        BreakPoint.Add(Points)
        PointToEdit = Points

        If TypeOf EntityTmp2 Is Line Then
            Dim LineToEdit As New Line
            LineToEdit = EntityTmp2
            Dim LineEdit As New Line
            If CheckDifferences(LineInput.StartPoint.X, LineInput.EndPoint.X) = True Then
                If PointDistance(LineToEdit.StartPoint, Points) > PointDistance(LineToEdit.EndPoint, Points) Then
                    LineEdit = New Line(LineToEdit.StartPoint, New Point3d(Round(LineInput.StartPoint.X, 5), Round(LineToEdit.EndPoint.Y, 5), 0))
                Else
                    LineEdit = New Line(LineToEdit.EndPoint, New Point3d(Round(LineInput.StartPoint.X, 5), Round(LineToEdit.StartPoint.Y, 5), 0))
                End If
            ElseIf CheckDifferences(LineInput.StartPoint.Y, LineInput.EndPoint.Y) = True Then
                If PointDistance(LineToEdit.StartPoint, Points) > PointDistance(LineToEdit.EndPoint, Points) Then
                    LineEdit = New Line(LineToEdit.StartPoint, New Point3d(Round(LineToEdit.EndPoint.X, 5), Round(LineInput.StartPoint.Y, 5), 0))
                Else
                    LineEdit = New Line(LineToEdit.EndPoint, New Point3d(Round(LineToEdit.StartPoint.X, 5), Round(LineInput.StartPoint.Y, 5), 0))
                End If
            End If

            LineEdit.Color = LineToEdit.Color
            LineEdit.Layer = LineToEdit.Layer
            LineEdit.Linetype = LineToEdit.Linetype

            'split the line
            SplitResult = BreakingLine(LineInput, BreakPoint, LineEdit)

            'add the new splitted line and edited line into current line list
            EntitiesToAdd.Add(SplitResult(0))
            EntitiesToAdd.Add(SplitResult(1))
            EntitiesToAdd.Add(LineEdit)

            RemovedEntities.Add(EntityTmp)
            RemovedEntities.Add(EntityTmp2)

            EntityIndexes.Add(AllEntities.IndexOf(EntityTmp))
            EntityIndexes.Add(AllEntities.IndexOf(EntityTmp2))

            LineIndexes.Add(LineEntities.IndexOf(LineInput))
            LineIndexes.Add(LineEntities.IndexOf(LineToEdit))

        ElseIf TypeOf EntityTmp2 Is Arc Then

            'split the line
            SplitResult = BreakingLine(LineInput, BreakPoint)

            'add the new splitted line and edited line into current line list
            EntitiesToAdd.Add(SplitResult(0))
            EntitiesToAdd.Add(SplitResult(1))
            RemovedEntities.Add(EntityTmp)
            EntityIndexes.Add(AllEntities.IndexOf(EntityTmp))
            LineIndexes.Add(LineEntities.IndexOf(LineInput))
        End If

    End Sub

    'used in intersecting line
    Public Overloads Sub SplitLine(ByVal LineInput As Line, ByVal Points As Point3d, ByRef EntitiesToAdd As List(Of Entity), _
                              ByRef RemovedEntities As List(Of Entity), ByVal Index As Integer, _
                              ByRef EntityIndexes As List(Of Integer), ByVal AllEntities As List(Of Entity), _
                              ByRef LineIndexes As List(Of Integer), ByVal LineEntities As List(Of Line))

        'set the break point
        BreakPoint = New Point3dCollection
        BreakPoint.Add(Points)

        'split the line
        SplitResult = BreakingLine(LineInput, BreakPoint)

        'add the new splitted line into current line list
        EntitiesToAdd.Add(SplitResult(0))
        EntitiesToAdd.Add(SplitResult(1))
        RemovedEntities.Add(AllEntities(Index))
        EntityIndexes.Add(Index)
        LineIndexes.Add(LineEntities.IndexOf(LineInput))
    End Sub

    Public Function GetBreakPoint(ByVal PointA As Double, ByVal PointB As Double) As Double
        If CheckDifferences(PointA, PointB) Then
            Return PointA
        End If
    End Function

    'check differences between two points
    Public Overloads Function CheckDifferences(ByRef PointA As Double, ByRef PointB As Double) As Boolean
        If Abs(PointA - PointB) <= adskClass.AppPreferences.ToleranceValues Then
            Return True
        Else
            Return False
        End If
    End Function

    'check differences between two points
    Public Overloads Function CheckDifferences(ByRef PointA As Point3d, ByRef PointB As Point3d) As Boolean
        If Abs(PointA.X - PointB.X) <= adskClass.AppPreferences.ToleranceValues _
        And Abs(PointA.Y - PointB.Y) <= adskClass.AppPreferences.ToleranceValues Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function PointDistance(ByVal point1 As Point3d, ByVal point2 As Point3d) As Double
        Return Sqrt(((point1.X - point2.X) ^ 2) + ((point1.Y - point2.Y) ^ 2))
    End Function

End Class
