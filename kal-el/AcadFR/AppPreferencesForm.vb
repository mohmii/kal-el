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
        AppPreferences.RemoveUEE = Me.RemoveEntities.Checked
        AppPreferences.DrawPP = Me.PreProcess.Checked


        adskClass.AppPreferences.WSDir = Me.Directory.Text
        adskClass.AppPreferences.AutoRegLine = Me.AutoRegLine.Checked
        adskClass.AppPreferences.AutoRegSchem = Me.AutoRegScheme.Checked
        adskClass.AppPreferences.MultiAnalysis = Me.MultiAnalysis.Checked
        adskClass.AppPreferences.RemoveUEE = Me.RemoveEntities.Checked
        adskClass.AppPreferences.DrawPP = Me.PreProcess.Checked

        If Me.PreProcess.Checked = True Then
            If Me.Tolerance.Value = 0 Then
                Me.Tolerance.Value = 0.1
            End If

            If Me.Schematic.Value = 0 Then
                Me.Schematic.Value = 0.1
            End If

            If Me.HoleDiaTol.Value = 0 Then
                Me.HoleDiaTol.Value = 0.1
            End If

            If Me.UnderholeDiaTol.Value = 0 Then
                Me.UnderholeDiaTol.Value = 0.1
            End If

            AppPreferences.ToleranceValues = Me.Tolerance.Value
            AppPreferences.SchematicSymbol = Me.Schematic.Value
            AppPreferences.HoleTolerance = Me.HoleDiaTol.Value
            AppPreferences.UnderholeTolerance = Me.UnderholeDiaTol.Value

            adskClass.AppPreferences.ToleranceValues = Me.Tolerance.Value
            adskClass.AppPreferences.SchematicSymbol = Me.Schematic.Value
            adskClass.AppPreferences.HoleTolerance = Me.HoleDiaTol.Value
            adskClass.AppPreferences.UnderholeTolerance = Me.UnderholeDiaTol.Value
        Else
            AppPreferences.ToleranceValues = 0.1
            AppPreferences.SchematicSymbol = 0.1
            AppPreferences.HoleTolerance = 0.1
            AppPreferences.UnderholeTolerance = 0.1

            adskClass.AppPreferences.ToleranceValues = 0.1
            adskClass.AppPreferences.SchematicSymbol = 0.1
            adskClass.AppPreferences.HoleTolerance = 0.1
            adskClass.AppPreferences.UnderholeTolerance = 0.1
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
            Me.RemoveEntities.Checked = AppPreferences.RemoveUEE
            Me.PreProcess.Checked = AppPreferences.DrawPP
            Me.Tolerance.Value = AppPreferences.ToleranceValues
            Me.Schematic.Value = AppPreferences.SchematicSymbol
            Me.HoleDiaTol.Value = AppPreferences.HoleTolerance
            Me.UnderholeDiaTol.Value = AppPreferences.UnderholeTolerance
        End If
    End Sub

    Private Sub PreProcess_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PreProcess.CheckStateChanged, RemoveEntities.CheckStateChanged

        If Me.PreProcess.CheckState = 1 Then
            Me.Tolerance.Value = 0.1
            Me.Schematic.Value = 0.1
            Me.HoleDiaTol.Value = 0.1
            Me.UnderholeDiaTol.Value = 0.5
            Me.Tolerance.Enabled = True
            Me.Schematic.Enabled = True
            Me.HoleDiaTol.Enabled = True
            Me.UnderholeDiaTol.Enabled = True
        End If
        If Me.PreProcess.CheckState = 0 Then
            Me.Tolerance.Enabled = False
            Me.Schematic.Enabled = False
            Me.HoleDiaTol.Enabled = False
            Me.UnderholeDiaTol.Enabled = False
        End If
    End Sub
End Class