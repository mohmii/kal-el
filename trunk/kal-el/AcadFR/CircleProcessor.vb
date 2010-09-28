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

Public Class CircleProcessor

    Private CheckResult() As String
    Private ListLoopTemp As List(Of List(Of Entity))
    Private LoopTemp As List(Of Entity)

    Public Sub ClassifyCircles(ByVal CircMember As Integer, ByVal Check2Database As DatabaseConn, ByVal result As IEnumerable(Of Circle), ByVal Surface As Integer, _
                               ByRef Feature As OutputFormat, ByVal RefPoint As Point3d, ByVal ProjectView As ViewProp)

        If CircMember = 2 Then

            'continue to identify tap and cbore hole with the fr database
            CheckResult = Check2Database.CheckDoubleHole(result, Surface)

            'checking the result
            If IsNothing(CheckResult) Then

                'loop for each circle in group of circles
                For Each circle As Circle In result
                    'create new unidentified feature
                    Feature = New OutputFormat
                    'set the feature property
                    Feature.EntityMember = CircMember
                    Feature.FeatureName = "Entity " + SelectionCommand.UnIdentifiedFeature.Count.ToString
                    Feature.ObjectId.Add(circle.ObjectId)
                    'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                    Feature.MiscProp(0) = FindTheirJapsName("Drill")
                    Feature.MiscProp(1) = CheckTheSurface(setView.viewis, Surface)
                    Feature.OriginAndAddition(0) = SetXPosition(circle.Center.X, RefPoint.X, ProjectView)
                    Feature.OriginAndAddition(1) = SetYPosition(circle.Center.Y, RefPoint.Y, ProjectView)
                    Feature.OriginAndAddition(2) = 0
                    Feature.OriginAndAddition(3) = Round(circle.Radius * 2, 3)
                    Feature.OriginAndAddition(4) = 0
                    Feature.OriginAndAddition(5) = 0
                    Feature.OriginAndAddition(6) = 0
                    Feature.OriginAndAddition(7) = 0
                    ListLoopTemp = New List(Of List(Of Entity))
                    ListLoopTemp.Add(LoopTemp)
                    Feature.ListLoop = ListLoopTemp
                    If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(circle) Then
                        SelectionCommand.HiddenFeature.Add(Feature)
                        SelectionCommand.HiddenEntity.Add(circle)
                    Else
                        'add to the unidentified feature list
                        SelectionCommand.UnIdentifiedFeature.Add(Feature)
                        SelectionCommand.TmpUnidentifiedFeature.Add(Feature)
                        'OrganizeList.AddListToExisting2(Feature)
                        SelectionCommand.UnIdentifiedCounter = SelectionCommand.UnIdentifiedCounter + 1
                        AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                    End If
                Next
            Else
                'create new feature for cbore or tap
                Feature = New OutputFormat
                'set the feature property
                Feature.EntityMember = CircMember
                Feature.FeatureName = CheckResult(0)
                Feature.ObjectId.Add(result.First.ObjectId)
                Feature.ObjectId.Add(result.Last.ObjectId)
                'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                Feature.MiscProp(0) = FindTheirJapsName(CheckResult(0))
                Feature.MiscProp(1) = CheckTheSurface(setView.viewis, Surface)
                Feature.OriginAndAddition(0) = SetXPosition(result.FirstOrDefault.Center.X, RefPoint.X, ProjectView)
                Feature.OriginAndAddition(1) = SetYPosition(result.FirstOrDefault.Center.Y, RefPoint.Y, ProjectView)
                Feature.OriginAndAddition(2) = 0
                Feature.OriginAndAddition(3) = CheckResult(3)
                Feature.OriginAndAddition(4) = CheckResult(4)
                Feature.OriginAndAddition(5) = CheckResult(5)
                Feature.OriginAndAddition(6) = CheckResult(6)
                Feature.OriginAndAddition(7) = 0
                ListLoopTemp = New List(Of List(Of Entity))
                ListLoopTemp.Add(LoopTemp)
                Feature.ListLoop = ListLoopTemp
                If setView.CBHidden = True And Check2Database.CheckBottomTap(result) Then
                    SelectionCommand.HiddenFeature.Add(Feature)
                    SelectionCommand.HiddenEntity.Add(result.FirstOrDefault)
                Else
                    'add to the identified feature list
                    SelectionCommand.IdentifiedFeature.Add(Feature)
                    SelectionCommand.TmpIdentifiedFeature.Add(Feature)
                    'OrganizeList.AddListToExisting(Feature)
                    SelectionCommand.IdentifiedCounter = SelectionCommand.IdentifiedCounter + 1
                    AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)
                End If
            End If
        Else
            If CircMember = 1 Then
                'check if the group is ream or not

                If Check2Database.CheckIfReam(result) Then
                    'set the checkresult to ream
                    'CheckResult = New String() {"Ream", result.SingleOrDefault.Id.ToString(), ""}
                    CheckResult = Check2Database.WhichReam(result)
                    'create new feature
                    Feature = New OutputFormat
                    'set the feature property
                    Feature.EntityMember = CircMember
                    Feature.FeatureName = CheckResult(0)
                    Feature.ObjectId.Add(result.FirstOrDefault.ObjectId)
                    'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                    Feature.MiscProp(0) = FindTheirJapsName(CheckResult(0))
                    Feature.MiscProp(1) = setView.viewis
                    Feature.OriginAndAddition(0) = SetXPosition(result.FirstOrDefault.Center.X, RefPoint.X, ProjectView)
                    Feature.OriginAndAddition(1) = SetYPosition(result.FirstOrDefault.Center.Y, RefPoint.Y, ProjectView)
                    Feature.OriginAndAddition(2) = 0
                    Feature.OriginAndAddition(3) = CheckResult(2)
                    Feature.OriginAndAddition(4) = CheckResult(3)
                    Feature.OriginAndAddition(5) = 0
                    Feature.OriginAndAddition(6) = 0
                    Feature.OriginAndAddition(7) = 0
                    ListLoopTemp = New List(Of List(Of Entity))
                    ListLoopTemp.Add(LoopTemp)
                    Feature.ListLoop = ListLoopTemp
                    If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(result.FirstOrDefault) Then
                        SelectionCommand.HiddenFeature.Add(Feature)
                        SelectionCommand.HiddenEntity.Add(result.FirstOrDefault)
                    Else
                        'add to the identified feature list
                        SelectionCommand.IdentifiedFeature.Add(Feature)
                        SelectionCommand.TmpIdentifiedFeature.Add(Feature)
                        'OrganizeList.AddListToExisting(Feature)
                        SelectionCommand.IdentifiedCounter = SelectionCommand.IdentifiedCounter + 1
                        AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)
                    End If
                Else
                    'create a new unidentified feature
                    Feature = New OutputFormat
                    'set the faeture property
                    Feature.EntityMember = CircMember
                    Feature.FeatureName = "Drill"
                    Feature.ObjectId.Add(result.FirstOrDefault.ObjectId)
                    'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                    Feature.MiscProp(0) = FindTheirJapsName("Drill")
                    Feature.MiscProp(1) = setView.viewis
                    Feature.OriginAndAddition(0) = SetXPosition(result.FirstOrDefault.Center.X, RefPoint.X, ProjectView)
                    Feature.OriginAndAddition(1) = SetYPosition(result.FirstOrDefault.Center.Y, RefPoint.Y, ProjectView)
                    Feature.OriginAndAddition(2) = 0
                    Feature.OriginAndAddition(3) = Round(result.FirstOrDefault.Radius * 2, 3)
                    Feature.OriginAndAddition(4) = 0
                    Feature.OriginAndAddition(5) = 0
                    Feature.OriginAndAddition(6) = 0
                    Feature.OriginAndAddition(7) = 0
                    ListLoopTemp = New List(Of List(Of Entity))
                    ListLoopTemp.Add(LoopTemp)
                    Feature.ListLoop = ListLoopTemp
                    If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(result.FirstOrDefault) Then
                        SelectionCommand.HiddenFeature.Add(Feature)
                        SelectionCommand.HiddenEntity.Add(result.FirstOrDefault)
                    Else
                        'add to the unidentified feature list
                        SelectionCommand.IdentifiedFeature.Add(Feature)
                        SelectionCommand.TmpIdentifiedFeature.Add(Feature)
                        'OrganizeList.AddListToExisting(Feature)
                        SelectionCommand.IdentifiedCounter = SelectionCommand.IdentifiedCounter + 1
                        AddToTable(Feature, adskClass.myPalette.IFList, adskClass.myPalette.IdentifiedFeature)
                    End If
                End If

            Else
                'loop for each circle in group of circles
                For Each circle As Circle In result
                    'create new unidentified feature
                    Feature = New OutputFormat
                    'set the feature property
                    Feature.EntityMember = CircMember
                    Feature.FeatureName = "Entity " + SelectionCommand.UnIdentifiedFeature.Count.ToString
                    Feature.ObjectId.Add(circle.ObjectId)
                    'Feature.SurfaceName = FindTheirEngViewName(setView.viewis)
                    Feature.MiscProp(0) = FindTheirJapsName("Drill")
                    Feature.MiscProp(1) = setView.viewis
                    Feature.OriginAndAddition(0) = SetXPosition(circle.Center.X, RefPoint.X, ProjectView)
                    Feature.OriginAndAddition(1) = SetYPosition(circle.Center.Y, RefPoint.Y, ProjectView)
                    Feature.OriginAndAddition(2) = 0
                    Feature.OriginAndAddition(3) = Round(circle.Radius * 2, 3)
                    Feature.OriginAndAddition(4) = 0
                    Feature.OriginAndAddition(5) = 0
                    Feature.OriginAndAddition(6) = 0
                    Feature.OriginAndAddition(7) = 0
                    ListLoopTemp = New List(Of List(Of Entity))
                    ListLoopTemp.Add(LoopTemp)
                    Feature.ListLoop = ListLoopTemp
                    If setView.CBHidden = True And Check2Database.CheckIfEntityHidden(circle) Then
                        SelectionCommand.HiddenFeature.Add(Feature)
                        SelectionCommand.HiddenEntity.Add(circle)
                    Else
                        'add to the unidentified feature list
                        SelectionCommand.UnIdentifiedFeature.Add(Feature)
                        SelectionCommand.TmpUnidentifiedFeature.Add(Feature)
                        'OrganizeList.AddListToExisting2(Feature)
                        SelectionCommand.UnIdentifiedCounter = SelectionCommand.UnIdentifiedCounter + 1
                        AddToTable(Feature, adskClass.myPalette.UFList, adskClass.myPalette.UnidentifiedFeature)
                    End If
                Next
            End If
        End If
    End Sub

    'looking the feature name in japanese
    Private Function FindTheirJapsName(ByVal FeatureName As String) As String

        If FeatureName.ToLower.Contains("ream") Then
            Return "リーマ穴" + RTrim(FeatureName).Substring(4)
        End If

        If FeatureName.ToLower.Contains("sunkbolt") Then
            Return "段付きボルト穴" + RTrim(FeatureName).Substring(8)
        End If

        If FeatureName.ToLower.Contains("drill") Then
            Return "ドリル穴"
        End If

        If FeatureName.ToLower.Contains("tap, pt") Then
            Return "ＰＴタップ穴" + RTrim(FeatureName).Substring(3)
        End If

        If FeatureName.ToLower.Contains("tap, m") Then
            Return "タップ穴" + RTrim(FeatureName).Substring(3)
        End If

        Return ""
    End Function

    'check the surface name
    Private Function CheckTheSurface(ByVal SurfaceName As String, ByVal SurfaceIndex As Integer) As String
        If SurfaceIndex = 2 Then
            Return SearchOppositeSurf(SurfaceName)
        Else
            Return SurfaceName
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

    Private TempCoordinate As Double

    'setting x coordinate for hole feature
    Private Function SetXPosition(ByVal a As Double, ByVal XReferencePoint As Double, ByVal view As ViewProp) As Double
        TempCoordinate = New Double

        TempCoordinate = Round(a - XReferencePoint, 3)

        'convert coordinate only for bottom view
        If view.ViewType.ToLower.Equals("bottom") Then
            If TempCoordinate < 0 Then
                TempCoordinate = view.BoundProp(4) - Abs(TempCoordinate)
            Else
                TempCoordinate = TempCoordinate - view.BoundProp(4)
            End If

            'TempCoordinate = -1 * TempCoordinate
        End If

        Return TempCoordinate

    End Function

    'setting y coordinate for hole feature
    Private Function SetYPosition(ByVal a As Double, ByVal YReferencePoint As Double, ByVal view As ViewProp) As Double
        TempCoordinate = New Double

        TempCoordinate = Round(a - YReferencePoint, 3)

        'convert coordinate only for bottom view
        If view.ViewType.ToLower.Equals("bottom") Then
            If TempCoordinate < 0 Then
                TempCoordinate = view.BoundProp(5) - Abs(TempCoordinate)
            Else
                TempCoordinate = TempCoordinate - view.BoundProp(5)
            End If

            'TempCoordinate = -1 * TempCoordinate
        End If

        Return TempCoordinate

    End Function

    'add the recognized feature into the machining feature table
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
End Class
