Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports FR
Imports System.Linq
Imports System.Data.OleDb

'class for database matching process
Public Class DatabaseConn

    'prepare the some database to be the fr data manager
    Private FRDatabase As New FRDataManager()
    Private SolidLines, TmpSolidLines As List(Of SolidLine)
    Private HiddenLines, TmpHiddenLines As List(Of HiddenLine)
    Private AuxiliaryLines, TmpAuxiliaryLines As List(Of AuxiliaryLine)
    Private TopTap, TmpTopTap As List(Of TopTapLineType)
    Private BottomTap, TmpBottomTap As List(Of BottomTapLineType)
    Private ReamData, TmpReamData As List(Of ReamHole)
    Private CBoreData, TmpCBoreData As List(Of CounterBore)
    Private TapData, TmpTapData As List(Of Tap)

    'get all line properties in the database
    Public Sub InitLinesDb()
        'get drawing line database
        SolidLines = New List(Of SolidLine)(FRDatabase.GetAllSolidLines())
        HiddenLines = New List(Of HiddenLine)(FRDatabase.GetAllHiddenLines())
        AuxiliaryLines = New List(Of AuxiliaryLine)(FRDatabase.GetAllAuxiliaryLines())

        'get schematic line database
        TopTap = New List(Of TopTapLineType)(FRDatabase.GetAllTopTapLineTypes())
        BottomTap = New List(Of BottomTapLineType)(FRDatabase.GetAllBottomTapLineTypes())
    End Sub

    'get all hole features in the database
    Public Sub InitHoleDb()

        TapData = New List(Of Tap)(FRDatabase.GetAllTaps())
        ReamData = New List(Of ReamHole)(FRDatabase.GetAllReamHoles())
        CBoreData = New List(Of CounterBore)(FRDatabase.GetAllCounterBores())

    End Sub

    'get the solid line
    Public ReadOnly Property GetSolidLine()
        Get
            Return SolidLines
        End Get
    End Property

    'get the hidden line
    Public ReadOnly Property GetHiddenLine()
        Get
            Return HiddenLines
        End Get
    End Property

    'get the auxiliary line
    Public ReadOnly Property GetAuxiliaryLine()
        Get
            Return AuxiliaryLines
        End Get
    End Property

    'get the top tap line
    Public ReadOnly Property GetTopTapLine()
        Get
            Return TopTap
        End Get
    End Property

    'get the bottom tap line
    Public ReadOnly Property GetBottomTapLine()
        Get
            Return BottomTap
        End Get
    End Property

    'get the tap hole
    Public ReadOnly Property GetTapHole()
        Get
            Return TapData
        End Get
    End Property

    'get the ream hole
    Public ReadOnly Property GetReamHole()
        Get
            Return ReamData
        End Get
    End Property

    'get the counter bore hole
    Public ReadOnly Property GetCboreHole()
        Get
            Return CBoreData
        End Get
    End Property

    Private SL As SolidLine
    Private HL As HiddenLine
    Private AL As AuxiliaryLine
    Private TTL As TopTapLineType
    Private BTL As BottomTapLineType

    Public Sub AddToSolidLineDatabase(ByVal TableRow As System.Windows.Forms.DataGridViewRow)
        SL = New SolidLine
        SL.LayerName = TableRow.Cells("Layer").Value
        SL.LayerType = TableRow.Cells("Linetype").Value
        SL.LayerColor = TableRow.Cells("Color").Value
        FRDatabase.AddSolidLine(SL)
        'SL.LayerName = Ent.Layer.ToString
        'SL.LayerType = Ent.Linetype.ToString
        'SL.LayerColor = Ent.Color.ToString
    End Sub

    Public Sub AddToHiddenLineDatabase(ByVal TableRow As System.Windows.Forms.DataGridViewRow)
        HL = New HiddenLine
        HL.LayerName = TableRow.Cells("Layer").Value
        HL.LayerType = TableRow.Cells("Linetype").Value
        HL.LayerColor = TableRow.Cells("Color").Value
        FRDatabase.AddHiddenLine(HL)
    End Sub

    Public Sub AddToAuxiliaryLineDatabase(ByVal TableRow As System.Windows.Forms.DataGridViewRow)
        AL = New AuxiliaryLine
        AL.LayerName = TableRow.Cells("Layer").Value
        AL.LayerType = TableRow.Cells("Linetype").Value
        AL.LayerColor = TableRow.Cells("Color").Value
        FRDatabase.AddAuxiliaryLine(AL)
    End Sub

    Public Sub AddToTopTapLineDatabase(ByVal TableRow As System.Windows.Forms.DataGridViewRow)
        TTL = New TopTapLineType
        TTL.TapLineName = TableRow.Cells("HoleLayer").Value
        TTL.TapLineType = TableRow.Cells("HoleLineType").Value
        TTL.TapLineColor = TableRow.Cells("HoleColor").Value
        TTL.UnHoleLineName = TableRow.Cells("UnderholeLayer").Value
        TTL.UnHoleLineType = TableRow.Cells("UnderholeLineType").Value
        TTL.UnHoleLineColor = TableRow.Cells("UnderholeColor").Value
        FRDatabase.AddTopTapLineType(TTL)
        'TTL.TapLineName = CircleGroup(0).Layer.ToString
        'TTL.TapLineType = CircleGroup(0).Linetype.ToString
        'TTL.TapLineColor = CircleGroup(0).Color.ToString
        'TTL.UnHoleLineName = CircleGroup(1).Layer.ToString
        'TTL.UnHoleLineType = CircleGroup(1).Linetype.ToString
        'TTL.UnHoleLineColor = CircleGroup(1).Color.ToString
    End Sub

    Public Sub AddToBottomTapLineDatabase(ByVal TableRow As System.Windows.Forms.DataGridViewRow)
        BTL = New BottomTapLineType
        BTL.TapLineName = TableRow.Cells("HoleLayer").Value
        BTL.TapLineType = TableRow.Cells("HoleLineType").Value
        BTL.TapLineColor = TableRow.Cells("HoleColor").Value
        BTL.UnHoleLineName = TableRow.Cells("UnderholeLayer").Value
        BTL.UnHoleLineType = TableRow.Cells("UnderholeLineType").Value
        BTL.UnHoleLineColor = TableRow.Cells("UnderholeColor").Value
        FRDatabase.AddBottomTapLineType(BTL)
    End Sub

    Private Function ReturnNull(ByVal IsByLayer As String) As String
        If IsByLayer.ToLower.Equals("bylayer") Then
            Return "null"
        Else
            Return IsByLayer.ToLower
        End If

    End Function

    'check if the entity is one of the entity defined in the database
    Public Function CheckIfEntity(ByVal LineObject As Entity) As Boolean
        If setView.CBVisible = True And setView.CBHidden = False Then
            If CheckIfEntitySolid(LineObject) = True Then
                Return True
            Else
                Return False
            End If
        ElseIf setView.CBVisible = False And setView.CBHidden = True Then
            If CheckIfEntityHidden(LineObject) = True Then
                Return True
            Else
                Return False
            End If
        ElseIf setView.CBVisible = True And setView.CBHidden = True Then
            If CheckIfEntitySolid(LineObject) = True Or CheckIfEntityHidden(LineObject) = True Then
                Return True
            Else
                Return False
            End If
            'Else
            '    MsgBox("There are no design surface selection that has been made." + vbCrLf + vbCrLf + _
            '           "Hint: Please check the FR preferences setting for view, design surface", MsgBoxStyle.Exclamation)
            '    Return False
        End If

        'SolidLines = TmpSolidLines
        'Dim query = (From SolidLine In SolidLines _
        '    Where (String.Equals(SolidLine.LayerName.ToLower, LineObject.Layer.ToLower) And _
        '    String.Equals(SolidLine.LayerType.ToLower, ReturnNull(LineObject.Linetype)) And _
        '    String.Equals(SolidLine.LayerColor.ToLower, LineObject.Color.ColorNameForDisplay.ToLower)) _
        '    Select SolidLine).ToList()
        'SolidLines = query

        'If (SolidLines.Count = 0) Then
        '    HiddenLines = TmpHiddenLine
        '    Dim nextquery = (From HiddenLine In HiddenLines _
        '        Where (String.Equals(HiddenLine.LayerName.ToLower, LineObject.Layer.ToLower) And _
        '        String.Equals(HiddenLine.LayerType.ToLower, ReturnNull(LineObject.Linetype)) And _
        '        String.Equals(HiddenLine.LayerColor.ToLower, LineObject.Color.ColorNameForDisplay.ToLower)) _
        '        Select HiddenLine).ToList()
        '    HiddenLines = nextquery

        '    If (HiddenLines.Count = 0) Then
        '        Return False
        '    Else
        '        Return True
        '    End If
        'Else
        '    Return True
        'End If
    End Function

    'check if the entity is a solid type in the database
    Public Function CheckIfEntitySolid(ByVal lineObject As Entity) As Boolean

        Dim query = (From SolidLine In SolidLines _
            Where (String.Equals(SolidLine.LayerName.ToLower, lineObject.Layer.ToLower) And _
            String.Equals(SolidLine.LayerType.ToLower, ReturnNull(lineObject.Linetype)) And _
            String.Equals(SolidLine.LayerColor.ToLower, lineObject.Color.ColorNameForDisplay.ToLower)) _
            Select SolidLine).ToList()

        TmpSolidLines = query

        If (TmpSolidLines.Count = 0) Then
            Return False
        Else
            Return True
        End If
    End Function

    'check if the entity is a hidden type in the database
    Public Function CheckIfEntityHidden(ByVal lineObject As Entity) As Boolean

        Dim query = (From HiddenLine In HiddenLines _
            Where (String.Equals(HiddenLine.LayerName.ToLower, lineObject.Layer.ToLower) And _
            String.Equals(HiddenLine.LayerType.ToLower, ReturnNull(lineObject.Linetype)) And _
            String.Equals(HiddenLine.LayerColor.ToLower, lineObject.Color.ColorNameForDisplay.ToLower)) _
            Select HiddenLine).ToList()

        TmpHiddenLines = query

        If (TmpHiddenLines.Count = 0) Then
            Return False
        Else
            Return True
        End If
    End Function

    'check if the entity is a auxiliary type in the database
    Public Function CheckIfEntityAuxiliary(ByVal lineObject As Entity) As Boolean

        Dim query = (From AuxiliaryLine In AuxiliaryLines _
            Where (String.Equals(AuxiliaryLine.LayerName.ToLower, lineObject.Layer.ToLower) And _
            String.Equals(AuxiliaryLine.LayerType.ToLower, ReturnNull(lineObject.Linetype)) And _
            String.Equals(AuxiliaryLine.LayerColor.ToLower, lineObject.Color.ColorNameForDisplay.ToLower)) _
            Select AuxiliaryLine).ToList()

        TmpAuxiliaryLines = query

        If (TmpAuxiliaryLines.Count = 0) Then
            Return False
        Else
            Return True
        End If
    End Function



    'checking the Top Tap Table
    Public Function CheckTopTap(ByVal Result As IEnumerable(Of Circle)) As Boolean
        If Result.Last.Radius > Result.First.Radius Then

            Dim query = (From TopTapItem In TopTap _
                        Where (String.Equals(TopTapItem.TapLineName.ToLower, Result.Last.Layer.ToLower) And _
                               String.Equals(TopTapItem.TapLineColor.ToLower, Result.Last.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.TapLineType.ToLower, ReturnNull(Result.Last.Linetype)) And _
                               String.Equals(TopTapItem.UnHoleLineName.ToLower, Result.First.Layer.ToLower) And _
                               String.Equals(TopTapItem.UnHoleLineColor.ToLower, Result.First.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.UnHoleLineType.ToLower, ReturnNull(Result.First.Linetype))) _
                        Select TopTapItem).ToList()
            TmpTopTap = query

            If TmpTopTap.Count = 0 Then
                Return False
            Else
                Return True
            End If
        Else

            Dim query = (From TopTapItem In TopTap _
                        Where (String.Equals(TopTapItem.TapLineName.ToLower, Result.First.Layer.ToLower) And _
                               String.Equals(TopTapItem.TapLineColor.ToLower, Result.First.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.TapLineType.ToLower, ReturnNull(Result.First.Linetype)) And _
                               String.Equals(TopTapItem.UnHoleLineName, Result.Last.Layer) And _
                               String.Equals(TopTapItem.UnHoleLineColor.ToLower, Result.Last.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.UnHoleLineType.ToLower, ReturnNull(Result.Last.Linetype))) _
                        Select TopTapItem).ToList()
            TmpTopTap = query

            If TmpTopTap.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End If
    End Function

    'checking the Bottom Tap Table
    Public Function CheckBottomTap(ByVal Result As IEnumerable(Of Circle)) As Boolean
        If Result.Last.Radius > Result.First.Radius Then

            Dim query = (From BottomTapItem In BottomTap _
                        Where (String.Equals(BottomTapItem.TapLineName.ToLower, (Result.Last.Layer.ToLower)) And _
                               String.Equals(BottomTapItem.TapLineColor.ToLower, (Result.Last.Color.ColorNameForDisplay.ToLower)) And _
                               String.Equals(BottomTapItem.TapLineType.ToLower, ReturnNull(Result.Last.Linetype)) And _
                               String.Equals(BottomTapItem.UnHoleLineName.ToLower, (Result.First.Layer.ToLower)) And _
                               String.Equals(BottomTapItem.UnHoleLineColor.ToLower, (Result.First.Color.ColorNameForDisplay.ToLower)) And _
                               String.Equals(BottomTapItem.UnHoleLineType.ToLower, ReturnNull(Result.First.Linetype))) _
                        Select BottomTapItem).ToList()
            TmpBottomTap = query

            If TmpBottomTap.Count = 0 Then
                Return False
            Else
                Return True
            End If
        Else

            Dim query = (From BottomTapItem In BottomTap _
                        Where (String.Equals(BottomTapItem.TapLineName, Result.First.Layer) And _
                               String.Equals(BottomTapItem.TapLineColor.ToLower, Result.First.Color.ColorNameForDisplay) And _
                               String.Equals(BottomTapItem.TapLineType.ToLower, ReturnNull(Result.First.Linetype)) And _
                               String.Equals(BottomTapItem.UnHoleLineName, Result.Last.Layer) And _
                               String.Equals(BottomTapItem.UnHoleLineColor.ToLower, Result.Last.Color.ColorNameForDisplay) And _
                               String.Equals(BottomTapItem.UnHoleLineType.ToLower, ReturnNull(Result.Last.Linetype))) _
                        Select BottomTapItem).ToList()
            TmpBottomTap = query

            If TmpBottomTap.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End If
    End Function

    'function for checking the floating point variabel
    Private Function isequal(ByVal x As Double, ByVal y As Double) As Boolean
        If Math.Abs(x - y) > 0.1 Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function CheckIfReam(ByVal Result As IEnumerable(Of Circle)) As Boolean

        Dim query = (From ReamItem In ReamData _
            Where (isequal(ReamItem.Diameter, (Result.SingleOrDefault.Radius * 2)) = True) _
            Select ReamItem).ToList()
        TmpReamData = query

        If (TmpReamData.Count = 0) Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function WhichReam(ByVal Result As IEnumerable(Of Circle)) As String()

        Dim query = (From ReamItem In ReamData _
            Where (isequal(ReamItem.Diameter, (Result.SingleOrDefault.Radius * 2)) = True) _
            Select ReamItem).ToList()
        TmpReamData = query

        If (TmpReamData.Count = 0) Then
            Return Nothing
        Else
            Return New String() {"Ream, R-" + TmpReamData.SingleOrDefault.Diameter.ToString, Result.FirstOrDefault.Id.ToString, _
                                 TmpReamData.FirstOrDefault.Diameter.ToString, TmpReamData.FirstOrDefault.Depth.ToString}
        End If

    End Function

    Public Function CheckDoubleHole(ByVal Result As IEnumerable(Of Circle), ByRef Surface As Integer) As String()

        If setView.CBVisible = True And setView.CBHidden = False Then
            If CheckTopTap(Result) = True Then
                Surface = 1
                Return GetHoleDimension(Result)
            Else
                Return Nothing
            End If
        ElseIf setView.CBVisible = False And setView.CBHidden = True Then
            If CheckBottomTap(Result) = True Then
                Surface = 2
                Return GetHoleDimension(Result)
            Else
                Return Nothing
            End If
        ElseIf setView.CBVisible = True And setView.CBHidden = True Then
            If CheckTopTap(Result) = True Then
                Surface = 1
                Return GetHoleDimension(Result)
            ElseIf CheckBottomTap(Result) = True Then
                Surface = 2
                Return GetHoleDimension(Result)
            Else
                Return Nothing
            End If
        Else
            MsgBox("There are no design surface selection that has been made." + vbCrLf + vbCrLf + _
                   "Hint: Please check the FR preferences setting for view, design surface", MsgBoxStyle.Exclamation)
            Return Nothing
        End If
    End Function

    'crosscheck the tap name in database by their outside diameter
    Public Function GetHoleDimension(ByVal Result As IEnumerable(Of Circle)) As String()

        'check where is the bigger diameter post in the group: it is the first element or _
        'the last element (the group will be always considered had two element)
        If Result.Last.Radius > Result.First.Radius Then

            'checking in tap database
            Dim query = (From TapItem In TapData _
                        Where (isequal(TapItem.TapDiameter, (Result.Last.Radius * 2)) = True And _
                        isequal(TapItem.UnHoleDiameter, (Result.First.Radius * 2)) = True) _
                        Select TapItem).ToList()
            TmpTapData = query

            If TmpTapData.Count = 0 Then
                'if there is no match, continue searching in cbore database
                Dim nextquery = (From CBoreItem In CBoreData _
                        Where (isequal(CBoreItem.Diameter, (Result.Last.Radius * 2)) = True And _
                        isequal(CBoreItem.InnerDiameter, (Result.First.Radius * 2)) = True) _
                        Select CBoreItem).ToList()
                TmpCBoreData = nextquery

                If TmpCBoreData.Count = 0 Then
                    'if there is no match also in cbore output the function to nothing
                    Return Nothing
                Else
                    'If there is, tell what type of cbore it is
                    Return New String() {("SunkBolt, " + TmpCBoreData.SingleOrDefault.Name), Result.First.Id.ToString(), Result.Last.Id.ToString(), _
                                         TmpCBoreData.SingleOrDefault.InnerDiameter.ToString, "0", TmpCBoreData.SingleOrDefault.Diameter.ToString, _
                                         TmpCBoreData.SingleOrDefault.Depth.ToString}
                End If
            Else
                'if there is, tell what type of tap it is
                Return New String() {("Tap, " + TmpTapData.SingleOrDefault.Name), Result.First.Id.ToString(), Result.Last.Id.ToString(), _
                                     TmpTapData.SingleOrDefault.TapDiameter.ToString, TmpTapData.SingleOrDefault.TapDepth.ToString, _
                                     TmpTapData.SingleOrDefault.UnHoleDiameter.ToString, TmpTapData.SingleOrDefault.UnHoleDepth.ToString}
            End If
        Else
            'from this to next were the same process like the previous process
            Dim query = (From TapItem In TapData _
                        Where (isequal(TapItem.TapDiameter, (Result.First.Radius * 2)) = True And _
                        isequal(TapItem.UnHoleDiameter, (Result.Last.Radius * 2)) = True) _
                        Select TapItem).ToList()

            TmpTapData = query

            If TmpTapData.Count = 0 Then

                Dim nextquery = (From CBoreItem In CBoreData _
                        Where (isequal(CBoreItem.Diameter, (Result.First.Radius * 2)) = True And _
                        isequal(CBoreItem.InnerDiameter, (Result.Last.Radius * 2)) = True) _
                        Select CBoreItem).ToList()

                TmpCBoreData = nextquery

                If TmpCBoreData.Count = 0 Then
                    Return Nothing
                Else
                    Return New String() {("SunkBolt, " + TmpCBoreData.SingleOrDefault.Name), Result.Last.Id.ToString(), Result.First.Id.ToString(), _
                                         TmpCBoreData.SingleOrDefault.InnerDiameter.ToString, "0", TmpCBoreData.SingleOrDefault.Diameter.ToString, _
                                         TmpCBoreData.SingleOrDefault.Depth.ToString}
                End If
            Else
                Return New String() {("Tap, " + TmpTapData.SingleOrDefault.Name), Result.Last.Id.ToString(), Result.First.Id.ToString(), _
                                     TmpTapData.SingleOrDefault.TapDiameter.ToString, TmpTapData.SingleOrDefault.TapDepth.ToString, _
                                     TmpTapData.SingleOrDefault.UnHoleDiameter.ToString, TmpTapData.SingleOrDefault.UnHoleDepth.ToString}
            End If
        End If
    End Function

    Public myConn As New OleDbConnection
    Public myCmd As New OleDbCommand
    Public myDA As New OleDbDataAdapter
    Public myDR As OleDbDataReader
    Dim ClassFR As New FR.FRConnectionStringManager

    Function IsConnected() As Boolean
        Dim Db As String
        Db = ClassFR.GetConnectionString
        Try
            'Checks first if already connected to database,if connected, it will be disconnected.
            If myConn.State = ConnectionState.Open Then myConn.Close()
            myConn.ConnectionString = Db
            myConn.Open()
            IsConnected = True
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Function
End Class
