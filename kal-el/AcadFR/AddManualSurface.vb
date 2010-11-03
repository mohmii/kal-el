Imports System.Windows.Forms

Public Class AddManualSurface
    Public Shared SelSurfMan As String
    Public Shared SelSurfManStat As Boolean
    Public Shared CBHole As Boolean
    Public Shared CBMill As Boolean
    Public Shared CBPoly As Boolean

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        SelSurfMan = Me.surfacetype.SelectedItem.ToString
        SelSurfManStat = True
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Dispose()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Dispose()
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            Me.surfacetype.Enabled = True
        Else
            Me.surfacetype.Enabled = False
        End If
    End Sub

    Private Sub AddManualSurface_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'initiate default value of all variables
        Me.OK_Button.Enabled = False
        Me.CheckBoxHole.Checked = False
        Me.CheckBoxMill.Checked = False
        Me.CheckBoxPoly.Checked = False
        Me.surfacetype.Items.Clear()
        SelSurfManStat = False
        CBHole = False
        CBMill = False
        CBPoly = False
        SelSurfMan = ""

        For Each ProjectionView As ViewProp In SelectionCommand.ProjectionView
            If ProjectionView.GenerationStat = True Then
                Me.surfacetype.Items.Add(ProjectionView.ViewType)
            End If
        Next

        If RadioButton1.Checked = True Then
            surfacetype.Enabled = True
        Else
            surfacetype.Enabled = False
        End If

    End Sub

    Private Sub CheckBoxHole_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxHole.CheckedChanged
        If Me.CheckBoxHole.Checked = True Then
            Me.CheckBoxMill.Checked = False
            Me.CheckBoxPoly.Checked = False
            'Me.OK_Button.Enabled = True
            CBHole = True
            CBMill = False
            CBPoly = False
        ElseIf Me.CheckBoxHole.Checked = False And Me.CheckBoxMill.Checked = False And Me.CheckBoxPoly.Checked = False Then
            'Me.OK_Button.Enabled = False
            CBHole = False
            CBMill = False
            CBPoly = False
        End If
        CheckOK()
    End Sub

    Private Sub CheckBoxMill_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxMill.CheckedChanged
        If Me.CheckBoxMill.Checked = True Then
            Me.CheckBoxHole.Checked = False
            Me.CheckBoxPoly.Checked = False
            'Me.OK_Button.Enabled = True
            CBHole = False
            CBMill = True
            CBPoly = False
        ElseIf Me.CheckBoxHole.Checked = False And Me.CheckBoxMill.Checked = False And Me.CheckBoxPoly.Checked = False Then
            'Me.OK_Button.Enabled = False
            CBHole = False
            CBMill = False
            CBPoly = False
        End If
        CheckOK()
    End Sub

    Private Sub CheckBoxPoly_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxPoly.CheckedChanged
        If Me.CheckBoxPoly.Checked = True Then
            Me.CheckBoxHole.Checked = False
            Me.CheckBoxMill.Checked = False
            'Me.OK_Button.Enabled = True
            CBHole = False
            CBMill = False
            CBPoly = True
        ElseIf Me.CheckBoxHole.Checked = False And Me.CheckBoxMill.Checked = False And Me.CheckBoxPoly.Checked = False Then
            'Me.OK_Button.Enabled = False
            CBHole = False
            CBMill = False
            CBPoly = False
        End If
        CheckOK()
    End Sub

    Private Sub surfacetype_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles surfacetype.SelectedIndexChanged
        CheckOK()
    End Sub

    'checking the availability of OK botton
    Private Sub CheckOK()
        If surfacetype.SelectedItem Is Nothing Or (CheckBoxHole.Checked = False And CheckBoxMill.Checked = False And CheckBoxPoly.Checked = False) Then
            Me.OK_Button.Enabled = False
        Else
            Me.OK_Button.Enabled = True
        End If
    End Sub
End Class