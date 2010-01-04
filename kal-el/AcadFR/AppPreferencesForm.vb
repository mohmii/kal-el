Imports System.Windows.Forms
Imports System.IO

Public Class AppPreferencesForm

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            Using FolderBrowser As New FolderBrowserDialog
                FolderBrowser.ShowNewFolderButton = False
                FolderBrowser.ShowDialog()
                Me.Directory.ResetText()
                Me.Directory.Text = FolderBrowser.SelectedPath
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Dispose()
    End Sub

    Private AppPreferences As New AppPreferences

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save.Click
        AppPreferences.WSDir = Me.Directory.Text
        AppPreferences.ToleranceValues = Me.Tolerance.Value
        AppPreferences.SchematicSymbol = Me.Schematic.Value
        AppPreferences.DrawPP = Me.PreProcess.Checked
        AppPreferences.SetWorkSpaceDir()

        adskClass.AppPreferences.WSDir = Me.Directory.Text
        adskClass.AppPreferences.ToleranceValues = Me.Tolerance.Value
        adskClass.AppPreferences.SchematicSymbol = Me.Schematic.Value
        adskClass.AppPreferences.DrawPP = Me.PreProcess.Checked
        Me.Dispose()
    End Sub

    Private Sub AppPreferencesForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If AppPreferences.ReadPreferences() Then
            Me.Directory.Text = AppPreferences.WSDir
            Me.Tolerance.Value = AppPreferences.ToleranceValues
            Me.Schematic.Value = AppPreferences.SchematicSymbol
            Me.PreProcess.Checked = AppPreferences.DrawPP
        End If

    End Sub
End Class