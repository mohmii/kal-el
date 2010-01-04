Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports FR
Imports System.Linq
Imports System.Data.OleDb

'class for database matching process
Public Class DatabaseConn

    'prepare the some database to be the fr data manager
    Private FRDatabase As New FRDataManager

    ''retrieve collection of top tap hole line type from the fr database
    'Private TopTapLTArray As ICollection(Of TopTapLineType) = FRDatabase.GetAllTopTapLineTypes()

    ''retrieve collection of bottom tap hole line type from the fr database
    'Private BotTapLTArray As ICollection(Of BottomTapLineType) = FRDatabase.GetAllBottomTapLineTypes()

    'retrieve collection of solid line from the fr database
    Private SolidLines As List(Of SolidLine) = New List(Of SolidLine)(FRDatabase.GetAllSolidLines())
    Private TmpSolidLines As List(Of SolidLine) = SolidLines

    'retrieve collection of Hidden line from the fr database
    Private HiddenLines As List(Of HiddenLine) = New List(Of HiddenLine)(FRDatabase.GetAllHiddenLines())
    Private TmpHiddenLine As List(Of HiddenLine) = HiddenLines

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
        Else
            MsgBox("There are no design surface selection that has been made." + vbCrLf + vbCrLf + _
                   "Hint: Please check the FR preferences setting for view, design surface", MsgBoxStyle.Exclamation)
            Return False
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
        SolidLines = TmpSolidLines
        Dim query = (From SolidLine In SolidLines _
            Where (String.Equals(SolidLine.LayerName.ToLower, lineObject.Layer.ToLower) And _
            String.Equals(SolidLine.LayerType.ToLower, ReturnNull(lineObject.Linetype)) And _
            String.Equals(SolidLine.LayerColor.ToLower, lineObject.Color.ColorNameForDisplay.ToLower)) _
            Select SolidLine).ToList()
        SolidLines = query

        If (SolidLines.Count = 0) Then
            Return False
        Else
            Return True
        End If
    End Function

    'check if the entity is a solid type in the database
    Public Function CheckIfEntityHidden(ByVal lineObject As Entity) As Boolean
        HiddenLines = TmpHiddenLine
        Dim query = (From HiddenLine In HiddenLines _
            Where (String.Equals(HiddenLine.LayerName.ToLower, lineObject.Layer.ToLower) And _
            String.Equals(HiddenLine.LayerType.ToLower, ReturnNull(lineObject.Linetype)) And _
            String.Equals(HiddenLine.LayerColor.ToLower, lineObject.Color.ColorNameForDisplay.ToLower)) _
            Select HiddenLine).ToList()
        HiddenLines = query

        If (HiddenLines.Count = 0) Then
            Return False
        Else
            Return True
        End If
    End Function

    'retrieve collection of Hidden line from the fr database
    Private AuxiliaryLines As List(Of AuxiliaryLine) = New List(Of AuxiliaryLine)(FRDatabase.GetAllAuxiliaryLines())
    Private TmpAuxiliaryLine As List(Of AuxiliaryLine) = AuxiliaryLines

    'retrieve collection of ream hole from the fr database
    Private ReamData As List(Of ReamHole) = New List(Of ReamHole)(FRDatabase.GetAllReamHoles())
    Private TmpReamData As List(Of ReamHole) = ReamData

    'create list from cbore data in database
    Private CBoreData As List(Of CounterBore) = New List(Of CounterBore)(FRDatabase.GetAllCounterBores())
    Private TmpCBoreData As List(Of CounterBore) = CBoreData

    'create list from tap data in database
    Private TapData As List(Of Tap) = New List(Of Tap)(FRDatabase.GetAllTaps())
    Private TmpTapData As List(Of Tap) = TapData

    'create list from top tap data to be use in querying
    Private TopTap As List(Of TopTapLineType) = New List(Of TopTapLineType)(FRDatabase.GetAllTopTapLineTypes())
    Private TmpTopTap As List(Of TopTapLineType) = TopTap

    'create list from bottom tap data to be use in querying
    Private BottomTap As List(Of BottomTapLineType) = New List(Of BottomTapLineType)(FRDatabase.GetAllBottomTapLineTypes())
    Private TmpBottomTap As List(Of BottomTapLineType) = BottomTap

    'checking the Top Tap Table
    Private Function CheckTopTap(ByVal Result As IEnumerable(Of Circle)) As Boolean
        If Result.Last.Radius > Result.First.Radius Then
            TopTap = TmpTopTap
            Dim query = (From TopTapItem In TopTap _
                        Where (String.Equals(TopTapItem.TapLineName.ToLower, Result.Last.Layer.ToLower) And _
                               String.Equals(TopTapItem.TapLineColor.ToLower, Result.Last.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.TapLineType.ToLower, ReturnNull(Result.Last.Linetype)) And _
                               String.Equals(TopTapItem.UnHoleLineName.ToLower, Result.First.Layer.ToLower) And _
                               String.Equals(TopTapItem.UnHoleLineColor.ToLower, Result.First.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.UnHoleLineType.ToLower, ReturnNull(Result.First.Linetype))) _
                        Select TopTapItem).ToList()
            TopTap = query

            If TopTap.Count = 0 Then
                Return False
            Else
                Return True
            End If
        Else
            TopTap = TmpTopTap
            Dim query = (From TopTapItem In TopTap _
                        Where (String.Equals(TopTapItem.TapLineName.ToLower, Result.First.Layer.ToLower) And _
                               String.Equals(TopTapItem.TapLineColor.ToLower, Result.First.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.TapLineType.ToLower, ReturnNull(Result.First.Linetype)) And _
                               String.Equals(TopTapItem.UnHoleLineName, Result.Last.Layer) And _
                               String.Equals(TopTapItem.UnHoleLineColor.ToLower, Result.Last.Color.ColorNameForDisplay.ToLower) And _
                               String.Equals(TopTapItem.UnHoleLineType.ToLower, ReturnNull(Result.Last.Linetype))) _
                        Select TopTapItem).ToList()
            TopTap = query

            If TopTap.Count = 0 Then
                Return False
            Else
                Return True
            End If
        End If
    End Function

    'checking the Bottom Tap Table
    Private Function CheckBottomTap(ByVal Result As IEnumerable(Of Circle)) As Boolean
        If Result.Last.Radius > Result.First.Radius Then
            BottomTap = TmpBottomTap
            Dim query = (From BottomTapItem In BottomTap _
                        Where (String.Equals(BottomTapItem.TapLineName.ToLower, (Result.Last.Layer.ToLower)) And _
                               String.Equals(BottomTapItem.TapLineColor.ToLower, (Result.Last.Color.ColorNameForDisplay.ToLower)) And _
                               String.Equals(BottomTapItem.TapLineType.ToLower, ReturnNull(Result.Last.Linetype)) And _
                               String.Equals(BottomTapItem.UnHoleLineName.ToLower, (Result.First.Layer.ToLower)) And _
                               String.Equals(BottomTapItem.UnHoleLineColor.ToLower, (Result.First.Color.ColorNameForDisplay.ToLower)) And _
                               String.Equals(BottomTapItem.UnHoleLineType.ToLower, ReturnNull(Result.First.Linetype))) _
                        Select BottomTapItem).ToList()
            BottomTap = query

            If BottomTap.Count = 0 Then
                Return False
            Else
                Return True
            End If
        Else
            BottomTap = TmpBottomTap
            Dim query = (From BottomTapItem In BottomTap _
                        Where (String.Equals(BottomTapItem.TapLineName, Result.First.Layer) And _
                               String.Equals(BottomTapItem.TapLineColor.ToLower, Result.First.Color.ColorNameForDisplay) And _
                               String.Equals(BottomTapItem.TapLineType.ToLower, ReturnNull(Result.First.Linetype)) And _
                               String.Equals(BottomTapItem.UnHoleLineName, Result.Last.Layer) And _
                               String.Equals(BottomTapItem.UnHoleLineColor.ToLower, Result.Last.Color.ColorNameForDisplay) And _
                               String.Equals(BottomTapItem.UnHoleLineType.ToLower, ReturnNull(Result.Last.Linetype))) _
                        Select BottomTapItem).ToList()
            BottomTap = query

            If BottomTap.Count = 0 Then
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
        ReamData = TmpReamData
        Dim query = (From ReamItem In ReamData _
            Where (isequal(ReamItem.Diameter, (Result.SingleOrDefault.Radius * 2)) = True) _
            Select ReamItem).ToList()
        ReamData = query

        If (ReamData.Count = 0) Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function WhichReam(ByVal Result As IEnumerable(Of Circle)) As String()
        ReamData = TmpReamData
        Dim query = (From ReamItem In ReamData _
            Where (isequal(ReamItem.Diameter, (Result.SingleOrDefault.Radius * 2)) = True) _
            Select ReamItem).ToList()
        ReamData = query

        If (ReamData.Count = 0) Then
            Return Nothing
        Else
            Return New String() {"Ream", Result.FirstOrDefault.Id.ToString, ReamData.FirstOrDefault.Diameter.ToString, _
                                 ReamData.FirstOrDefault.Depth.ToString}
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
            TapData = TmpTapData
            Dim query = (From TapItem In TapData _
                        Where (isequal(TapItem.TapDiameter, (Result.Last.Radius * 2)) = True And _
                        isequal(TapItem.UnHoleDiameter, (Result.First.Radius * 2)) = True) _
                        Select TapItem).ToList()
            TapData = query

            If TapData.Count = 0 Then
                'if there is no match, continue searching in cbore database
                CBoreData = TmpCBoreData
                Dim nextquery = (From CBoreItem In CBoreData _
                        Where (isequal(CBoreItem.Diameter, (Result.Last.Radius * 2)) = True And _
                        isequal(CBoreItem.InnerDiameter, (Result.First.Radius * 2)) = True) _
                        Select CBoreItem).ToList()
                CBoreData = nextquery

                If CBoreData.Count = 0 Then
                    'if there is no match also in cbore output the function to nothing
                    Return Nothing
                Else
                    'If there is, tell what type of cbore it is
                    Return New String() {("SunkBolt, " + CBoreData.SingleOrDefault.Name), Result.First.Id.ToString(), Result.Last.Id.ToString(), _
                                         CBoreData.SingleOrDefault.InnerDiameter.ToString, "0", CBoreData.SingleOrDefault.Diameter.ToString, _
                                         CBoreData.SingleOrDefault.Depth.ToString}
                End If
            Else
                'if there is, tell what type of tap it is
                Return New String() {("Tap, " + TapData.SingleOrDefault.Name), Result.First.Id.ToString(), Result.Last.Id.ToString(), _
                                     TapData.SingleOrDefault.TapDiameter.ToString, TapData.SingleOrDefault.TapDepth.ToString, _
                                     TapData.SingleOrDefault.UnHoleDiameter.ToString, TapData.SingleOrDefault.UnHoleDepth.ToString}
            End If
        Else
            'from this to next were the same process like the previous process
            TapData = TmpTapData
            Dim query = (From TapItem In TapData _
                        Where (isequal(TapItem.TapDiameter, (Result.First.Radius * 2)) = True And _
                        isequal(TapItem.UnHoleDiameter, (Result.Last.Radius * 2)) = True) _
                        Select TapItem).ToList()

            TapData = query

            If TapData.Count = 0 Then

                CBoreData = TmpCBoreData
                Dim nextquery = (From CBoreItem In CBoreData _
                        Where (isequal(CBoreItem.Diameter, (Result.First.Radius * 2)) = True And _
                        isequal(CBoreItem.InnerDiameter, (Result.Last.Radius * 2)) = True) _
                        Select CBoreItem).ToList()

                CBoreData = nextquery

                If CBoreData.Count = 0 Then
                    Return Nothing
                Else
                    Return New String() {("SunkBolt, " + CBoreData.SingleOrDefault.Name), Result.Last.Id.ToString(), Result.First.Id.ToString(), _
                                         CBoreData.SingleOrDefault.InnerDiameter.ToString, "0", CBoreData.SingleOrDefault.Diameter.ToString, _
                                         CBoreData.SingleOrDefault.Depth.ToString}
                End If
            Else
                Return New String() {("Tap, " + TapData.SingleOrDefault.Name), Result.Last.Id.ToString(), Result.First.Id.ToString(), _
                                     TapData.SingleOrDefault.TapDiameter.ToString, TapData.SingleOrDefault.TapDepth.ToString, _
                                     TapData.SingleOrDefault.UnHoleDiameter.ToString, TapData.SingleOrDefault.UnHoleDepth.ToString}
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
