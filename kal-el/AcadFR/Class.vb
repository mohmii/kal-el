Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.Windows
Imports System.Windows
Imports System.Math
Imports System.Linq

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
            If (myPaletteSet = Nothing) Then
                ' create a new palette set, with a unique guid
                myPaletteSet = New PaletteSet("Feature Recognition", New Guid("F5337918-A32C-4e7a-82A7-198F15F26662"))
                ' now create a palette inside, this has our tree control
                myPalette = New UserControl3
                ' now add the palette to the paletteset
                myPaletteSet.Add("Palette1", myPalette)
            Else
                If SelectionCommand.IdentifiedFeature.Count <> 0 Or SelectionCommand.UnIdentifiedFeature.Count <> 0 _
                Or SelectionCommand.ProjectionView.Count <> 0 Then
                    '"There were still ongoing feature recognition process for current/last drawing.Are you going to start a new one"
                    If MsgBox("まだ図面の形状認識処理が進行中です。" + vbCrLf + _
                              "新規の形状認識を開始しますか？", MsgBoxStyle.YesNo, "AcadFR") _
                              = MsgBoxResult.Yes Then

                        SelectionCommand.IdentifiedFeature.Clear()
                        SelectionCommand.UnIdentifiedFeature.Clear()
                        SelectionCommand.ProjectionView.Clear()

                        myPaletteSet.Dispose()
                        ' create a new palette set, with a unique guid
                        myPaletteSet = New PaletteSet("Feature Recognition", New Guid("F5337918-A32C-4e7a-82A7-198F15F26662"))
                        ' now create a palette inside, this has our tree control
                        myPalette = New UserControl3
                        ' now add the palette to the paletteset
                        myPaletteSet.Add("Palette1", myPalette)
                    End If
                End If

            End If

            'initiate the workspace value
            'AppPreferences.ReadPreferences()

        Catch ex As Exception
            MsgBox("プロダクトデータをロードできません。")
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

    Private ed As Editor
    Private values2() As TypedValue
    Private sfilter2 As SelectionFilter
    Private Opts As PromptSelectionOptions = Nothing
    Private res As PromptSelectionResult
    Private SS As SelectionSet
    Public Shared IdArray As ObjectId()
    Public Shared ObjIDsClassify As List(Of ObjectId)
    Private AcConnector As AcadConn
    Private DbConnector As DatabaseConn
    Private DLock As DocumentLock

    Private TmpSetView As setView
    Private LineTypes As LinetypesPresetting
    Private Schematic As SchematicPresetting
    Private zoom As AcadApplication
    Private DwgPreprocessor As DwgProcessor
    Private ProgBar As ProgressForm

    <CommandMethod("regent")> _
    Public Sub RegEnt()
        zoom = Application.AcadApplication
        zoom.ZoomAll()
        ed = Application.DocumentManager.MdiActiveDocument.Editor

        'create filter with only processing the surface define in the preferences
        Dim values2() As TypedValue = {New TypedValue(DxfCode.LayerName, 0)}
        sfilter2 = New SelectionFilter(values2)

        'selection option for not to select the same object
        Opts = New PromptSelectionOptions()
        Opts.AllowDuplicates = False
        Opts.MessageForAdding = "前処理したい線素を選ぶ:" 'Select the entities that need to be checked:

        'get the result of selection
        res = ed.GetSelection(Opts)
        If res.Status = PromptStatus.OK Then
            SS = res.Value
            IdArray = SS.GetObjectIds
            ObjIDsClassify = New List(Of ObjectId)

            For Each id As ObjectId In IdArray
                ObjIDsClassify.Add(id)
            Next

            AcConnector = New AcadConn

            AcConnector.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)

            Try
                Using AcConnector.myT

                    AcConnector.OpenBlockTableRec()
                    AcConnector.btr.UpgradeOpen()

                    DbConnector = New DatabaseConn
                    DbConnector.InitLinesDb()
                    DbConnector.InitHoleDb()

                    TmpSetView = New setView
                    setView.CBVisible = True
                    setView.CBHidden = True

                    ProgBar = New ProgressForm
                    ProgBar.Text = "の事前設定" 'Preset
                    ProgBar.ProgressBar1.Maximum = 4
                    ProgBar.Label1.Text = Round((((ProgBar.ProgressBar1.Value) / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
                    ProgBar.ProgressBar1.Value = 0
                    ProgBar.Show()
                    System.Windows.Forms.Application.DoEvents()

                    BreakByBlock(ObjIDsClassify, AcConnector)

                    ClasifiyEnt(ObjIDsClassify, AcConnector)

                    ProgBar.ProgressBar1.Value = 1
                    ProgBar.Label1.Text = Round((((ProgBar.ProgressBar1.Value) / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
                    System.Windows.Forms.Application.DoEvents()

                    If UIEntities.Count > 0 And adskClass.AppPreferences.AutoRegLine = True Then
                        LineTypes = New LinetypesPresetting
                        LineTypes.OpenLinetypes(LineTypes, UIEntities)
                        ProgBar.Close()
                        ProgBar.Dispose()
                        AcConnector.myT.Commit()
                    Else
                        'lanjut ke schematic kalo ga ditemukan linetypes
                        SchemProc(AcConnector)
                    End If
                End Using
            Catch ex As Exception
                MsgBox(ex.ToString)
            Finally
                AcConnector.myT.Dispose()
            End Try
        End If
    End Sub

    'prosedur schematic kalo tidak ditemukan linetypes
    Public Overloads Sub SchemProc(ByRef AcConnector As AcadConn)
        DbConnector = New DatabaseConn
        DbConnector.InitLinesDb()
        DbConnector.InitHoleDb()

        ClasifiyEnt(ObjIDsClassify, AcConnector)

        ProgBar.ProgressBar1.Value = 2
        ProgBar.Label1.Text = Round((((ProgBar.ProgressBar1.Value) / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
        System.Windows.Forms.Application.DoEvents()

        If CircEntities.Count > 0 Then
            'menghapus lingkaran redundan

            Dim EntIndexList As New List(Of Integer)
            Dim EntTemp As Entity
            Dim EntityToDel As Entity
            Dim IDIndexRemove As New List(Of Integer)

            'mencari kesamaan lingkaran
            Dim CircTemp1 As New Circle
            Dim CircTemp2 As New Circle
            Dim CircIndexList As New List(Of Integer)

            For CircTemp1Index As Integer = 0 To CircEntities.Count - 2
                CircTemp1 = New Circle
                CircTemp1 = CircEntities(CircTemp1Index)
                For circTemp2Index As Integer = CircTemp1Index + 1 To CircEntities.Count - 1
                    CircTemp2 = New Circle
                    CircTemp2 = CircEntities(circTemp2Index)
                    If isequalCirc(CircTemp1, CircTemp2) = True Then
                        EntTemp = CircTemp1
                        EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                        CircIndexList.Add(CircEntities.IndexOf(CircTemp1))
                        IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                    End If
                Next
            Next

            'membuang entitas yang duplikat
            EntIndexList.Sort()
            For i As Integer = (EntIndexList.Count - 1) To 0 Step (-1)
                EntityToDel = AcConnector.myT.GetObject(AllEntities(EntIndexList(i)).ObjectId, OpenMode.ForWrite, True)
                EntityToDel.Erase()
                AllEntities.RemoveAt(EntIndexList(i))
            Next

            'membuang entitas circle yang duplikat
            CircIndexList.Sort()
            For i As Integer = (CircIndexList.Count - 1) To 0 Step (-1)
                CircEntities.RemoveAt(CircIndexList(i))
            Next

            'membuang id di list object ID
            IDIndexRemove.Sort()
            For i As Integer = (IDIndexRemove.Count - 1) To 0 Step (-1)
                adskClass.ObjIDsClassify.RemoveAt(IDIndexRemove(i))
            Next

            Dim getGroup = From item In CircEntities _
                           Group item By CircXCenter = item.Center Into GroupMember = Group _
                           Select Honor = GroupMember

            'method for schematic presetting
            Dim SchematicStat As Boolean = False
            UI2CircList = New List(Of IEnumerable(Of Circle))
            UI2CircListAll = New List(Of IEnumerable(Of Circle))
            Dim UI2CircStat As Boolean
            For Each result As IEnumerable(Of Circle) In getGroup
                If result.Count = 2 And DbConnector.CheckTopTap(result) = False And DbConnector.CheckBottomTap(result) = False Then
                    SchematicStat = True
                    UI2CircStat = True
                    For Each UI2C As IEnumerable(Of Circle) In UI2CircList
                        If CheckUI2Circle(result, UI2C) = False Then
                            UI2CircStat = False
                        End If
                    Next
                    If UI2CircStat = True Then
                        If result.First.Radius > result.Last.Radius Then
                            UI2CircList.Add(result)
                        Else
                            UI2CircList.Add(result.Reverse)
                        End If
                    End If

                    If result.First.Radius > result.Last.Radius Then
                        UI2CircListAll.Add(result)
                    Else
                        UI2CircListAll.Add(result.Reverse)
                    End If

                End If
            Next

            'membuka form jika ada kelompok 2 lingkaran yg belum teridentifikasi
            If UI2CircList.Count > 0 And adskClass.AppPreferences.AutoRegSchem = True Then
                Schematic = New SchematicPresetting
                Schematic.OpenSchematic(Schematic, UI2CircList)
                ProgBar.Close()
                ProgBar.Dispose()
                AcConnector.myT.Commit()
            Else
                EntityProcessing(AcConnector)
            End If
        Else
            EntityProcessing(AcConnector)
        End If
    End Sub

    'prosedur schematic kalo ditemukan linetypes (form linetipyes terbuka)
    Public Overloads Sub SchemProc()
        AcConnector = New AcadConn
        AcConnector.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)

        DLock = Application.DocumentManager.MdiActiveDocument.LockDocument

        Try
            Using AcConnector.myT
                AcConnector.OpenBlockTableRec()
                AcConnector.btr.UpgradeOpen()

                DbConnector = New DatabaseConn
                DbConnector.InitLinesDb()
                DbConnector.InitHoleDb()

                ProgBar = New ProgressForm
                ProgBar.Text = "の事前設定" 'Preset
                ProgBar.ProgressBar1.Maximum = 4
                ProgBar.ProgressBar1.Value = 2
                ProgBar.Label1.Text = Round((((ProgBar.ProgressBar1.Value) / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
                ProgBar.Show()
                System.Windows.Forms.Application.DoEvents()

                ClasifiyEnt(ObjIDsClassify, AcConnector)

                If CircEntities.Count > 0 Then
                    'menghapus lingkaran redundan

                    Dim EntIndexList As New List(Of Integer)
                    Dim EntTemp As Entity
                    Dim EntityToDel As Entity
                    Dim IDIndexRemove As New List(Of Integer)

                    'mencari kesamaan arc
                    Dim CircTemp1 As New Circle
                    Dim CircTemp2 As New Circle
                    Dim CircIndexList As New List(Of Integer)

                    For CircTemp1Index As Integer = 0 To CircEntities.Count - 2
                        CircTemp1 = New Circle
                        CircTemp1 = CircEntities(CircTemp1Index)
                        For circTemp2Index As Integer = CircTemp1Index + 1 To CircEntities.Count - 1
                            CircTemp2 = New Circle
                            CircTemp2 = CircEntities(circTemp2Index)
                            If isequalCirc(CircTemp1, CircTemp2) = True Then
                                EntTemp = CircTemp1
                                EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                                CircIndexList.Add(CircEntities.IndexOf(CircTemp1))
                                IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                            End If
                        Next
                    Next

                    'membuang entitas yang duplikat
                    EntIndexList.Sort()
                    For i As Integer = (EntIndexList.Count - 1) To 0 Step (-1)
                        EntityToDel = AcConnector.myT.GetObject(AllEntities(EntIndexList(i)).ObjectId, OpenMode.ForWrite, True)
                        EntityToDel.Erase()
                        AllEntities.RemoveAt(EntIndexList(i))
                    Next

                    'membuang entitas circle yang duplikat
                    CircIndexList.Sort()
                    For i As Integer = (CircIndexList.Count - 1) To 0 Step (-1)
                        CircEntities.RemoveAt(CircIndexList(i))
                    Next

                    'membuang id di list object ID
                    IDIndexRemove.Sort()
                    For i As Integer = (IDIndexRemove.Count - 1) To 0 Step (-1)
                        adskClass.ObjIDsClassify.RemoveAt(IDIndexRemove(i))
                    Next

                    Dim getGroup = From item In CircEntities _
                                   Group item By CircXCenter = item.Center Into GroupMember = Group _
                                   Select Honor = GroupMember

                    'method for schematic presetting
                    Dim SchematicStat As Boolean = False
                    UI2CircList = New List(Of IEnumerable(Of Circle))
                    UI2CircListAll = New List(Of IEnumerable(Of Circle))
                    Dim UI2CircStat As Boolean
                    For Each result As IEnumerable(Of Circle) In getGroup
                        If result.Count = 2 And DbConnector.CheckTopTap(result) = False And DbConnector.CheckBottomTap(result) = False Then
                            SchematicStat = True
                            UI2CircStat = True
                            For Each UI2C As IEnumerable(Of Circle) In UI2CircList
                                If CheckUI2Circle(result, UI2C) = False Then
                                    UI2CircStat = False
                                End If
                            Next
                            If UI2CircStat = True Then
                                If result.First.Radius > result.Last.Radius Then
                                    UI2CircList.Add(result)
                                Else
                                    UI2CircList.Add(result.Reverse)
                                End If
                            End If

                            If result.First.Radius > result.Last.Radius Then
                                UI2CircListAll.Add(result)
                            Else
                                UI2CircListAll.Add(result.Reverse)
                            End If

                        End If
                    Next

                    'membuka form jika ada kelompok 2 lingkaran yg belum teridentifikasi
                    If UI2CircList.Count > 0 And adskClass.AppPreferences.AutoRegSchem = True Then
                        Schematic = New SchematicPresetting
                        Schematic.OpenSchematic(Schematic, UI2CircList)
                        ProgBar.Close()
                        ProgBar.Dispose()
                        AcConnector.myT.Commit()
                    Else
                        EntityProcessing(AcConnector)
                    End If
                Else
                    EntityProcessing(AcConnector)
                End If
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            AcConnector.myT.Dispose()
        End Try
    End Sub

    'prosedur cek duplikasi entitas kalo ditemukan tidak schematic (form schematic tidak terbuka)
    Public Overloads Sub EntityProcessing(ByRef AcConnector As AcadConn)

        ProgBar.ProgressBar1.Value = 3
        ProgBar.Label1.Text = Round((((ProgBar.ProgressBar1.Value) / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
        System.Windows.Forms.Application.DoEvents()

        DbConnector = New DatabaseConn
        DbConnector.InitLinesDb()
        DbConnector.InitHoleDb()

        'membuang duplikasi arc dan line
        ClasifiyEnt(ObjIDsClassify, AcConnector)

        Dim EntIndexList As New List(Of Integer)
        Dim EntTemp As Entity
        Dim EntityToDel As Entity
        Dim IDIndexRemove As New List(Of Integer)

        'mencari kesamaan arc
        Dim ArcTemp1 As New Arc
        Dim ArcTemp2 As New Arc
        Dim ArcIndexList As New List(Of Integer)

        For ArcTemp1Index As Integer = 0 To ArcEntities.Count - 2
            ArcTemp1 = New Arc
            ArcTemp1 = ArcEntities(ArcTemp1Index)
            For ArcTemp2Index As Integer = ArcTemp1Index + 1 To ArcEntities.Count - 1
                ArcTemp2 = New Arc
                ArcTemp2 = ArcEntities(ArcTemp2Index)
                If isequalArc(ArcTemp1, ArcTemp2) = True Then
                    EntTemp = ArcTemp1
                    EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                    ArcIndexList.Add(ArcEntities.IndexOf(ArcTemp1))
                    IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                End If
            Next
        Next

        'mencari kesamaan line
        Dim LineTemp1 As New Line
        Dim LineTemp2 As New Line
        Dim LineIndexList As New List(Of Integer)
        Dim EraseStat As Boolean

        For LineTemp1Index As Integer = 0 To LineEntities.Count - 2
            LineTemp1 = New Line
            LineTemp1 = LineEntities(LineTemp1Index)
            For LineTemp2Index As Integer = LineTemp1Index + 1 To LineEntities.Count - 1
                LineTemp2 = New Line
                LineTemp2 = LineEntities(LineTemp2Index)
                If isequalLine(LineTemp1, LineTemp2) = True Then
                    EntTemp = LineTemp1
                    EraseStat = False
                    For Each i As Integer In EntIndexList
                        If AllEntities.IndexOf(EntTemp) = i Then
                            EraseStat = True
                            Exit For
                        End If
                    Next
                    If EraseStat = False Then
                        EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                        LineIndexList.Add(LineEntities.IndexOf(LineTemp1))
                        IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                    End If
                    
                ElseIf isSegmentLine(LineTemp1, LineTemp2) = True Then
                    EntTemp = LineTemp1
                    EraseStat = False
                    For Each i As Integer In EntIndexList
                        If AllEntities.IndexOf(EntTemp) = i Then
                            EraseStat = True
                            Exit For
                        End If
                    Next
                    If EraseStat = False Then
                        EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                        LineIndexList.Add(LineEntities.IndexOf(LineTemp1))
                        IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                    End If
                ElseIf isSegmentLine(LineTemp2, LineTemp1) = True Then
                    EntTemp = LineTemp2
                    EraseStat = False
                    For Each i As Integer In EntIndexList
                        If AllEntities.IndexOf(EntTemp) = i Then
                            EraseStat = True
                            Exit For
                        End If
                    Next
                    If EraseStat = False Then
                        EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                        LineIndexList.Add(LineEntities.IndexOf(LineTemp2))
                        IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                    End If
                End If
            Next
        Next

        'membuang entitas yang duplikat
        EntIndexList.Sort()
        For i As Integer = (EntIndexList.Count - 1) To 0 Step (-1)
            EntityToDel = AcConnector.myT.GetObject(AllEntities(EntIndexList(i)).ObjectId, OpenMode.ForWrite, True)
            EntityToDel.Erase()
            AllEntities.RemoveAt(EntIndexList(i))
        Next

        'membuang entitas arc yang duplikat
        ArcIndexList.Sort()
        For i As Integer = (ArcIndexList.Count - 1) To 0 Step (-1)
            ArcEntities.RemoveAt(ArcIndexList(i))
        Next

        'membuang entitas line yang duplikat
        LineIndexList.Sort()
        For i As Integer = (LineIndexList.Count - 1) To 0 Step (-1)
            LineEntities.RemoveAt(LineIndexList(i))
        Next

        'membuang id di list object ID
        IDIndexRemove.Sort()
        For i As Integer = (IDIndexRemove.Count - 1) To 0 Step (-1)
            adskClass.ObjIDsClassify.RemoveAt(IDIndexRemove(i))
        Next

        'breaking line
        'If adskClass.AppPreferences.DrawPP = True Then
        '    DwgPreprocessor = New DwgProcessor
        '    DwgPreprocessor.StartPreProcessing(AllEntities, LineEntities, AcConnector.myT)
        'End If

        'commit the change
        AcConnector.myT.Commit()

        ProgBar.Label1.Text = Round(((ProgBar.ProgressBar1.Maximum / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
        ProgBar.ProgressBar1.Value = 4
        System.Windows.Forms.Application.DoEvents()

        ProgBar.Close()
        ProgBar.Dispose()

        zoom = Application.AcadApplication
        zoom.ZoomAll()

    End Sub

    'prosedur cek duplikasi entitas kalo ditemukan schematic (form schematic terbuka)
    Public Overloads Sub EntityProcessing()
        AcConnector = New AcadConn
        AcConnector.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)

        DLock = Application.DocumentManager.MdiActiveDocument.LockDocument

        Try
            Using AcConnector.myT
                AcConnector.OpenBlockTableRec()
                AcConnector.btr.UpgradeOpen()

                DbConnector = New DatabaseConn
                DbConnector.InitLinesDb()
                DbConnector.InitHoleDb()

                ProgBar = New ProgressForm
                ProgBar.Text = "の事前設定" 'Preset
                ProgBar.ProgressBar1.Maximum = 4
                ProgBar.ProgressBar1.Value = 3
                ProgBar.Label1.Text = Round((((ProgBar.ProgressBar1.Value) / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
                ProgBar.Show()
                System.Windows.Forms.Application.DoEvents()

                'membuang duplikasi arc dan line
                ClasifiyEnt(ObjIDsClassify, AcConnector)

                Dim EntIndexList As New List(Of Integer)
                Dim EntTemp As Entity
                Dim EntityToDel As Entity
                Dim IDIndexRemove As New List(Of Integer)

                'mencari kesamaan arc
                Dim ArcTemp1 As New Arc
                Dim ArcTemp2 As New Arc
                Dim ArcIndexList As New List(Of Integer)

                For ArcTemp1Index As Integer = 0 To ArcEntities.Count - 2
                    ArcTemp1 = New Arc
                    ArcTemp1 = ArcEntities(ArcTemp1Index)
                    For ArcTemp2Index As Integer = ArcTemp1Index + 1 To ArcEntities.Count - 1
                        ArcTemp2 = New Arc
                        ArcTemp2 = ArcEntities(ArcTemp2Index)
                        If isequalArc(ArcTemp1, ArcTemp2) = True Then
                            EntTemp = ArcTemp1
                            EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                            ArcIndexList.Add(ArcEntities.IndexOf(ArcTemp1))
                            IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                        End If
                    Next
                Next

                'mencari kesamaan line
                Dim LineTemp1 As New Line
                Dim LineTemp2 As New Line
                Dim LineIndexList As New List(Of Integer)

                For LineTemp1Index As Integer = 0 To LineEntities.Count - 2
                    LineTemp1 = New Line
                    LineTemp1 = LineEntities(LineTemp1Index)
                    For LineTemp2Index As Integer = LineTemp1Index + 1 To LineEntities.Count - 1
                        LineTemp2 = New Line
                        LineTemp2 = LineEntities(LineTemp2Index)
                        If isequalLine(LineTemp1, LineTemp2) = True Then
                            EntTemp = LineTemp1
                            EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                            LineIndexList.Add(LineEntities.IndexOf(LineTemp1))
                            IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                        ElseIf isSegmentLine(LineTemp1, LineTemp2) = True Then
                            EntTemp = LineTemp1
                            EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                            LineIndexList.Add(LineEntities.IndexOf(LineTemp1))
                            IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                        ElseIf isSegmentLine(LineTemp2, LineTemp1) = True Then
                            EntTemp = LineTemp2
                            EntIndexList.Add(AllEntities.IndexOf(EntTemp))
                            LineIndexList.Add(LineEntities.IndexOf(LineTemp2))
                            IDIndexRemove.Add(ObjIDsClassify.IndexOf(EntTemp.ObjectId))
                        End If
                    Next
                Next

                'membuang entitas yang duplikat
                EntIndexList.Sort()
                For i As Integer = (EntIndexList.Count - 1) To 0 Step (-1)
                    EntityToDel = AcConnector.myT.GetObject(AllEntities(EntIndexList(i)).ObjectId, OpenMode.ForWrite, True)
                    EntityToDel.Erase()
                    AllEntities.RemoveAt(EntIndexList(i))
                Next

                'membuang entitas arc yang duplikat
                ArcIndexList.Sort()
                For i As Integer = (ArcIndexList.Count - 1) To 0 Step (-1)
                    ArcEntities.RemoveAt(ArcIndexList(i))
                Next

                'membuang entitas line yang duplikat
                LineIndexList.Sort()
                For i As Integer = (LineIndexList.Count - 1) To 0 Step (-1)
                    LineEntities.RemoveAt(LineIndexList(i))
                Next

                'membuang id di list object ID
                IDIndexRemove.Sort()
                For i As Integer = (IDIndexRemove.Count - 1) To 0 Step (-1)
                    adskClass.ObjIDsClassify.RemoveAt(IDIndexRemove(i))
                Next

                'breaking line
                'If adskClass.AppPreferences.DrawPP = True Then
                '    DwgPreprocessor = New DwgProcessor
                '    DwgPreprocessor.StartPreProcessing(AllEntities, LineEntities, AcConnector.myT)
                'End If

                'commit the change
                AcConnector.myT.Commit()

                ProgBar.Label1.Text = Round(((ProgBar.ProgressBar1.Maximum / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
                ProgBar.ProgressBar1.Value = 4
                System.Windows.Forms.Application.DoEvents()

                ProgBar.Close()
                ProgBar.Dispose()

                zoom = Application.AcadApplication
                zoom.ZoomAll()

            End Using
        Catch ex As Exception
            ProgBar.Label1.Text = Round(((ProgBar.ProgressBar1.Maximum / ProgBar.ProgressBar1.Maximum) * 100), 0).ToString
            ProgBar.ProgressBar1.Value = 4
            System.Windows.Forms.Application.DoEvents()
            ProgBar.Close()
            ProgBar.Dispose()

            MsgBox(ex.ToString)
        Finally
            AcConnector.myT.Dispose()
        End Try
    End Sub


    Private id As ObjectId
    Private CircEntities As List(Of Circle)
    Private ArcEntities As List(Of Arc)
    Private PLEntities As List(Of Polyline)
    Public Shared LineEntities As List(Of Line)
    Public Shared AllEntities As List(Of Entity)
    Private Entity As Object

    'List of unidentified entities
    Private UIEntities As List(Of Entity)
    Public Shared UIEntitiesAll As List(Of Entity)
    Private UI2CircList As List(Of IEnumerable(Of Circle))
    Public Shared UI2CircListAll As List(Of IEnumerable(Of Circle))

    'Variabel untuk isi kosongnya LineTypes and SchematicPresetting
    Private LineTypesStat As Boolean

    Private MaxPoint, MinPoint As List(Of Point3d)
    Private ListOfDiameter As List(Of Double)
    Private ListOfCenter As List(Of Point3d)

    Private Sub ClasifiyEnt(ByVal Ids As List(Of ObjectId), ByVal AcadConnection As AcadConn)
        'collection of entities
        CircEntities = New List(Of Circle)
        LineEntities = New List(Of Line)
        ArcEntities = New List(Of Arc)
        PLEntities = New List(Of Polyline)
        AllEntities = New List(Of Entity)
        UIEntities = New List(Of Entity)
        UIEntitiesAll = New List(Of Entity)

        Dim ErasedEnt As New List(Of Entity)
        Dim ErasedEntIdx As New List(Of Integer)
        Dim LineTemp As Line
        Dim ent As Entity
        Dim ObjIDAdd As New List(Of ObjectId)
        Dim ObjIDRemoveIdx As New List(Of Integer)
        LineTypesStat = New Boolean

        MaxPoint = New List(Of Point3d)
        MinPoint = New List(Of Point3d)

        ListOfCenter = New List(Of Point3d)
        ListOfDiameter = New List(Of Double)

        Dim AcConnector2 As New AcadConn
        Dim UIEntStat As Boolean

        AcConnector2.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        Using AcConnector2.myT
            AcConnector2.OpenBlockTableRec()
            AcConnector2.btr.UpgradeOpen()

            'classify the entities into three categories (circle, line, and arc)
            For Each id In ObjIDsClassify
                Entity = AcConnector2.myT.GetObject(id, OpenMode.ForWrite, True)
                'if not entity and not auxiliary
                If DbConnector.CheckIfEntity(Entity) = False And DbConnector.CheckIfEntityAuxiliary(Entity) = False Then
                    'penentuan status LineTypePresetting
                    LineTypesStat = True
                    UIEntStat = True
                    For Each UIE As Entity In UIEntities
                        If UIE.Layer = Entity.Layer And UIE.Linetype = Entity.Linetype And UIE.Color = Entity.Color Then
                            UIEntStat = False
                        End If
                    Next

                    If UIEntStat = True Then
                        UIEntities.Add(Entity)
                    End If

                    UIEntitiesAll.Add(Entity)

                ElseIf DbConnector.CheckIfEntity(Entity) = False And DbConnector.CheckIfEntityAuxiliary(Entity) = True _
                And adskClass.AppPreferences.RemoveUEE = True Then
                    'erase auxiliary entity id remove unessential entities was checked
                    Entity.Erase()
                    ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(Entity.ObjectId))
                Else
                    'add circle, line and arc entities
                    If DbConnector.CheckIfEntity(Entity) = True And Not (TypeOf (Entity) Is DBPoint) And Not (TypeOf (Entity) Is DBText) Then
                        If TypeOf (Entity) Is Circle Then
                            CircEntities.Add(Entity)
                            MaxPoint.Add(Entity.GeometricExtents.MaxPoint)
                            MinPoint.Add(Entity.GeometricExtents.MinPoint)
                            AllEntities.Add(Entity)
                        ElseIf TypeOf (Entity) Is Line Then
                            LineTemp = Entity
                            If isequal(LineTemp.StartPoint.X, LineTemp.EndPoint.X) = True Then
                                ent = New Line(New Point3d(Round(LineTemp.StartPoint.X, 5), LineTemp.StartPoint.Y, 0), New Point3d(Round(LineTemp.EndPoint.X, 5), LineTemp.EndPoint.Y, 0))
                                ent.Layer = Entity.Layer
                                ent.Linetype = Entity.Linetype
                                ent.ColorIndex = Entity.ColorIndex
                                AcConnector2.btr.AppendEntity(ent)
                                AcConnector2.myT.AddNewlyCreatedDBObject(ent, True)
                                ObjIDAdd.Add(ent.ObjectId)
                                ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(Entity.ObjectId))
                                ErasedEnt.Add(Entity)
                                ErasedEntIdx.Add(ErasedEnt.IndexOf(Entity))
                                Entity.Erase()
                                Entity = ent
                            ElseIf isequal(LineTemp.StartPoint.Y, LineTemp.EndPoint.Y) = True Then
                                ent = New Line(New Point3d(LineTemp.StartPoint.X, Round(LineTemp.StartPoint.Y, 5), 0), New Point3d(LineTemp.EndPoint.X, Round(LineTemp.EndPoint.Y, 5), 0))
                                ent.Layer = Entity.Layer
                                ent.Linetype = Entity.Linetype
                                ent.ColorIndex = Entity.ColorIndex
                                AcConnector2.btr.AppendEntity(ent)
                                AcConnector2.myT.AddNewlyCreatedDBObject(ent, True)
                                ObjIDAdd.Add(ent.ObjectId)
                                ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(Entity.ObjectId))
                                ErasedEnt.Add(Entity)
                                ErasedEntIdx.Add(ErasedEnt.IndexOf(Entity))
                                Entity.Erase()
                                Entity = ent
                            End If
                            LineEntities.Add(Entity)
                            MaxPoint.Add(Entity.GeometricExtents.MaxPoint)
                            MinPoint.Add(Entity.GeometricExtents.MinPoint)
                            AllEntities.Add(Entity)
                        ElseIf TypeOf (Entity) Is Arc Then
                            ArcEntities.Add(Entity)
                            MaxPoint.Add(Entity.GeometricExtents.MaxPoint)
                            MinPoint.Add(Entity.GeometricExtents.MinPoint)
                            AllEntities.Add(Entity)
                        ElseIf TypeOf (Entity) Is Polyline Then
                            PLEntities.Add(Entity)
                            MaxPoint.Add(Entity.GeometricExtents.MaxPoint)
                            MinPoint.Add(Entity.GeometricExtents.MinPoint)
                            AllEntities.Add(Entity)
                        Else
                            ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(Entity.ObjectId))
                        End If
                    Else
                        ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(Entity.ObjectId))
                    End If
                End If
            Next id

            'remove object id
            ObjIDRemoveIdx.Sort()
            For i As Integer = (ObjIDRemoveIdx.Count - 1) To 0 Step (-1)
                ObjIDsClassify.RemoveAt(ObjIDRemoveIdx(i))
            Next

            'add object id
            For Each id In ObjIDAdd
                ObjIDsClassify.Add(id)
            Next

            AcConnector2.myT.Commit()
        End Using
        AcConnector2.myT.Dispose()
    End Sub

    'prosedur breaking by block
    Private Sub BreakByBlock(ByRef ObjIDsClassify As List(Of ObjectId), ByVal AcadConnection As AcadConn)
        Dim AcConnector2 As New AcadConn
        Dim EntityBreak As Entity
        Dim acDBObjColl As DBObjectCollection = New DBObjectCollection()
        Dim ObjIDAdd As New List(Of ObjectId)
        Dim ObjIDRemoveIdx As New List(Of Integer)

        AcConnector2.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
        Using AcConnector2.myT
            AcConnector2.OpenBlockTableRec()
            AcConnector2.btr.UpgradeOpen()

            'classify the entities into three categories (circle, line, and arc)
            For Each id In ObjIDsClassify

                EntityBreak = AcConnector2.myT.GetObject(id, OpenMode.ForWrite, True)

                If EntityBreak.Layer.ToLower = "byblock" Or EntityBreak.Linetype.ToLower = "byblock" Or EntityBreak.Color.ColorNameForDisplay.ToLower = "byblock" Then
                    acDBObjColl = New DBObjectCollection()
                    EntityBreak.Explode(acDBObjColl)
                    ObjIDRemoveIdx.Add(ObjIDsClassify.IndexOf(EntityBreak.ObjectId))
                    EntityBreak.Erase()
                    For Each EntBreakRes As Entity In acDBObjColl
                        AcConnector2.btr.AppendEntity(EntBreakRes)
                        AcConnector2.myT.AddNewlyCreatedDBObject(EntBreakRes, True)
                        ObjIDAdd.Add(EntBreakRes.ObjectId)
                    Next
                End If

            Next id

            'remove object id
            ObjIDRemoveIdx.Sort()
            For i As Integer = (ObjIDRemoveIdx.Count - 1) To 0 Step (-1)
                ObjIDsClassify.RemoveAt(ObjIDRemoveIdx(i))
            Next

            'add object id
            For Each id In ObjIDAdd
                ObjIDsClassify.Add(id)
            Next

            AcConnector2.myT.Commit()
        End Using
        AcConnector2.myT.Dispose()
    End Sub
    Private Function isequal(ByVal x As Double, ByVal y As Double) As Boolean
        If Math.Abs(x - y) > adskClass.AppPreferences.ToleranceValues Then
            Return False
        Else
            Return True
        End If
    End Function

    'method for checking unidentifed two circle
    Private Function CheckUI2Circle(ByRef result0 As IEnumerable(Of Circle), ByRef result1 As IEnumerable(Of Circle)) As Boolean
        If result0.First.Radius > result0.Last.Radius Then
            If result0(0).Layer = result1(0).Layer And result0(0).Linetype = result1(0).Linetype And result0(0).Color = result1(0).Color _
            And result0(1).Layer = result1(1).Layer And result0(1).Linetype = result1(1).Linetype And result0(1).Color = result1(1).Color Then
                Return False
            Else
                Return True
            End If
        Else
            If result0(1).Layer = result1(0).Layer And result0(1).Linetype = result1(0).Linetype And result0(1).Color = result1(0).Color _
            And result0(0).Layer = result1(1).Layer And result0(0).Linetype = result1(1).Linetype And result0(0).Color = result1(1).Color Then
                Return False
            Else
                Return True
            End If
        End If
    End Function

    'cek apakah dua buah circle sama
    Private Function isequalCirc(ByVal Circ1 As Circle, ByVal Circ2 As Circle) As Boolean
        If isequalpoint(Circ1.Center, Circ2.Center) = True And isequal(Circ1.Radius, Circ2.Radius) = True Then
            Return True
        Else
            Return False
        End If
    End Function

    'cek apakah dua buah arc sama
    Private Function isequalArc(ByVal Arc1 As Arc, ByVal Arc2 As Arc) As Boolean
        If ((isequalpoint(Arc1.StartPoint, Arc2.StartPoint) = True And isequalpoint(Arc1.EndPoint, Arc2.EndPoint) = True) _
            Or (isequalpoint(Arc1.StartPoint, Arc2.EndPoint) = True And isequalpoint(Arc1.EndPoint, Arc2.StartPoint) = True)) _
            And isequalpoint(Arc1.Center, Arc2.Center) = True And isequal(Arc1.Radius, Arc2.Radius) = True Then
            Return True
        Else
            Return False
        End If
    End Function

    'cek apakah dua buah line sama
    Private Function isequalLine(ByVal line1 As Line, ByVal line2 As Line) As Boolean
        If (isequalpoint(line1.StartPoint, line2.StartPoint) = True And isequalpoint(line1.EndPoint, line2.EndPoint) = True) _
        Or (isequalpoint(line1.StartPoint, line2.EndPoint) = True And isequalpoint(line1.EndPoint, line2.StartPoint) = True) Then
            Return True
        Else
            Return False
        End If
    End Function

    'cek apakah line1 merupakan segmen dari line2 dengan syarat bukan line yg identik
    Private Function isSegmentLine(ByVal line1 As Line, ByVal line2 As Line) As Boolean
        'fungsi ini hanya bisa dengan syarat bukan garis yg sama
        If PointOnline(line1.StartPoint, line2.StartPoint, line2.EndPoint) = 2 And PointOnline(line1.EndPoint, line2.StartPoint, line2.EndPoint) = 2 Then
            Return True
        Else
            Return False
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