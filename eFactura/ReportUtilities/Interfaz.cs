using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportUtilities.DataSet;
using ReportUtilities.DataSet.DS_FacturacionElectronicaTableAdapters;
using ReportUtilities.Ftp;
using ReportUtilities.Tools;
using System.Data;
using System.IO;
using ReportUtilities.Reportes.Clases;
namespace ReportUtilities
{
    public class Interfaz : IIntertaz
    {

        public bool GenerarRIDE(string id_cliente, int secuencia, string tipo_doc, string nombre_archivo, bool abrirArchivo = false)
        {
            cab_documentoTableAdapter ta = new cab_documentoTableAdapter();
            DS_FacturacionElectronica.cab_documentoDataTable dt = ta.GetDataBySecuencia(secuencia, tipo_doc);
            if (dt.Count > 0)
            {
                var doc = Documentos(tipo_doc, secuencia, id_cliente);
                if (doc != null)
                {
                    doc.ReportFileName = nombre_archivo;
                    var result = doc.Print(doc.Parametros);
                    if (abrirArchivo)
                    {
                        doc.OpenFile(nombre_archivo);
                    }
                    return result;
                }
            }
            return false;
        }

        public void ConfigurarServicioFTP(string user, string password, string host, int port = 21)
        {
            Configuraciones.UsuarioFTP = user;
            Configuraciones.PasswordFTP = password;
            Configuraciones.HostFTP = host;
            Configuraciones.PuertoFTP = port.ToString();
        }

        public bool SubirArchivosAlServidor(string cliente_id, string tipo_documento, string nombre_archivo)
        {
            try
            {
                DS_FacturacionElectronica.tipo_documentoDataTable dtTipoDocumento = new DS_FacturacionElectronica.tipo_documentoDataTable();
                tipo_documentoTableAdapter taTipoDocumento = new tipo_documentoTableAdapter();
                taTipoDocumento.Fill(dtTipoDocumento);
                ftp FtpClient = new ftp();
                FtpClient.createDirectory(Configuraciones.RutaRepositorioFTP + "/" + cliente_id);
                foreach (var item in dtTipoDocumento)
                {
                    FtpClient.createDirectory(Configuraciones.RutaRepositorioFTP + "/" + cliente_id + "/" + item.CODIGO);
                }
                string ext = "";
                for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            ext = ".pdf";
                            break;
                        case 1:
                            ext = ".xml";
                            break;
                        case 2:
                            ext = "_au.xml";
                            break;
                    }
                    FtpClient.upload(Configuraciones.RutaRepositorioFTP + "/" + cliente_id + "/" + tipo_documento + "/" + nombre_archivo + ext, this.RepositorioLocal(cliente_id, tipo_documento) + "/" + nombre_archivo + ext);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Logs.WriteErrorLog(ex);
            }
            return false;
        }

        public void ConfigurarServicioCorreo(string user, string password, string host = "smtp.gmail.com", int port = 587, bool EnableSsl = true)
        {
            Configuraciones.UsuarioEmail = user;
            Configuraciones.PasswordEmail = password;
            Configuraciones.HostEmail = host;
            Configuraciones.PuertoEmail = port.ToString();
            Configuraciones.HabilitarSSL = EnableSsl.ToString();
        }

        public void ConfigurarServicioCorreoSecundario(string user, string password, string host = "smtp.gmail.com", int port = 587, bool EnableSsl = true)
        {
            Configuraciones.UsuarioEmail2 = user;
            Configuraciones.PasswordEmail2 = password;
            Configuraciones.HostEmail2 = host;
            Configuraciones.PuertoEmail2 = port.ToString();
            Configuraciones.HabilitarSSL2 = EnableSsl.ToString();
        }

        public bool EnviarCorreo(string subject, string body, string fromAddress, List<string> correos, List<string> archivos = null)
        {
            Email email = new Email();
            email.from = fromAddress;
            email.toAddress = correos;
            email.Archivos = archivos;
            email.subject = subject;
            email.body = body;
            return email.Send();
        }

        public bool EnviarPorCorreoSecundario(string subject, string body, string fromAddress, List<string> correos, List<string> archivos = null)
        {
            Email email = new Email();
            email.from = fromAddress;
            email.toAddress = correos;
            email.Archivos = archivos;
            email.subject = subject;
            email.body = body;
            return email.Send("2");
        }

