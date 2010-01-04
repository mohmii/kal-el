Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

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
        Upper = ((Math.Round(LineB.EndPoint.X, 3) - Math.Round(LineB.StartPoint.X, 3)) * (Math.Round(LineB.StartPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3))) _
        - ((Math.Round(LineB.EndPoint.Y, 3) - Math.Round(LineB.StartPoint.Y, 3)) * (Math.Round(LineB.StartPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)))
        Lower = ((Math.Round(LineB.EndPoint.X, 3) - Math.Round(LineB.StartPoint.X, 3)) * (Math.Round(LineA.EndPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3))) _
        - ((Math.Round(LineB.EndPoint.Y, 3) - Math.Round(LineB.StartPoint.Y, 3)) * (Math.Round(LineA.EndPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)))
        rParam = Upper / Lower

        Upper = ((Math.Round(LineA.EndPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)) * (Math.Round(LineB.StartPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3))) _
        - ((Math.Round(LineA.EndPoint.Y, 3) - Math.Round(LineA.StartPoint.Y, 3)) * (Math.Round(LineB.StartPoint.X, 3) - Math.Round(LineA.StartPoint.X, 3)))
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

        xParam = Math.Round(lineA.StartPoint.X, 3) + ((Math.Round(lineA.EndPoint.X, 3) - Math.Round(lineA.StartPoint.X, 3)) * sParam)
        yParam = Math.Round(lineA.StartPoint.Y, 3) + ((Math.Round(lineA.EndPoint.Y, 3) - Math.Round(lineA.StartPoint.Y, 3)) * rParam)

        Return New Point3d(xParam, yParam, 0)

    End Function

    Private DummyLine1, DummyLine2 As Line
    Private SplitResult As DBObjectCollection
    Private AcadConn As AcadConn

    'breaking line
    Public function BreakingLine(ByVal LineInput As Line, ByVal Points As Point3dCollection)

        AcadConn = New AcadConn
        AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        Try
            Using AcadConn.myT

                SplitResult = LineInput.GetSplitCurves(Points)

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
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            AcadConn.myT.Dispose()
        End Try

        Return SplitResult

    End Function

End Class
