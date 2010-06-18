Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Windows
Imports System.Windows

Imports FR

Public Class adskClass

    ' declare a paletteset object, this will only be created once
    Public Shared myPaletteSet As PaletteSet
    ' we need a palette which will be housed by the paletteSet
    Public Shared myPalette As UserControl3
    Public Shared TempName As String

    'we need an initial workspace value
    Public Shared AppPreferences As New AppPreferences

    ' palette command
    <CommandMethod("palette")> _
    Public Sub palette()

        ' check to see if it is valid
        Try
            If SelectionCommand.IdentifiedFeature.Count <> 0 Then
                SelectionCommand.IdentifiedFeature.Clear()
            End If

            If SelectionCommand.UnIdentifiedFeature.Count <> 0 Then
                SelectionCommand.UnIdentifiedFeature.Clear()
            End If

            If SelectionCommand.ProjectionView.Count <> 0 Then
                SelectionCommand.ProjectionView.Clear()
            End If

            If (myPaletteSet = Nothing) Then
                ' create a new palette set, with a unique guid
                myPaletteSet = New PaletteSet("Feature Recognition", New Guid("F5337918-A32C-4e7a-82A7-198F15F26662"))
                ' now create a palette inside, this has our tree control
                myPalette = New UserControl3
                ' now add the palette to the paletteset
                myPaletteSet.Add("Palette1", myPalette)
            Else
                myPaletteSet.Dispose()
                ' create a new palette set, with a unique guid
                myPaletteSet = New PaletteSet("Feature Recognition", New Guid("F5337918-A32C-4e7a-82A7-198F15F26662"))
                ' now create a palette inside, this has our tree control
                myPalette = New UserControl3
                ' now add the palette to the paletteset
                myPaletteSet.Add("Palette1", myPalette)
            End If

            'initiate the workspace value
            AppPreferences.ReadPreferences()

        Catch ex As Exception
            MsgBox("Cannot load the feature list")
        End Try

        ' now display the paletteset
        myPaletteSet.Visible = True
        myPaletteSet.Dock = DockSides.Left
        myPaletteSet.KeepFocus = True

    End Sub

    'method for load the preference setting window
    <CommandMethod("pref")> _
    Public Sub Pref()
        Try
            Using AppPreferencesForm As New AppPreferencesForm
                AppPreferencesForm.ShowDialog()
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Shared ListForm As FRListForm

    'method for load the database interface
    <CommandMethod("loadform")> _
    Public Sub LoadForm()
        Try
            ListForm = New FRListForm
            Using ListForm
                ListForm.ShowDialog()
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    'method for undo last action window

    'Private AcadConn As AcadConn
    'Private entity As Entity
    'Private obj As DBObject
    'Private LineTmp As Line

    '<CommandMethod("rollback")> _
    'Public Sub rollback()
    '    If Not (SelectionCommand.ProjectionView.Count = 0) Then
    '        AcadConn = New AcadConn
    '        Try
    '            AcadConn.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
    '            Using AcadConn.myT

    '                AcadConn.OpenBlockTableRec(AcadConn.myT)

    '                For Each View As ViewProp In SelectionCommand.ProjectionView
    '                    For Each idTmp As ObjectId In AcadConn.btr
    '                        entity = AcadConn.myT.GetObject(idTmp, OpenMode.ForRead)
    '                        obj = AcadConn.myT.GetObject(idTmp, OpenMode.ForRead)
    '                        If (TypeOf obj Is Line) And String.Equals(entity.Layer, "77") Then
    '                            entity.UpgradeOpen()
    '                            entity.Erase(True)
    '                        End If
    '                    Next
    '                Next

    '                AcadConn.myT.Commit()
    '            End Using
    '        Catch ex As Exception
    '            MsgBox(ex.ToString)
    '        Finally
    '            AcadConn.myT.Dispose()
    '        End Try

    '    End If
    'End Sub


End Class