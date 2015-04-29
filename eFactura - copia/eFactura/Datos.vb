Imports System.Xml
Imports ReportUtilities

Module Datos
    Public usuarios As String
    Public clave As String
    Public servidor As String
    Public servicio As String
    Public LOGIN As String
    Public ruta_fotos_vendedores As String
    Public cod_vendedor As String
    Public cod_bodega As String
    Public numero_proforma As String
    Public numero_orden As String
    Public factura_proforma As String
    Public ptoEmi As String
    Public estab As String
    Public nombre_usuario As String
    Public gsMultiEmpresa As String
    Public G_PtoVta As String
    Public Const CG_Vigente = "Vigente"
    Public Const CG_Cod_Alterno_Bodega = "O"

    Public Sub carga_datos()
        Dim Xml As XmlDocument
        Dim NodeList As XmlNodeList
        Dim Node As XmlNode
        Try
            Xml = New XmlDocument()
            Xml.Load(Environment.CurrentDirectory() + "/db.xml")
            NodeList = Xml.SelectNodes("/datos/dato")

            For Each Node In NodeList
                With Node.Attributes
                    usuarios = .GetNamedItem("usuario").Value
                    clave = .GetNamedItem("clave").Value
                    servidor = .GetNamedItem("servidor").Value
                    servicio = .GetNamedItem("servicio").Value
                    estab = .GetNamedItem("estab").Value
                    ptoEmi = .GetNamedItem("ptoEmi").Value
                End With
            Next
        Catch ex As Exception
            Console.WriteLine(ex.GetType.ToString & vbNewLine & ex.Message.ToString)
            Logs.WriteErrorLog(ex)
        Finally
        End Try
    End Sub

End Module
