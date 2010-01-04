Imports Autodesk.AutoCAD.DatabaseServices

Public Class LoopSet

    'id for each loop
    Private IdLoop As Integer
    Public Property IdLoopSet()
        Get
            Return IdLoop
        End Get
        Set(ByVal value)
            IdLoop = value
        End Set
    End Property

    'group of line
    Private LoopElements As List(Of Entity)
    Public Property LoopElement(ByVal index As Integer) As Entity
        Get
            Return LoopElements(index)
        End Get
        Set(ByVal value As Entity)
            LoopElements(index) = value
        End Set
    End Property

End Class
