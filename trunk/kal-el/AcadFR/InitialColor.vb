Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Colors

Public Class InitialColor
    Private ColourId As ObjectId '= New String() {"", ""}

    Public Property ColorId() As ObjectId
        Get
            Return ColourId
        End Get
        Set(ByVal value As ObjectId)
            ColourId = value
        End Set
    End Property

    Private ColourIndex As Short
    Public Property ColorIndex() As Short
        Get
            Return ColourIndex
        End Get
        Set(ByVal value As Short)
            ColourIndex = value
        End Set
    End Property

End Class
