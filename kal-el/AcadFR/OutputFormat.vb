Imports Autodesk.AutoCAD.DatabaseServices

'creating format for machining feature
Public Class OutputFormat
    'place for feature name
    Private FeatName As String = Nothing

    Public Property FeatureName() As String
        Get
            Return FeatName
        End Get
        Set(ByVal value As String)
            FeatName = value
        End Set
    End Property

    'place for surface where the feature locate
    Private Surface As String = Nothing

    Public Property SurfaceName() As String
        Get
            Return Surface
        End Get
        Set(ByVal value As String)
            Surface = value
        End Set
    End Property

    'place for telling how much the entity building the feature
    Private Member As Integer = Nothing

    Public Property EntityMember() As Integer
        Get
            Return Member
        End Get
        Set(ByVal value As Integer)
            Member = value
        End Set
    End Property

    'place for id/ids from ACAD entity that belongs to this feature
    Public ObjectId As New List(Of ObjectId) '= New String() {"", ""}

    'place for miscellaneous properties {name,surface,orientation,chamfer,quality} for each feature
    Private Misc() As String = New String() {FeatName, Surface, "2", "-1", "0"}

    Public Property MiscProp(ByVal index As Integer) As String
        Get
            Return Misc(index)
        End Get
        Set(ByVal value As String)
            Misc(index) = value
        End Set
    End Property

    'place for the origin {x,y,z} and additional properties {d1,d2,d3,d4,angle} for each feature
    Private OriAndAdd() As Double = New Double() {0, 0, 0, 0, 0, 0, 0, 0}

    Public Property OriginAndAddition(ByVal index As Integer) As String
        Get
            Return OriAndAdd(index)
        End Get
        Set(ByVal value As String)
            OriAndAdd(index) = value
        End Set
    End Property

    'place for telling where the entity index location in autocad database
    Private IndexLocation As Integer = Nothing

    Public Property ArrayLocation() As Integer
        Get
            Return IndexLocation
        End Get
        Set(ByVal value As Integer)
            IndexLocation = value
        End Set
    End Property

    'place for the loop building the feature
    Private Loops As List(Of List(Of Entity))

    Public Property ListLoop() As List(Of List(Of Entity))
        Get
            Return Loops
        End Get
        Set(ByVal value As List(Of List(Of Entity)))
            Loops = value
        End Set
    End Property

    'count for Solid Line
    Private SLCount As Integer = Nothing
    Public Property SolidLineCount() As Integer
        Get
            Return SLCount
        End Get
        Set(ByVal value As Integer)
            SLCount = value
        End Set
    End Property

    'count for Solid Line in Bounding Box
    Private SLBCount As Integer = Nothing
    Public Property SolidLineInBoundCount() As Integer
        Get
            Return SLBCount
        End Get
        Set(ByVal value As Integer)
            SLBCount = value
        End Set
    End Property

    'count for Virtual Line
    Private VLCount As Integer = Nothing
    Public Property VirtualLineCount() As Integer
        Get
            Return VLCount
        End Get
        Set(ByVal value As Integer)
            VLCount = value
        End Set
    End Property

    'count for Hidden Line
    Private HLCount As Integer = Nothing
    Public Property HiddenLineCount() As Integer
        Get
            Return HLCount
        End Get
        Set(ByVal value As Integer)
            HLCount = value
        End Set
    End Property

    'count for Solid Arc
    Private SACount As Integer = Nothing
    Public Property SolidArcCount() As Integer
        Get
            Return SACount
        End Get
        Set(ByVal value As Integer)
            SACount = value
        End Set
    End Property

    'count for Hidden Arc in Bounding Box
    Private HACount As Integer = Nothing
    Public Property HiddenArcCount() As Integer
        Get
            Return HACount
        End Get
        Set(ByVal value As Integer)
            HACount = value
        End Set
    End Property

    'status for Solid-SolidInBound Combination
    Private SeqBound As Boolean
    Public Property SequenceSolidBound() As Boolean
        Get
            Return SeqBound
        End Get
        Set(ByVal value As Boolean)
            SeqBound = value
        End Set
    End Property

    'status for Solid-Hidden Combination
    Private SeqHid As Boolean
    Public Property SequenceSolidHidden() As Boolean
        Get
            Return SeqHid
        End Get
        Set(ByVal value As Boolean)
            SeqHid = value
        End Set
    End Property

End Class
