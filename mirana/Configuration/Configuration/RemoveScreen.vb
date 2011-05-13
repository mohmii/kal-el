Public Class RemoveScreen

    Private CheckedLang As String

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        CheckedLang = Me.RadioButton1.Text
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        CheckedLang = Me.RadioButton2.Text
    End Sub

    Private SetupRegistry As SetupRegistry
    Private Message As String

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        SetupRegistry = New SetupRegistry
        SetupRegistry.RemoveSetup(CheckedLang, Message)
        Me.TextBox1.Text = Message
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Dispose()
    End Sub
End Class