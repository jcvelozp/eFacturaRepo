Public Class General
    Public allowCreate As Boolean = False
    Public allowDelete As Boolean = False

    Public Const passAdmin As String = "ssaaa4"

    Public Sub SetPermissions(ByRef create As Boolean, ByRef delete As Boolean)
        create = allowCreate
        delete = allowDelete
    End Sub
End Class