        public string PlantillaRIDE(string NombreCliente, string NumeroDocumento, string InformacionEmpresa = "")
        {
            string mensaje = Configuraciones.MensajeAEnviar;

            mensaje = mensaje.Replace("#TITULO#", Configuraciones.PruebaTitulo);
            mensaje = mensaje.Replace("#CONTENIDO#", Configuraciones.PruebaDescripcion);

            if (String.IsNullOrEmpty(InformacionEmpresa))
            {
                InformacionEmpresa = Configuraciones.PruebaInfoEmpresa;
            }
            mensaje = mensaje.Replace("#INFO_EMPRESA#", InformacionEmpresa);
            mensaje = mensaje.Replace("#DIRECCION#", Configuraciones.PruebaDireccion);
            mensaje = mensaje.Replace("#FECHA_HORA#", DateTime.Now.ToString());
            mensaje = mensaje.Replace("#CLIENTE#", NombreCliente);

            mensaje = mensaje.Replace("#NUMERO_DOCUMENTO#", NumeroDocumento);

            return mensaje;
        }

        public string PlantillaRegistro(string NombreCliente, string Usuario, string Password, string InformacionEmpresa = "")
        {
            string mensaje = Configuraciones.MensajeAEnviarRegistro;

            mensaje = mensaje.Replace("#TITULO#", Configuraciones.PruebaTituloRegistro);
            mensaje = mensaje.Replace("#CONTENIDO#", Configuraciones.PruebaDescripcionRegistro);

            if (String.IsNullOrEmpty(InformacionEmpresa))
            {
                InformacionEmpresa = Configuraciones.PruebaInfoEmpresaRegistro;
            }
            mensaje = mensaje.Replace("#INFO_EMPRESA#", InformacionEmpresa);
            mensaje = mensaje.Replace("#DIRECCION#", Configuraciones.PruebaDireccionRegistro);
            mensaje = mensaje.Replace("#FECHA_HORA#", DateTime.Now.ToString());
            mensaje = mensaje.Replace("#CLIENTE#", NombreCliente);

            mensaje = mensaje.Replace("#USUARIO#", Usuario);
            mensaje = mensaje.Replace("#PASSWORD#", Password);
            return mensaje;
        }

        public List<string> GetCorreosPorCedulaRUC(string cedulaRUC)
        {
            List<string> correos = new List<string>();
            try
            {
                mailTableAdapter taMails = new mailTableAdapter();
                var mails = taMails.GetDataByRUC(cedulaRUC);
                foreach (var item in mails)
                {
                    correos.Add(item.mail);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return correos;
        }

        public string RepositorioLocal(string ruc_cedula, string tipo_doc)
        {
            string path = Configuraciones.RutaRepositorioLocal + ruc_cedula + "\\" + tipo_doc + "\\";
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
            return path;
        }

        public ReportUtilities.Reportes.Clases.Plantilla Documentos(string tipo_doc, int secuencia, string ruc_cedula)
        {
            ReportUtilities.Reportes.Clases.Plantilla doc = null;
            switch (tipo_doc)
            {
                case "01":
                    doc = new Factura(secuencia, ruc_cedula);
                    break;
                case "04":
                    doc = new NotaCredito(secuencia, ruc_cedula);
                    break;
                case "05":
                    doc = new NotaDebito(secuencia, ruc_cedula);
                    break;
                case "06":
                    doc = new GuiaRemision(secuencia, ruc_cedula);
                    break;
                case "07":
                    doc = new Retencion(secuencia, ruc_cedula);
                    break;
            }
            return doc;
        }

        public DataSet.DS_FacturacionElectronica.usuarioRow GetUsuario(string cedula)
        {
            DataSet.DS_FacturacionElectronica.usuarioRow user = null;
            if (!String.IsNullOrEmpty(cedula))
            {
                var dt = new DataSet.DS_FacturacionElectronicaTableAdapters.usuarioTableAdapter().GetDataByCedula(cedula);
                if (dt.Count > 0)
                {
                    user = dt[0];
                }
            }
            return user;
        }
    }
}

