Imports System.Windows.Forms

Public Class setView

    Public viewset As Integer
    Public Shared viewis As String
    Public Shared CBVisible As Boolean
    Public Shared CBHidden As Boolean
    Public Shared CBHole As Boolean
    Public Shared CBMill As Boolean
    Public Shared CBPoly As Boolean
    Public Shared CBDoubleMirror As Boolean
    Private selection As SelectionCommand

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        If PromptUserSelection() = True Then
            'dispose the dialog
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Dispose()

            'save all the parameter from the dialog
            viewis = Me.viewtype.SelectedItem
            CBVisible = Me.CheckBoxVis.Checked
            CBHidden = Me.CheckBoxHid.Checked
            CBHole = Me.CheckBoxHole.Checked
            CBMill = Me.CheckBoxMill.Checked
            CBPoly = Me.CheckBoxPoly.Checked
            CBDoubleMirror = Me.CheckBoxDoubleMirror.Checked

            'start to initiate selection process
            selection = New SelectionCommand
            selection.OptionSel()

        End If

    End Sub

    'get the correct dialog if user is inserting wrong view parameter
    Private PromptDialog As String
    Private Sub GetPromptDialog()

        PromptDialog = ""

        If Me.viewtype.SelectedItem = "" And CheckIfFeatureIsSelected() = False Then
            PromptDialog = "先に設計面を選んでください。また、形状種類を少なくても一つ選んでください。"
        ElseIf Me.viewtype.SelectedItem = "" Then
            PromptDialog = "先に設計面を選んでください。"
        ElseIf CheckIfFeatureIsSelected() = False Then
            PromptDialog = "形状種類を少なくても一つ選んでください。"
        End If

        MsgBox(PromptDialog, MsgBoxStyle.OkOnly)

    End Sub

    'notify user to insert the right parameter
    Private Function PromptUserSelection() As Boolean
        If Me.viewtype.SelectedItem <> "" And CheckIfFeatureIsSelected() = True Then
            Return True
        Else
            GetPromptDialog()
            Return False
        End If

    End Function

    'check if one kind feature is already selected by user
    Private Function CheckIfFeatureIsSelected() As Boolean
        If Me.CheckBoxHole.Checked = False And Me.CheckBoxMill.Checked = False And Me.CheckBoxPoly.Checked = False Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Dispose()
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            Me.viewtype.Enabled = True
        Else
            Me.viewtype.Enabled = False
        End If

    End Sub

    Private ViewNeed2Add(6) As String
    Private ListOfView As List(Of String)

    Private Sub setView_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If SelectionCommand.ProjectionView.Count <> 0 Then
            ViewNeed2Add = New String() {1, 1, 1, 1, 1, 1}
            ListOfView = New List(Of String)
            For i As Integer = 0 To Me.viewtype.Items.Count - 1
                ListOfView.Add(Me.viewtype.Items(i))
                For Each ProjectionView As ViewProp In SelectionCommand.ProjectionView
                    If ProjectionView.ViewType.Equals(Me.viewtype.Items(i)) And ProjectionView.GenerationStat = True Then
                        ViewNeed2Add(i) = 0
                    End If
                Next
            Next

            Me.viewtype.Items.Clear()

            For i As Integer = 0 To ListOfView.Count - 1
                If ViewNeed2Add(i) = 1 Then
                    Me.viewtype.Items.Add(ListOfView(i))
                End If
            Next
        End If
        'hapus surface yang dipilih
        'Dim I As Integer
        'Dim index As Integer

        'adskClass.myPalette.ComboBox2.SelectAll()
        'For I = 0 To adskClass.myPalette.ComboBox2.Items.Count - 1
        '    adskClass.myPalette.ComboBox2.SelectedIndex = I
        '    adskClass.TempName = adskClass.myPalette.ComboBox2.Text
        '    index = viewtype.FindString(adskClass.TempName)
        '    If index >= 0 Then viewtype.Items.RemoveAt(index)
        'Next
        'adskClass.myPalette.ComboBox2.Text = ""

        'viewtype.Refresh()

        If RadioButton1.Checked = True Then
            viewtype.Enabled = True
        Else
            viewtype.Enabled = False
        End If
    End Sub

    
End Class
