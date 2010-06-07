Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices

Imports System.IO

Public Class PLprocessor
    Private fw As StreamWriter
    Private FilePath As FileInfo
    Private Directory As String
    Private Plane As ViewProp

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

        'Write the proceeding information
        WriteProlog()

        'prepare the plane for acquire the boundary information
        Plane = New ViewProp
        Plane = Feature.Planelocation

        'write all the entities
        WritePL(Feature.Pline)

        'write the ending information
        WriteEnding()

        fw.Flush()
        fw.Close()

    End Sub

    'write the dxf prolog
    Private Sub WriteProlog()
        fw.WriteLine("  0" + vbCrLf + _
                     "SECTION" + vbCrLf + _
                     "  2" + vbCrLf + _
                     "HEADER" + vbCrLf + _
                     "  9" + vbCrLf + _
                     "$ACADVER" + vbCrLf + _
                     "  1" + vbCrLf + _
                     "AC1009" + vbCrLf + _
                     "  0" + vbCrLf + _
                     "ENDSEC")
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
        fw.WriteLine("  0" + vbCrLf + _
                     "SECTION" + vbCrLf + _
                     "  2" + vbCrLf + _
                     "ENTITIES" + vbCrLf + _
                     "  0" + vbCrLf + _
                     "POLYLINE")

        'print the first vertex
        WritePlane()
        fw.WriteLine(" 10" + vbCrLf + _
                     "0.0" + vbCrLf + _
                     " 20" + vbCrLf + _
                     "0.0" + vbCrLf + _
                     " 30" + vbCrLf + _
                     "0.0")

        'check if the polyline is closed or not
        If PolylineTmp.Closed Then
            fw.WriteLine(" 70" + vbCrLf + _
                         "     1")
        End If

        'write the extrude direction
        WriteExtrudeDirection(Plane.ViewType)

        'write the second until the last vertex
        For i As Integer = 0 To (PolylineTmp.NumberOfVertices - 1)

            fw.WriteLine("  0" + vbCrLf + _
                         "VERTEX")
            WritePlane()
            fw.WriteLine(" 10" + vbCrLf + _
                         (PolylineTmp.GetPoint3dAt(i).X - GetReference(10)).ToString + vbCrLf + _
                         " 20" + vbCrLf + _
                         (PolylineTmp.GetPoint3dAt(i).Y - GetReference(20)).ToString + vbCrLf + _
                         " 30" + vbCrLf + _
                         PolylineTmp.GetPoint3dAt(i).Z.ToString)

            'check the bulge
            If PolylineTmp.GetBulgeAt(i) <> 0 Then
                fw.WriteLine(" 42" + vbCrLf + _
                             PolylineTmp.GetBulgeAt(i).ToString)
            End If

        Next

    End Sub

    'write the plane location and other misc information
    Private Sub WritePlane()
        fw.WriteLine("100" + vbCrLf + _
                     "AcDbEntity" + vbCrLf + _
                     "  8" + vbCrLf + _
                     Plane.ViewType.ToUpper + vbCrLf + _
                     "100" + vbCrLf + _
                     "AcDb2dPolyline" + vbCrLf + _
                     " 66" + vbCrLf + _
                     "     1")
    End Sub

    Private Direction() As Integer

    'write extrude direction
    Private Sub WriteExtrudeDirection(ByVal Surface As String)

        Select Case Surface.ToLower
            Case "top"
                Direction = New Integer() {0, 0, 1}
            Case "bottom"
                Direction = New Integer() {0, 0, -1}
            Case "left"
                Direction = New Integer() {-1, 0, 0}
            Case "right"
                Direction = New Integer() {1, 0, 0}
            Case "front"
                Direction = New Integer() {0, -1, 0}
            Case "back"
                Direction = New Integer() {0, 1, 0}
        End Select

        fw.WriteLine("210" + vbCrLf + _
                     Direction(0).ToString + vbCrLf + _
                     "220" + vbCrLf + _
                     Direction(1).ToString + vbCrLf + _
                     "230" + vbCrLf + _
                     Direction(2).ToString)
    End Sub

    'Write the ending information
    Private Sub WriteEnding()
        fw.WriteLine("  0" + vbCrLf + _
                     "SEQEND" + vbCrLf + _
                     "  5" + vbCrLf + _
                     "78" + vbCrLf + _
                     "100" + vbCrLf + _
                     "AcDbEntity" + vbCrLf + _
                     "  8" + vbCrLf + _
                     Plane.ViewType.ToUpper + vbCrLf + _
                     "  0" + vbCrLf + _
                     "ENDSEC" + vbCrLf + _
                     "  0" + vbCrLf + _
                     "EOF")
    End Sub

    'Check the bulge at each vertex
    Private Sub CheckBulge(ByVal index As Integer)

    End Sub
End Class
