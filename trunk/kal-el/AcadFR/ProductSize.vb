Public Class ProductSize

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'If Not (Me.Length1.Value = 0) And Not (Me.Width1.Value = 0) And _
        'Not (Me.Height1.Value = 0) Then
        '    'GenerateFL.ProdSize(0) = Me.Length1.Value
        '    'GenerateFL.ProdSize(1) = Me.Width1.Value
        '    'GenerateFL.ProdSize(2) = Me.Height1.Value
        Me.DialogResult = Forms.DialogResult.OK
        Me.Dispose()
        'End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.DialogResult = Forms.DialogResult.Cancel
        Me.Dispose()
    End Sub

End Class