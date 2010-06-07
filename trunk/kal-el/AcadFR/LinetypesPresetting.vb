Imports FR
Imports System.Linq
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices

Public Class LinetypesPresetting

    'variabel yg dibutuhkan
    Private SolidUIF As SolidLine
    Private HiddenUIF As HiddenLine
    Private AuxUIF As AuxiliaryLine
    Private ProceedStat As New Boolean

    'setiap isi sel di klik
    Private Sub LinetypesList_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles LinetypesList.CellContentClick
        ProceedStat = True
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.LinetypesList.Rows
            If Row.Cells("Solid").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = True
                Row.Cells("Hidden").Value = False
                Row.Cells("Auxiliary").Value = False
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Hidden").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = False
                Row.Cells("Hidden").Value = True
                Row.Cells("Auxiliary").Value = False
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Auxiliary").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = False
                Row.Cells("Hidden").Value = False
                Row.Cells("Auxiliary").Value = True
                Row.Cells("Ignore").Value = False
            End If

            If Row.Cells("Ignore").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = True Then
                Row.Cells("Solid").Value = False
                Row.Cells("Hidden").Value = False
                Row.Cells("Auxiliary").Value = False
                Row.Cells("Ignore").Value = True
            End If

            If Row.Cells("Solid").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("Hidden").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("Auxiliary").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False _
            And Row.Cells("Ignore").GetEditedFormattedValue(Row.Index, Forms.DataGridViewDataErrorContexts.Formatting) = False Then
                ProceedStat = False
            End If
        Next

        If ProceedStat = True Then
            Me.Proceed.Enabled = True
        Else
            Me.Proceed.Enabled = False
        End If

    End Sub

    'jika Proceed diklik
    Private Sub Proceed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Proceed.Click
        'masukin ke database
        For Each Row As System.Windows.Forms.DataGridViewRow In Me.LinetypesList.Rows
            If Row.Cells("Solid").FormattedValue = True Then
                'masukkan Row.Cells("Layer").Value , Row.Cells("LineType").Value , Row.Cells("Color").Value ke database Solid
                SolidUIF = New SolidLine
            ElseIf Row.Cells("Hidden").FormattedValue = True Then
                'masukkan Row.Cells("Layer").Value , Row.Cells("LineType").Value , Row.Cells("Color").Value ke database Hidden
                HiddenUIF = New HiddenLine
            ElseIf Row.Cells("Auxiliary").FormattedValue = True Then
                'masukkan Row.Cells("Layer").Value , Row.Cells("LineType").Value , Row.Cells("Color").Value ke database Auxiliary
                AuxUIF = New AuxiliaryLine
            End If
        Next

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Dispose()

    End Sub

    'jika cancel diklik
    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Dispose()
    End Sub

    'Dim LTList As System.Data.DataTable

    'Private Sub LinetypesList_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    '    LTList = New System.Data.DataTable("Linetypes")
    '    CreateTable(LTList, Me.IdentifiedFeature)
    'End Sub

End Class