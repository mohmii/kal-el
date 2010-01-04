Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.Math

Public Class VirtualLine

    Private SplitResult As DBObjectCollection

    Public Sub CheckBound(ByVal BB As List(Of Line), ByRef EntityList As List(Of Entity))

        Dim PointCol As Point3dCollection
        Dim SamplePoint As Point3d
        Dim BoundBreak As New List(Of Line)
        Dim DummyLine1, DummyLine2, DummyLine3 As Line
        Dim PointStatus As Boolean = True

        For i As Integer = 0 To 3
            BoundBreak.Add(BB(i))
        Next

        While PointStatus = True
            PointStatus = False
            DummyLine1 = New Line
            DummyLine2 = New Line
            DummyLine3 = New Line
            For Each BoundTemp As Line In BoundBreak
                For Each EntityTemp As Entity In EntityList
                    If TypeOf EntityTemp Is Line Then
                        Dim LineTemp As New Line
                        If Not PartOfLine(LineTemp, BoundTemp) = 0 Then
                            SamplePoint = New Point3d
                            PointCol = New Point3dCollection

                            If PartOfLine(LineTemp, BoundTemp) = 11 Then
                                If LineTemp.StartPoint.Y > LineTemp.EndPoint.Y Then
                                    SamplePoint = LineTemp.StartPoint
                                Else
                                    SamplePoint = LineTemp.EndPoint
                                End If
                            ElseIf PartOfLine(LineTemp, BoundTemp) = 12 Then
                                If LineTemp.StartPoint.Y > LineTemp.EndPoint.Y Then
                                    SamplePoint = LineTemp.EndPoint
                                Else
                                    SamplePoint = LineTemp.StartPoint
                                End If
                            ElseIf PartOfLine(LineTemp, BoundTemp) = 21 Then
                                If LineTemp.StartPoint.X > LineTemp.EndPoint.X Then
                                    SamplePoint = LineTemp.StartPoint
                                Else
                                    SamplePoint = LineTemp.EndPoint
                                End If
                            ElseIf PartOfLine(LineTemp, BoundTemp) = 22 Then
                                If LineTemp.StartPoint.X > LineTemp.EndPoint.X Then
                                    SamplePoint = LineTemp.EndPoint
                                Else
                                    SamplePoint = LineTemp.StartPoint
                                End If
                            End If
                            PointCol.Add(SamplePoint)

                            SplitResult = New DBObjectCollection
                            SplitResult = BoundTemp.GetSplitCurves(PointCol)

                            DummyLine1 = SplitResult(0)
                            DummyLine2 = SplitResult(1)
                            DummyLine3 = BoundTemp

                            PointStatus = True
                            Exit For
                        End If
                    End If
                Next
                If PointStatus = True Then
                    Exit For
                End If
            Next
            If PointStatus = True Then
                BoundBreak.Add(DummyLine1)
                BoundBreak.Add(DummyLine2)
                BoundBreak.Remove(DummyLine3)
            End If
        End While

        For Each BoundTemp As Line In BoundBreak
            For Each EntityTemp As Entity In EntityList
                If TypeOf EntityTemp Is Line Then
                    Dim LineTemp As New Line
                    If PartOfLine(LineTemp, BoundTemp) = 0 Then

                    End If
                End If
            Next
        Next

    End Sub

    Private Function PartOfLine(ByVal LinePart As Line, ByVal TheLine As Line) As Integer
        'original Part of Line
        If PointOnline(LinePart.StartPoint, TheLine.StartPoint, TheLine.EndPoint) = 2 And PointOnline(LinePart.EndPoint, TheLine.StartPoint, TheLine.EndPoint) = 2 Then

        End If
    End Function

    Private Function isequal(ByVal x As Double, ByVal y As Double) As Boolean
        If Abs(x - y) > 0.1 Then
            Return False
        Else
            Return True
        End If
    End Function

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

End Class
