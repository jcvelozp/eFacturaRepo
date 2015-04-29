
Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Configuration

Public Class ClsConexion
    Private cmd As MySqlCommand
    Private con As New MySqlConnection
    'Private connlink As String = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=Kerly-PC)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orclt)));User Id=master;Password=master;Pooling=false;Connection Lifetime=200;; "
    Private connlink As String '"Server=localhost;Database=dbFactura;Uid=root;Pwd=;Convert Zero Datetime=True;Allow Zero Datetime=True"


    Public Sub New()
        connlink = GetConnectionString()
        Console.Out.WriteLine(connlink)
        AbrirConexion()
    End Sub

    Function GetConexion() As MySqlConnection
        Return con
    End Function

    Sub AbrirConexion()
        If Not (Me.con.State = ConnectionState.Open) Then
            con.ConnectionString = connlink
            con.Open()
        End If
    End Sub

    Sub AbrirConexion(ByVal CadenaConexion As String)
        Try
            If Not (Me.con.State = ConnectionState.Open) Then
                con.ConnectionString = CadenaConexion
                con.Open()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub CerrarConexion()
        If (Me.con.State = ConnectionState.Open) Then
            con.Close()
        End If
    End Sub

    Public Function GetCadenaDeConexion() As String
        Return Me.con.ConnectionString
    End Function

    Public Function GetConnectionString(Optional ByVal strConnection As String = "") As String
        'Declare a string to hold the connection string
        Dim sReturn As New String("")
        'Check to see if they provided a connection string name
        If Not String.IsNullOrEmpty(strConnection) Then
            'Retrieve the connection string fromt he app.config
            sReturn = ConfigurationManager.ConnectionStrings(strConnection).ConnectionString
        Else
            'Since they didnt provide the name of the connection string
            'just grab the default on from app.config
            sReturn = ConfigurationManager.ConnectionStrings("ReportUtilities.Properties.Settings.dbfacturaConnectionString").ConnectionString()
        End If
        'Return the connection string to the calling method
        Return sReturn
    End Function


End Class



