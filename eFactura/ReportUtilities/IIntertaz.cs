using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities
{
   public interface IIntertaz
    {
       
       bool GenerarRIDE(string id_cliente,int secuencia,string tipo_doc, string nombre_archivo,bool abrirArchivo=false);
       void ConfigurarServicioFTP(string user, string password,string host,int port);
       bool SubirArchivosAlServidor(string cliente_id,string tipo_documento,string nombre_archivo);
       void ConfigurarServicioCorreo(string user, string password, string host = "smtp.gmail.com", int port = 587, bool EnableSsl = true);
       bool EnviarCorreo(string subject, string body, string fromAddress, List<string> correos, List<string> archivos); 
    }
}
