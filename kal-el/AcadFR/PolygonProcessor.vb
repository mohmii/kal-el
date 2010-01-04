Imports Autodesk.AutoCAD.Geometry

Public Class PolygonProcessor

    'search the polygon area
    Public Sub GetArea(ByVal Points As List(Of Point3d))

        Area = New Double
        Area = 0

        For i = 0 To Points.Count - 1
            j = (i + 1) Mod Points.Count
            Area = Area + Points(i).X * Points(j).Y - Points(j).X * Points(i).Y
        Next i

        Area = Area / 2

    End Sub

    Private Area, CX, CY, P As Double
    Private i, j As Integer

    'search the polygon area centroid
    Public Function GetCentroid(ByVal Points As List(Of Point3d)) As Point3d

        CX = 0
        CY = 0

        For i = 0 To Points.Count - 1
            j = (i + 1) Mod Points.Count
            P = Points(i).X * Points(j).Y - Points(j).X * Points(i).Y
            CX = CX + (Points(i).X + Points(j).X) * P
            CY = CY + (Points(i).Y + Points(j).Y) * P
        Next i

        CX = CX / (6 * Area)
        CY = CY / (6 * Area)

        Return New Point3d(CX, CY, 0)

    End Function
End Class
