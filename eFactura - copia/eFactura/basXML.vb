Imports ReportUtilities.Tools
Imports MySql.Data.MySqlClient
Imports System.Xml

Public Class basXML
    Dim con As New MySqlConnection
    Dim cmd As New MySqlCommand
    Dim reader As MySqlDataReader
    Dim ds As Data.DataSet
    Dim adapter As MySqlDataAdapter
    Dim prm As MySqlParameter
    Dim cls As New ClsConexion

    Public Shared Function TryParseXml(ByVal xml As String) As Boolean
        Try
            Dim doc As New XmlDocument
            doc.LoadXml(xml)
            Return True
        Catch ex As Exception
            ReportUtilities.Logs.WriteErrorLog(ex, "Error en la recepción de documentos. " & xml)
            Return False
        End Try
    End Function


    Public Function consultaDatos(ByVal empresa As Integer, ByVal ambiente As Integer, ByVal emision As Integer, ByVal tipodoc As String, ByVal codNum As String, ByVal seqCompte As String, ByVal digVerif As Integer) As DataSet
        Dim cadena As String = ""
        'Dim prm As OracleParameter
        Try
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text

            If tipodoc = "07" Then
                cadena = "SELECT e.razonsocial, e.razoncomercial, e.ruc rucEmpresa, c.secuencia AS secuencial, c.num_doc numero,  c.establecimiento, c.pto_emision, e.direccion AS dirMatriz, c.fecha, "
                cadena = cadena & " e.contespecial AS ContEsp, e.obligaconta AS ObligCont, c.tipo_id_cliente tipoId, c.razons_cliente  razons, id_cliente ruc, direccion_cliente dirEstab,"
                cadena = cadena & " d.codigo, d.codigoRetencion, d.base, d.porcentaje, d.valorRetenido, d.codSustento, d.numSustento, d.fechaEmision, c.tipo_doc, c.cod_cliente_int, c.tipo_doc, c.cod_cliente_int, "
                cadena = cadena & " tipo_comp_modifica, num_comp_modifica, fec_comp_modifica, motivo "
                cadena = cadena & " FROM cab_documento c INNER JOIN det_retencion d ON c.secuencia = d.cab_documento "
                cadena = cadena & " INNER JOIN info_empresa e ON e.codigo=c.empresa WHERE c.estado='A'"
                cadena = cadena & " AND c.empresa='" & empresa & "' AND c.tipo_doc='" & tipodoc & "' and c.secuencia=" & seqCompte
            ElseIf tipodoc = "06" Then
                cadena = " SELECT e.razonsocial, e.razoncomercial, e.ruc rucEmpresa, e.direccion AS dirMatriz, c.secuencia secuencial, c.tipo_doc, c.num_doc numero, c.establecimiento, c.pto_emision, c.fecha, c.tipo_id_cliente tipoId, c.razons_cliente razons, c.id_cliente ruc,"
                cadena = cadena & " e.contespecial AS ContEsp, e.obligaconta AS ObligCont, c.direccion_cliente dirEstab, c.estado, c.empresa, c.cod_cliente_int, c.direccion_partida dirPartida, c.rise, c.fecha_ini_tras,"
                cadena = cadena & " c.fecha_fin_tras, c.placa, t.idDestinatario, t.razonSocialDest, t.dirDestinatario, t.motivoTraslado, t.docAduanero, t.codEstabDest, t.ruta, t.docSustento, t.numSustento,"
                cadena = cadena & " t.autSustento, t.fechaSustento, d.codInterno, d.codAdicional, d.descripcion, d.cantidad"
                cadena = cadena & " FROM cab_documento c INNER JOIN info_empresa e ON e.codigo=c.empresa INNER JOIN det_guia_detalle d ON d.secuencia_cab=c.secuencia INNER JOIN det_guia_destino t ON d.secuencia_cab=t.secuencia_cab WHERE c.estado='A'"
                cadena = cadena & " AND c.empresa='" & empresa & "' AND c.tipo_doc='" & tipodoc & "' and c.secuencia=" & seqCompte
            Else
                cadena = "SELECT e.razonsocial, e.razoncomercial, e.ruc rucEmpresa, c.secuencia AS secuencial, c.num_doc numero, e.direccion AS dirMatriz,  c.establecimiento, c.pto_emision, c.fecha, "
                cadena = cadena & " e.contespecial AS ContEsp, e.obligaconta AS ObligCont, c.tipo_id_cliente tipoId, c.razons_cliente  razons, id_cliente ruc, direccion_cliente dirEstab,"
                cadena = cadena & " SUM(precio*cantidad)-SUM(descuento) totalVenta, SUM(descuento) totalDescuento, SUM(BASE_SIN_IVA) totalSinImpuestos, "
                cadena = cadena & " SUM(CASE WHEN valor_iva=0 THEN 0 ELSE base_imp_iva END) baseImponibleIva, SUM(valor_iva) valorIva, c.tipo_doc, c.cod_cliente_int, "
                cadena = cadena & " tipo_comp_modifica, num_comp_modifica, fec_comp_modifica, motivo "
                cadena = cadena & " FROM cab_documento c INNER JOIN det_documento d ON c.secuencia = d.secuencia_cab "
                cadena = cadena & " INNER JOIN info_empresa e ON e.codigo=c.empresa WHERE c.estado='A'"
                cadena = cadena & " AND c.empresa='" & empresa & "' AND c.tipo_doc='" & tipodoc & "' and c.secuencia=" & seqCompte
                cadena = cadena & " GROUP BY e.razonsocial, e.razoncomercial, e.ruc, c.secuencia, e.direccion, c.fecha,  c.establecimiento, c.pto_emision, "
                cadena = cadena & " e.contespecial, e.obligaconta, c.tipo_id_cliente, c.razons_cliente, id_cliente, tipo_comp_modifica, num_comp_modifica, fec_comp_modifica"
            End If
            adapter.SelectCommand.CommandText = cadena
            ds = New Data.DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & cadena, True)
        End Try
        Return ds
    End Function

    Public Function cambiarEstadoClaveContingencia(sClave) As Boolean
        Dim sql As String = ""
        Try
            cls.AbrirConexion()
            con = cls.GetConexion()
            cmd.CommandType = System.Data.CommandType.Text
            Sql = "update claves_contingencia set estado='P' where clave='" & sClave & "' "
            cmd.CommandText = Sql
            cmd.Connection = con
            cmd.ExecuteNonQuery()
            cls.CerrarConexion()
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & Sql, True)
            Return False
        End Try
    End Function
    Public Function consultaDatosDet(ByVal empresa As Integer, ByVal ambiente As Integer, ByVal emision As Integer, ByVal tipodoc As String, ByVal codNum As String, ByVal seqCompte As String, ByVal digVerif As Integer) As DataSet
        Dim cadena As String = ""
        Try
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text

            If tipodoc = "07" Then
                cadena = "SELECT d.codigo, d.codigoRetencion, d.base, d.porcentaje, d.valorRetenido, d.codSustento, d.numSustento, d.fechaEmision"
                cadena = cadena & " FROM det_retencion d WHERE d.cab_documento =" & seqCompte
            ElseIf tipodoc = "06" Then
                cadena = "SELECT t.idDestinatario, t.razonSocialDest, t.dirDestinatario, t.motivoTraslado, t.docAduanero, t.codEstabDest, t.ruta, t.docSustento, t.numSustento,"
                cadena = cadena & " t.autSustento, t.fechaSustento, d.codInterno, d.codAdicional, d.descripcion, d.cantidad "
                cadena = cadena & " FROM det_guia_detalle d inner join det_guia_destino t on d.secuencia_cab=t.secuencia_cab WHERE d.secuencia_cab =" & seqCompte
            Else
                cadena = "select d.codigo_item codigoPrincipal, descripcion_item descripcion, d.cantidad, d.precio precioUnitario, d.descuento,"
                cadena = cadena & " (precio*cantidad) - descuento precioTotalSinImpuesto, base_imp_iva baseImponibleIva, valor_iva valorIva, base_sin_iva "
                cadena = cadena & " FROM det_documento d where d.secuencia_cab =" & seqCompte
            End If
            adapter.SelectCommand.CommandText = cadena

            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & cadena, True)
        End Try
        Return ds
    End Function
    Public Function ConsultarEmpresa() As DataSet
        Dim sql As String = ""
        Try
            sql = "select codigo, razonSocial nombre from info_empresa"
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql

            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return ds
    End Function

    Public Function consDocASubir(ByVal empresa As Integer, ByVal tipodoc As String) As DataSet
        Dim cadena As String = ""
        Try
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            cadena = "select c.secuencia, c.tipo_doc TIPO, c.establecimiento ESTABLECIMIENTO,  c.pto_emision PTO_EMISION,  c.num_doc NUMERO, fecha FECHA, c.id_cliente IDENTIFICACION, razons_cliente AGENTE, empresa EMPRESA, c.cod_cliente_int COD_CLIENTE_INT, (select clave_acceso from documentos s where s.sec_cab_doc=c.secuencia and s.tipo_doc=c.tipo_doc) CLAVEACCESO, "
            cadena = cadena & " (select num_autoriza from documentos s where s.sec_cab_doc=c.secuencia and s.tipo_doc=c.tipo_doc) CLAVEAUTORIZACION  from cab_documento c where estado='A' and c.empresa='" & empresa & "'"
            If tipodoc <> "" Then
                cadena = cadena & " and c.tipo_doc='" & tipodoc & "'"
            End If
            cadena = cadena & " order by c.tipo_doc"
            adapter.SelectCommand.CommandText = cadena

            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & cadena, True)
        End Try
        Return ds
    End Function

    Public Function consError(ByVal tipodoc As String) As DataSet
        Dim cadena As String = ""
        Try
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            cadena = "SELECT sec_cab Documento, if(tipo_error='A','Autorizacion','Recepcion') tipo, error FROM log_error where tipo_doc='" & tipodoc & "' order by secuencia desc"
            'cadena = "SELECT c.secuencia, C.NUM_DOC NUMERO, C.TIPO_DOC TIPO, C.ESTABLECIMIENTO ESTABLECIMIENTO,  C.PTO_EMISION PTO_EMISION, FECHA, RAZONS_CLIENTE CLIENTE, EMPRESA, c.cod_cliente_int FROM cab_documento c WHERE estado='A' AND C.EMPRESA='" & empresa & "' AND TIPO_DOC='" & tipodoc & "'"
            adapter.SelectCommand.CommandText = cadena
            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & cadena, True)
        End Try
        Return ds
    End Function


    Public Function consultaParametro(ByVal empresa As String) As DataSet
        Dim sql As String = ""
        Try
            sql = "select * from info_empresa where codigo='" & empresa & "'"
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql
            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return ds
    End Function

    Public Function consTipoEmision() As DataSet
        Dim sql As String = ""
        Try
            sql = "SELECT codigo, descripcion nombre FROM tipo_emision"
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql
            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return ds
    End Function

    Public Function consAmbiente() As DataSet
        Dim sql As String = ""
        Try
            sql = "SELECT codigo, descripcion nombre FROM ambiente"
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql

            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return ds
    End Function

    Public Function ConsultarTipoDoc() As DataSet
        Dim sql As String = ""
        Try
            sql = "SELECT codigo, nombre FROM tipo_documento"
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql

            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return ds
    End Function

    Public Function insertarDocumento(secuencia As Integer, tipodoc As String, cliente As String, vNombreArchivo As String, numAutoriza As String, claveAcceso As String, RUC As String, emision As Integer) As Boolean
        Dim frmConfig As New ReportUtilities.FrmConfiguraciones
        Dim sql As String = ""
        Try

            Dim interfaz As New ReportUtilities.Interfaz

            cls.AbrirConexion()
            borrarDocumento(secuencia, tipodoc, cliente)

            con = cls.GetConexion()
            cmd.CommandType = System.Data.CommandType.Text
            sql = "insert into documentos(sec_cab_doc, tipo_doc, cliente, ruta_xml, ruta_autoriza, num_autoriza, clave_acceso, emision, ambiente, estado) "
            sql = sql & "values(" & secuencia & ",'" & tipodoc & "','" & cliente & "','" & (interfaz.RepositorioLocal(RUC, tipodoc)).Replace("\", "\\") & vNombreArchivo & ".xml" & "','" & (interfaz.RepositorioLocal(RUC, tipodoc)).Replace("\", "\\") & vNombreArchivo & "_au.xml" & "','" & numAutoriza & "','" & claveAcceso & "'," & emision & "," & Configuraciones.Get("Ambiente") & ",'"
            sql = sql & IIf(emision = 1, "A", "C") & "')"
            cmd.CommandText = sql
            cmd.Connection = con
            cmd.ExecuteNonQuery()
            cls.CerrarConexion()
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
            Return False
        End Try
    End Function

    Public Function borrarDocumento(secuencia As Integer, tipodoc As String, cliente As String) As Boolean
        Dim frmConfig As New ReportUtilities.FrmConfiguraciones
        Dim sql As String = ""
        Try
            Dim interfaz As New ReportUtilities.Interfaz
            cls.AbrirConexion()
            con = cls.GetConexion()
            cmd.CommandType = System.Data.CommandType.Text
            sql = "delete from documentos where sec_cab_doc='" & secuencia & "' and tipo_doc='" & tipodoc & "' "
            cmd.CommandText = sql
            cmd.Connection = con
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
            Return False
        End Try
    End Function

    Public Function insertarLogMail(secuencia As Integer, tipodoc As String, RUC As String, mail As String) As Boolean
        Dim frmConfig As New ReportUtilities.FrmConfiguraciones
        Dim sql As String = ""
        Try
            Dim interfaz As New ReportUtilities.Interfaz
            cls.AbrirConexion()
            con = cls.GetConexion()
            cmd.CommandType = System.Data.CommandType.Text
            sql = "insert into log_mail(sec_cab_doc, cliente, mail, tipo_doc) "
            sql = sql & "values(" & secuencia & ",'" & RUC & "','" & mail & "','" & tipodoc & "' )"
            cmd.CommandText = sql
            cmd.Connection = con
            cmd.ExecuteNonQuery()
            cls.CerrarConexion()
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
            Return False
        End Try
    End Function


    Public Function actualizaDocumento(secuencia As String, tipodoc As String, numAutoriza As String, fecAutoriza As String) As Boolean
        Dim sql As String = ""
        Try
            cls.AbrirConexion()
            con = cls.GetConexion()
            cmd.CommandType = System.Data.CommandType.Text
            Sql = "update documentos set num_autoriza='" & numAutoriza & "', fecha_autoriza='" & fecAutoriza & "' where sec_cab_doc='" & secuencia & "' and tipo_doc='" & tipodoc & "' "
            cmd.CommandText = Sql
            cmd.Connection = con
            cmd.ExecuteNonQuery()
            cls.CerrarConexion()
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
            Return False
        End Try
    End Function

    Public Function actualizaEstado(pSecuencia As String, pTipodoc As String) As Boolean
        Dim sql As String = ""
        Try
            cls.AbrirConexion()
            con = cls.GetConexion()
            cmd.CommandType = System.Data.CommandType.Text
            Sql = "update cab_documento set estado='V' where secuencia='" & pSecuencia & "' and tipo_doc='" & pTipodoc & "' "
            cmd.CommandText = Sql
            cmd.Connection = con
            cmd.ExecuteNonQuery()
            cls.CerrarConexion()
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & Sql, True)
            Return False
        End Try
    End Function

    Public Function logError(secuencia As String, tipodoc As String, tipoError As String, verror As String) As Boolean
        Dim sql As String = ""
        Try
            cls.AbrirConexion()
            con = cls.GetConexion()
            cmd.CommandType = System.Data.CommandType.Text
            sql = "insert into log_error(sec_cab, tipo_doc, tipo_error, error) values(" & secuencia & ",'" & tipodoc & "','" & tipoError & "',""" & verror & """)"
            cmd.CommandText = Sql
            cmd.Connection = cls.GetConexion
            cmd.ExecuteNonQuery()
            cls.CerrarConexion()
            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
            Return False
        End Try
    End Function


    Public Function verificaDocumento(secuencia, tipodoc, cliente, ruta, rutaAutoriza) As DataSet
        Dim sql As String = ""
        Try
            con = cls.GetConexion()
            cls.AbrirConexion()
            sql = "SELECT * from documentos where sec_cab_doc='" & secuencia & "' and tipo_doc='" & tipodoc & "' and num_autoriza <> ''"
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql
            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
            Return ds
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return Nothing
    End Function


    Public Function consClaveContingencia() As String
        Dim sClave As String = ""
        Dim sql As String = ""
        Try
            sql = "SELECT clave FROM claves_contingencia WHERE estado='A' limit 1"
            con = cls.GetConexion()
            cls.AbrirConexion()
            cmd = New MySqlCommand(sql)
            cmd.Connection = con
            sClave = cmd.ExecuteScalar()
            con.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return sClave
    End Function

    Public Function consultaMailNotifica() As DataSet
        Dim sql As String = ""
        Try
            con = cls.GetConexion()
            cls.AbrirConexion()
            sql = "SELECT * from mail_error "
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql
            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
            Return ds
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return Nothing
    End Function

    Public Function consultaMailTipoDoc(tipodoc As String) As DataSet
        Dim sql As String = ""
        Try
            sql = "SELECT * from mail_tipo_documento where tipo_doc='" & tipodoc & "'"
            con = cls.GetConexion()
            cls.AbrirConexion()
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql
            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
            Return ds
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return Nothing
    End Function

    Public Function verificarClaveAcceso(secuencia As String, tipodoc As String)
        Dim sql As String = ""
        Try
            con = cls.GetConexion()
            cls.AbrirConexion()
            sql = "SELECT * from documentos where sec_cab_doc='" & secuencia & "' and tipo_doc='" & tipodoc & "' and clave_acceso <> ''"
            adapter = New MySqlDataAdapter
            adapter.SelectCommand = New MySqlCommand
            adapter.SelectCommand.Connection = con
            adapter.SelectCommand.CommandType = CommandType.Text
            adapter.SelectCommand.CommandText = sql
            ds = New DataSet
            adapter.Fill(ds)
            con.Close()
            Return ds
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
            ReportUtilities.Logs.WriteErrorLog(ex, "SQL con error:" & sql, True)
        End Try
        Return Nothing
    End Function

End Class
