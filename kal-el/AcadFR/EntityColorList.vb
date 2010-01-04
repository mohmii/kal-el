Imports Autodesk.AutoCAD.Colors

Public Class EntityColorList
    Private Id As String = Nothing

    Public Property ObjectId() As String
        Get
            Return Id
        End Get
        Set(ByVal value As String)
            Id = value
        End Set
    End Property

    'place for surface where the feature locate
    Private Color As Color

    Public Property ColorName() As color
        Get
            Return Color
        End Get
        Set(ByVal value As Color)
            Color = value
        End Set
    End Property

    Public Function SameId(ByVal s As String) As Boolean

    End Function
End Class
