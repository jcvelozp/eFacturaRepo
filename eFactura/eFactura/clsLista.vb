Imports System.Collections
Public Class clsLista
    Inherits CollectionBase

    Public Sub add(cli As String)
        MyBase.List.Add(cli)
    End Sub
    Default Public Property Item(index As Integer) As String
        Get
            Return CType(MyBase.List(index), String)
        End Get
        Set(value As String)
            MyBase.List(index) = value
        End Set
    End Property
End Class
