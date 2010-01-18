Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Colors
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Interop
Imports Autodesk.AutoCAD.Geometry

Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Public Class UserControl3

    Private uiSetView As setView
    Private zoom As AcadApplication
    
    'works when the add button was being pressed
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddEntity.Click
        zoom = Application.AcadApplication
        zoom.ZoomAll()

        uiSetView = New setView

        Using uiSetView
            uiSetView.ShowDialog()
        End Using

    End Sub

    Private RowSelectedIndex As Integer
    'clear all the feature that listed on machining feature list
    Private Sub Undo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Undo.Click
        MsgBox("アンドウする投影面を選ぶ.", MsgBoxStyle.Information)
        Try
            zoom = Application.AcadApplication
            zoom.ZoomAll()

            If remove_Line() = False Then
                MsgBox("Object '" & adskClass.TempName & "' Cannot Deleted By Application You must Erased Manual", MsgBoxStyle.Information)
            Else
                Me.IdentifiedFeature.ClearSelection()
                Me.UnidentifiedFeature.ClearSelection()

                RowSelectedIndex = Me.IdentifiedFeature.Rows.Count - 1
                While RowSelectedIndex >= 0
                    If Me.IdentifiedFeature.Rows(RowSelectedIndex).Cells("Surface").Value = adskClass.TempName Then
                        Me.IdentifiedFeature.Rows.RemoveAt(RowSelectedIndex)
                    End If
                    RowSelectedIndex = RowSelectedIndex - 1
                End While

                RowSelectedIndex = Me.UnidentifiedFeature.Rows.Count - 1
                While RowSelectedIndex >= 0
                    If Me.UnidentifiedFeature.Rows(RowSelectedIndex).Cells("Surface").Value = adskClass.TempName Then
                        Me.UnidentifiedFeature.Rows.RemoveAt(RowSelectedIndex)
                    End If
                    RowSelectedIndex = RowSelectedIndex - 1
                End While

                'hapus surface yang dipilih
                Dim index As Integer
                index = adskClass.myPalette.ComboBox2.Items.IndexOf(adskClass.TempName)
                adskClass.myPalette.ComboBox2.Items.RemoveAt(index)
                For i As Integer = 0 To SelectionCommand.ProjectionView.Count - 1
                    If SelectionCommand.ProjectionView(i).ViewType = adskClass.TempName Then
                        SelectionCommand.ProjectionView.RemoveAt(i)
                    End If
                Next

                MakeItBlank()

            End If
        Catch ex As Exception
            ex.ToString()
        End Try
    End Sub

    Private Opts As PromptSelectionOptions
    Private res As PromptSelectionResult

    Private SS As Autodesk.AutoCAD.EditorInput.SelectionSet
    Private Idtemp As ObjectId
    Private tempIdArray() As ObjectId

    Private AppPreferences As New AppPreferences
    Private GenFL As GenerateFL
    Private fw As StreamWriter
    Private FilePath As FileInfo
    Private Biner2Check As List(Of String)
    Private StatusProductSize As Boolean
    Private SaveDialog As System.Windows.Forms.SaveFileDialog

    Private Sub SaveList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveList.Click
        Try
            If AppPreferences.ReadPreferences() Then
                Me.IdentifiedFeature.SelectAll()
                If Me.IdentifiedFeature.SelectedRows.Count <> 0 Then

                    'initialize the save file dialog form
                    SaveDialog = New System.Windows.Forms.SaveFileDialog
                    SaveDialog.FileName = "frcad2pcad"
                    SaveDialog.Filter = "Text file (*.txt) | *.txt"
                    SaveDialog.InitialDirectory = AppPreferences.WSDir '+ Path.DirectorySeparatorChar

                    If SaveDialog.ShowDialog() = Forms.DialogResult.OK Then

                        GenFL = New GenerateFL
                        Biner2Check = New List(Of String)
                        For Each Row As System.Windows.Forms.DataGridViewRow In Me.IdentifiedFeature.SelectedRows
                            Biner2Check.Add(Row.Cells("Biner").Value)
                        Next

                        If Biner2Check.Contains("0") Then
                            MsgBox("保存に失敗しました。" + vbCrLf + "すべての形状にチェックマークが付いてから保存してください。", MsgBoxStyle.Exclamation)
                        Else
                            'FilePath = New FileInfo(AppPreferences.WSDir + Path.DirectorySeparatorChar + "frcad2pcad.txt")
                            FilePath = New FileInfo(SaveDialog.FileName)

                            If FilePath.Exists Then
                                FilePath.Delete()
                                'FilePath = New FileInfo(AppPreferences.WSDir + Path.DirectorySeparatorChar + "frcad2pcad.txt")
                                FilePath = New FileInfo(SaveDialog.FileName)
                            End If

                            fw = FilePath.CreateText
                            GenFL.GenProdSize(fw, SelectionCommand.ProjectionView, StatusProductSize)
                            If StatusProductSize = True Then
                                GenFL.GenRefTxt(fw, SelectionCommand.ProjectionView)
                                GenFL.GenFeatTxt(fw, Me.IdentifiedFeature)
                                fw.Flush()
                                fw.Close()
                                MsgBox("プロダクトデータ保存完了!!", MsgBoxStyle.Information)
                                Me.IdentifiedFeature.ClearSelection()
                                'Process.Start(AppPreferences.WSDir + Path.DirectorySeparatorChar + "frcad2pcad.txt")
                                Process.Start(SaveDialog.FileName)
                            Else
                                fw.Flush()
                                fw.Close()
                                MsgBox("Saving feature list could not be completed!!", MsgBoxStyle.Information)
                                Exit Sub
                            End If
                        End If

                    End If

                Else
                    MsgBox("The saving process could not be done. There are no identified feature on the list.", MsgBoxStyle.Exclamation)
                End If
            Else
                MsgBox("設定ページに、収納場所を指定してください", MsgBoxStyle.Exclamation)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    'autocad variable declaration 

    Friend DocLock As DocumentLock
    Friend DrawEditor As Editor
    Friend Entity As Entity

    'dummy list for support the process higlighting entities
    Friend TempId As String
    Friend ColorId As New List(Of String)
    Friend ColorName As New List(Of Short)
    Friend LineWeightId As New List(Of LineWeight)

    'Friend TempId As String
    Private PastEntityColor As New List(Of InitialColor)
    Private PastEntityColor2 As New List(Of InitialColor)
    Private EntityColor As InitialColor

    'set a place for item selection in listbox1
    Friend ChosenIndex As New List(Of Integer)
    Friend ChosenIndex2 As New List(Of Integer)

    Friend Function SameId(ByVal s As String) As Boolean

        If (Entity.Id.ToString = s) Then
            Return True
        Else
            Return False
        End If
    End Function

    'method for setting the entity color defaults
    Private Sub RoleBackColor(ByVal PastEntColor As List(Of InitialColor), _
                              ByVal BlockTableRecInstances As BlockTableRecord)

        If Not (PastEntColor.Count = 0) Then
            For Each ObjectColorId As InitialColor In PastEntColor
                For Each idTmp As ObjectId In BlockTableRecInstances
                    'acquire the entity from the object id
                    Entity = AcadConnection.myT.GetObject(idTmp, OpenMode.ForWrite)
                    If Entity.ObjectId = ObjectColorId.ColorId Then
                        Entity.Color = Color.FromColorIndex(ColorMethod.ByAci, ObjectColorId.ColorIndex)
                    End If
                Next
            Next
        End If

    End Sub

    'method for highlighting the selected entities
    Private Sub HighlightEntity(ByVal FeatureListId As List(Of ObjectId), _
                                ByVal BlockTableRecInstances As BlockTableRecord, _
                                ByVal PastEntColor As List(Of InitialColor))

        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

        'create loop checking for each element in selected item
        For Each ObjectIdTmp As ObjectId In FeatureListId

            'create a loop checking for each objectid id autocad database
            For Each idTmp As ObjectId In BlockTableRecInstances

                'acquire the entity from the object id
                Entity = AcadConnection.myT.GetObject(idTmp, OpenMode.ForWrite)

                'test if the entity id were the same as entity id in the selected item
                If Entity.ObjectId = ObjectIdTmp Then

                    EntityColor = New InitialColor
                    EntityColor.ColorId = Entity.ObjectId
                    EntityColor.ColorIndex = Entity.ColorIndex
                    PastEntColor.Add(EntityColor)

                    'change the color and the lineweight for making the highlights
                    Entity.Color = Color.FromColorIndex(ColorMethod.ByColor, 10)

                    'save the changed entity id
                    TempId = Entity.Id.ToString

                End If
            Next
        Next
        
        'ed.WriteMessage(" *")
    End Sub

    Private Sub FillIn(ByVal SelectedIndex As List(Of Integer), _
                               ByVal FeatureList As List(Of OutputFormat))
        Me.ComboBox2.Text = FeatureList.Item(SelectedIndex.Item(0)).MiscProp(1) 'surface
    End Sub

    'procedure for filling the machining parameter fields
    Private Sub FillInTheBlank(ByVal FeatureList As List(Of OutputFormat))

        If FeatureList(0).MiscProp(0).Contains("ＰＴタップ穴") Then
            Me.ComboBox1.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(0, 6)
            Me.ComboBox3.Enabled = True
            Me.ComboBox3.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(8)
        ElseIf FeatureList(0).MiscProp(0).Contains("タップ穴") Then
            Me.ComboBox1.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(0, 4)
            Me.ComboBox3.Enabled = True
            Me.ComboBox3.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(6)
        ElseIf FeatureList(0).MiscProp(0).Contains("リーマ穴") And FeatureList(0).MiscProp(0).Contains("R-") Then
            Me.ComboBox1.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(0, 4)
            Me.ComboBox3.Enabled = True
            Me.ComboBox3.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(6)
        ElseIf FeatureList(0).MiscProp(0).Contains("段付きボルト穴") Then
            Me.ComboBox1.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(0, 7)
            Me.ComboBox3.Enabled = True
            Me.ComboBox3.Text = LTrim(FeatureList(0).MiscProp(0)).Substring(9)
        Else
            Me.ComboBox1.Text = FeatureList(0).MiscProp(0) 'name
            Me.ComboBox3.Enabled = False
            Me.ComboBox3.Text = ""
        End If

        Me.ComboBox2.Text = FeatureList(0).MiscProp(1) 'surface
        Me.NumericUpDown4.Value = FeatureList(0).MiscProp(2) 'orientation
        Me.NumericUpDown5.Value = FeatureList(0).MiscProp(3) 'chamfer
        Me.NumericUpDown6.Value = FeatureList(0).MiscProp(4) 'quality
        Me.NumericUpDown1.Value = FeatureList(0).OriginAndAddition(0) 'origin.x
        Me.NumericUpDown2.Value = FeatureList(0).OriginAndAddition(1) 'origin.y
        Me.NumericUpDown3.Value = FeatureList(0).OriginAndAddition(2) 'origin.z
        Me.NumericUpDown7.Value = FeatureList(0).OriginAndAddition(3) 'D1
        Me.NumericUpDown8.Value = FeatureList(0).OriginAndAddition(4) 'D2
        Me.NumericUpDown9.Value = FeatureList(0).OriginAndAddition(5) 'D3
        Me.NumericUpDown10.Value = FeatureList(0).OriginAndAddition(6) 'D4
        Me.NumericUpDown11.Value = FeatureList(0).OriginAndAddition(7) 'angle

    End Sub

    Private counter As Integer

    Private Sub Clear2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete2.Click
        StartDeleting(Me.UnidentifiedFeature)
    End Sub

    Private Sub Delete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete.Click
        StartDeleting(Me.IdentifiedFeature)
    End Sub

    Private ClickPlace As Integer
    Private Index() As Integer

    'used for updating list
    Private Sub UpdateList(ByVal index As Integer, ByVal FeatureList As List(Of OutputFormat), _
                           ByVal IndexCollection As List(Of Integer))

        SetUpFeatureName(Me.ComboBox1.SelectedIndex, FeatureList(index).FeatureName, FeatureList(index).MiscProp(0))
        FeatureList(index).MiscProp(1) = Me.ComboBox2.SelectedItem 'surface
        FeatureList(index).MiscProp(2) = Me.NumericUpDown4.Value 'orientation
        FeatureList(index).MiscProp(3) = Me.NumericUpDown5.Value 'chamfer
        FeatureList(index).MiscProp(4) = Me.NumericUpDown6.Value 'quality
        FeatureList(index).OriginAndAddition(2) = Me.NumericUpDown3.Value 'Z
        FeatureList(index).OriginAndAddition(3) = Me.NumericUpDown7.Value 'D1
        FeatureList(index).OriginAndAddition(4) = Me.NumericUpDown8.Value 'D2
        FeatureList(index).OriginAndAddition(5) = Me.NumericUpDown9.Value 'D3
        FeatureList(index).OriginAndAddition(6) = Me.NumericUpDown10.Value 'D4
        FeatureList(index).OriginAndAddition(7) = Me.NumericUpDown11.Value 'angle

        If IndexCollection.Count = 1 Then
            FeatureList(index).MiscProp(1) = Me.ComboBox2.SelectedItem 'surface
            FeatureList(index).OriginAndAddition(0) = Me.NumericUpDown1.Value 'X
            FeatureList(index).OriginAndAddition(1) = Me.NumericUpDown2.Value 'Y
        End If
    End Sub

    Private Sub SetUpFeatureName(ByVal SelectedIndex As Integer, ByRef EngFeatureName As String, ByRef JapsFeatureName As String)
        Select Case SelectedIndex
            Case 0
                EngFeatureName = "Tap"
                JapsFeatureName = "タップ穴"
            Case 1
                EngFeatureName = "Tap, PT"
                JapsFeatureName = "ＰＴタップ穴"
            Case 2
                EngFeatureName = "Ream"
                JapsFeatureName = "リーマ穴"
            Case 3
                EngFeatureName = "Drill"
                JapsFeatureName = "ドリル穴"
            Case 4
                EngFeatureName = "BlindBore"
                JapsFeatureName = "底付き穴"
            Case 5
                EngFeatureName = "ThroughBore"
                JapsFeatureName = "貫通穴"
            Case 6
                EngFeatureName = "SunkBolt"
                JapsFeatureName = "段付きボルト穴"
            Case 7
                EngFeatureName = "Ring"
                JapsFeatureName = "円形溝"
            Case 8
                EngFeatureName = "Boring"
                JapsFeatureName = ""
        End Select

        If Me.ComboBox3.Enabled = True Then
            JapsFeatureName = JapsFeatureName + ", " + Me.ComboBox3.SelectedItem
        End If

    End Sub

    'used for restoring the initial parameter for each selected identified and unidentified features
    Private Sub Defaults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Accepted.Click
        If Me.Label16.Text <> "0" Then
            For Each Row As System.Windows.Forms.DataGridViewRow In Me.IdentifiedFeature.SelectedRows
                Row.Cells("State").Value = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\tick.png")
                Row.Cells("Biner").Value = "1"
            Next
        End If
    End Sub

    Private Sub ZoomWindow(ByVal point As ViewProp)
        Dim Min() As Double = New Double() {point.BoundProp(0) - 25, point.BoundProp(1) - 25, 0}
        Dim Max() As Double = New Double() {point.BoundProp(2) + 25, point.BoundProp(3) + 50, 0}
        zoom.ZoomWindow(Min, Max)
    End Sub

    Private AcadConnection As AcadConn

    Private HoleFeatProperties As UserControl2
    Private tes As System.Windows.Forms.ToolStripItem

    Public Function remove_Line() As Boolean
        AcadConnection = New AcadConn
        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        DocLock = Application.DocumentManager.MdiActiveDocument.LockDocument
        DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim values() As TypedValue = {New TypedValue(DxfCode.Color, 10)}
        Dim sfilter As New SelectionFilter(values) ' Create the filter using our values...

        Try
            Using DocLock
                AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
                Using AcadConnection.myT
                    Opts = New PromptSelectionOptions()
                    Opts.AllowDuplicates = False

                    res = ed.GetSelection(Opts, sfilter)

                    If res.Status = PromptStatus.OK Then
                        SS = res.Value
                        tempIdArray = SS.GetObjectIds()
                        AcadConnection.OpenBlockTableRec()
                        If Not (PastEntityColor.Count = 0) Then
                            RoleBackColor(PastEntityColor, AcadConnection.btr)
                            PastEntityColor.Clear()
                        ElseIf Not (PastEntityColor2.Count = 0) Then
                            RoleBackColor(PastEntityColor2, AcadConnection.btr)
                            PastEntityColor2.Clear()
                        End If

                        For Each Idtemp In tempIdArray
                            Dim Ent As Entity = CType(AcadConnection.myT.GetObject(Idtemp, OpenMode.ForWrite), Entity)

                            If TypeOf Ent Is MText Then
                                Dim Context As MText = Ent
                                adskClass.TempName = Context.Contents.ToString
                            End If
                            If Not (TypeOf Ent Is Circle) Then Ent.Erase()

                        Next
                    End If
                    AcadConnection.myT.Commit()

                End Using
                DrawEditor.UpdateScreen()
                remove_Line = True
            End Using

        Catch ex As Exception
            MsgBox(ex.ToString)
            remove_Line = False
        Finally
            AcadConnection.myT.Dispose()
        End Try
    End Function

    Private Sub ComboBox1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectionChangeCommitted
        ComboBox3.Enabled = False
        ComboBox3.Items.Clear()
        Select Case ComboBox1.SelectedIndex
            Case 0, 1, 2, 6
                ComboBox3.Enabled = True
                SearchSM(ComboBox1.SelectedIndex)
            Case Else
                ComboBox3.Enabled = False
        End Select
    End Sub

    Private Sub NameList_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox3.SelectedIndexChanged
        If ComboBox3.SelectedItem <> Nothing Then
            ShowDetail(ComboBox1.SelectedIndex, ComboBox3.SelectedItem)
        End If
    End Sub

    Sub SearchSM(ByVal SearchWord As Integer)
        Dim ConDb As New DatabaseConn
        Dim Name As New UserControl3
        Select Case SearchWord

            Case 0, 1, 2, 6
                Try
                    If ConDb.IsConnected() = True Then
                        'Queries to database
                        Select Case SearchWord
                            Case 0
                                'strSQL = "SELECT * FROM TapData WHERE Name LIKE 'M%'"
                                ConDb.myCmd.CommandText = "SELECT * FROM TapData WHERE Name LIKE 'M%' Order By ID"
                                ConDb.myCmd.Connection = ConDb.myConn
                                ConDb.myDA.SelectCommand = ConDb.myCmd
                                ConDb.myDR = ConDb.myCmd.ExecuteReader()
                                'Display results to table
                                Me.ComboBox3.Items.Clear()
                                While (ConDb.myDR.Read())
                                    Me.ComboBox3.Items.Add(ConDb.myDR("Name"))
                                End While
                            Case 1
                                ConDb.myCmd.CommandText = "SELECT * FROM TapData WHERE Name LIKE 'PT%' Order By ID"
                                ConDb.myCmd.Connection = ConDb.myConn
                                ConDb.myDA.SelectCommand = ConDb.myCmd
                                ConDb.myDR = ConDb.myCmd.ExecuteReader()
                                'Display results to table
                                Me.ComboBox3.Items.Clear()
                                While (ConDb.myDR.Read())
                                    Me.ComboBox3.Items.Add(ConDb.myDR("Name"))
                                End While
                            Case 2
                                ConDb.myCmd.CommandText = "SELECT * FROM ReamData Order By ID"
                                ConDb.myCmd.Connection = ConDb.myConn
                                ConDb.myDA.SelectCommand = ConDb.myCmd
                                ConDb.myDR = ConDb.myCmd.ExecuteReader()
                                'Display results to table
                                Me.ComboBox3.Items.Clear()
                                While (ConDb.myDR.Read())
                                    Me.ComboBox3.Items.Add("R-" & (ConDb.myDR("Diameter")))
                                End While
                            Case 6
                                ConDb.myCmd.CommandText = "SELECT * FROM CounterBore Order By ID"
                                ConDb.myCmd.Connection = ConDb.myConn
                                ConDb.myDA.SelectCommand = ConDb.myCmd
                                ConDb.myDR = ConDb.myCmd.ExecuteReader()
                                'Display results to table
                                Me.ComboBox3.Items.Clear()
                                While (ConDb.myDR.Read())
                                    Me.ComboBox3.Items.Add(ConDb.myDR("M"))
                                End While
                        End Select

                        'Me.NameList.Visible = True
                        ComboBox3.Select()
                        ConDb.myConn.Close()
                    End If
                Catch ex As Exception
                    'Me.NameList.Visible = False
                    MsgBox(ex.Message, , "Search")
                    ConDb.myConn.Close()
                End Try

        End Select
    End Sub

    Sub ShowDetail(ByVal Parameter As Integer, ByVal SearchWord As String)
        Dim ConDb As New DatabaseConn
        'MsgBox(Parameter & " " & SearchWord)
        If ConDb.IsConnected() = True Then
            Select Case Parameter
                Case 0, 1
                    ConDb.myCmd.CommandText = "SELECT * FROM TapData WHERE Name = '" & SearchWord & "' "
                    ConDb.myCmd.Connection = ConDb.myConn
                    ConDb.myDA.SelectCommand = ConDb.myCmd
                    ConDb.myDR = ConDb.myCmd.ExecuteReader()
                    ConDb.myDR.Read()
                    Me.NumericUpDown7.Value = ConDb.myDR("TapDia")
                    Me.NumericUpDown8.Value = ConDb.myDR("TapDepth")
                    Me.NumericUpDown9.Value = ConDb.myDR("UnHoleDia")
                    Me.NumericUpDown10.Value = ConDb.myDR("UnHoleDepth")
                    Me.NumericUpDown11.Value = 0
                    'ComboBox1.Text = Parameter & ", " & SearchWord
                Case 2
                    Dim LenData As Integer
                    Dim Data As String
                    LenData = Len(SearchWord) - 2
                    Data = Mid(SearchWord, 3, LenData)

                    ConDb.myCmd.CommandText = "SELECT * FROM ReamData WHERE Diameter = " & Data & ""
                    ConDb.myCmd.Connection = ConDb.myConn
                    ConDb.myDA.SelectCommand = ConDb.myCmd
                    ConDb.myDR = ConDb.myCmd.ExecuteReader()
                    ConDb.myDR.Read()
                    Me.NumericUpDown7.Value = ConDb.myDR("Diameter")
                    Me.NumericUpDown8.Value = ConDb.myDR("Depth")
                    Me.NumericUpDown9.Value = 0
                    Me.NumericUpDown10.Value = 0
                    'ComboBox1.Text = Parameter & ", " & SearchWord
                Case 6
                    ConDb.myCmd.CommandText = "SELECT * FROM CounterBore WHERE M = '" & SearchWord & "'"
                    ConDb.myCmd.Connection = ConDb.myConn
                    ConDb.myDA.SelectCommand = ConDb.myCmd
                    ConDb.myDR = ConDb.myCmd.ExecuteReader()
                    ConDb.myDR.Read()
                    Me.NumericUpDown7.Value = ConDb.myDR("con_bore_d")
                    Me.NumericUpDown8.Value = 0
                    Me.NumericUpDown9.Value = ConDb.myDR("con_boreD")
                    Me.NumericUpDown10.Value = ConDb.myDR("con_boreH")
                    'ComboBox1.Text = Parameter & ", " & SearchWord
            End Select

        End If
    End Sub

    Private SelectedIF, SelectedUF As New List(Of OutputFormat)

    Private Sub IdentifiedFeature_CellContentClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IdentifiedFeature.SelectionChanged
        If Me.IdentifiedFeature.Focused Then
            Me.Label17.Text = "0"                                               'set the selected items counter in Unidentified Table
            Me.Label16.Text = Me.IdentifiedFeature.SelectedRows.Count.ToString  'set the selected items counter in Identified Table
            Me.UnidentifiedFeature.ClearSelection()                             'clear all selection in Unidentified table
            Me.Accepted.Enabled = True                                          'enable the Accepted button
            ComboBox3.Enabled = False                                           'enable the feature description combobox

            'only happens when selection is empty
            If SelectedIF.Count = 0 Then
                zoom = Application.AcadApplication
                zoom.ZoomAll()
            End If

            'clear previous selection
            SelectedIF.Clear()

            'collect all selected features
            For Each Row As System.Windows.Forms.DataGridViewRow In Me.IdentifiedFeature.SelectedRows
                If Not SelectedIF.Contains(Row.Cells("Object").Value) Then
                    SelectedIF.Add(Row.Cells("Object").Value)
                End If
            Next

            'set the X, Y parameter combo box, if multiple selection those combobox are disabled, if single selection _
            'those combo box are enabled
            If SelectedIF.Count = 1 Then
                Me.NumericUpDown1.Enabled = True
                Me.NumericUpDown2.Enabled = True
            Else
                Me.NumericUpDown1.Enabled = False
                Me.NumericUpDown2.Enabled = False
            End If

            Try
                If SelectedIF.Count <> 0 Then

                    'fill all the machining parameters field
                    FillInTheBlank(SelectedIF)

                    'create a document lock and acquire the information from the current drawing editor
                    DocLock = Application.DocumentManager.MdiActiveDocument.LockDocument
                    DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

                    'initiate a new connection
                    AcadConnection = New AcadConn

                    Using DocLock
                        AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
                        Using AcadConnection.myT
                            'initial setting for opening the connection for read the autocad database
                            AcadConnection.OpenBlockTableRec()

                            'dummy variable for preventing clearing the current highlighted entity/entities
                            TempId = Nothing

                            'roleback each precious selected entities to their default color
                            RoleBackColor(PastEntityColor2, AcadConnection.btr)
                            PastEntityColor2.Clear()
                            RoleBackColor(PastEntityColor, AcadConnection.btr)

                            PastEntityColor = New List(Of InitialColor)

                            For Each SelectedFeature As OutputFormat In SelectedIF
                                'highlight the current selected entities
                                HighlightEntity(SelectedFeature.ObjectId, AcadConnection.btr, PastEntityColor)
                            Next

                            'committing the autocad transaction
                            AcadConnection.myT.Commit()
                        End Using
                    End Using

                    'refresh the drawing editor after highlighting
                    'DrawEditor.UpdateScreen()
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            Finally
                'DrawEditor.WriteMessage(" *" + vbCrLf)
                DrawEditor.UpdateScreen()
                AcadConnection.myT.Dispose()
            End Try
        End If
    End Sub

    Private Sub UnidentifiedFeature_CellContentClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnidentifiedFeature.SelectionChanged
        If Me.UnidentifiedFeature.Focused Then
            Me.Label16.Text = "0"
            Me.Label17.Text = Me.UnidentifiedFeature.SelectedRows.Count.ToString
            Me.IdentifiedFeature.ClearSelection()
            Me.Accepted.Enabled = False
            ComboBox3.Enabled = False

            If SelectedUF.Count = 0 Then
                zoom = Application.AcadApplication
                zoom.ZoomAll()
            End If

            SelectedUF.Clear()
            'collect selected item
            For Each Row As System.Windows.Forms.DataGridViewRow In Me.UnidentifiedFeature.SelectedRows
                If Not SelectedUF.Contains(Row.Cells("Object").Value) Then
                    SelectedUF.Add(Row.Cells("Object").Value)
                End If
            Next

            If SelectedUF.Count = 1 Then
                Me.NumericUpDown1.Enabled = True
                Me.NumericUpDown2.Enabled = True
            Else
                Me.NumericUpDown1.Enabled = False
                Me.NumericUpDown2.Enabled = False
            End If

            Try
                If SelectedUF.Count <> 0 Then

                    FillInTheBlank(SelectedUF)
                    
                    'create a document lock and acquire the information from the current drawing editor
                    DocLock = Application.DocumentManager.MdiActiveDocument.LockDocument
                    DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

                    AcadConnection = New AcadConn

                    Using DocLock

                        AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
                        Using AcadConnection.myT

                            'initial setting for opening the connection for read the autocad database

                            AcadConnection.OpenBlockTableRec()

                            'dummy variable for preventing clearing the current highlighted entity/entities
                            TempId = Nothing

                            RoleBackColor(PastEntityColor, AcadConnection.btr)
                            PastEntityColor.Clear()

                            RoleBackColor(PastEntityColor2, AcadConnection.btr)

                            PastEntityColor2 = New List(Of InitialColor)

                            For Each SelectedFeature As OutputFormat In SelectedUF
                                HighlightEntity(SelectedFeature.ObjectId, AcadConnection.btr, PastEntityColor2)
                            Next

                            'committing the autocad transaction
                            AcadConnection.myT.Commit()
                        End Using
                    End Using

                    'try to refresh the drawing editor after highlighting
                    DrawEditor.UpdateScreen()
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            Finally
                AcadConnection.myT.Dispose()
            End Try
        End If
    End Sub

    Private Sub IdentifiedFeature_CellContentClick_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles IdentifiedFeature.MouseDown
        If e.Button.Equals(Forms.MouseButtons.Right) And e.Clicks.Equals(1) Then
            If Me.Label16.Text <> "0" Then
                Me.IdentifiedFeature.ContextMenuStrip = ContextMenuStrip1
                If Me.Label16.Text = "1" Then
                    Me.ContextMenuStrip1.Items(0).Enabled = True
                    Me.ContextMenuStrip1.Items(1).Enabled = True
                Else
                    Me.ContextMenuStrip1.Items(0).Enabled = False
                    Me.ContextMenuStrip1.Items(1).Enabled = False
                End If
            End If
        Else
            Me.IdentifiedFeature.ContextMenuStrip = Nothing
        End If

    End Sub

    Private Sub UnidentifiedFeature_CellContentClick_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles UnidentifiedFeature.MouseDown
        If e.Button.Equals(Forms.MouseButtons.Right) And e.Clicks.Equals(1) Then
            If Me.Label17.Text <> "0" Then
                Me.UnidentifiedFeature.ContextMenuStrip = ContextMenuStrip1
                If Me.Label17.Text = "1" Then
                    Me.ContextMenuStrip1.Items(0).Enabled = True
                    Me.ContextMenuStrip1.Items(1).Enabled = True
                Else
                    Me.ContextMenuStrip1.Items(0).Enabled = False
                    Me.ContextMenuStrip1.Items(1).Enabled = False
                End If
            End If
        Else
            Me.UnidentifiedFeature.ContextMenuStrip = Nothing
        End If
    End Sub

    Private Sub ZoomStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ZoomStripMenuItem1.Click
        If Me.Label16.Text <> "0" Then
            StartZooming(Me.IdentifiedFeature.SelectedRows(0))
        End If

        If Me.Label17.Text <> "0" Then
            StartZooming(Me.UnidentifiedFeature.SelectedRows(0))
        End If
    End Sub

    Private Feature2Zoom As OutputFormat

    Private Sub StartZooming(ByVal SelectedRow As System.Windows.Forms.DataGridViewRow)
        Try
            zoom = Application.AcadApplication
            DocLock = Application.DocumentManager.MdiActiveDocument.LockDocument
            DrawEditor = Application.DocumentManager.MdiActiveDocument.Editor

            Feature2Zoom = New OutputFormat
            AcadConnection = New AcadConn

            Using DocLock
                AcadConnection.StartTransaction(Application.DocumentManager.MdiActiveDocument.Database)
                Using AcadConnection.myT
                    AcadConnection.OpenBlockTableRec()
                    Feature2Zoom = SelectedRow.Cells("Object").Value
                    For Each idTmp As ObjectId In AcadConnection.btr
                        'acquire the entity from the object id
                        Entity = AcadConnection.myT.GetObject(idTmp, OpenMode.ForRead)
                        'test if the entity id were the as entity id in the selected item
                        If Entity.ObjectId = Feature2Zoom.Objectid(0) Then
                            Dim obj As DBObject
                            obj = AcadConnection.myT.GetObject(idTmp, OpenMode.ForRead)
                            For Each i As ViewProp In SelectionCommand.ProjectionView
                                If String.Equals(i.ViewType, Feature2Zoom.MiscProp(1)) Then
                                    ZoomWindow(i)
                                End If
                            Next
                        End If
                    Next
                    AcadConnection.myT.Commit()
                End Using
            End Using
            DrawEditor.UpdateScreen()
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            AcadConnection.myT.Dispose()
        End Try
    End Sub

    'procedure to highlighting same features
    Private Sub HighlightSameFeatures(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HighlightStripMenuItem1.Click

        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

        If Me.IdentifiedFeature.SelectedRows.Count <> 0 Then
            StartHighlighting(Me.IdentifiedFeature)
            
        End If

        If Me.UnidentifiedFeature.SelectedRows.Count <> 0 Then
            StartHighlighting(Me.UnidentifiedFeature)
        End If
    End Sub

    Private Feature2Highlight, Feature2Compare As OutputFormat

    Private Sub StartHighlighting(ByVal Table2Check As System.Windows.Forms.DataGridView)

        'set a field for comparison feature
        Feature2Compare = New OutputFormat

        'initiate feature to highlight
        Feature2Highlight = New OutputFormat
        Feature2Highlight = Table2Check.SelectedRows(0).Cells("Object").Value

        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

        'initiate the progress bar
        acedSetStatusBarProgressMeter("Testing", 0, Table2Check.Rows.Count)
        Dim i As Integer

        'higlighting process
        For Each Row As System.Windows.Forms.DataGridViewRow In Table2Check.Rows

            Feature2Compare = Row.Cells("Object").Value
            If Feature2Highlight.MiscProp(0).Equals(Feature2Compare.MiscProp(0)) And _
            Feature2Highlight.OriginAndAddition(3).Equals(Feature2Compare.OriginAndAddition(3)) Then
                Row.Selected = True
            End If

            'add the progress bar
            i = i + 1
            'System.Threading.Thread.Sleep(1)
            acedSetStatusBarProgressMeterPos(i)
            System.Windows.Forms.Application.DoEvents()

        Next

        acedRestoreStatusBar()
    End Sub

    'deleting process when delete command in context menu strip was being clicked
    Private Sub DeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteStripMenuItem1.Click

        'delete the selected items in Identified Feature table
        If Me.Label16.Text <> 0 Then
            StartDeleting(Me.IdentifiedFeature)
        End If

        'delete the selected items in Unidentified Feature table
        If Me.Label17.Text <> 0 Then
            StartDeleting(Me.UnidentifiedFeature)
        End If
    End Sub

    Private RowIndex As List(Of Integer)

    Public Sub StartDeleting(ByVal Table2Check As System.Windows.Forms.DataGridView)
        Try
            RowIndex = New List(Of Integer) 'selected rows index

            'get all the indexes
            For Each Rows As System.Windows.Forms.DataGridViewRow In Table2Check.SelectedRows
                RowIndex.Add(Rows.Index)
            Next

            RowIndex.Sort()

            'clear table selection
            Table2Check.ClearSelection()

            Dim i As Integer
            i = RowIndex.Count - 1

            While i >= 0
                Table2Check.Rows.RemoveAt(RowIndex(i))
                i = i - 1
            End While
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Private Sub Update1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Update1.Click
        Try
            If CheckEditingStatus(Me.ComboBox1.SelectedIndex) = True Then
                If Me.Label16.Text <> "0" Then
                    StartUpdate(Me.IdentifiedFeature)
                ElseIf Me.Label17.Text <> "0" Then
                    StartUpdate(Me.UnidentifiedFeature)
                End If
            End If
            
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Function CheckEditingStatus(ByVal FeatureIndex As Integer) As Boolean
        Dim EngName, JapName As String

        EngName = ""
        JapName = ""
        SetUpFeatureName(FeatureIndex, EngName, JapName)

        If EngName.Equals("Tap") Or EngName.Equals("Tap, PT") Then

            If Me.NumericUpDown7.Value > Me.NumericUpDown9.Value Then 'D1 > D3
                If Me.NumericUpDown8.Value < Me.NumericUpDown10.Value Then 'D2 < D4
                    Return True
                Else
                    MsgBox("D2とD4の寸法を調べてください。" + vbCrLf + vbCrLf + "タップ或いはPTタップの場合、D2はD4より小さい値でないといけません。" _
                           , MsgBoxStyle.Exclamation)
                    Me.NumericUpDown8.Focus()
                    Return False
                End If
            Else
                MsgBox("D1とD3の寸法を調べてください。" + vbCrLf + vbCrLf + "タップ或いはPTタップの場合、D1はD3より大きい値でないといけません。" _
                       , MsgBoxStyle.Exclamation)
                Me.NumericUpDown7.Focus()
                Return False
            End If
        End If

        If EngName.Equals("Ream") Or EngName.Equals("Drill") Or EngName.Equals("BlindBore") Or _
        EngName.Equals("ThroughBore") Or EngName.Equals("Boring") Then
            If Me.NumericUpDown9.Value <> 0 Or Me.NumericUpDown10.Value <> 0 Then 'D3 & D4 must be zero
                MsgBox("D3とD4の寸法を調べてください。" + vbCrLf + vbCrLf + "リーマ穴、ドリル穴、底付穴、貫通穴、およびボーリング穴の場合、D3とD4はセロでないとけません。" _
                       , MsgBoxStyle.Exclamation)
                Me.NumericUpDown9.Focus()
                Return False
            Else
                Return True
            End If
        End If

        If EngName.Equals("SunkBolt") Then
            If Me.NumericUpDown7.Value < Me.NumericUpDown9.Value Then 'D1 < D3
                If Me.NumericUpDown8.Value > Me.NumericUpDown10.Value Then 'D2 > D4
                    Return True
                Else
                    MsgBox("D2とD4の寸法を調べてください。" + vbCrLf + vbCrLf + "段付穴の場合、D2はD4より大きい値でないといけません。" _
                           , MsgBoxStyle.Exclamation)
                    Me.NumericUpDown8.Focus()
                    Return False
                End If
            Else
                MsgBox("D1とD3の寸法を調べてください。" + vbCrLf + vbCrLf + "段付穴の場合、D1はD3より小さい値でないといけません。" _
                           , MsgBoxStyle.Exclamation)
                Me.NumericUpDown7.Focus()
                Return False
            End If
        End If

        If EngName.Equals("Ring") Then
            If Me.NumericUpDown7.Value < Me.NumericUpDown8.Value Then 'D1 < D2
                If Me.NumericUpDown9.Value >= 0 Then 'D3 >= 0
                    Return True
                Else
                    MsgBox("D3の寸法を調べてください。" + vbCrLf + vbCrLf + "円形溝の場合、D3はゼロかゼロより大きい値でないといけません。" _
                           , MsgBoxStyle.Exclamation)
                    Me.NumericUpDown9.Focus()
                    Return False
                End If
            Else
                MsgBox("D1とD2の寸法を調べてください。" + vbCrLf + vbCrLf + "円形溝の場合、D1はD2より小さい値でないといけません。" _
                           , MsgBoxStyle.Exclamation)
                Me.NumericUpDown7.Focus()
                Return False
            End If
        End If
    End Function

    Private Feature2Update As OutputFormat
    Private NewUpdatedFeature As OutputFormat

    Private Sub StartUpdate(ByVal Table2Check As System.Windows.Forms.DataGridView)
        RowIndex = New List(Of Integer)         'set new field for selected row indexes

        'get all the selected indexes
        For Each Rows As System.Windows.Forms.DataGridViewRow In Table2Check.SelectedRows
            RowIndex.Add(Rows.Index)
        Next

        'start updating
        For i As Integer = 0 To RowIndex.Count - 1
            NewUpdatedFeature = New OutputFormat    'set new field for the new machining parameter

            'get all the new machining parameters
            SetUpFeatureName(Me.ComboBox1.SelectedIndex, NewUpdatedFeature.FeatureName, NewUpdatedFeature.MiscProp(0))
            NewUpdatedFeature.MiscProp(1) = Me.ComboBox2.SelectedItem 'surface
            NewUpdatedFeature.MiscProp(2) = Me.NumericUpDown4.Value 'orientation
            NewUpdatedFeature.MiscProp(3) = Me.NumericUpDown5.Value 'chamfer
            NewUpdatedFeature.MiscProp(4) = Me.NumericUpDown6.Value 'quality
            NewUpdatedFeature.OriginAndAddition(2) = Me.NumericUpDown3.Value 'Z
            NewUpdatedFeature.OriginAndAddition(3) = Me.NumericUpDown7.Value 'D1
            NewUpdatedFeature.OriginAndAddition(4) = Me.NumericUpDown8.Value 'D2
            NewUpdatedFeature.OriginAndAddition(5) = Me.NumericUpDown9.Value 'D3
            NewUpdatedFeature.OriginAndAddition(6) = Me.NumericUpDown10.Value 'D4
            NewUpdatedFeature.OriginAndAddition(7) = Me.NumericUpDown11.Value 'angle        

            'get the current binded feature from the selected rows
            Feature2Update = New OutputFormat
            Feature2Update = Table2Check.Rows(RowIndex(i)).Cells("Object").Value

            For Each ObjectIdTmp As ObjectId In Feature2Update.ObjectId
                NewUpdatedFeature.ObjectId.Add(ObjectIdTmp)
            Next

            'get the unique parameter only for the X and Y location
            If RowIndex.Count = 1 Then
                NewUpdatedFeature.OriginAndAddition(0) = Me.NumericUpDown1.Value 'X
                NewUpdatedFeature.OriginAndAddition(1) = Me.NumericUpDown2.Value 'Y
            Else
                NewUpdatedFeature.OriginAndAddition(0) = Feature2Update.OriginAndAddition(0)
                NewUpdatedFeature.OriginAndAddition(1) = Feature2Update.OriginAndAddition(1)
            End If

            Table2Check.Rows(RowIndex(i)).Cells("Object").Value = NewUpdatedFeature

            If ComboBox3.Enabled = False Then
                Table2Check.Rows(RowIndex(i)).Cells("Name").Value = Me.ComboBox1.Text
            Else
                Table2Check.Rows(RowIndex(i)).Cells("Name").Value = Me.ComboBox1.Text + ", " + Me.ComboBox3.Text
            End If

            Table2Check.Rows(RowIndex(i)).Cells("State").Value = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\tick.png")
            Table2Check.Rows(RowIndex(i)).Cells("Biner").Value = "1"
        Next

        'clear all the selection
        'Table2Check.ClearSelection()
    End Sub

    Private Sub Insert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Insert.Click
        Try
            If SelectedUF.Count <> 0 Then
                'StartUpdate(Me.UnidentifiedFeature)
                RowIndex = New List(Of Integer)         'set new field for selected row indexes

                'get all the selected indexes
                For Each Rows As System.Windows.Forms.DataGridViewRow In Me.UnidentifiedFeature.SelectedRows
                    RowIndex.Add(Rows.Index)
                Next

                RowIndex.Sort()

                Me.UnidentifiedFeature.ClearSelection()

                Dim i As Integer
                i = RowIndex.Count - 1

                While i >= 0
                    Dim NewRow As DataRow = IFList.NewRow()
                    NewRow("State") = Me.UnidentifiedFeature.Rows(RowIndex(i)).Cells("State").Value
                    NewRow("Name") = Me.UnidentifiedFeature.Rows(RowIndex(i)).Cells("Name").Value
                    NewRow("Surface") = Me.UnidentifiedFeature.Rows(RowIndex(i)).Cells("Surface").Value
                    NewRow("Biner") = Me.UnidentifiedFeature.Rows(RowIndex(i)).Cells("Biner").Value
                    NewRow("Object") = Me.UnidentifiedFeature.Rows(RowIndex(i)).Cells("Object").Value

                    IFList.Rows.Add(NewRow)
                    Me.UnidentifiedFeature.Rows.RemoveAt(RowIndex(i))

                    i = i - 1
                End While
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public IFList, UFList As System.Data.DataTable

    Private Sub CreateTable(ByVal List As System.Data.DataTable, ByVal Table As System.Windows.Forms.DataGridView)
        Dim StatusColumn = New System.Data.DataColumn("State", GetType(System.Drawing.Image))
        Dim NameColumn As New System.Data.DataColumn("Name", GetType(String))
        Dim SurfaceColumn As New System.Data.DataColumn("Surface", GetType(String))
        Dim BinerColumn As New System.Data.DataColumn("Biner", GetType(String))
        Dim ObjectColumn As New System.Data.DataColumn("Object", GetType(OutputFormat))

        List.Columns.AddRange(New System.Data.DataColumn() {StatusColumn, NameColumn, SurfaceColumn, BinerColumn, ObjectColumn})
        Table.DataSource = List

        Table.Columns("Biner").Visible = False
        Table.Columns("Object").Visible = False
    End Sub

    Private InitialFeature As OutputFormat

    Private ProductVer As String

    Private Sub UserControl3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        IFList = New System.Data.DataTable("Features")
        CreateTable(IFList, Me.IdentifiedFeature)

        UFList = New System.Data.DataTable("Features")
        CreateTable(UFList, Me.UnidentifiedFeature)
        MakeItBlank()

        Me.version.Text = "Version " + ProductVersion

    End Sub

    Public Sub MakeItBlank()
        'initiate blank feature
        Me.ComboBox1.Text = ""
        Me.ComboBox2.Text = ""
        Me.NumericUpDown4.Value = 0 'orientation
        Me.NumericUpDown5.Value = 0 'chamfer
        Me.NumericUpDown6.Value = 0 'quality
        Me.NumericUpDown1.Value = 0 'origin.x
        Me.NumericUpDown2.Value = 0 'origin.y
        Me.NumericUpDown3.Value = 0 'origin.z
        Me.NumericUpDown7.Value = 0 'D1
        Me.NumericUpDown8.Value = 0 'D2
        Me.NumericUpDown9.Value = 0 'D3
        Me.NumericUpDown10.Value = 0 'D4
        Me.NumericUpDown11.Value = 0 'angle
        Me.PictureBox1.Image = Nothing
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        FindTheirPicture(Me.ComboBox1.SelectedIndex)
    End Sub

    Private Sub FindTheirPicture(ByVal FeatureIndex As Integer)
        Try
            Select Case FeatureIndex
                Case 0 Or 1 'Tap and Tap PT
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holetap.bmp")
                Case 2 'Ream
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holereamer.bmp")
                Case 3 'Drill
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holedrill.bmp")
                Case 4 'BlindBore
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holebldbore.bmp")
                Case 5 'ThroughBore
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holethrbore.bmp")
                Case 6 'SunkBolt
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holesnkbolt.bmp")
                Case 7 'Ring
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holering.bmp")
                Case 8 'Boring
                    Me.PictureBox1.Image = System.Drawing.Image.FromFile(FrToolbarApp.ModulePath + "\Images\holeboring.bmp")
            End Select
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    <DllImport("acad.exe", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl, _
               EntryPoint:="?acedSetStatusBarProgressMeter@@YAHPBDHH@Z")> _
        Private Shared Function acedSetStatusBarProgressMeter(ByVal label As String, ByVal minPos As Integer, _
                                                       ByVal maxPos As Integer) As Integer
    End Function


    <DllImport("acad.exe", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl, _
               EntryPoint:="?acedSetStatusBarProgressMeterPos@@YAHH@Z")> _
        Private Shared Function acedSetStatusBarProgressMeterPos(ByVal pos As Integer) As Integer
    End Function

    <DllImport("acad.exe", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.Cdecl, _
               EntryPoint:="?acedRestoreStatusBar@@YAXXZ")> _
    Private Shared Function acedRestoreStatusBar() As Integer
    End Function

    '<CommandMethod("PB")> _
    Private Sub ProgressBar()
        acedSetStatusBarProgressMeter("Testing", 0, 100)

        For i As Integer = 0 To 100
            For j As Integer = 0 To 10
                System.Threading.Thread.Sleep(1)
                acedSetStatusBarProgressMeterPos(i)
                System.Windows.Forms.Application.DoEvents()
            Next
        Next
        acedRestoreStatusBar()
    End Sub

    'Single View Logic
    Public Sub SingleView(ByVal Feat As List(Of OutputFormat))
        Me.ComboBox1.Items.Clear()
        If Feat.Item(0).SolidLineCount = 4 And Feat.Item(0).SolidLineInBoundCount = 2 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 _
               And Feat.Item(0).SequenceSolidBound = True Then
            Me.ComboBox1.Items.Add("Square Slot")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf Feat.Item(0).SolidLineCount = 4 And Feat.Item(0).SolidLineInBoundCount = 2 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 _
              And Feat.Item(0).SequenceSolidBound = False Then
            Me.ComboBox1.Items.Add("2-side Pocket")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf Feat.Item(0).SolidLineCount = 4 And Feat.Item(0).SolidLineInBoundCount = 3 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 Then
            Me.ComboBox1.Items.Add("Square Step")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf Feat.Item(0).SolidLineCount = 3 And Feat.Item(0).SolidLineInBoundCount = 0 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 1 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 Then
            Me.ComboBox1.Items.Add("Square Slot")
        ElseIf Feat.Item(0).SolidLineCount = 2 And Feat.Item(0).SolidLineInBoundCount = 0 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 2 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 Then
            Me.ComboBox1.Items.Add("Square Step")
        ElseIf Feat.Item(0).SolidLineCount = 3 And Feat.Item(0).SolidLineInBoundCount = 3 _
               And Feat.Item(0).HiddenLineCount = 1 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 Then
            Me.ComboBox1.Items.Add("Square Slot")
            Me.ComboBox1.Items.Add("Square Step")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf Feat.Item(0).SolidLineCount = 2 And Feat.Item(0).SolidLineInBoundCount = 2 _
               And Feat.Item(0).HiddenLineCount = 2 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 _
               And Feat.Item(0).SequenceSolidHidden = True Then
            Me.ComboBox1.Items.Add("Square Slot")
            Me.ComboBox1.Items.Add("4-side Pocket")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf Feat.Item(0).SolidLineCount = 2 And Feat.Item(0).SolidLineInBoundCount = 2 _
               And Feat.Item(0).HiddenLineCount = 2 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 _
               And Feat.Item(0).SequenceSolidHidden = False Then
            Me.ComboBox1.Items.Add("3-side Pocket")
            Me.ComboBox1.Items.Add("2-side Pocket")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf (Feat.Item(0).SolidLineCount = 4 And Feat.Item(0).SolidLineInBoundCount = 0 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 4 And Feat.Item(0).HiddenArcCount = 0) _
               Or (Feat.Item(0).SolidLineCount = 0 And Feat.Item(0).SolidLineInBoundCount = 0 _
               And Feat.Item(0).HiddenLineCount = 4 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 4) Then
            Me.ComboBox1.Items.Add("4-side Pocket")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf Feat.Item(0).SolidLineCount = 1 And Feat.Item(0).SolidLineInBoundCount = 1 _
               And Feat.Item(0).HiddenLineCount = 3 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0 Then
            Me.ComboBox1.Items.Add("4-side Pocket")
            Me.ComboBox1.Items.Add("3-side Pocket")
            Me.ComboBox1.Items.Add("Long Hole")
            Me.ComboBox1.Items.Add("Blind Slot")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf (Feat.Item(0).SolidLineCount = 4 And Feat.Item(0).SolidLineInBoundCount = 1 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 2 And Feat.Item(0).HiddenArcCount = 0) _
               Or (Feat.Item(0).SolidLineCount = 1 And Feat.Item(0).SolidLineInBoundCount = 1 _
               And Feat.Item(0).HiddenLineCount = 3 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 2) Then
            Me.ComboBox1.Items.Add("3-side Pocket")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf (Feat.Item(0).SolidLineCount = 4 And Feat.Item(0).SolidLineInBoundCount = 1 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 0) Then
            Me.ComboBox1.Items.Add("3-side Pocket")
            Me.ComboBox1.Items.Add("Blind Slot")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf (Feat.Item(0).SolidLineCount = 4 And Feat.Item(0).SolidLineInBoundCount = 2 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 1 And Feat.Item(0).HiddenArcCount = 0) _
               Or (Feat.Item(0).SolidLineCount = 2 And Feat.Item(0).SolidLineInBoundCount = 2 _
               And Feat.Item(0).HiddenLineCount = 2 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 1) Then
            Me.ComboBox1.Items.Add("2-side Pocket")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf (Feat.Item(0).SolidLineCount = 2 And Feat.Item(0).SolidLineInBoundCount = 0 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 4 And Feat.Item(0).HiddenArcCount = 0) _
               Or (Feat.Item(0).SolidLineCount = 0 And Feat.Item(0).SolidLineInBoundCount = 0 _
               And Feat.Item(0).HiddenLineCount = 2 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 4) Then
            Me.ComboBox1.Items.Add("Long Hole")
            Me.ComboBox1.Items.Add("Nothing")
        ElseIf (Feat.Item(0).SolidLineCount = 3 And Feat.Item(0).SolidLineInBoundCount = 1 _
               And Feat.Item(0).HiddenLineCount = 0 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 2 And Feat.Item(0).HiddenArcCount = 0) _
               Or (Feat.Item(0).SolidLineCount = 1 And Feat.Item(0).SolidLineInBoundCount = 1 _
               And Feat.Item(0).HiddenLineCount = 2 And Feat.Item(0).VirtualLineCount = 0 _
               And Feat.Item(0).SolidArcCount = 0 And Feat.Item(0).HiddenArcCount = 2) Then
            Me.ComboBox1.Items.Add("Blind Slot")
            Me.ComboBox1.Items.Add("Nothing")
        Else
            Me.ComboBox1.Items.Add("Square Slot")
            Me.ComboBox1.Items.Add("Square Step")
            Me.ComboBox1.Items.Add("4-side Pocket")
            Me.ComboBox1.Items.Add("3-side Pocket")
            Me.ComboBox1.Items.Add("2-side Pocket")
            Me.ComboBox1.Items.Add("Long Hole")
            Me.ComboBox1.Items.Add("Blind Slot")
            Me.ComboBox1.Items.Add("Other Feature")
            Me.ComboBox1.Items.Add("Nothing")
        End If
    End Sub

    Public Sub CheckUnidentified(ByVal ListEntity1 As List(Of Entity), ByVal ListEntity2 As List(Of Entity), ByVal FeatureList As List(Of OutputFormat), ByVal TmpFeatureList As List(Of OutputFormat), ByRef FeatureCounter As Integer)
        'remove from unidentified feature list if the feature is listed as unidentified before
        For Each OutputUnident As OutputFormat In FeatureList
            If OutputUnident.ListLoop.Contains(ListEntity1) Or OutputUnident.ListLoop.Contains(ListEntity2) Then
                For j As Integer = 0 To FeatureList.Count - 1
                    If adskClass.myPalette.UnidentifiedFeature.Rows(j).Cells("Name").Value = OutputUnident.FeatureName Then
                        adskClass.myPalette.UnidentifiedFeature.ClearSelection()
                        adskClass.myPalette.UnidentifiedFeature.Rows(j).Selected = True
                        adskClass.myPalette.StartDeleting(adskClass.myPalette.UnidentifiedFeature)
                        FeatureList.Remove(OutputUnident)
                        TmpFeatureList.Remove(OutputUnident)
                        FeatureCounter = FeatureCounter - 1
                        Exit For
                    End If
                Next
                Exit For
            End If
        Next
    End Sub

    Private InitialRowCount As Integer
    Public DeletedRowCount As Integer

    Public Sub DeleteMillCandidate()
        'hitung dulu jumlah row awal
        InitialRowCount = Me.UnidentifiedFeature.Rows.Count

        For i As Integer = 0 To (InitialRowCount - 1)
            'cari untuk setiap row apakah cell name isinya sama dengan mill candidate, jika ya select row tersebut
            If String.Equals(Me.UnidentifiedFeature.Rows(i).Cells("Name").Value, "Mill Candidate") Then
                Me.UnidentifiedFeature.Rows(i).Selected = True
            End If
        Next

        'perintah hapus row yang terseleksi
        StartDeleting(Me.UnidentifiedFeature)

        'hitung jumlah row yang terhapus
        DeletedRowCount = InitialRowCount - Me.UnidentifiedFeature.Rows.Count

    End Sub
End Class





