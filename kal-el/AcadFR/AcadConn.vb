Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Public Class AcadConn

    Public Sub StartTransaction(ByVal ActiveDocDatabase As Database)
        db = ActiveDocDatabase
        tm = db.TransactionManager
        myT = tm.StartTransaction
    End Sub

    Private db As Database
    Public tm As Autodesk.AutoCAD.DatabaseServices.TransactionManager
    Public myT As Transaction

    Public Sub OpenBlockTableRec()
        bt = myT.GetObject(db.BlockTableId, OpenMode.ForRead)
        btrId = bt.Item(BlockTableRecord.ModelSpace)
        btr = myT.GetObject(btrId, OpenMode.ForRead, True)
    End Sub

    Private bt As BlockTable
    Public btrId As ObjectId
    Public btr As BlockTableRecord
End Class
