Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Interop
Imports System.Linq
Imports System.Math
Imports System.IO
Imports FR

Public Class DwgProcessor

    Private PointStatus As Boolean = True
    Private PointCol As Point3dCollection
    Private PointToEdit As Point3d
    Private EntityIndex, LineIndex As List(Of Integer)
    Private AddedEntities, RemovedEntities As New List(Of Entity)
    Private iWhile, iFor As Integer
    Private BreakFailed As Boolean = False

    Private GetPoints As GetPoints
    Private AllPoints As List(Of Point3d)
    Private GroupOfEntity As List(Of AllPoints)
    Private UnAdjacentPoints As List(Of Point3d)
    Private GeometryProcessor As GeometryProcessor

    Private AllSinglePoints As List(Of Point3d)
    Private CheckThisPoint, BreakAtThisPoint As Point3d
    Private BreakThisLine, LineToCheck As Line
    Private EntityToAdd, EntityToDel As Entity
    Private BreakThisEntities As Entity()
    Private AllLineEntitites, BreakThisLines As List(Of Line)
    Private NumOfPoints As Integer

    Private SplitStatus As Boolean
    Private PointStatusList As List(Of Boolean)

    'collect only single point
    Private Sub SelectSinglePoints(ByVal Points As List(Of Point3d), ByVal NewPoints As List(Of Point3d))

        AllSinglePoints = New List(Of Point3d)

        For Each PointTmp As Point3d In Points
            If GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.Count = 1 Then
                AllSinglePoints.Add(PointTmp)
            End If
        Next
    End Sub

    Private ProgBar As ProgressForm

    'used for pre-pocessing drawing
    Public Sub StartPreProcessing(ByRef AllEntities As List(Of Entity), ByRef LineEntities As List(Of Line), _
                                  ByVal Transaction As Transaction)

        PointStatus = True
        iWhile = 0
        iFor = 0
        Dim ProgBarStat As Boolean = False
        ProgBar = New ProgressForm
        ProgBar.Text = "ミリング形状処理中" 'Processing Milling Fatures
        'break point in the middle of the line
        Try
            While PointStatus = True And BreakFailed = False

                iWhile = iWhile + 1

                PointStatus = False
                BreakFailed = False
                AllPoints = New List(Of Point3d)
                GroupOfEntity = New List(Of AllPoints)
                UnAdjacentPoints = New List(Of Point3d)
                GetPoints = New GetPoints
                EntityIndex = New List(Of Integer)
                LineIndex = New List(Of Integer)
                GeometryProcessor = New GeometryProcessor
                AddedEntities = New List(Of Entity)
                RemovedEntities = New List(Of Entity)
                PointToEdit = New Point3d
                AllSinglePoints = New List(Of Point3d)
                BreakThisLine = New Line

                'get all points 
                GetPoints.UnAdjacentPointExtractor(AllEntities, AllPoints, GroupOfEntity, UnAdjacentPoints)

                iFor = 0

                SelectSinglePoints(AllPoints, AllSinglePoints)

                UserControl3.acedSetStatusBarProgressMeter("Line Processor", 0, AllSinglePoints.Count)
                ProgBar.ProgressBar1.Maximum = AllSinglePoints.Count
                
                If ProgBarStat = False Then
                    ProgBar.Show()
                    ProgBarStat = True
                End If

                ProgBar.ProgressBar1.Value = 0
                System.Windows.Forms.Application.DoEvents()

                Dim iLoadBar As Integer

                For Each PointTmp As Point3d In AllSinglePoints

                    iFor = iFor + 1
                    ProgBar.ProgressBar1.Value = iLoadBar + 1
                    ProgBar.Label1.Text = Round(((ProgBar.ProgressBar1.Value / AllSinglePoints.Count) * 100), 0).ToString
                    System.Windows.Forms.Application.DoEvents()
                    For Each EntityTmp As Entity In AllEntities
                        If TypeOf EntityTmp Is Line Then
                            BreakThisLine = EntityTmp
                            If isequalpoint(PointTmp, BreakThisLine.StartPoint) = False _
                            And isequalpoint(PointTmp, BreakThisLine.EndPoint) = False _
                            And GeometryProcessor.PointOnline(PointTmp, BreakThisLine.StartPoint, BreakThisLine.EndPoint) = 2 _
                            And Not RemovedEntities.Contains(EntityTmp) Then
                                PointCol = New Point3dCollection
                                EntityToAdd = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList(0).Entity
                                If GeometryProcessor.CheckDifferences(BreakThisLine.StartPoint.X, BreakThisLine.EndPoint.X) = True Then
                                    If GeometryProcessor.CheckDifferences(PointTmp.X, BreakThisLine.StartPoint.X) Then
                                        BreakAtThisPoint = New Point3d(BreakThisLine.StartPoint.X, PointTmp.Y, PointTmp.Z)
                                        GeometryProcessor.SplitLine(BreakThisLine, BreakAtThisPoint, AddedEntities, RemovedEntities, _
                                                                    EntityTmp, EntityIndex, AllEntities, LineIndex, LineEntities, _
                                                                    PointToEdit, EntityToAdd)
                                        PointStatus = True
                                        BreakFailed = False
                                        Exit For
                                    Else
                                        BreakFailed = True
                                    End If

                                ElseIf GeometryProcessor.CheckDifferences(BreakThisLine.StartPoint.Y, BreakThisLine.EndPoint.Y) = True Then
                                    If GeometryProcessor.CheckDifferences(PointTmp.Y, BreakThisLine.StartPoint.Y) Then
                                        BreakAtThisPoint = New Point3d(PointTmp.X, BreakThisLine.StartPoint.Y, PointTmp.Z)
                                        GeometryProcessor.SplitLine(BreakThisLine, BreakAtThisPoint, AddedEntities, RemovedEntities, _
                                                                    EntityTmp, EntityIndex, AllEntities, LineIndex, LineEntities, _
                                                                    PointToEdit, EntityToAdd)
                                        PointStatus = True
                                        BreakFailed = False
                                        Exit For
                                    Else
                                        BreakFailed = True
                                    End If
                                Else
                                    LineToCheck = GroupOfEntity(AllPoints.IndexOf(PointTmp)).EntityList.Item(0).Line
                                    BreakAtThisPoint = GeometryProcessor.IntersectionPoint(BreakThisLine, LineToCheck)

                                    If GeometryProcessor.CheckDifferences(PointTmp, BreakAtThisPoint) = True Then
                                        GeometryProcessor.SplitLine(BreakThisLine, BreakAtThisPoint, AddedEntities, RemovedEntities, _
                                                                    EntityTmp, EntityIndex, AllEntities, LineIndex, LineEntities, _
                                                                    BreakAtThisPoint, EntityToAdd)

                                        PointStatus = True
                                        BreakFailed = False
                                        Exit For
                                    Else
                                        BreakFailed = True
                                    End If
                                End If
                            End If
                        End If
                    Next
                    If PointStatus = True Then
                        Exit For
                    End If
                    'add the progress bar
                    iLoadBar = iLoadBar + 1

                    'System.Threading.Thread.Sleep(1)
                    UserControl3.acedSetStatusBarProgressMeterPos(iLoadBar)
                    System.Windows.Forms.Application.DoEvents()
                Next
                ProgBar.ProgressBar1.Value = AllSinglePoints.Count
                ProgBar.Label1.Text = "100"
                System.Windows.Forms.Application.DoEvents()
                UserControl3.acedRestoreStatusBar()

                'membuang dan menambah jika ada entitas yang di-split serta mengedit 
                If PointStatus = True Then
                    'membuang entitas yang di-split dan diedit
                    EntityIndex.Sort()
                    For i As Integer = (EntityIndex.Count - 1) To 0 Step (-1)
                        EntityToDel = Transaction.GetObject(AllEntities(EntityIndex(i)).ObjectId, OpenMode.ForWrite, True)
                        EntityToDel.Erase()
                        AllEntities.RemoveAt(EntityIndex(i))
                    Next

                    'membuang garis yang di-split
                    LineIndex.Sort()
                    For j As Integer = (LineIndex.Count - 1) To 0 Step (-1)
                        LineEntities.RemoveAt(LineIndex(j))
                    Next

                    'menambah entitas hasil split
                    For k As Integer = 0 To AddedEntities.Count - 1
                        AllEntities.Add(AddedEntities(k))
                        LineEntities.Add(AddedEntities(k))
                    Next
                End If

            End While
        Catch ex As Exception
            ProgBar.ProgressBar1.Value = AllSinglePoints.Count
            ProgBar.Label1.Text = "100"
            System.Windows.Forms.Application.DoEvents()
            ProgBar.Close()
            ProgBar.Dispose()
            'Error in breaking line entities, please break line manually and just click ok
            'MsgBox("線素の分解でエラーが起こりました。　線素を手動で分解して下さい。")
        End Try
        ProgBar.ProgressBar1.Value = AllSinglePoints.Count
        ProgBar.Label1.Text = "100"
        ProgBar.Close()
        ProgBar.Dispose()
    End Sub

    'variable for breaking lines
    Private Function isequalpoint(ByVal point1 As Point3d, ByVal point2 As Point3d) As Boolean
        If Math.Abs(point1.X - point2.X) <= adskClass.AppPreferences.ToleranceValues _
        And Math.Abs(point1.Y - point2.Y) <= adskClass.AppPreferences.ToleranceValues _
        And Math.Abs(point1.Z - point2.Z) <= adskClass.AppPreferences.ToleranceValues Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
