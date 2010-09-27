Public Class FormToFocus
    Public Sub OpenForm(ByRef FormToFoc As FormToFocus)
        FormToFoc.Show()
        Button1_Click(Nothing, Nothing)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.DialogResult = Forms.DialogResult.OK
        Me.Dispose()
    End Sub
End Class