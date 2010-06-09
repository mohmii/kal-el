Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Interop
Imports System.Linq
Imports System.Math
Imports FR

Imports System
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.ComponentModel

Public Class GetPoints
    'list of all points
    Private ListOfEntity As AllPoints
    Private UnAdjacentGroupOfEntity As List(Of AllPoints)
    'Private AllPoints As List(Of Point3d)
    'Private GroupOfEntity As List(Of AllPoints)

    'list of not adjacent points
    'Private UnAdjacentPoints As List(Of Point3d)

    'list of adjacent points
    Private AdjacentPoints As List(Of Point3d)

    Private Angle As Double
    Private EntityCheckingTmp As EntityProp

    'transition variable from entity to arc or line type
    Private LineTrans As Line
    Private ArcTrans As Arc

    'temporary variable for getting unredundant point
    Private TempStartPoint As Point3d
    Private TempEndPoint As Point3d

    Public Sub UnAdjacentPointExtractor(ByVal AllEntity As List(Of Entity), ByRef AllPoints As List(Of Point3d), ByRef GroupOfEntity As List(Of AllPoints), ByRef UnAdjacentPoints As List(Of Point3d))
        'AllPoints = New List(Of Point3d)
        'GroupOfEntity = New List(Of AllPoints)
        'UnAdjacentPoints = New List(Of Point3d)
        AdjacentPoints = New List(Of Point3d)
        UnAdjacentGroupOfEntity = New List(Of AllPoints)

        For Each EntityTmp As Entity In AllEntity
            If TypeOf EntityTmp Is Line Then
                LineTrans = EntityTmp
                TempStartPoint = New Point3d(Round(LineTrans.StartPoint.X, 5), Round(LineTrans.StartPoint.Y, 5), Round(LineTrans.StartPoint.Z, 5))
                TempEndPoint = New Point3d(Round(LineTrans.EndPoint.X, 5), Round(LineTrans.EndPoint.Y, 5), Round(LineTrans.EndPoint.Z, 5))

                If AllPoints.Contains(TempStartPoint) Then
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Line = LineTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.EndPoint.Y - EntityCheckingTmp.StartPoint.Y, EntityCheckingTmp.EndPoint.X - EntityCheckingTmp.StartPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = LineTrans.Linetype
                    GroupOfEntity(AllPoints.IndexOf(EntityCheckingTmp.StartPoint)).EntityList.Add(EntityCheckingTmp)
                ElseIf Not AllPoints.Contains(TempStartPoint) Then
                    AllPoints.Add(TempStartPoint)
                    ListOfEntity = New AllPoints
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Line = LineTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.EndPoint.Y - EntityCheckingTmp.StartPoint.Y, EntityCheckingTmp.EndPoint.X - EntityCheckingTmp.StartPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = LineTrans.Linetype
                    ListOfEntity.EntityList.Add(EntityCheckingTmp)
                    GroupOfEntity.Add(ListOfEntity)
                End If

                If AllPoints.Contains(TempEndPoint) Then
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Line = LineTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.StartPoint.Y - EntityCheckingTmp.EndPoint.Y, EntityCheckingTmp.StartPoint.X - EntityCheckingTmp.EndPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = LineTrans.Linetype
                    GroupOfEntity(AllPoints.IndexOf(EntityCheckingTmp.EndPoint)).EntityList.Add(EntityCheckingTmp)
                ElseIf Not AllPoints.Contains(TempEndPoint) Then
                    AllPoints.Add(TempEndPoint)
                    ListOfEntity = New AllPoints
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Line = LineTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.StartPoint.Y - EntityCheckingTmp.EndPoint.Y, EntityCheckingTmp.StartPoint.X - EntityCheckingTmp.EndPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = LineTrans.Linetype
                    ListOfEntity.EntityList.Add(EntityCheckingTmp)
                    GroupOfEntity.Add(ListOfEntity)
                End If

            ElseIf TypeOf EntityTmp Is Arc Then
                ArcTrans = EntityTmp
                TempStartPoint = New Point3d(Round(ArcTrans.StartPoint.X, 5), Round(ArcTrans.StartPoint.Y, 5), Round(ArcTrans.StartPoint.Z, 5))
                TempEndPoint = New Point3d(Round(ArcTrans.EndPoint.X, 5), Round(ArcTrans.EndPoint.Y, 5), Round(ArcTrans.EndPoint.Z, 5))

                If AllPoints.Contains(TempStartPoint) Then
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Arc = ArcTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.EndPoint.Y - EntityCheckingTmp.StartPoint.Y, EntityCheckingTmp.EndPoint.X - EntityCheckingTmp.StartPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = ArcTrans.Linetype
                    GroupOfEntity(AllPoints.IndexOf(EntityCheckingTmp.StartPoint)).EntityList.Add(EntityCheckingTmp)
                ElseIf Not AllPoints.Contains(TempStartPoint) Then
                    AllPoints.Add(TempStartPoint)
                    ListOfEntity = New AllPoints
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Arc = ArcTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.EndPoint.Y - EntityCheckingTmp.StartPoint.Y, EntityCheckingTmp.EndPoint.X - EntityCheckingTmp.StartPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = ArcTrans.Linetype
                    ListOfEntity.EntityList.Add(EntityCheckingTmp)
                    GroupOfEntity.Add(ListOfEntity)
                End If

                If AllPoints.Contains(TempEndPoint) Then
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Arc = ArcTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.StartPoint.Y - EntityCheckingTmp.EndPoint.Y, EntityCheckingTmp.StartPoint.X - EntityCheckingTmp.EndPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = ArcTrans.Linetype
                    GroupOfEntity(AllPoints.IndexOf(EntityCheckingTmp.EndPoint)).EntityList.Add(EntityCheckingTmp)
                ElseIf Not AllPoints.Contains(TempEndPoint) Then
                    AllPoints.Add(TempEndPoint)
                    ListOfEntity = New AllPoints
                    EntityCheckingTmp = New EntityProp
                    EntityCheckingTmp.Entity = EntityTmp
                    EntityCheckingTmp.Arc = ArcTrans
                    EntityCheckingTmp.StartPoint = TempStartPoint
                    EntityCheckingTmp.EndPoint = TempEndPoint
                    EntityCheckingTmp.Angle = Atan2(EntityCheckingTmp.StartPoint.Y - EntityCheckingTmp.EndPoint.Y, EntityCheckingTmp.StartPoint.X - EntityCheckingTmp.EndPoint.X) * (180 / PI)
                    EntityCheckingTmp.LineType = ArcTrans.Linetype
                    ListOfEntity.EntityList.Add(EntityCheckingTmp)
                    GroupOfEntity.Add(ListOfEntity)
                End If
            End If
        Next

        For Each PointTmp As Point3d In AllPoints
            If Not AdjacentPoints.Contains(PointTmp) Then
                UnAdjacentPoints.Add(PointTmp)
                UnAdjacentGroupOfEntity.Add(GroupOfEntity(AllPoints.IndexOf(PointTmp)))
                For Each EntityTmp As Entity In AllEntity
                    If TypeOf EntityTmp Is Line Then
                        LineTrans = EntityTmp
                        TempStartPoint = New Point3d(Round(LineTrans.StartPoint.X, 5), Round(LineTrans.StartPoint.Y, 5), Round(LineTrans.StartPoint.Z, 5))
                        TempEndPoint = New Point3d(Round(LineTrans.EndPoint.X, 5), Round(LineTrans.EndPoint.Y, 5), Round(LineTrans.EndPoint.Z, 5))
                        If (TempStartPoint = PointTmp) And Not AdjacentPoints.Contains(TempEndPoint) Then
                            AdjacentPoints.Add(TempEndPoint)
                        ElseIf (TempEndPoint = PointTmp) And Not AdjacentPoints.Contains(TempStartPoint) Then
                            AdjacentPoints.Add(TempStartPoint)
                        End If
                    ElseIf TypeOf EntityTmp Is Arc Then
                        ArcTrans = EntityTmp
                        TempStartPoint = New Point3d(Round(ArcTrans.StartPoint.X, 5), Round(ArcTrans.StartPoint.Y, 5), Round(ArcTrans.StartPoint.Z, 5))
                        TempEndPoint = New Point3d(Round(ArcTrans.EndPoint.X, 5), Round(ArcTrans.EndPoint.Y, 5), Round(ArcTrans.EndPoint.Z, 5))
                        If (TempStartPoint = PointTmp) And Not AdjacentPoints.Contains(TempEndPoint) Then
                            AdjacentPoints.Add(TempEndPoint)
                        ElseIf (TempEndPoint = PointTmp) And Not AdjacentPoints.Contains(TempStartPoint) Then
                            AdjacentPoints.Add(TempStartPoint)
                        End If
                    End If
                Next
            End If
        Next
    End Sub
End Class