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
        AppPreferences.AutoRegLine = Me.AutoRegLine.Checked
        AppPreferences.AutoRegSchem = Me.AutoRegScheme.Checked
        AppPreferences.MultiAnalysis = Me.MultiAnalysis.Checked
        AppPreferences.DrawPP = Me.PreProcess.Checked

        adskClass.AppPreferences.WSDir = Me.Directory.Text
        adskClass.AppPreferences.AutoRegLine = Me.AutoRegLine.Checked
        adskClass.AppPreferences.AutoRegSchem = Me.AutoRegScheme.Checked
        adskClass.AppPreferences.MultiAnalysis = Me.MultiAnalysis.Checked
        adskClass.AppPreferences.DrawPP = Me.PreProcess.Checked

        If Me.PreProcess.Checked = True Then
            AppPreferences.ToleranceValues = Me.Tolerance.Value
            AppPreferences.SchematicSymbol = Me.Schematic.Value

            adskClass.AppPreferences.ToleranceValues = Me.Tolerance.Value
            adskClass.AppPreferences.SchematicSymbol = Me.Schematic.Value
        Else
            AppPreferences.ToleranceValues = 0
            AppPreferences.SchematicSymbol = 0

            adskClass.AppPreferences.ToleranceValues = 0
            adskClass.AppPreferences.SchematicSymbol = 0
        End If

        AppPreferences.SetWorkSpaceDir()

        Me.Dispose()
    End Sub

    Private Sub AppPreferencesForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If AppPreferences.ReadPreferences() Then
            Me.Directory.Text = AppPreferences.WSDir
            Me.AutoRegLine.Checked = AppPreferences.AutoRegLine
            Me.AutoRegScheme.Checked = AppPreferences.AutoRegSchem
            Me.MultiAnalysis.Checked = AppPreferences.MultiAnalysis
            Me.PreProcess.Checked = AppPreferences.DrawPP
            Me.Tolerance.Value = AppPreferences.ToleranceValues
            Me.Schematic.Value = AppPreferences.SchematicSymbol
        End If
    End Sub

    Private Sub PreProcess_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PreProcess.CheckStateChanged

        If Me.PreProcess.CheckState = 1 Then
            Me.Tolerance.Enabled = True
            Me.Schematic.Enabled = True
        End If
        If Me.PreProcess.CheckState = 0 Then
            Me.Tolerance.Enabled = False
            Me.Schematic.Enabled = False
        End If
    End Sub
End Class