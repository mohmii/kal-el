Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Public Class VirtualLine

    'variables prepared for generate virtual lines
    Private LineTrans As Line
    Private LinesOnBound As List(Of Line)
    Private PointsOnBound As List(Of Point3d)
    Private BoundingBoxPoints As List(Of Point3d)
    Private PointStatus As Boolean
    Private GeometryProcessor As GeometryProcessor
    Private EntityIndex As List(Of Integer)
    Private PointIndex As List(Of Integer)
    Private SplitBB As List(Of Line)

    Public Sub GenerateVL(ByVal BBMinX As Double, ByVal BBMinY As Double, ByVal BBMaxX As Double, ByVal BBMaxY As Double, _
                          ByVal BBLine As List(Of Line), ByRef VirtualLines As List(Of Line), ByRef LineEntities As List(Of Line), _
                          ByRef AllEntities As List(Of Entity), ByVal Conn As AcadConn)

        'variables prepared for generate virtual lines
        LineTrans = New Line
        LinesOnBound = New List(Of Line)
        PointsOnBound = New List(Of Point3d)
        BoundingBoxPoints = New List(Of Point3d)

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

        For Each LineTmp As Line In BBLine
            VirtualLines.Add(LineTmp)
        Next

        'break bounding box in every point
        PointIndex = New List(Of Integer)
        SplitBB = New List(Of Line)

        While PointsOnBound.Count > 0
            PointStatus = False
            EntityIndex = New List(Of Integer)
            PointIndex = New List(Of Integer)
            GeometryProcessor = New GeometryProcessor

            For Each PointTemp As Point3d In PointsOnBound
                For Each LineTemp As Line In VirtualLines
                    If GeometryProcessor.PointOnline(PointTemp, LineTemp.StartPoint, LineTemp.EndPoint) = 2 And isequalpoint(PointTemp, LineTemp.StartPoint) = False _
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
            lineTmp.ColorIndex = 10
            'lineTmp.Layer = 2
            lineTmp.Linetype = "CONTINUOUS"
            Conn.btr.AppendEntity(lineTmp)
            Conn.myT.AddNewlyCreatedDBObject(lineTmp, True)
            LineEntities.Add(lineTmp)
            AllEntities.Add(lineTmp)
        Next

    End Sub

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

    Private Function LineBreak(ByVal LineTemp As Line, ByVal PointTemp As Point3d)
        Dim SplittedLines As New List(Of Line)
        SplittedLines.Add(New Line(LineTemp.StartPoint, PointTemp))
        SplittedLines.Add(New Line(LineTemp.EndPoint, PointTemp))
        Return SplittedLines
    End Function

End Class
