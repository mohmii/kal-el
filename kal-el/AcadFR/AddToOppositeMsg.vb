Imports System.Windows.Forms

Public Class AddToOppositeMsg
    Public KeepStat As New Boolean
    Private Sub Keep_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Keep_Button.Click
        KeepStat = True
        If adskClass.myPalette.Label16.Text <> "0" Then
            adskClass.myPalette.StartAddToOppositeSurface(adskClass.myPalette.IdentifiedFeature)
        End If

        If adskClass.myPalette.Label17.Text <> "0" Then
            adskClass.myPalette.StartAddToOppositeSurface(adskClass.myPalette.UnidentifiedFeature)
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.Yes
        Me.close()
    End Sub

    Private Sub Delete_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete_Button.Click
        KeepStat = False
        If adskClass.myPalette.Label16.Text <> "0" Then
            adskClass.myPalette.StartAddToOppositeSurface(adskClass.myPalette.IdentifiedFeature)
            adskClass.myPalette.StartDeleting(adskClass.myPalette.IdentifiedFeature)
        End If

        If adskClass.myPalette.Label17.Text <> "0" Then
            adskClass.myPalette.StartAddToOppositeSurface(adskClass.myPalette.UnidentifiedFeature)
            adskClass.myPalette.StartDeleting(adskClass.myPalette.UnidentifiedFeature)
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.No
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

End Class
