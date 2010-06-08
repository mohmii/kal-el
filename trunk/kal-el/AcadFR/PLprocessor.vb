Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Interop
Imports System.Math
Imports System.IO

Public Class PLprocessor
    Private fw As StreamWriter
    Private FilePath As FileInfo
    Private Directory As String
    Private Plane As ViewProp
    Private ProdSize() As Double
    Private TranslatedPoint, NewPoint, RelativePoint As Point3d
    Private Direction() As Integer

    'get & set the product size
    Public Property ProductSize() As Double()
        Get
            Return ProdSize
        End Get
        Set(ByVal value As Double())
            ProdSize = value
        End Set
    End Property

    'get & set the path directory for saving the dxf file
    Public Property PathDirectory() As String
        Get
            Return Directory
        End Get
        Set(ByVal value As String)
            'cut only the folder
            Directory = value.Remove(value.LastIndexOf(Path.DirectorySeparatorChar))
        End Set
    End Property

    'export the polyline feature into dxf format
    Public Sub Export2Dxf(ByVal Feature As OutputFormat, ByVal counter As Integer)
        'create the file name
        FilePath = New FileInfo(Directory + Path.DirectorySeparatorChar + "pl" + counter.ToString + ".dxf")
        fw = FilePath.CreateText

        'prepare the plane for acquire the boundary information
        Plane = New ViewProp
        Plane = Feature.Planelocation

        'write all the entities
        WritePL(Feature.Pline)

        'write the ending information
        WriteEnding(Feature.Pline)

        fw.Flush()
        fw.Close()

    End Sub

    'get the reference (10 = x, 20 = y, 30 = z)
    Private Function GetReference(ByVal value As Integer) As String
        If value.Equals(10) Then
            Return Plane.BoundProp(0).ToString 'BBMinX
        End If

        If value.Equals(20) Then
            Return Plane.BoundProp(1).ToString 'BBMinY
        End If

        If value.Equals(30) Then
            Return Plane.RefProp(2).ToString
        End If

        Return Nothing
    End Function

    'write the polyline
    Private Sub WritePL(ByVal PolylineTmp As Polyline)
        fw.WriteLine("0" + vbCrLf + _
                     "SECTION" + vbCrLf + _
                     "2" + vbCrLf + _
                     "ENTITIES" + vbCrLf + _
                     "0" + vbCrLf + _
                     "POLYLINE" + vbCrLf + _
                     "5" + vbCrLf + _
                     "1050")

        'print the first vertex
        WritePlane()
        fw.WriteLine("10" + vbCrLf + _
                     "0.0" + vbCrLf + _
                     "20" + vbCrLf + _
                     "0.0" + vbCrLf + _
                     "30" + vbCrLf + _
                     vbTab + FindTranslation(Plane.ViewType))

        'check if the polyline is closed or not
        If PolylineTmp.Closed Then
            fw.WriteLine("70" + vbCrLf + _
                         "1")
        End If

        'write the extrude direction
        WriteExtrudeDirection(Plane.ViewType)

        'write the second until the last vertex
        For i As Integer = 0 To (PolylineTmp.NumberOfVertices - 1)

            fw.WriteLine("0" + vbCrLf + _
                         "VERTEX" + vbCrLf + _
                         "5" + vbCrLf + _
                         (1050 + (i + 1)).ToString)
            WritePlane()

            'get the correct point
            TranslatedPoint = New Point3d
            TranslatedPoint = GetTranslatedPoint(PolylineTmp.GetPoint2dAt(i))

            fw.WriteLine("10" + vbCrLf + _
                         vbTab + TranslatedPoint.X.ToString + vbCrLf + _
                         "20" + vbCrLf + _
                         vbTab + TranslatedPoint.Y.ToString + vbCrLf + _
                         "30" + vbCrLf + _
                         vbTab + FindTranslation(Plane.ViewType))

            'check the bulge
            If PolylineTmp.GetBulgeAt(i) <> 0 Then
                fw.WriteLine(" 42" + vbCrLf + _
                             Getbulge(PolylineTmp.GetBulgeAt(i)).ToString)
            End If
        Next

    End Sub

    'find the axis slide for each surface
    Private Function FindTranslation(ByVal Surface As String) As String

        If Surface.ToLower.Equals("top") Then
            Return ProdSize(2).ToString
        End If

        If Surface.ToLower.Equals("right") Then
            Return ProdSize(0).ToString
        End If

        If Surface.ToLower.Equals("back") Then
            Return (-1 * ProdSize(1)).ToString
        End If

        Return "0.0"
    End Function

    'find the relative point
    Private Function GetTranslatedPoint(ByVal CurrentPoint As Point2d) As Point3d

        RelativePoint = New Point3d(CurrentPoint.X - GetReference(10), CurrentPoint.Y - GetReference(20), 0)

        'find the correct x value for each surface
        Select Case Plane.ViewType.ToLower
            Case "bottom", "back"
                NewPoint = New Point3d(Abs(ProdSize(0) - RelativePoint.X), RelativePoint.Y, RelativePoint.Z)
            Case "left"
                NewPoint = New Point3d(Abs(ProdSize(1) - RelativePoint.X), RelativePoint.Y, RelativePoint.Z)
            Case Else
                NewPoint = RelativePoint
        End Select

        Return NewPoint
    End Function

    'get the correct bulge for each surface
    Private Function GetBulge(ByVal BulgeValue As Double) As Double
        Select Case Plane.ViewType.ToLower
            Case "bottom", "left", "back"
                Return (-1 * BulgeValue)
            Case Else
                Return BulgeValue
        End Select
    End Function

    'write the plane location and other misc information
    Private Sub WritePlane()
        fw.WriteLine("100" + vbCrLf + _
                     "AcDbEntity" + vbCrLf + _
                     "8" + vbCrLf + _
                     Plane.ViewType.ToUpper + vbCrLf + _
                     "100" + vbCrLf + _
                     "AcDb2dPolyline" + vbCrLf + _
                     "66" + vbCrLf + _
                     "1")
    End Sub

    'write extrude direction
    Private Sub WriteExtrudeDirection(ByVal Surface As String)

        Select Case Surface.ToLower
            Case "top"
                Direction = New Integer() {0, 0, 1}
            Case "bottom"
                Direction = New Integer() {0, 0, 1}
            Case "left"
                Direction = New Integer() {1, 0, 0}
            Case "right"
                Direction = New Integer() {1, 0, 0}
            Case "front"
                Direction = New Integer() {0, -1, 0}
            Case "back"
                Direction = New Integer() {0, -1, 0}
        End Select

        fw.WriteLine("210" + vbCrLf + _
                     Direction(0).ToString + vbCrLf + _
                     "220" + vbCrLf + _
                     Direction(1).ToString + vbCrLf + _
                     "230" + vbCrLf + _
                     Direction(2).ToString)
    End Sub

    'Write the ending information
    Private Sub WriteEnding(ByVal Pline As Polyline)
        fw.WriteLine("0" + vbCrLf + _
                     "SEQEND" + vbCrLf + _
                     "5" + vbCrLf + _
                     (1050 + Pline.NumberOfVertices + 1).ToString + vbCrLf + _
                     "100" + vbCrLf + _
                     "AcDbEntity" + vbCrLf + _
                     "  8" + vbCrLf + _
                     Plane.ViewType.ToUpper + vbCrLf + _
                     "  0" + vbCrLf + _
                     "ENDSEC" + vbCrLf + _
                     "  0" + vbCrLf + _
                     "EOF")
    End Sub

End Class
