Imports System.IO
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.ApplicationServices
Imports System.Math

'create text files for further processing in PCAD/CAM
Public Class GenerateFL

    Private counter As Integer

    'setting the 3 digits decimals
    Private Function SetDot(ByVal s As String) As String
        If s.Contains(".") Then
            Return s
        Else
            Return (s + ".000")
        End If
    End Function

    'return the projection view code
    Private Function ViewIndex(ByVal s As String) As Integer
        Select Case s.ToLower
            Case "top" : Return 1
            Case "bottom" : Return 2
            Case "right" : Return 3
            Case "left" : Return 4
            Case "front" : Return 5
            Case "back" : Return 6
        End Select
    End Function

    Private CodeName As String

    'return the feature tag name
    Private Function FindCodeName(ByVal s As OutputFormat) As String
        CodeName = ""
        If s.MiscProp(0).Contains("ＰＴタップ穴") Then
            CodeName = "TAPPT"
        End If

        If s.MiscProp(0).Contains("タップ穴") Then
            CodeName = "TAP"
        End If

        If s.MiscProp(0).Contains("リーマ穴") Then
            CodeName = "REAMER"
        End If

        If s.MiscProp(0).Contains("ドリル穴") Then
            CodeName = "DRILL"
        End If

        If s.MiscProp(0).Contains("段付きボルト穴") Then
            CodeName = "SNKBOLT"
        End If

        If s.MiscProp(0).Contains("底付き穴") Then
            CodeName = "BLDBORE"
        End If

        If s.MiscProp(0).Contains("貫通穴") Then
            CodeName = "THRBORE"
        End If

        If s.MiscProp(0).Contains("円形溝") Then
            CodeName = "RING"
        End If

        If s.MiscProp(0).Contains("ボーリング穴") Then
            CodeName = "BORING"
        End If

        If s.MiscProp(0).Contains("Square Slot") Then
            CodeName = "SQRSLOT"
        End If

        If s.MiscProp(0).Contains("Square Step") Then
            CodeName = "SQRSTEP"
        End If

        If s.MiscProp(0).Contains("4-side Pocket") Then
            CodeName = "4SPOCKET"
        End If

        If s.MiscProp(0).Contains("3-side Pocket") Then
            CodeName = "3SPOCKET"
        End If

        If s.MiscProp(0).Contains("2-side Pocket") Then
            CodeName = "2SPOCKET"
        End If

        If s.MiscProp(0).Contains("Long Hole") Then
            CodeName = "LNGHOLE"
        End If

        If s.MiscProp(0).Contains("Blind Slot") Then
            CodeName = "BLDSLOT"
        End If

        Return CodeName
    End Function

    Private ViewNum As Integer
    Private Status As Boolean

    Public ProdSize() As Double

    Public Property SetProdSize(ByVal index As Integer) As Double
        Get
            Return ProdSize(index)
        End Get
        Set(ByVal value As Double)
            ProdSize(index) = value
        End Set
    End Property

    Private Check As Integer
    Private SetProd As ProductSize

    Public Sub GenProdSize(ByVal fw As StreamWriter, ByVal ReadyFL As List(Of ViewProp), ByRef Status As Boolean)
        Try
            ProdSize = New Double() {0, 0, 0}
            For Each view As ViewProp In ReadyFL
                If view.ViewType.Contains("TOP") Or view.ViewType.Contains("BOTTOM") Then
                    If ProdSize(0) < view.BoundProp(4) Then
                        ProdSize(0) = view.BoundProp(4)
                    End If

                    If ProdSize(1) < view.BoundProp(5) Then
                        ProdSize(1) = view.BoundProp(5)
                    End If
                End If

                If view.ViewType.Contains("FRONT") Or view.ViewType.Contains("BACK") Then
                    If ProdSize(0) < view.BoundProp(4) Then
                        ProdSize(0) = view.BoundProp(4)
                    End If

                    If ProdSize(2) < view.BoundProp(5) Then
                        ProdSize(2) = view.BoundProp(5)
                    End If
                End If

                If view.ViewType.Contains("LEFT") Or view.ViewType.Contains("RIGHT") Then
                    If ProdSize(1) < view.BoundProp(4) Then
                        ProdSize(1) = view.BoundProp(4)
                    End If

                    If ProdSize(2) < view.BoundProp(5) Then
                        ProdSize(2) = view.BoundProp(5)
                    End If
                End If
            Next

            Check = 4
            Status = True

            For i As Integer = 0 To 2
                If ProdSize(i) = 0 Then
                    Check = i
                End If
            Next

            If Not (Check = 4) Then
                Select Case Check
                    Case 0 : SetProd = New ProductSize
                        'shows the dialog box for insert X
                        Using SetProd
                            SetProd.Length1.Enabled = True
                            SetProd.Width1.Enabled = False
                            SetProd.Height1.Enabled = False
                            SetProd.Length1.Value = ProdSize(0)
                            SetProd.Width1.Value = ProdSize(1)
                            SetProd.Height1.Value = ProdSize(2)
                            SetProd.ShowDialog()

                            If SetProd.DialogResult = Forms.DialogResult.OK And SetProd.Length1.Value <> 0 _
                            And SetProd.Width1.Value <> 0 And SetProd.Height1.Value <> 0 Then
                                ProdSize(0) = SetProd.Length1.Value
                                Status = True
                            ElseIf SetProd.DialogResult = Forms.DialogResult.Cancel Then
                                Status = False
                                Exit Sub
                            End If

                        End Using
                    Case 1 : SetProd = New ProductSize
                        'shows the dialog box for insert Y
                        Using SetProd

                            SetProd.Length1.Enabled = False
                            SetProd.Width1.Enabled = True
                            SetProd.Height1.Enabled = False
                            SetProd.Length1.Value = ProdSize(0)
                            SetProd.Width1.Value = ProdSize(1)
                            SetProd.Height1.Value = ProdSize(2)
                            SetProd.ShowDialog()

                            If SetProd.DialogResult = Forms.DialogResult.OK And SetProd.Length1.Value <> 0 _
                            And SetProd.Width1.Value <> 0 And SetProd.Height1.Value <> 0 Then
                                ProdSize(1) = SetProd.Width1.Value
                                Status = True
                            ElseIf SetProd.DialogResult = Forms.DialogResult.Cancel Then
                                Status = False
                                Exit Sub
                            End If

                        End Using
                    Case 2 : SetProd = New ProductSize
                        'shows the dialog box for insert Z
                        Using SetProd

                            SetProd.Length1.Enabled = False
                            SetProd.Width1.Enabled = False
                            SetProd.Height1.Enabled = True
                            SetProd.Length1.Value = ProdSize(0)
                            SetProd.Width1.Value = ProdSize(1)
                            SetProd.Height1.Value = ProdSize(2)
                            SetProd.ShowDialog()

                            If SetProd.DialogResult = Forms.DialogResult.OK And SetProd.Length1.Value <> 0 _
                           And SetProd.Width1.Value <> 0 And SetProd.Height1.Value <> 0 Then
                                ProdSize(2) = SetProd.Height1.Value
                                Status = True
                            ElseIf SetProd.DialogResult = Forms.DialogResult.Cancel Then
                                Status = False
                                Exit Sub
                            End If

                        End Using
                End Select
            End If

            fw.WriteLine(ProdSize(0).ToString + " " + ProdSize(1).ToString + " " + ProdSize(2).ToString)

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    'create the reference of each view information
    Public Sub GenRefTxt(ByVal fw As StreamWriter, ByVal ReadyFL As List(Of ViewProp))
        Try
            If ReadyFL.Count >= 6 Then
                ViewNum = ReadyFL.Count
            Else
                ViewNum = 6
            End If

            For i As Integer = 1 To ViewNum
                Status = False
                For Each view As ViewProp In ReadyFL
                    If ViewIndex(view.ViewType) = i Then
                        fw.WriteLine(i.ToString + " " + SetDot(view.RefProp(0).ToString) + " " _
                                     + SetDot(view.RefProp(1).ToString) + " " + SetDot(view.RefProp(2).ToString))
                        Status = True
                    End If
                Next

                If Status = False Then
                    fw.WriteLine(i.ToString + " " + "0.000" + " " + "0.000" + _
                                 " " + "0.000")
                End If
            Next

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Function isequal(ByVal x As Double, ByVal y As Double) As Boolean
        If Math.Abs(x - y) > 0.1 Then
            Return False
        Else
            Return True
        End If
    End Function

    'check d2 parameter
    Private Function CheckD2Param(ByVal s As OutputFormat) As String
        If (FindCodeName(s) = "SNKBOLT" Or FindCodeName(s) = "CRBORE") And _
            (isequal(s.OriginAndAddition(4), 0) = True) Then
            Return (s.OriginAndAddition(6) + 1)
        ElseIf (FindCodeName(s) = "DRILL" Or FindCodeName(s) = "REAMER" Or _
            FindCodeName(s) = "THRBORE" Or FindCodeName(s) = "BLDBORE" Or _
            FindCodeName(s) = "HOLE") And (isequal(s.OriginAndAddition(4), 0) = True) Then
            Return 1
        Else
            Return s.OriginAndAddition(4)
        End If
    End Function

    Private Function BottomConverter(ByVal input As Double, ByVal surface As String) As Double
        If ViewIndex(surface) = 2 Then
            Return (input * (-1))
        Else
            Return input
        End If
    End Function

    Private Feature2Export As OutputFormat
    'create the feature information
    Public Sub GenFeatTxt(ByVal fw As StreamWriter, ByVal ReadyFL As System.Windows.Forms.DataGridView, _
                          ByRef FeatureList As List(Of OutputFormat))
        Try
            counter = 1
            For Each feat As System.Windows.Forms.DataGridViewRow In ReadyFL.Rows
                Feature2Export = New OutputFormat
                Feature2Export = feat.Cells("Object").Value
                Dim StringTmp As String = FindCodeName(Feature2Export)
                fw.WriteLine(counter.ToString + ". " + FindCodeName(Feature2Export) + " " _
                             + ViewIndex(Feature2Export.MiscProp(1)).ToString + " " _
                             + Feature2Export.MiscProp(2) + " " _
                             + Feature2Export.MiscProp(3) + " " _
                             + SetDot(Feature2Export.MiscProp(4)) + " " _
                             + SetDot(Feature2Export.OriginAndAddition(0).ToString) + " " _
                             + SetDot(Feature2Export.OriginAndAddition(1).ToString) + " " _
                             + SetDot(Feature2Export.OriginAndAddition(2).ToString) + " " _
                             + SetDot(Feature2Export.OriginAndAddition(3).ToString) + " " _
                             + SetDot(CheckD2Param(Feature2Export)) + " " _
                             + SetDot(Feature2Export.OriginAndAddition(5).ToString) + " " _
                             + SetDot(Feature2Export.OriginAndAddition(6).ToString) + " " _
                             + SetDot(Feature2Export.OriginAndAddition(7).ToString))
                counter = counter + 1
                FeatureList.Add(Feature2Export)
            Next
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Entity As DBObject
    Private CircleTmp As Circle
    Private LineTmp As Line

End Class
