Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports System.IO
Imports System.Math

Public Class AppPreferences
    Private HomePath As DirectoryInfo
    Private FilePath As FileInfo

    Private TmpFileWrite As StreamWriter
    Private TmpFileRead As StreamReader

    Private pref As New List(Of String)

    Public Function ReadPreferences()
        SystemDrive = System.Environment.GetEnvironmentVariable("systemdrive")
        PathString = SystemDrive + (System.Environment.GetEnvironmentVariable("HomePath") _
                                    + Path.DirectorySeparatorChar + ".AcadFR")
        HomePath = New DirectoryInfo(PathString)
        FilePath = New FileInfo((PathString + Path.DirectorySeparatorChar + "Preferences.txt"))
        GetPreferences(HomePath, FilePath)
        Return (Not IsNothing(WorkSpaceDir)) And (Not IsNothing(TolValue))
    End Function

    Private Sub GetPreferences(ByVal path1 As DirectoryInfo, ByVal path2 As FileInfo)
        If path1.Exists Then
            If path2.Exists Then
                TmpFileRead = path2.OpenText()

                While TmpFileRead.Peek >= 0
                    pref.Add(TmpFileRead.ReadLine)
                End While

                If pref.Count = 6 Then
                    WorkSpaceDir = pref(0)
                    TolValue = pref(1)
                    Schema = pref(2)
                    VisSurface = pref(3)
                    HidSurface = pref(4)
                    PreProc = pref(5)
                End If

                TmpFileRead.Close()
            End If
        End If
    End Sub

    Private Sub CheckWorkSpaceDir(ByVal path1 As DirectoryInfo, ByVal path2 As FileInfo)
        If Not path1.Exists Then
            path1.Create()
            path1.Attributes = FileAttribute.Hidden
        End If

        TmpFileWrite = path2.CreateText()
        TmpFileWrite.WriteLine(WorkSpaceDir)
        TmpFileWrite.WriteLine(TolValue)
        TmpFileWrite.WriteLine(Schema)
        TmpFileWrite.WriteLine(VisSurface.ToString)
        TmpFileWrite.WriteLine(HidSurface.ToString)
        TmpFileWrite.Write(PreProc.ToString)
        TmpFileWrite.Flush()
        TmpFileWrite.Close()

    End Sub

    Private SystemDrive, PathString, WorkSpaceDir, TolValue, Schema As String

    Public Sub SetWorkSpaceDir()
        SystemDrive = System.Environment.GetEnvironmentVariable("systemdrive")
        PathString = SystemDrive + (System.Environment.GetEnvironmentVariable("HomePath") _
                                    + Path.DirectorySeparatorChar + ".AcadFR")
        HomePath = New DirectoryInfo(PathString)
        FilePath = New FileInfo((PathString + Path.DirectorySeparatorChar + "Preferences.txt"))
        CheckWorkSpaceDir(HomePath, FilePath)
    End Sub

    Public Property WSDir() As String
        Get
            Return WorkSpaceDir
        End Get
        Set(ByVal value As String)
            WorkSpaceDir = value
        End Set
    End Property

    Public Property ToleranceValues() As Double
        Get
            Return TolValue
        End Get
        Set(ByVal value As Double)
            TolValue = Round(value, 2).ToString
        End Set
    End Property

    Public Property SchematicSymbol() As Double
        Get
            Return Schema
        End Get
        Set(ByVal value As Double)
            Schema = Round(value, 2).ToString
        End Set
    End Property

    Private VisSurface, HidSurface, PreProc As Boolean

    Public Property VisibleSurface() As Boolean
        Get
            Return VisSurface
        End Get
        Set(ByVal value As Boolean)
            VisSurface = value
        End Set
    End Property

    Public Property HiddenSurface() As Boolean
        Get
            Return HidSurface
        End Get
        Set(ByVal value As Boolean)
            HidSurface = value
        End Set
    End Property

    Public Property DrawPP() As Boolean
        Get
            Return PreProc
        End Get
        Set(ByVal value As Boolean)
            PreProc = value
        End Set
    End Property
End Class
