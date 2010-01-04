Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices

Public Class ViewProp
    Private ViewName As String

    Public Property ViewType() As String
        Get
            Return ViewName
        End Get
        Set(ByVal value As String)
            ViewName = value
        End Set
    End Property

    Private BB(3) As Line
    Property BoundingBox() As Line()
        Get
            Return BB
        End Get
        Set(ByVal value As Line())
            BB = value
        End Set
    End Property

    Private BoundBox() As Double = New Double() {0, 0, 0, 0, 0, 0}

    Public Property BoundProp(ByVal index As Integer) As String
        Get
            Return BoundBox(index)
        End Get
        Set(ByVal value As String)
            BoundBox(index) = value
        End Set
    End Property

    Private RefPoint() As Double = New Double() {0, 0, 0}

    Public Property RefProp(ByVal index As Integer) As Double
        Get
            Return RefPoint(index)
        End Get
        Set(ByVal value As Double)
            RefPoint(index) = value
        End Set
    End Property

    'variable and method for accessing viewtag (Reference or Corressponding)
    Private ViewID As String
    Public Property ViewTag() As String
        Get
            Return ViewID
        End Get
        Set(ByVal value As String)
            ViewID = value
        End Set
    End Property

    'variabel and method for accessing all circle entities
    Private CircEnt As List(Of Circle)
    Public Property CircleEntities() As List(Of Circle)
        Get
            Return CircEnt
        End Get
        Set(ByVal value As List(Of Circle))
            CircEnt = value
        End Set
    End Property

    'variabel and method for accessing all line entities
    Private LineEnt As List(Of Line)
    Public Property LineEntities() As List(Of Line)
        Get
            Return LineEnt
        End Get
        Set(ByVal value As List(Of Line))
            LineEnt = value
        End Set
    End Property

    'variabel and method for accessing all arc entities
    Private ArcEnt As List(Of Arc)
    Public Property ArcEntities() As List(Of Arc)
        Get
            Return ArcEnt
        End Get
        Set(ByVal value As List(Of Arc))
            ArcEnt = value
        End Set
    End Property

    'variabel and method for accessing all entities
    Private Ent As List(Of Entity)
    Public Property Entities() As List(Of Entity)
        Get
            Return Ent
        End Get
        Set(ByVal value As List(Of Entity))
            Ent = value
        End Set
    End Property

    Private MLoop As List(Of Entity)
    Public Property MainLoop() As List(Of Entity)
        Get
            Return MLoop
        End Get
        Set(ByVal value As List(Of Entity))
            MLoop = value
        End Set
    End Property

    Private Gloop As List(Of List(Of Entity))
    Public Property GroupLoop() As List(Of List(Of Entity))
        Get
            Return Gloop
        End Get
        Set(ByVal value As List(Of List(Of Entity)))
            Gloop = value
        End Set
    End Property

    Private GloopPoints As List(Of List(Of Point3d))
    Public Property GroupLoopPoints() As List(Of List(Of Point3d))
        Get
            Return GloopPoints
        End Get
        Set(ByVal value As List(Of List(Of Point3d)))
            GloopPoints = value
        End Set
    End Property

End Class
