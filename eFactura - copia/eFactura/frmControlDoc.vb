Imports System.Xml
Imports System.IO
Imports System.Data
Imports ReportUtilities
Imports MySql.Data
Imports System.Globalization
Imports System.Text
Imports ReportUtilities.Tools

Public Class frmControlDoc
    Dim xmldoc As New XmlDocument

    Public InformacionComprobante As SetInformacionComprobante = Nothing
    Public Delegate Sub SetInformacionComprobante(ByVal info As String)
    Public Sub OcurrioError(ByVal ex As Exception, Optional MoreInfo As String = "")
        If Not Configuraciones.ModoServicio Then
            MessageBox.Show("e-tractomaq", "INFO:" & MoreInfo & Environment.NewLine & ex.ToString, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        Logs.WriteErrorLog(ex, MoreInfo, Tools.Configuraciones.EnviarCorreoEnExcepciones)
    End Sub

    Public Sub Alerta(ByVal value As String, Optional MoreInfo As String = "", Optional customEnableNotification As Boolean? = Nothing)
        If Not Configuraciones.ModoServicio Then
            MessageBox.Show("e-tractomaq", value & Environment.NewLine & "INFO:" & Environment.NewLine & MoreInfo, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
        If customEnableNotification Is Nothing Then
            customEnableNotification = Tools.Configuraciones.EnviarCorreoEnExcepciones
        End If
        Logs.WriteLog(value, MoreInfo, customEnableNotification)
    End Sub

    Public Function Generar() As Boolean
        Dim dsEmpresa As DataSet
        Dim cls As New basXML
        Dim flag As Boolean = False
        Try
            carga_datos()
            ConsultarEmpresa()
            ConsultarTipoDoc()
            dsEmpresa = cls.consultaParametro(cmbEmpresa.SelectedValue)
            txtRuc.Text = dsEmpresa.Tables(0).Rows(0).Item("Ruc")
            documentosPendientes("")
            If dgvDocumento.Rows.Count > 0 Then
                btnGenera_Click(Nothing, Nothing)
                flag = True
            End If
            GC.Collect()
        Catch ex As AppDomainUnloadedException
            OcurrioError(ex.InnerException)
        End Try
        Return flag
    End Function

    Private Sub btnSalir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalir.Click
        Try
            Me.Dispose()
            Me.Close()
            ' CerrarF()
        Catch ex As Exception
            OcurrioError(ex.InnerException)
        End Try
    End Sub

    Private Sub btnGenera_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenera.Click
        Try
            lstDocs.Items.Clear()
            For fila = 0 To dgvDocumento.Rows.Count - 2
                Select Case dgvDocumento.Rows(fila).Cells("TIPO").Value
                    Case "01"
                        generarXMLFact(dgvDocumento.Rows(fila).Cells("TIPO").Value, fila)
                        Exit Select
                    Case "05"
                        generarXMLND(dgvDocumento.Rows(fila).Cells("TIPO").Value, fila)
                        Exit Select
                    Case "04"
                        generarXMLNC(dgvDocumento.Rows(fila).Cells("TIPO").Value, fila)
                        Exit Select
                    Case "07"
                        generarXMLRetencion(dgvDocumento.Rows(fila).Cells("TIPO").Value, fila)
                        Exit Select
                    Case "06"
                        generarXMLGuia(dgvDocumento.Rows(fila).Cells("TIPO").Value, fila)
                        Exit Select
                End Select
            Next
            btnBuscar_Click(sender, e)
        Catch ex As Exception
            OcurrioError(ex.InnerException)
        End Try
    End Sub

    Sub generarXMLFact(tipoDoc As String, fila As Integer)
        Dim XMLobj As Xml.XmlTextWriter
        Try
            Dim cls As New basXML
            Dim result As Integer
            Dim ds As New DataSet
            Dim dsDet As New DataSet
            Dim importe As Double = 0
            Dim numDocumento As String = ""
            Dim codNumerico As String = ""
            Dim codClienteInt As String = ""
            Dim vRuta As String = ""
            Dim vRutaF As String = ""
            Dim vNombreArchivo As String = ""
            Dim vRutaAu As String = ""
            Dim rucCliente As String
            Dim claveAcc As String
            Dim claveAut As String
            Dim interfaz As New Interfaz
            Dim sClaveContingencia As String
            Dim nombreCliente As String = ""

            codNumerico = dgvDocumento.Rows(fila).Cells("secuencia").Value
            Do While Len(codNumerico) < 8
                codNumerico = "0" & codNumerico
            Loop

            numDocumento = dgvDocumento.Rows(fila).Cells("numero").Value
            Do While Len(numDocumento) < 8
                numDocumento = "0" & numDocumento
            Loop

            If InformacionComprobante IsNot Nothing Then
                InformacionComprobante("Ejecutando Tipo de Documento:" & tipoDoc & " con Secuencia:" & codNumerico)
            End If

            codClienteInt = dgvDocumento.Rows(fila).Cells("cod_cliente_int").Value
            rucCliente = dgvDocumento.Rows(fila).Cells("identificacion").Value
            nombreCliente = Mid(dgvDocumento.Rows(fila).Cells("AGENTE").Value, 1, 300)

            claveAcc = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAcceso").Value), "", dgvDocumento.Rows(fila).Cells("claveAcceso").Value)
            claveAut = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value), "", dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value)

            vNombreArchivo = "FAC_" & numDocumento
            vRuta = interfaz.RepositorioLocal(rucCliente, tipoDoc) & vNombreArchivo & ".xml"
            vRutaAu = interfaz.RepositorioLocal(rucCliente, tipoDoc) & vNombreArchivo & "_au.xml"

            If claveAcc = "" Then
                ds = cls.consultaDatos(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipoDoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)
                dsDet = cls.consultaDatosDet(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipoDoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)

                If ds.Tables(0).Rows.Count > 0 Then
                    Dim enc As New System.Text.UTF8Encoding
                    XMLobj = New Xml.XmlTextWriter(vRuta, enc)
                    XMLobj.Formatting = Xml.Formatting.Indented
                    XMLobj.Indentation = 3
                    XMLobj.WriteStartDocument()

                    importe = 0
                    XMLobj.WriteStartElement("factura")
                    XMLobj.WriteAttributeString("id", "comprobante")
                    XMLobj.WriteAttributeString("version", "1.0.0")
                    XMLobj.WriteStartElement("infoTributaria")
                    XMLobj.WriteElementString("ambiente", Configuraciones.Get("Ambiente"))
                    XMLobj.WriteElementString("tipoEmision", Configuraciones.Get("TipoEmision"))
                    XMLobj.WriteElementString("razonSocial", Mid(ds.Tables(0).Rows(0).Item("razonSocial"), 1, 300))
                    If IIf(IsDBNull(ds.Tables(0).Rows(0).Item("razoncomercial")), "", ds.Tables(0).Rows(0).Item("razoncomercial")) <> "" Then
                        XMLobj.WriteElementString("nombreComercial", Mid(ds.Tables(0).Rows(0).Item("razoncomercial"), 1, 300))
                    End If

                    XMLobj.WriteElementString("ruc", ds.Tables(0).Rows(0).Item("rucEmpresa"))

                    Dim sClave As String
                    If Configuraciones.Get("TipoEmision") = 1 Then 'NORMAL o CONTINGENCIA
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipoDoc & ds.Tables(0).Rows(0).Item("rucEmpresa") & Configuraciones.Get("Ambiente") & dgvDocumento.Rows(fila).Cells("establecimiento").Value & dgvDocumento.Rows(fila).Cells("pto_Emision").Value & ds.Tables(0).Rows(0).Item("numero") & codNumerico & Configuraciones.Get("TipoEmision")
                    Else
                        sClaveContingencia = cls.consClaveContingencia()
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipoDoc & sClaveContingencia & Configuraciones.Get("TipoEmision")
                        cls.cambiarEstadoClaveContingencia(sClaveContingencia)
                    End If


                    result = sMod11(sClave)
                    XMLobj.WriteElementString("claveAcceso", sClave & result)

                    XMLobj.WriteElementString("codDoc", tipoDoc)
                    XMLobj.WriteElementString("estab", dgvDocumento.Rows(fila).Cells("establecimiento").Value)
                    XMLobj.WriteElementString("ptoEmi", dgvDocumento.Rows(fila).Cells("pto_Emision").Value)
                    XMLobj.WriteElementString("secuencial", numDocumento)
                    XMLobj.WriteElementString("dirMatriz", Mid(ds.Tables(0).Rows(0).Item("dirMatriz"), 1, 300))
                    XMLobj.WriteEndElement() 'infoTributaria

                    'For i = 0 To ds.Tables(0).Rows.Count - 1
                    XMLobj.WriteStartElement("infoFactura")
                    XMLobj.WriteElementString("fechaEmision", ds.Tables(0).Rows(0).Item("fecha").ToString)
                    XMLobj.WriteElementString("dirEstablecimiento", Mid(ds.Tables(0).Rows(0).Item("dirEstab"), 1, 300))
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("ContEsp")) Then
                        If ds.Tables(0).Rows(0).Item("ContEsp") <> "" Then
                            XMLobj.WriteElementString("contribuyenteEspecial", ds.Tables(0).Rows(0).Item("ContEsp"))
                        End If
                    End If
                    XMLobj.WriteElementString("obligadoContabilidad", IIf(IsDBNull(ds.Tables(0).Rows(0).Item("ObligCont")), "", ds.Tables(0).Rows(0).Item("ObligCont")))
                    XMLobj.WriteElementString("tipoIdentificacionComprador", ds.Tables(0).Rows(0).Item("tipoId"))

                    XMLobj.WriteElementString("razonSocialComprador", nombreCliente)
                    rucCliente = ds.Tables(0).Rows(0).Item("ruc")
                    XMLobj.WriteElementString("identificacionComprador", rucCliente)

                    XMLobj.WriteElementString("totalSinImpuestos", Format(ds.Tables(0).Rows(0).Item("totalVenta"), "########0.00"))
                    XMLobj.WriteElementString("totalDescuento", Format(ds.Tables(0).Rows(0).Item("totalDescuento"), "########0.00"))

                    importe = ds.Tables(0).Rows(0).Item("totalVenta")

                    XMLobj.WriteStartElement("totalConImpuestos")
                    If ds.Tables(0).Rows(0).Item("totalSinImpuestos") > 0 Then
                        XMLobj.WriteStartElement("totalImpuesto")
                        XMLobj.WriteElementString("codigo", 2) 'ES IVA
                        XMLobj.WriteElementString("codigoPorcentaje", 0) 'C0DIGO PARA IVA 0%
                        XMLobj.WriteElementString("baseImponible", Format(ds.Tables(0).Rows(0).Item("totalSinImpuestos"), "########0.00"))
                        XMLobj.WriteElementString("tarifa", 0)
                        XMLobj.WriteElementString("valor", Format(0, "########0.00"))
                        XMLobj.WriteEndElement() 'totalImpuesto
                    End If

                    If ds.Tables(0).Rows(0).Item("baseImponibleIva") > 0 Then
                        XMLobj.WriteStartElement("totalImpuesto")
                        XMLobj.WriteElementString("codigo", 2) 'ES IVA
                        XMLobj.WriteElementString("codigoPorcentaje", 2) 'C0DIGO PARA IVA 12%
                        XMLobj.WriteElementString("baseImponible", Format(ds.Tables(0).Rows(0).Item("baseImponibleIva"), "########0.00"))
                        XMLobj.WriteElementString("tarifa", 0)
                        XMLobj.WriteElementString("valor", Format(ds.Tables(0).Rows(0).Item("valorIva"), "########0.00"))
                        XMLobj.WriteEndElement() 'totalImpuesto

                        importe = importe + ds.Tables(0).Rows(0).Item("valorIva")
                    End If

                    XMLobj.WriteEndElement() 'totalConImpuestos

                    XMLobj.WriteElementString("propina", Format(0, "##0.00"))
                    XMLobj.WriteElementString("importeTotal", Format(importe, "########0.00"))
                    XMLobj.WriteElementString("moneda", "DOLAR")

                    XMLobj.WriteEndElement() 'infoFactura

                    XMLobj.WriteStartElement("detalles")
                    Dim i As Integer
                    For i = 0 To dsDet.Tables(0).Rows.Count - 1
                        XMLobj.WriteStartElement("detalle")
                        XMLobj.WriteElementString("codigoPrincipal", Mid(dsDet.Tables(0).Rows(i).Item("codigoPrincipal"), 1, 25))
                        XMLobj.WriteElementString("descripcion", Mid(dsDet.Tables(0).Rows(i).Item("descripcion"), 1, 300))
                        XMLobj.WriteElementString("cantidad", Format(dsDet.Tables(0).Rows(i).Item("cantidad"), "########0.00"))
                        XMLobj.WriteElementString("precioUnitario", Format(dsDet.Tables(0).Rows(i).Item("precioUnitario"), "########0.00"))
                        XMLobj.WriteElementString("descuento", Format(dsDet.Tables(0).Rows(i).Item("descuento"), "########0.00"))
                        XMLobj.WriteElementString("precioTotalSinImpuesto", Format(dsDet.Tables(0).Rows(i).Item("precioTotalSinImpuesto"), "########0.00"))

                        XMLobj.WriteStartElement("impuestos")
                        XMLobj.WriteStartElement("impuesto")

                        If dsDet.Tables(0).Rows(i).Item("baseImponibleIva") > 0 Then
                            XMLobj.WriteElementString("codigo", 2)
                            XMLobj.WriteElementString("codigoPorcentaje", 2)
                            XMLobj.WriteElementString("tarifa", 12)
                            XMLobj.WriteElementString("baseImponible", Format(dsDet.Tables(0).Rows(i).Item("baseImponibleIva"), "########0.00"))
                            XMLobj.WriteElementString("valor", Format(dsDet.Tables(0).Rows(i).Item("valorIva"), "########0.00"))
                        Else

                            XMLobj.WriteElementString("codigo", 2)
                            XMLobj.WriteElementString("codigoPorcentaje", 0)
                            XMLobj.WriteElementString("tarifa", 0)
                            XMLobj.WriteElementString("baseImponible", Format(dsDet.Tables(0).Rows(i).Item("base_sin_iva"), "########0.00"))
                            XMLobj.WriteElementString("valor", Format(dsDet.Tables(0).Rows(i).Item("valorIva"), "########0.00"))
                        End If
                        XMLobj.WriteEndElement() 'impuesto
                        XMLobj.WriteEndElement() 'impuestos
                        XMLobj.WriteEndElement() 'detalle
                    Next
                    XMLobj.WriteEndElement() 'detalles
                    XMLobj.WriteEndElement() 'factura               
                    XMLobj.Close()

                    invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipoDoc, rucCliente, codClienteInt, vRuta, vRutaAu, sClave & result, numDocumento, "FAC", vNombreArchivo, nombreCliente)

                Else
                    Alerta("Error en el XML, número de secuencia:" & dgvDocumento.Rows(fila).Cells("secuencia").Value & " Tipo de Documento:" & tipoDoc)
                End If
            Else
                invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipoDoc, rucCliente, codClienteInt, vRuta, vRutaAu, claveAcc, numDocumento, "FAC", vNombreArchivo, nombreCliente)
            End If
        Catch ex As Exception
            XMLobj.Close()
            OcurrioError(ex)
        End Try
    End Sub

    Private Sub frmControlDoc_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Try

        Catch ex As Exception
            OcurrioError(ex)
        End Try
    End Sub

    Private Sub frmControlDoc_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try

        Catch ex As Exception
            OcurrioError(ex)
        End Try
    End Sub

    Private Sub frmControlDoc_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim dsEmpresa As DataSet
        Dim cls As New basXML

        Try
            carga_datos()
            Configuraciones.Get("TipoEmision")
            ConsultarEmpresa()
            ConsultarTipoDoc()
            dsEmpresa = cls.consultaParametro(cmbEmpresa.SelectedValue)
            txtRuc.Text = dsEmpresa.Tables(0).Rows(0).Item("Ruc")
            documentosPendientes("")
            If dgvDocumento.Rows.Count > 0 Then
                btnGenera_Click(sender, e)
            End If

            GC.Collect()
            Me.Dispose()
            Me.Close()

        Catch ex As AppDomainUnloadedException
            OcurrioError(ex, e.GetType().FullName & " The appdomain MyDomain does not exist.")
        End Try
    End Sub

    Private Sub ConsultarTipoDoc()
        Try
            Dim ds As New DataSet
            Dim cls As New basXML
            ds = cls.ConsultarTipoDoc
            cmbTipoDoc.DataSource = ds.Tables(0)
            cmbTipoDoc.DisplayMember = "nombre"
            cmbTipoDoc.ValueMember = "codigo"
        Catch ex As Exception
            OcurrioError(ex)
        End Try
    End Sub

    Private Sub ConsultarEmpresa()
        Try
            Dim ds As New DataSet
            Dim cls As New basXML
            ds = cls.ConsultarEmpresa
            cmbEmpresa.DataSource = ds.Tables(0)
            cmbEmpresa.DisplayMember = "nombre"
            cmbEmpresa.ValueMember = "codigo"
        Catch ex As Exception
            OcurrioError(ex)
        End Try
    End Sub

    Public Function modulo11(ByVal cadena As String)
        Dim pivote As Integer = 2
        Dim longitudCadena As Integer = cadena.Length()
        Dim b As Integer = 1
        Dim i As Integer
        Dim temporal As Integer
        Dim cantidadTotal As Integer

        Do While Len(cadena) < 8
            cadena = "0" & cadena
        Loop

        For i = 0 To longitudCadena - 1
            If (pivote = 8) Then
                pivote = 2
            End If
            temporal = Integer.Parse("" + cadena.Substring(i, b))
            b = +1
            temporal *= pivote
            pivote = +1
            cantidadTotal += temporal
        Next
        cantidadTotal = 11 - cantidadTotal Mod 11
        If cantidadTotal = 11 Then
            cantidadTotal = 0
        ElseIf cantidadTotal = 10 Then
            cantidadTotal = 1
        End If
        Return cantidadTotal
    End Function

    Private Sub btnBuscar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuscar.Click
        Try
            documentosPendientes(cmbTipoDoc.SelectedValue)
        Catch ex As Exception
            OcurrioError(ex)
        End Try
    End Sub

    Private Sub documentosPendientes(tipodoc As String)
        Try
            Dim ds As New DataSet
            Dim cls As New basXML
            Dim j As Integer

            ds = cls.consDocASubir(cmbEmpresa.SelectedValue, tipodoc)
            dgvDocumento.DataSource = Nothing
            dgvDocumento.DataSource = ds.Tables(0)

            For j = 0 To dgvDocumento.Columns.Count - 1
                dgvDocumento.Columns(j).ReadOnly = True
            Next
            dgvDocumento.Columns("tipo").Visible = False
            dgvDocumento.Columns("secuencia").Visible = False
            dgvDocumento.Columns("establecimiento").Width = 110
            dgvDocumento.Columns("agente").Width = 200
            dgvDocumento.Columns("cod_cliente_int").Visible = False
            dgvDocumento.Columns("empresa").Visible = False
        Catch ex As Exception
            OcurrioError(ex)
        End Try
    End Sub

    Function invertirCadena(cadena As String)
        Dim cadenaInvertida As String = ""
        Dim x As Integer
        For x = cadena.Length() - 1 To x > 0 Step -1
            cadenaInvertida = cadenaInvertida + cadena(x)
        Next
        Return cadenaInvertida
    End Function

    Function obtenerSumaPorDigitos(cadena As String)
        Dim pivote As Integer = 2
        Dim longitudCadena As Integer = cadena.Length
        Dim cantidadTotal As Double = 0
        Dim b As Integer = 1
        Dim i As Integer
        For i = 0 To longitudCadena - 1 Step 1
            If (pivote = 8) Then
                pivote = 2
            End If
            Dim temporal As Integer
            temporal = Integer.Parse(cadena(i))
            b = b + 1
            temporal *= pivote
            pivote = pivote + 1
            cantidadTotal += temporal
        Next
        cantidadTotal = 11 - cantidadTotal Mod 11

        If cantidadTotal = 11 Then
            cantidadTotal = 0
        ElseIf cantidadTotal = 10 Then
            cantidadTotal = 1
        End If

        Return cantidadTotal
    End Function

    Public Function sMod11(cadena As String)
        Return obtenerSumaPorDigitos(invertirCadena(cadena))
    End Function

    Sub generarXMLND(tipodoc As String, fila As Integer)
        Dim XMLobj As Xml.XmlTextWriter
        Try
            Dim cls As New basXML
            Dim result As Integer
            Dim ds As New DataSet
            Dim dsDet As New DataSet
            Dim importe As Double = 0
            Dim numDocumento As String = ""
            Dim codNumerico As String = ""
            Dim codClienteInt As String = ""
            Dim vRuta As String = ""
            Dim vRutaF As String = ""
            Dim vNombreArchivo As String = ""
            Dim vRutaAu As String = ""
            Dim rucCliente As String
            Dim claveAcc As String = ""
            Dim claveAut As String = ""
            Dim sClaveContingencia As String = ""
            Dim nombreCliente As String = ""

            codNumerico = dgvDocumento.Rows(fila).Cells("secuencia").Value
            Do While Len(codNumerico) < 8
                codNumerico = "0" & codNumerico
            Loop

            numDocumento = dgvDocumento.Rows(fila).Cells("numero").Value
            Do While Len(numDocumento) < 8
                numDocumento = "0" & numDocumento
            Loop
            codClienteInt = dgvDocumento.Rows(fila).Cells("cod_cliente_int").Value
            rucCliente = dgvDocumento.Rows(fila).Cells("identificacion").Value
            nombreCliente = Mid(dgvDocumento.Rows(fila).Cells("AGENTE").Value, 1, 300)

            If InformacionComprobante IsNot Nothing Then
                InformacionComprobante("Ejecutando Tipo de Documento:" & tipodoc & " con Secuencia:" & codNumerico)
            End If

            claveAcc = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAcceso").Value), "", dgvDocumento.Rows(fila).Cells("claveAcceso").Value)
            claveAut = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value), "", dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value)

            Dim interfaz As New Interfaz
            vNombreArchivo = "ND_" & numDocumento
            vRuta = interfaz.RepositorioLocal(rucCliente, tipodoc) & vNombreArchivo & ".xml"
            vRutaAu = interfaz.RepositorioLocal(rucCliente, tipodoc) & vNombreArchivo & "_au.xml"

            If claveAcc = "" Then
                ds = cls.consultaDatos(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipodoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)
                dsDet = cls.consultaDatosDet(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipodoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)

                If ds.Tables(0).Rows.Count > 0 Then

                    Dim enc As New System.Text.UTF8Encoding
                    vNombreArchivo = "ND_" & numDocumento

                    XMLobj = New Xml.XmlTextWriter(vRuta, enc)
                    XMLobj.Formatting = Xml.Formatting.Indented
                    XMLobj.Indentation = 3
                    XMLobj.WriteStartDocument()
                    importe = 0
                    XMLobj.WriteStartElement("notaDebito")
                    XMLobj.WriteAttributeString("id", "comprobante")
                    XMLobj.WriteAttributeString("version", "1.0.0")
                    XMLobj.WriteStartElement("infoTributaria")
                    XMLobj.WriteElementString("ambiente", Configuraciones.Get("Ambiente"))
                    XMLobj.WriteElementString("tipoEmision", Configuraciones.Get("TipoEmision"))
                    XMLobj.WriteElementString("razonSocial", Mid(ds.Tables(0).Rows(0).Item("razonSocial"), 1, 300))
                    If IIf(IsDBNull(ds.Tables(0).Rows(fila).Item("razoncomercial")), "", ds.Tables(0).Rows(0).Item("razoncomercial")) <> "" Then
                        XMLobj.WriteElementString("nombreComercial", Mid(ds.Tables(0).Rows(0).Item("razoncomercial"), 1, 300))
                    End If
                    XMLobj.WriteElementString("ruc", ds.Tables(0).Rows(0).Item("ruc"))

                    Dim sClave As String
                    If Configuraciones.Get("TipoEmision") = 1 Then 'NORMAL o CONTINGENCIA
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipodoc & ds.Tables(0).Rows(0).Item("rucEmpresa") & Configuraciones.Get("Ambiente") & dgvDocumento.Rows(fila).Cells("establecimiento").Value & dgvDocumento.Rows(fila).Cells("pto_Emision").Value & ds.Tables(0).Rows(0).Item("numero") & codNumerico & Configuraciones.Get("TipoEmision")
                    Else
                        sClaveContingencia = cls.consClaveContingencia()
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipodoc & sClaveContingencia & Configuraciones.Get("TipoEmision")
                        cls.cambiarEstadoClaveContingencia(sClaveContingencia)
                    End If
                    result = sMod11(sClave)
                    XMLobj.WriteElementString("claveAcceso", sClave & result)

                    XMLobj.WriteElementString("codDoc", tipodoc)
                    XMLobj.WriteElementString("estab", dgvDocumento.Rows(fila).Cells("establecimiento").Value)
                    XMLobj.WriteElementString("ptoEmi", dgvDocumento.Rows(fila).Cells("pto_Emision").Value)
                    XMLobj.WriteElementString("secuencial", numDocumento)
                    XMLobj.WriteElementString("dirMatriz", Mid(ds.Tables(0).Rows(0).Item("dirMatriz"), 1, 300))
                    XMLobj.WriteEndElement() 'infoTributaria
                    XMLobj.WriteStartElement("infoNotaDebito")
                    XMLobj.WriteElementString("fechaEmision", ds.Tables(0).Rows(0).Item("fecha").ToString)
                    XMLobj.WriteElementString("dirEstablecimiento", Mid(ds.Tables(0).Rows(0).Item("dirEstab"), 1, 300))
                    XMLobj.WriteElementString("tipoIdentificacionComprador", ds.Tables(0).Rows(0).Item("tipoId"))
                    XMLobj.WriteElementString("razonSocialComprador", nombreCliente)

                    rucCliente = ds.Tables(0).Rows(0).Item("ruc")
                    XMLobj.WriteElementString("identificacionComprador", rucCliente)

                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("ContEsp")) Then
                        If (ds.Tables(0).Rows(0).Item("ContEsp")) <> "" Then
                            XMLobj.WriteElementString("contribuyenteEspecial", ds.Tables(0).Rows(0).Item("ContEsp"))
                        End If
                    End If
                    XMLobj.WriteElementString("obligadoContabilidad", IIf(IsDBNull(ds.Tables(0).Rows(0).Item("ObligCont")), "", ds.Tables(0).Rows(0).Item("ObligCont")))

                    XMLobj.WriteElementString("rise", "RISE")
                    XMLobj.WriteElementString("codDocModificado", ds.Tables(0).Rows(0).Item("TIPO_COMP_MODIFICA"))
                    XMLobj.WriteElementString("numDocModificado", ds.Tables(0).Rows(0).Item("NUM_COMP_MODIFICA"))
                    XMLobj.WriteElementString("fechaEmisionDocSustento", ds.Tables(0).Rows(0).Item("FEC_COMP_MODIFICA"))
                    XMLobj.WriteElementString("totalSinImpuestos", Format(ds.Tables(0).Rows(0).Item("baseImponibleIva"), "########0.00"))

                    Dim vTotal As Double
                    vTotal = CDbl(Format(ds.Tables(0).Rows(0).Item("baseImponibleIva"), "########0.00")) + CDbl(Format(ds.Tables(0).Rows(0).Item("valorIva"), "########0.00"))
                    XMLobj.WriteStartElement("impuestos")
                    If ds.Tables(0).Rows(0).Item("totalSinImpuestos") > 0 Then
                        XMLobj.WriteStartElement("impuesto")
                        XMLobj.WriteElementString("codigo", 2) 'ES IVA
                        XMLobj.WriteElementString("codigoPorcentaje", 0) 'C0DIGO PARA IVA 0%
                        XMLobj.WriteElementString("tarifa", 0)
                        XMLobj.WriteElementString("baseImponible", Format(ds.Tables(0).Rows(0).Item("totalSinImpuestos"), "########0.00"))
                        XMLobj.WriteElementString("valor", Format(0, "########0.00"))
                        XMLobj.WriteEndElement() 'totalImpuesto
                    End If

                    If ds.Tables(0).Rows(fila).Item("baseImponibleIva") > 0 Then
                        XMLobj.WriteStartElement("impuesto")
                        XMLobj.WriteElementString("codigo", 2) 'ES IVA
                        XMLobj.WriteElementString("codigoPorcentaje", 2) 'C0DIGO PARA IVA 12%
                        XMLobj.WriteElementString("tarifa", 12)
                        XMLobj.WriteElementString("baseImponible", Format(ds.Tables(0).Rows(0).Item("baseImponibleIva"), "########0.00"))
                        XMLobj.WriteElementString("valor", Format(ds.Tables(0).Rows(0).Item("valorIva"), "########0.00"))
                        XMLobj.WriteEndElement() 'totalImpuesto
                    End If

                    XMLobj.WriteEndElement() 'totalConImpuestos

                    XMLobj.WriteElementString("valorTotal", Format(vTotal, "##0.00"))

                    XMLobj.WriteEndElement() 'infoFactura

                    XMLobj.WriteStartElement("motivos")
                    Dim i As Integer
                    For i = 0 To dsDet.Tables(0).Rows.Count - 1
                        XMLobj.WriteStartElement("motivo")
                        XMLobj.WriteElementString("razon", Mid(dsDet.Tables(0).Rows(i).Item("descripcion"), 1, 25))
                        XMLobj.WriteElementString("valor", Mid(dsDet.Tables(0).Rows(i).Item("baseImponibleIva"), 1, 300))
                        XMLobj.WriteEndElement() 'motivo
                    Next
                    XMLobj.WriteEndElement() 'motivos
                    XMLobj.WriteEndElement() 'ND               
                    XMLobj.Close()

                    invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipodoc, rucCliente, codClienteInt, vRuta, vRutaAu, sClave & result, numDocumento, "ND", vNombreArchivo, nombreCliente)
                Else
                    Alerta("Error en el XML, número de secuencia:" & dgvDocumento.Rows(fila).Cells("secuencia").Value & " Tipo de Documento:" & tipodoc)
                End If
            Else
                invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipodoc, rucCliente, codClienteInt, vRuta, vRutaAu, claveAcc, numDocumento, "ND", vNombreArchivo, nombreCliente)
            End If
        Catch ex As Exception
            XMLobj.Close()
            OcurrioError(ex)
            'Finally
        End Try
    End Sub

    Sub generarXMLNC(tipoDoc As String, fila As Integer)
        Dim XMLobj As Xml.XmlTextWriter
        Try
            Dim cls As New basXML
            Dim result As Integer
            Dim ds As New DataSet
            Dim dsDet As New DataSet
            Dim importe As Double = 0
            Dim numDocumento As String = ""
            Dim codNumerico As String = ""
            Dim codClienteInt As String = ""
            Dim vRuta As String = ""
            Dim vRutaF As String = ""
            Dim vNombreArchivo As String = ""
            Dim vRutaAu As String = ""
            Dim rucCliente As String
            Dim claveAcc As String = ""
            Dim claveAut As String = ""
            Dim sClaveContingencia As String = ""
            Dim nombreCliente As String = ""

            codNumerico = dgvDocumento.Rows(fila).Cells("secuencia").Value
            Do While Len(codNumerico) < 8
                codNumerico = "0" & codNumerico
            Loop

            numDocumento = dgvDocumento.Rows(fila).Cells("numero").Value
            Do While Len(numDocumento) < 8
                numDocumento = "0" & numDocumento
            Loop
            codClienteInt = dgvDocumento.Rows(fila).Cells("cod_cliente_int").Value
            rucCliente = dgvDocumento.Rows(fila).Cells("identificacion").Value
            nombreCliente = Mid(dgvDocumento.Rows(fila).Cells("AGENTE").Value, 1, 300)
            If InformacionComprobante IsNot Nothing Then
                InformacionComprobante("Ejecutando Tipo de Documento:" & tipoDoc & " con Secuencia:" & codNumerico)
            End If
            claveAcc = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAcceso").Value), "", dgvDocumento.Rows(fila).Cells("claveAcceso").Value)
            claveAut = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value), "", dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value)

            vNombreArchivo = "NC_" & numDocumento
            Dim interfaz As New Interfaz
            vRuta = interfaz.RepositorioLocal(rucCliente, tipoDoc) & vNombreArchivo & ".xml"
            vRutaAu = interfaz.RepositorioLocal(rucCliente, tipoDoc) & vNombreArchivo & "_au.xml"

            If claveAcc = "" Then
                ds = cls.consultaDatos(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipoDoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)
                dsDet = cls.consultaDatosDet(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipoDoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)

                If ds.Tables(0).Rows.Count > 0 Then

                    Dim enc As New System.Text.UTF8Encoding
                    vNombreArchivo = "NC_" & numDocumento

                    XMLobj = New Xml.XmlTextWriter(vRuta, enc)
                    XMLobj.Formatting = Xml.Formatting.Indented
                    XMLobj.Indentation = 3
                    XMLobj.WriteStartDocument()

                    importe = 0
                    XMLobj.WriteStartElement("notaCredito")
                    XMLobj.WriteAttributeString("id", "comprobante")
                    XMLobj.WriteAttributeString("version", "1.0.0")
                    XMLobj.WriteStartElement("infoTributaria")
                    XMLobj.WriteElementString("ambiente", Configuraciones.Get("Ambiente"))
                    XMLobj.WriteElementString("tipoEmision", Configuraciones.Get("TipoEmision"))
                    XMLobj.WriteElementString("razonSocial", Mid(ds.Tables(0).Rows(0).Item("razonSocial"), 1, 300))
                    If IIf(IsDBNull(ds.Tables(0).Rows(0).Item("razoncomercial")), "", ds.Tables(0).Rows(0).Item("razoncomercial")) <> "" Then
                        XMLobj.WriteElementString("nombreComercial", Mid(ds.Tables(0).Rows(0).Item("razoncomercial"), 1, 300))
                    End If

                    XMLobj.WriteElementString("ruc", ds.Tables(0).Rows(0).Item("rucEmpresa"))

                    Dim sClave As String
                    If Configuraciones.Get("TipoEmision") = 1 Then 'NORMAL o CONTINGENCIA
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipoDoc & ds.Tables(0).Rows(0).Item("rucEmpresa") & Configuraciones.Get("Ambiente") & dgvDocumento.Rows(fila).Cells("establecimiento").Value & dgvDocumento.Rows(fila).Cells("pto_Emision").Value & ds.Tables(0).Rows(0).Item("numero") & codNumerico & Configuraciones.Get("TipoEmision")
                    Else
                        sClaveContingencia = cls.consClaveContingencia()
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipoDoc & sClaveContingencia & Configuraciones.Get("TipoEmision")
                        cls.cambiarEstadoClaveContingencia(sClaveContingencia)
                    End If
                    result = sMod11(sClave)
                    XMLobj.WriteElementString("claveAcceso", sClave & result)

                    XMLobj.WriteElementString("codDoc", tipoDoc)
                    XMLobj.WriteElementString("estab", dgvDocumento.Rows(fila).Cells("establecimiento").Value)
                    XMLobj.WriteElementString("ptoEmi", dgvDocumento.Rows(fila).Cells("pto_Emision").Value)
                    XMLobj.WriteElementString("secuencial", numDocumento)
                    XMLobj.WriteElementString("dirMatriz", Mid(ds.Tables(0).Rows(0).Item("dirMatriz"), 1, 300))
                    XMLobj.WriteEndElement() 'infoTributaria

                    'For i = 0 To ds.Tables(0).Rows.Count - 1
                    XMLobj.WriteStartElement("infoNotaCredito")
                    XMLobj.WriteElementString("fechaEmision", ds.Tables(0).Rows(0).Item("fecha").ToString)
                    XMLobj.WriteElementString("dirEstablecimiento", Mid(ds.Tables(0).Rows(0).Item("dirEstab"), 1, 300))
                    XMLobj.WriteElementString("tipoIdentificacionComprador", ds.Tables(0).Rows(0).Item("tipoId"))
                    XMLobj.WriteElementString("razonSocialComprador", nombreCliente)
                    rucCliente = ds.Tables(0).Rows(0).Item("ruc")
                    XMLobj.WriteElementString("identificacionComprador", rucCliente)

                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("ContEsp")) Then
                        If (ds.Tables(0).Rows(0).Item("ContEsp")) <> "" Then
                            XMLobj.WriteElementString("contribuyenteEspecial", ds.Tables(0).Rows(0).Item("ContEsp"))
                        End If
                    End If
                    XMLobj.WriteElementString("obligadoContabilidad", IIf(IsDBNull(ds.Tables(0).Rows(0).Item("ObligCont")), "", ds.Tables(0).Rows(0).Item("ObligCont")))
                    XMLobj.WriteElementString("rise", "RISE")
                    XMLobj.WriteElementString("codDocModificado", ds.Tables(0).Rows(0).Item("TIPO_COMP_MODIFICA"))
                    XMLobj.WriteElementString("numDocModificado", ds.Tables(0).Rows(0).Item("num_comp_modifica"))
                    XMLobj.WriteElementString("fechaEmisionDocSustento", ds.Tables(0).Rows(0).Item("fec_comp_modifica").ToString)
                    XMLobj.WriteElementString("totalSinImpuestos", Format(ds.Tables(0).Rows(0).Item("totalVenta"), "########0.00"))
                    Dim valorNC As Double = 0
                    valorNC = ds.Tables(0).Rows(0).Item("totalVenta") + ds.Tables(0).Rows(0).Item("valorIva")
                    XMLobj.WriteElementString("valorModificacion", Format(valorNC, "########0.00"))
                    XMLobj.WriteElementString("moneda", "DOLAR")

                    XMLobj.WriteStartElement("totalConImpuestos")

                    If ds.Tables(0).Rows(0).Item("baseImponibleIva") > 0 Then
                        XMLobj.WriteStartElement("totalImpuesto")
                        XMLobj.WriteElementString("codigo", 2) 'ES IVA
                        XMLobj.WriteElementString("codigoPorcentaje", 2) 'C0DIGO PARA IVA 12%
                        XMLobj.WriteElementString("baseImponible", Format(ds.Tables(0).Rows(0).Item("baseImponibleIva"), "########0.00"))
                        XMLobj.WriteElementString("valor", Format(ds.Tables(0).Rows(0).Item("valorIva"), "########0.00"))
                        XMLobj.WriteEndElement() 'totalImpuesto
                    Else
                        XMLobj.WriteStartElement("totalImpuesto")
                        XMLobj.WriteElementString("codigo", 2) 'ES IVA
                        XMLobj.WriteElementString("codigoPorcentaje", 0) 'C0DIGO PARA IVA 0%
                        XMLobj.WriteElementString("baseImponible", Format(ds.Tables(0).Rows(0).Item("totalVenta"), "########0.00"))
                        XMLobj.WriteElementString("valor", Format(ds.Tables(0).Rows(0).Item("valorIva"), "########0.00"))
                        XMLobj.WriteEndElement() 'totalImpuesto
                    End If


                    XMLobj.WriteEndElement() 'totalConImpuestos

                    XMLobj.WriteElementString("motivo", ds.Tables(0).Rows(0).Item("motivo"))

                    XMLobj.WriteEndElement() 'infoNC

                    XMLobj.WriteStartElement("detalles")
                    Dim i As Integer
                    For i = 0 To dsDet.Tables(0).Rows.Count - 1
                        XMLobj.WriteStartElement("detalle")
                        XMLobj.WriteElementString("codigoInterno", Mid(dsDet.Tables(0).Rows(i).Item("codigoPrincipal"), 1, 25))
                        XMLobj.WriteElementString("codigoAdicional", Mid(dsDet.Tables(0).Rows(i).Item("codigoPrincipal"), 1, 25))
                        XMLobj.WriteElementString("descripcion", Mid(dsDet.Tables(0).Rows(i).Item("descripcion"), 1, 300))
                        XMLobj.WriteElementString("cantidad", Format(dsDet.Tables(0).Rows(i).Item("cantidad"), "########0.00"))
                        XMLobj.WriteElementString("precioUnitario", Format(dsDet.Tables(0).Rows(i).Item("precioUnitario"), "########0.00"))
                        XMLobj.WriteElementString("descuento", Format(dsDet.Tables(0).Rows(i).Item("descuento"), "########0.00"))
                        XMLobj.WriteElementString("precioTotalSinImpuesto", Format(dsDet.Tables(0).Rows(i).Item("precioTotalSinImpuesto"), "########0.00"))

                        XMLobj.WriteStartElement("impuestos")
                        XMLobj.WriteStartElement("impuesto")

                        If dsDet.Tables(0).Rows(i).Item("baseImponibleIva") > 0 Then
                            XMLobj.WriteElementString("codigo", 2)
                            XMLobj.WriteElementString("codigoPorcentaje", 2)
                            XMLobj.WriteElementString("tarifa", 12)
                            XMLobj.WriteElementString("baseImponible", Format(dsDet.Tables(0).Rows(i).Item("baseImponibleIva"), "########0.00"))
                            XMLobj.WriteElementString("valor", Format(dsDet.Tables(0).Rows(i).Item("valorIva"), "########0.00"))
                        Else

                            XMLobj.WriteElementString("codigo", 2)
                            XMLobj.WriteElementString("codigoPorcentaje", 0)
                            XMLobj.WriteElementString("tarifa", 0)
                            XMLobj.WriteElementString("baseImponible", Format(dsDet.Tables(0).Rows(i).Item("base_sin_iva"), "########0.00"))
                            XMLobj.WriteElementString("valor", Format(dsDet.Tables(0).Rows(i).Item("valorIva"), "########0.00"))
                        End If
                        XMLobj.WriteEndElement() 'impuesto
                        XMLobj.WriteEndElement() 'impuestos
                        XMLobj.WriteEndElement() 'detalle
                    Next
                    XMLobj.WriteEndElement() 'detalles
                    XMLobj.WriteEndElement() 'NC               
                    XMLobj.Close()
                    invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipoDoc, rucCliente, codClienteInt, vRuta, vRutaAu, sClave & result, numDocumento, "NC", vNombreArchivo, nombreCliente)
                Else
                    'MsgBox("Error", MsgBoxStyle.Exclamation, "XML")
                    Alerta("Error en el XML, número de secuencia:" & dgvDocumento.Rows(fila).Cells("secuencia").Value & " Tipo de Documento:" & tipoDoc)
                End If
            Else
                invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipoDoc, rucCliente, codClienteInt, vRuta, vRutaAu, claveAcc, numDocumento, "NC", vNombreArchivo, nombreCliente)
            End If
        Catch ex As Exception
            XMLobj.Close()
            OcurrioError(ex)
            'Finally

        End Try
    End Sub

    Sub generarXMLRetencion(tipodoc As String, fila As Integer)
        Dim XMLobj As Xml.XmlTextWriter
        Try
            Dim cls As New basXML
            Dim result As Integer
            Dim ds As New DataSet
            Dim dsDet As New DataSet
            Dim importe As Double = 0
            Dim numDocumento As String = ""
            Dim codNumerico As String = ""
            Dim codClienteInt As String = ""
            Dim vRuta As String = ""
            Dim vRutaF As String = ""
            Dim vNombreArchivo As String = ""
            Dim vRutaAu As String = ""
            Dim rucCliente As String
            Dim claveAcc As String = ""
            Dim claveAut As String = ""
            Dim sClaveContingencia As String = ""
            Dim nombrecliente As String = ""

            codNumerico = dgvDocumento.Rows(fila).Cells("secuencia").Value
            Do While Len(codNumerico) < 8
                codNumerico = "0" & codNumerico
            Loop

            numDocumento = dgvDocumento.Rows(fila).Cells("numero").Value
            Do While Len(numDocumento) < 8
                numDocumento = "0" & numDocumento
            Loop
            If InformacionComprobante IsNot Nothing Then
                InformacionComprobante("Ejecutando Tipo de Documento:" & tipodoc & " con Secuencia:" & codNumerico)
            End If
            codClienteInt = dgvDocumento.Rows(fila).Cells("cod_cliente_int").Value
            rucCliente = dgvDocumento.Rows(fila).Cells("identificacion").Value
            nombrecliente = Mid(dgvDocumento.Rows(fila).Cells("AGENTE").Value, 1, 300)

            claveAcc = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAcceso").Value), "", dgvDocumento.Rows(fila).Cells("claveAcceso").Value)
            claveAut = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value), "", dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value)

            vNombreArchivo = "RET_" & numDocumento
            Dim interfaz As New Interfaz
            vRuta = interfaz.RepositorioLocal(rucCliente, tipodoc) & vNombreArchivo & ".xml"
            vRutaAu = interfaz.RepositorioLocal(rucCliente, tipodoc) & vNombreArchivo & "_au.xml"

            If claveAcc = "" Then
                ds = cls.consultaDatos(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipodoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)
                dsDet = cls.consultaDatosDet(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipodoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)

                If ds.Tables(0).Rows.Count > 0 Then
                    Dim enc As New System.Text.UTF8Encoding
                    vNombreArchivo = "RET_" & numDocumento

                    XMLobj = New Xml.XmlTextWriter(vRuta, enc)
                    XMLobj.Formatting = Xml.Formatting.Indented
                    XMLobj.Indentation = 3
                    XMLobj.WriteStartDocument()
                    importe = 0
                    XMLobj.WriteStartElement("comprobanteRetencion")
                    XMLobj.WriteAttributeString("id", "comprobante")
                    XMLobj.WriteAttributeString("version", "1.0.0")
                    XMLobj.WriteStartElement("infoTributaria")
                    XMLobj.WriteElementString("ambiente", Configuraciones.Get("Ambiente"))
                    XMLobj.WriteElementString("tipoEmision", Configuraciones.Get("TipoEmision"))
                    XMLobj.WriteElementString("razonSocial", Mid(ds.Tables(0).Rows(0).Item("razonSocial"), 1, 300))
                    If IIf(IsDBNull(ds.Tables(0).Rows(0).Item("razoncomercial")), "", ds.Tables(0).Rows(0).Item("razoncomercial")) <> "" Then
                        XMLobj.WriteElementString("nombreComercial", Mid(ds.Tables(0).Rows(0).Item("razoncomercial"), 1, 300))
                    End If

                    XMLobj.WriteElementString("ruc", ds.Tables(0).Rows(0).Item("rucEmpresa"))

                    Dim sClave As String
                    If Configuraciones.Get("TipoEmision") = 1 Then 'NORMAL o CONTINGENCIA
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipodoc & ds.Tables(0).Rows(0).Item("rucEmpresa") & Configuraciones.Get("Ambiente") & dgvDocumento.Rows(fila).Cells("establecimiento").Value & dgvDocumento.Rows(fila).Cells("pto_Emision").Value & ds.Tables(0).Rows(0).Item("numero") & codNumerico & Configuraciones.Get("TipoEmision")
                    Else
                        sClaveContingencia = cls.consClaveContingencia()
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipodoc & sClaveContingencia & Configuraciones.Get("TipoEmision")
                        cls.cambiarEstadoClaveContingencia(sClaveContingencia)
                    End If
                    result = sMod11(sClave)
                    XMLobj.WriteElementString("claveAcceso", sClave & result)

                    XMLobj.WriteElementString("codDoc", tipodoc)
                    XMLobj.WriteElementString("estab", dgvDocumento.Rows(fila).Cells("establecimiento").Value)
                    XMLobj.WriteElementString("ptoEmi", dgvDocumento.Rows(fila).Cells("pto_Emision").Value)
                    XMLobj.WriteElementString("secuencial", numDocumento)
                    XMLobj.WriteElementString("dirMatriz", Mid(ds.Tables(0).Rows(0).Item("dirMatriz"), 1, 300))
                    XMLobj.WriteEndElement() 'infoTributaria
                    XMLobj.WriteStartElement("infoCompRetencion")
                    XMLobj.WriteElementString("fechaEmision", ds.Tables(0).Rows(0).Item("fecha").ToString)
                    XMLobj.WriteElementString("dirEstablecimiento", Mid(ds.Tables(0).Rows(0).Item("dirEstab"), 1, 300))
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("ContEsp")) Then
                        If (ds.Tables(0).Rows(0).Item("ContEsp")) <> "" Then
                            XMLobj.WriteElementString("contribuyenteEspecial", ds.Tables(0).Rows(0).Item("ContEsp"))
                        End If
                    End If
                    XMLobj.WriteElementString("obligadoContabilidad", IIf(IsDBNull(ds.Tables(0).Rows(0).Item("ObligCont")), "", ds.Tables(0).Rows(0).Item("ObligCont")))
                    XMLobj.WriteElementString("tipoIdentificacionSujetoRetenido", ds.Tables(0).Rows(0).Item("tipoId"))
                    XMLobj.WriteElementString("razonSocialSujetoRetenido", nombrecliente)
                    rucCliente = ds.Tables(0).Rows(0).Item("ruc")
                    XMLobj.WriteElementString("identificacionSujetoRetenido", rucCliente)

                    XMLobj.WriteElementString("periodoFiscal", Mid(ds.Tables(0).Rows(0).Item("fechaEmision").ToString, 4))
                    XMLobj.WriteEndElement() 'infoRetencion

                    XMLobj.WriteStartElement("impuestos")
                    Dim i As Integer
                    For i = 0 To dsDet.Tables(0).Rows.Count - 1
                        XMLobj.WriteStartElement("impuesto")
                        XMLobj.WriteElementString("codigo", dsDet.Tables(0).Rows(i).Item("codigo"))
                        XMLobj.WriteElementString("codigoRetencion", dsDet.Tables(0).Rows(i).Item("codigoRetencion"))
                        XMLobj.WriteElementString("baseImponible", Format(dsDet.Tables(0).Rows(i).Item("base"), "########0.00"))
                        XMLobj.WriteElementString("porcentajeRetener", dsDet.Tables(0).Rows(i).Item("porcentaje"))
                        XMLobj.WriteElementString("valorRetenido", Format(dsDet.Tables(0).Rows(i).Item("valorRetenido"), "########0.00"))
                        XMLobj.WriteElementString("codDocSustento", dsDet.Tables(0).Rows(i).Item("codSustento"))
                        XMLobj.WriteElementString("numDocSustento", dsDet.Tables(0).Rows(i).Item("numSustento"))
                        XMLobj.WriteElementString("fechaEmisionDocSustento", dsDet.Tables(0).Rows(i).Item("fechaEmision").ToString)
                        XMLobj.WriteEndElement() 'impuesto
                    Next
                    XMLobj.WriteEndElement() 'impuestos
                    XMLobj.WriteEndElement() 'Retencion              
                    XMLobj.Close()
                    invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipodoc, rucCliente, codClienteInt, vRuta, vRutaAu, sClave & result, numDocumento, "RET", vNombreArchivo, nombrecliente)
                Else
                    Alerta("Error en el XML, número de secuencia:" & dgvDocumento.Rows(fila).Cells("secuencia").Value & " Tipo de Documento:" & tipodoc)
                End If
            Else
                invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipodoc, rucCliente, codClienteInt, vRuta, vRutaAu, claveAcc, numDocumento, "RET", vNombreArchivo, nombrecliente)
            End If
        Catch ex As Exception
            XMLobj.Close()
            OcurrioError(ex)
            'Finally
        End Try
    End Sub

    Sub generarXMLGuia(tipoDoc As String, fila As Integer)
        Dim XMLobj As Xml.XmlTextWriter
        Try
            Dim cls As New basXML
            Dim result As Integer
            Dim ds As New DataSet
            Dim dsDet As New DataSet
            Dim importe As Double = 0
            Dim numDocumento As String = ""
            Dim codNumerico As String = ""
            Dim codClienteInt As String = ""
            Dim vRuta As String = ""
            Dim vRutaF As String = ""
            Dim vNombreArchivo As String = ""
            Dim vRutaAu As String = ""
            Dim rucCliente As String
            Dim claveAcc As String = ""
            Dim claveAut As String = ""
            Dim sClaveContingencia As String = ""
            Dim nombreCliente As String = ""

            codNumerico = dgvDocumento.Rows(fila).Cells("secuencia").Value
            Do While Len(codNumerico) < 8
                codNumerico = "0" & codNumerico
            Loop

            numDocumento = dgvDocumento.Rows(fila).Cells("numero").Value
            Do While Len(numDocumento) < 8
                numDocumento = "0" & numDocumento
            Loop

            codClienteInt = dgvDocumento.Rows(fila).Cells("cod_cliente_int").Value
            rucCliente = dgvDocumento.Rows(fila).Cells("identificacion").Value
            nombreCliente = Mid(dgvDocumento.Rows(fila).Cells("AGENTE").Value, 1, 300)
            If InformacionComprobante IsNot Nothing Then
                InformacionComprobante("Ejecutando Tipo de Documento:" & tipoDoc & " con Secuencia:" & codNumerico)
            End If

            claveAcc = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAcceso").Value), "", dgvDocumento.Rows(fila).Cells("claveAcceso").Value)
            claveAut = IIf(IsDBNull(dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value), "", dgvDocumento.Rows(fila).Cells("claveAutorizacion").Value)

            vNombreArchivo = "GR_" & numDocumento

            Dim interfaz As New Interfaz
            vRuta = interfaz.RepositorioLocal(rucCliente, tipoDoc) & vNombreArchivo & ".xml"
            vRutaAu = interfaz.RepositorioLocal(rucCliente, tipoDoc) & vNombreArchivo & "_au.xml"

            If claveAcc = "" Then
                ds = cls.consultaDatos(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipoDoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)
                dsDet = cls.consultaDatosDet(cmbEmpresa.SelectedValue, Configuraciones.Get("Ambiente"), Configuraciones.Get("TipoEmision"), tipoDoc, codNumerico, dgvDocumento.Rows(fila).Cells("secuencia").Value, result)

                If ds.Tables(0).Rows.Count > 0 Then

                    Dim enc As New System.Text.UTF8Encoding
                    vNombreArchivo = "GR_" & numDocumento

                    XMLobj = New Xml.XmlTextWriter(vRuta, enc)
                    XMLobj.Formatting = Xml.Formatting.Indented
                    XMLobj.Indentation = 3
                    XMLobj.WriteStartDocument()
                    importe = 0
                    XMLobj.WriteStartElement("guiaRemision")
                    XMLobj.WriteAttributeString("id", "comprobante")
                    XMLobj.WriteAttributeString("version", "1.0.0")
                    XMLobj.WriteStartElement("infoTributaria")
                    XMLobj.WriteElementString("ambiente", Configuraciones.Get("Ambiente"))
                    XMLobj.WriteElementString("tipoEmision", Configuraciones.Get("TipoEmision"))
                    XMLobj.WriteElementString("razonSocial", Mid(ds.Tables(0).Rows(0).Item("razonSocial"), 1, 300).Replace(".", " "))
                    If IIf(IsDBNull(ds.Tables(0).Rows(0).Item("razoncomercial")), "", ds.Tables(0).Rows(0).Item("razoncomercial")) <> "" Then
                        XMLobj.WriteElementString("nombreComercial", Mid(ds.Tables(0).Rows(0).Item("razoncomercial"), 1, 300))
                    End If

                    XMLobj.WriteElementString("ruc", ds.Tables(0).Rows(0).Item("rucEmpresa"))


                    Dim sClave As String
                    If Configuraciones.Get("TipoEmision") = 1 Then 'NORMAL o CONTINGENCIA
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipoDoc & ds.Tables(0).Rows(0).Item("rucEmpresa") & Configuraciones.Get("Ambiente") & dgvDocumento.Rows(fila).Cells("establecimiento").Value & dgvDocumento.Rows(fila).Cells("pto_Emision").Value & ds.Tables(0).Rows(0).Item("numero") & codNumerico & Configuraciones.Get("TipoEmision")
                    Else
                        sClaveContingencia = cls.consClaveContingencia()
                        sClave = ds.Tables(0).Rows(0).Item("fecha").ToString.Replace("/", "") & tipoDoc & sClaveContingencia & Configuraciones.Get("TipoEmision")
                        cls.cambiarEstadoClaveContingencia(sClaveContingencia)
                    End If
                    result = sMod11(sClave)
                    XMLobj.WriteElementString("claveAcceso", sClave & result)

                    XMLobj.WriteElementString("codDoc", tipoDoc)
                    XMLobj.WriteElementString("estab", dgvDocumento.Rows(fila).Cells("establecimiento").Value)
                    XMLobj.WriteElementString("ptoEmi", dgvDocumento.Rows(fila).Cells("pto_Emision").Value)
                    XMLobj.WriteElementString("secuencial", numDocumento)
                    XMLobj.WriteElementString("dirMatriz", Mid(ds.Tables(0).Rows(0).Item("dirMatriz"), 1, 300))
                    XMLobj.WriteEndElement() 'infoTributaria
                    XMLobj.WriteStartElement("infoGuiaRemision")
                    XMLobj.WriteElementString("dirEstablecimiento", ds.Tables(0).Rows(0).Item("dirEstab"))
                    XMLobj.WriteElementString("dirPartida", Mid(ds.Tables(0).Rows(0).Item("dirPartida"), 1, 300))
                    XMLobj.WriteElementString("razonSocialTransportista", nombreCliente)
                    rucCliente = ds.Tables(0).Rows(0).Item("ruc")

                    XMLobj.WriteElementString("tipoIdentificacionTransportista", ds.Tables(0).Rows(0).Item("tipoId"))
                    XMLobj.WriteElementString("rucTransportista", ds.Tables(0).Rows(0).Item("ruc"))
                    XMLobj.WriteElementString("rise", "RISE")
                    XMLobj.WriteElementString("obligadoContabilidad", IIf(IsDBNull(ds.Tables(0).Rows(0).Item("ObligCont")), "", ds.Tables(0).Rows(0).Item("ObligCont")))
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("ContEsp")) Then
                        If (ds.Tables(0).Rows(0).Item("ContEsp")) <> "" Then
                            XMLobj.WriteElementString("contribuyenteEspecial", ds.Tables(0).Rows(0).Item("ContEsp"))
                        End If
                    End If
                    XMLobj.WriteElementString("fechaIniTransporte", ds.Tables(0).Rows(0).Item("fecha_ini_tras").ToString)
                    XMLobj.WriteElementString("fechaFinTransporte", ds.Tables(0).Rows(0).Item("fecha_fin_tras").ToString)
                    XMLobj.WriteElementString("placa", ds.Tables(0).Rows(0).Item("placa"))

                    XMLobj.WriteEndElement() 'infoGuiaRemision

                    XMLobj.WriteStartElement("destinatarios")
                    Dim i As Integer
                    For i = 0 To dsDet.Tables(0).Rows.Count - 1
                        XMLobj.WriteStartElement("destinatario")
                        XMLobj.WriteElementString("identificacionDestinatario", Mid(dsDet.Tables(0).Rows(i).Item("idDestinatario"), 1, 25))
                        XMLobj.WriteElementString("razonSocialDestinatario", Mid(dsDet.Tables(0).Rows(i).Item("razonSocialDest"), 1, 25))
                        XMLobj.WriteElementString("dirDestinatario", Mid(dsDet.Tables(0).Rows(i).Item("dirDestinatario"), 1, 300))
                        XMLobj.WriteElementString("motivoTraslado", dsDet.Tables(0).Rows(i).Item("motivoTraslado").ToString)
                        If dsDet.Tables(0).Rows(i).Item("docAduanero").ToString <> "" Then
                            XMLobj.WriteElementString("docAduaneroUnico", dsDet.Tables(0).Rows(i).Item("docAduanero"))
                        End If
                        If dsDet.Tables(0).Rows(i).Item("codEstabDest").ToString <> "" Then
                            XMLobj.WriteElementString("codEstabDestino", dsDet.Tables(0).Rows(i).Item("codEstabDest"))
                        End If
                        XMLobj.WriteElementString("ruta", dsDet.Tables(0).Rows(i).Item("ruta"))
                        XMLobj.WriteElementString("codDocSustento", dsDet.Tables(0).Rows(i).Item("docSustento"))
                        XMLobj.WriteElementString("numDocSustento", dsDet.Tables(0).Rows(i).Item("numSustento"))
                        XMLobj.WriteElementString("numAutDocSustento", dsDet.Tables(0).Rows(i).Item("autSustento"))
                        XMLobj.WriteElementString("fechaEmisionDocSustento", dsDet.Tables(0).Rows(i).Item("fechaSustento").ToString)

                        XMLobj.WriteStartElement("detalles")
                        XMLobj.WriteStartElement("detalle")

                        XMLobj.WriteElementString("codigoInterno", dsDet.Tables(0).Rows(i).Item("codInterno"))
                        If dsDet.Tables(0).Rows(i).Item("codAdicional").ToString <> "" Then
                            XMLobj.WriteElementString("codigoAdicional", dsDet.Tables(0).Rows(i).Item("codAdicional"))
                        End If
                        XMLobj.WriteElementString("descripcion", dsDet.Tables(0).Rows(i).Item("descripcion"))
                        XMLobj.WriteElementString("cantidad", Format(dsDet.Tables(0).Rows(i).Item("cantidad"), "########0.00"))

                        XMLobj.WriteEndElement() 'detalle
                        XMLobj.WriteEndElement() 'detalles
                        XMLobj.WriteEndElement() 'destinatario
                    Next

                    XMLobj.WriteEndElement() 'Guia
                    XMLobj.Close()

                    invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipoDoc, rucCliente, codClienteInt, vRuta, vRutaAu, sClave & result, numDocumento, "GR", vNombreArchivo, nombreCliente)

                Else
                    Alerta("Error en el XML, número de secuencia:" & dgvDocumento.Rows(fila).Cells("secuencia").Value & " Tipo de Documento:" & tipoDoc)
                End If
            Else
                invocarWS(dgvDocumento.Rows(fila).Cells("secuencia").Value, tipoDoc, rucCliente, codClienteInt, vRuta, vRutaAu, claveAcc, numDocumento, "GR", vNombreArchivo, nombreCliente)
            End If
        Catch ex As Exception
            If XMLobj IsNot Nothing Then
                XMLobj.Close()
            End If
            OcurrioError(ex)
            'Finally
        End Try
    End Sub

    Sub invocarWS(secuencia, tipodoc, rucCliente, codClienteInt, vRuta, vRutaAu, sClaveAcc, num_doc, sigla, nombreArchivo, nombreCliente)
        Dim cls As New basXML
        Dim str As String = ""
        Dim strAu As String = ""
        Dim strFecAu As String = ""
        Dim dsAu As New DataSet
        Dim interfaz As New ReportUtilities.Interfaz()
        Dim ds As DataSet
        Dim emision As Integer
        Dim clave As String

        Try
            dsAu = cls.verificaDocumento(secuencia, tipodoc, codClienteInt, vRuta, vRutaAu)
            If dsAu.Tables(0).Rows.Count > 0 Then
                If Not String.IsNullOrEmpty(dsAu.Tables(0).Rows(0).Item("num_autoriza").ToString) Then
                    Alerta("Documento con secuencia:" & secuencia & " y tipo de documento:" & tipodoc & ", ya se encuentra autorizado")
                End If
            End If

            If dsAu.Tables(0).Rows.Count = 0 Then

                ds = cls.verificarClaveAcceso(secuencia, tipodoc)
                If ds.Tables(0).Rows.Count > 0 Then
                    emision = ds.Tables(0).Rows(0).Item("emision")
                    clave = ds.Tables(0).Rows(0).Item("clave_acceso")
                Else
                    emision = Configuraciones.Get("TipoEmision")
                    clave = ""
                End If

                If emision = 2 And clave <> "" Then
                    If recepcionDocumentos(vRuta, sClaveAcc, secuencia, tipodoc, sigla, num_doc, emision) Then
                        If autorizaDocumentos(sClaveAcc, secuencia, tipodoc, sigla, num_doc, vRutaAu, emision) Then

                        End If
                    End If
                Else

                    If emision = 1 Then 'NORMAL o CONTINGENCIA
                        If recepcionDocumentos(vRuta, sClaveAcc, secuencia, tipodoc, sigla, num_doc, emision) Then
                            If cls.insertarDocumento(secuencia, tipodoc, codClienteInt, nombreArchivo, strAu, sClaveAcc, rucCliente, emision) Then
                                If autorizaDocumentos(sClaveAcc, secuencia, tipodoc, sigla, num_doc, vRutaAu, emision) Then
                                    If generarRide(rucCliente, secuencia, tipodoc, sigla, num_doc) Then
                                        If enviarDocumentoMail(secuencia, tipodoc, rucCliente, sigla, num_doc, nombreCliente) Then

                                        Else
                                            Alerta("Correo Electronico no ha sido enviado al cliente " & rucCliente & " " & nombreCliente & " secuencia:" & secuencia & " y tipo de documento:" & tipodoc)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        If cls.insertarDocumento(secuencia, tipodoc, codClienteInt, nombreArchivo, strAu, sClaveAcc, rucCliente, emision) Then
                            If generarRide(rucCliente, secuencia, tipodoc, sigla, num_doc) Then
                                If enviarDocumentoMail(secuencia, tipodoc, rucCliente, sigla, num_doc, nombreCliente) Then

                                Else
                                    Alerta("Correo Electronico no ha sido enviado al cliente " & rucCliente & " " & nombreCliente & " secuencia:" & secuencia & " y tipo de documento:" & tipodoc)
                                End If
                            End If
                        End If
                    End If
                End If
            Else
                Alerta("Documento con secuencia:" & secuencia & " y tipo de documento:" & tipodoc & ", ya se encuentra autorizado")
            End If
        Catch ex As Exception
            OcurrioError(ex)
            Exit Sub
        End Try
    End Sub

    Private Function recepcionDocumentos(vruta As String, sClaveAcc As String, secuencia As String, tipodoc As String, sigla As String, num_doc As String, emision As Integer)
        Dim wsRecepcion As New RecepcionComp.RecepcionComprobanteClient
        Dim xmlnodeEstado As XmlNodeList
        Dim xmlnode As XmlNodeList
        Dim str As String = ""
        Dim s1 As String = ""
        Dim sCodigo As String = ""
        Dim doc As XDocument
        Dim cls As New basXML
        Dim brecibido As Boolean
        Dim ds As DataSet

        brecibido = True
        'If emision = 1 Then 'si es normal

        doc = XDocument.Load(vruta)
        Try
            Dim s_xml As String = "<?xml version=""1.0"" encoding=""utf-8""?>" & doc.ToString()

            Dim b = System.Text.Encoding.UTF8.GetBytes(s_xml)
            Dim s2 = System.Text.Encoding.UTF8.GetString(b)

            s1 = wsRecepcion.validarComprobante("1|" & Configuraciones.Get("Ambiente"), s2)
            If basXML.TryParseXml(s1) Then
                xmldoc.LoadXml(s1)
            Else
                brecibido = False
            End If
        Catch ex As Exception
            brecibido = False
            OcurrioError(ex, "Error en la recepción de documentos. " & s1)
            Return brecibido
        End Try

        Try
            xmlnodeEstado = xmldoc.GetElementsByTagName("estado")
            If (xmlnodeEstado IsNot Nothing) Then
                If xmlnodeEstado.Count > 0 Then
                    If xmlnodeEstado(0).ChildNodes.Count > 0 Then
                        If xmlnodeEstado(0).ChildNodes.Item(0).InnerText.Trim() = "DEVUELTA" Then
                            Alerta("Documento con secuencia:" & secuencia & " y tipo de documento:" & tipodoc & ", DEVUELTO", "", False)

                            xmlnode = xmldoc.GetElementsByTagName("identificador")
                            sCodigo = xmlnode(0).ChildNodes.Item(0).InnerText.Trim()

                            xmlnode = xmldoc.GetElementsByTagName("mensaje")
                            str = xmlnode(0).ChildNodes.Item(0).InnerText.Trim()

                            xmlnode = xmldoc.GetElementsByTagName("informacionAdicional")
                            str = str & " " & xmlnode(0).ChildNodes.Item(0).InnerText.Trim()

                            If sCodigo <> "70" Then
                                'notificaError(str, sigla, num_doc)
                            End If
                            cls.logError(secuencia, tipodoc, "R", str)
                            brecibido = False
                            Return brecibido
                        End If
                    Else
                        Logs.WriteLog("El xmlnodeEstado no contiene nodos hijos")
                    End If
                Else
                    Logs.WriteLog("El xmlnodeEstado no contiene elementos")
                End If
            Else
                Logs.WriteLog("El valor de xmlnodeEstado es null al intentar obtener el estado")
            End If
        Catch ex As Exception
            cls.logError(secuencia, tipodoc, "R", str)
            OcurrioError(ex, "Error en recepción de documento " & tipodoc & " secuencia:" & secuencia & "tipo de error: R" & " Mensaje de Error:" & str)
            brecibido = False
            Return brecibido
        End Try
        'End If
        Return brecibido
    End Function

    Private Function autorizaDocumentos(sClaveAcc As String, secuencia As String, tipodoc As String, sigla As String, num_doc As String, vRutaAu As String, emision As Integer)
        Dim xmlnodeEstado As XmlNodeList
        Dim xmlnode As XmlNodeList
        Dim str As String = ""
        Dim cls As New basXML
        Dim xmlnodeAutoriza As XmlNodeList
        Dim xmlnodeFecAutoriza As XmlNodeList
        Dim strAu As String = ""
        Dim strFecAu As String = ""
        Dim xmldoc1 As New XmlDocument
        Dim iPosicion As Integer
        Dim bAutorizado As Boolean
        bAutorizado = True
        iPosicion = -1


        'If emision = 1 Then 'si es normal
        Try
            Dim wsautoriza As New AutorizacionComp.AutorizacionComprobanteClient
            Dim respuesta As String = wsautoriza.autorizacionComprobante("1|" & Configuraciones.Get("Ambiente"), sClaveAcc)

            If basXML.TryParseXml(respuesta) Then
                xmldoc1.LoadXml(respuesta)
            Else
                Return False
            End If

            If xmldoc1 Is Nothing Then
                Logs.WriteLog("Error al cargar el XML en la funcion autorizaDocumentos", "Respuesta para cargar el XML:" & respuesta, False)
                Return False
            End If

            xmlnodeEstado = xmldoc1.GetElementsByTagName("estado")

            For i = 0 To xmlnodeEstado.Count - 1
                If xmlnodeEstado(i).ChildNodes.Item(0).InnerText.Trim() = "AUTORIZADO" Then
                    iPosicion = i
                End If
            Next

            If iPosicion = -1 Then
                iPosicion = 0
            End If

            Dim mensaje As String = String.Empty
            If xmlnodeEstado.Count = 0 Then
                'No tiene datos
                Return False
            ElseIf xmlnodeEstado(iPosicion).ChildNodes.Count = 0 Then
                'no tiene datos
                Return False
            ElseIf String.IsNullOrEmpty(xmlnodeEstado(iPosicion).ChildNodes.Item(0).InnerText) Then
                'no tiene datos
                Return False
            Else
                mensaje = xmlnodeEstado(iPosicion).ChildNodes.Item(0).InnerText.Trim()
            End If
            If mensaje = "NO AUTORIZADO" Then
                'MsgBox("NO AUTORIZADO")sistemas2014
                Alerta("Secuencia de Documento:" & secuencia & " Tipo de Documento:" & tipodoc & " no fue autorizado")
                xmlnode = xmldoc1.GetElementsByTagName("mensaje")


                str = xmlnode(iPosicion).ChildNodes.Item(0).InnerText.Trim()

                If str = "43" Then
                    str = "Clave de acceso registrada"
                    cls.logError(secuencia, tipodoc, "A", str)
                    Alerta("Secuencia de Documento:" & secuencia & " Tipo de Documento:" & tipodoc & " no fue autorizado por:" & str & "Tipo de Error:A")
                    'notificaError(str, sigla, num_doc)
                    bAutorizado = False
                    Return bAutorizado
                End If

                xmlnode = xmldoc1.GetElementsByTagName("informacionAdicional")
                Try
                    str = xmlnode(iPosicion).ChildNodes.Item(0).InnerText.Trim()
                    cls.logError(secuencia, tipodoc, "A", str)
                    Alerta("Secuencia de Documento:" & secuencia & " Tipo de Documento:" & tipodoc & " no fue autorizado por:" & str & "Tipo de Error:A")
                    'notificaError(str, sigla, num_doc)
                    bAutorizado = False
                Catch ex As Exception
                    OcurrioError(ex, "Autorizado=False")
                    bAutorizado = False
                    Return bAutorizado
                End Try
                bAutorizado = False
                Return bAutorizado
            Else
                xmlnodeEstado = xmldoc1.GetElementsByTagName("estado")

                For i = 0 To xmlnodeEstado.Count - 1
                    If xmlnodeEstado(i).ChildNodes.Item(0).InnerText.Trim() = "AUTORIZADO" Then
                        iPosicion = i
                    End If
                Next

                xmlnodeAutoriza = xmldoc1.GetElementsByTagName("numeroAutorizacion")
                'For i = 0 To xmlnodeAutoriza.Count - 1
                xmlnodeAutoriza(0).ChildNodes.Item(0).InnerText.Trim()
                strAu = xmlnodeAutoriza(0).ChildNodes.Item(0).InnerText.Trim()
                'Next

                xmlnodeFecAutoriza = xmldoc1.GetElementsByTagName("fechaAutorizacion")
                'For i = 0 To xmlnodeFecAutoriza.Count - 1
                xmlnodeFecAutoriza(iPosicion).ChildNodes.Item(0).InnerText.Trim()
                strFecAu = xmlnodeFecAutoriza(iPosicion).ChildNodes.Item(0).InnerText.Trim()
                'Next
            End If

            'Guardar archivo firmado
            xmlnode = xmldoc1.GetElementsByTagName("autorizacion")
            str = xmlnode(iPosicion).InnerXml

            Dim xd As XmlDocument
            xd = New XmlDocument()
            xd.LoadXml("<autorizaciones>" & str & "</autorizaciones>")
            xd.Save(vRutaAu)

            cls.actualizaDocumento(secuencia, tipodoc, strAu, strFecAu)
            cls.actualizaEstado(secuencia, tipodoc)

        Catch ex As Exception
            cls.logError(secuencia, tipodoc, "A", str & ex.Message)
            OcurrioError(ex, "Secuencia de Documento:" & secuencia & " Tipo de Documento:" & tipodoc & " no fue autorizado por:" & str & "Tipo de Error:A")
            bAutorizado = False
            Return bAutorizado
        End Try
        'Else
        'bAutorizado = False
        'End If
        Return bAutorizado
    End Function

    Private Function generarRide(rucCliente As String, secuencia As String, tipodoc As String, sigla As String, num_doc As String)
        Dim interfaz As New ReportUtilities.Interfaz()
        Dim bRetorna As New Boolean
        bRetorna = True

        Try
            Dim count As Integer = dgvDocumento.Rows.Count
            If interfaz.GenerarRIDE(rucCliente, secuencia, tipodoc, sigla & "_" & num_doc) Then
                lstDocs.Items.Add("RIDE Generado " & num_doc)
            End If
            bRetorna = True
        Catch ex As Exception
            OcurrioError(ex, "Error en generación de RIDE de documento:" & secuencia & " tipo de documento:" & tipodoc & " al cliente " & rucCliente)
            bRetorna = False
        End Try
        Return bRetorna
    End Function

    Private Function enviarDocumentoMail(secuencia As String, tipodoc As String, rucCliente As String, sigla As String, num_doc As String, nombreCliente As String)
        Dim interfaz As New ReportUtilities.Interfaz()
        Dim cls As New basXML
        Dim bRetorna As New Boolean
        bRetorna = True
        Try
            Dim sb As New StringBuilder
            Dim ds As New DataSet
            sb.AppendLine(interfaz.PlantillaRIDE(nombreCliente, num_doc))
            Dim correos As New List(Of String)
            correos = interfaz.GetCorreosPorCedulaRUC(rucCliente)
            'correos.Add("factura@e-tractomaq.com")
            ds = cls.consultaMailTipoDoc(tipodoc)
            For i = 0 To ds.Tables(0).Rows.Count - 1
                correos.Add(ds.Tables(0).Rows(i).Item("mail").ToString)
            Next

            Dim archivos As New List(Of String)
            archivos.Add(interfaz.RepositorioLocal(rucCliente, tipodoc) & "\" & sigla & "_" & num_doc & ".pdf")
            If Configuraciones.Get("TipoEmision") = 1 Then 'NORMAL o CONTINGENCIA
                archivos.Add(interfaz.RepositorioLocal(rucCliente, tipodoc) & "\" & sigla & "_" & num_doc & "_au.xml")
            End If
            interfaz.EnviarCorreo("Notificación Documento Electrónico TRACTOMAQ - :" & sigla & "_" & num_doc, sb.ToString, ReportUtilities.Tools.Configuraciones.UsuarioEmail, correos, archivos)
            bRetorna = True

            For i = 0 To correos.Count - 1
                cls.insertarLogMail(secuencia, tipodoc, rucCliente, correos(i))
            Next
            Me.subirDocumentoFTP(rucCliente, tipodoc, sigla & "_" & num_doc)
        Catch ex As Exception
            bRetorna = False
            OcurrioError(ex, "Error al enviar correo de documento:" & secuencia & " tipo de documento:" & tipodoc & " al cliente " & rucCliente)
        End Try
        Return bRetorna
    End Function

    Public Function subirDocumentoFTP(ByVal id_cliente As String, ByVal tipo_doc As String, ByVal nombre_archivo As String) As Boolean
        Try
            Dim interfaz As New ReportUtilities.Interfaz()
            Return interfaz.SubirArchivosAlServidor(id_cliente, tipo_doc, nombre_archivo)
        Catch ex As Exception
            OcurrioError(ex, "Error al subir el archivo:" & nombre_archivo & " por FTP, tipo de documento:" & tipo_doc & " al cliente " & id_cliente)
        End Try
        Return False
    End Function

    Private Sub notificaError(mensaje As String, sigla As String, num_doc As String)
        Dim sb1 As New StringBuilder
        Dim interfaz As New ReportUtilities.Interfaz()
        Dim ds As New DataSet
        Dim cls As New basXML
        Try
            sb1.AppendLine("Detalle de error: " & mensaje)
            Dim correos1 As New List(Of String)
            ds = cls.consultaMailNotifica()
            For i = 0 To ds.Tables(0).Rows.Count - 1
                correos1.Add(ds.Tables(0).Rows(i).Item("mail").ToString)
            Next
            interfaz.EnviarCorreo("Notificación de Novedades en Documento:" & sigla & "_" & num_doc, sb1.ToString, ReportUtilities.Tools.Configuraciones.UsuarioEmail, correos1)
        Catch ex As Exception
            OcurrioError(ex, "Notificación de Novedades en Documento:" & sigla & "_" & num_doc & Environment.NewLine & sb1.ToString)
        End Try
    End Sub

    Private Sub btnErrores_Click(sender As Object, e As EventArgs) Handles btnErrores.Click
        Dim frm As New frmErrores
        frm.Show()
    End Sub

    Private Sub btnConfiguracion_Click(sender As Object, e As EventArgs) Handles btnConfiguracion.Click
        Dim frm As New FrmConfiguraciones
        frm.Show()
    End Sub

End Class