'Imports Autodesk.AutoCAD.Runtime
'Imports Autodesk.AutoCAD.ApplicationServices
'Imports Autodesk.AutoCAD.EditorInput
'Imports Autodesk.AutoCAD.DatabaseServices
'Imports Autodesk.AutoCAD.Geometry
'Imports Autodesk.AutoCAD.Windows
'Imports System.Windows
'Imports Microsoft.VisualBasic



'Public Class OrganizeList
'    Private Shared JapFeatureName As String = Nothing

'    Private Shared ArrayOfName() As String

'    'method for adding feature name in extracted feature list
'    Public Shared Sub AddListToExisting(ByVal m As OutputFormat)
'        Try
'            'add it to list
'            adskClass.myPalette.ExistingList.Items.Add(m.MiscProp(0) + "   " + m.MiscProp(1).ToUpper)

'        Catch ex As Exception
'            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Report Failed")
'        End Try
'    End Sub

'    'method for adding feature name in urecognized feature list
'    Public Shared Sub AddListToExisting2(ByVal m As OutputFormat)
'        Try
'            'add it to list
'            adskClass.myPalette.ExistingList2.Items.Add(m.FeatureName + vbTab + "   " + m.MiscProp(1).ToUpper)          'Add item to listbox
'        Catch ex As Exception
'            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Report Failed")
'        End Try
'    End Sub

'    Public Shared Sub RemoveFromExist(ByVal i As Integer)
'        Try
'            adskClass.myPalette.ExistingList.Items.RemoveAt(i)
'        Catch ex As Exception
'            MsgBox(ex.ToString)
'        End Try
'    End Sub

'    Public Shared Sub RemoveFromExist2(ByVal i As Integer)
'        Try
'            adskClass.myPalette.ExistingList2.Items.RemoveAt(i)
'        Catch ex As Exception
'            MsgBox(ex.ToString)
'        End Try
'    End Sub

'End Class
