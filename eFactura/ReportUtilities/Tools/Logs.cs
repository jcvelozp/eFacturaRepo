using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities
{
    public static class Logs
    {
        public static string CustomDirectory { get; set; }
        public static string DefaultPathErrorFile
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\FlowErrorLog.txt";
            }
        }
        private static void WriteErrorLog(string errorMsg, string location, string error_dev, string addInfo = "",bool enviarCorreo=false)
        {
            try
            {
                System.IO.StreamWriter objWriter = null;
                try
                {
                    objWriter = new System.IO.StreamWriter(CustomDirectory + "eFacturaErrorLog.txt", true);
                }
                catch (Exception)
                {
                    objWriter = new System.IO.StreamWriter(DefaultPathErrorFile, true);
                }
                objWriter.WriteLine("***************************************************************************************");
                objWriter.WriteLine("ERROR DATE:  " + DateTime.Now);
                objWriter.WriteLine("WINDOWS USERNAME/USERDOMAINNAME:" + Environment.UserName + "/" + Environment.UserDomainName);
                objWriter.WriteLine("MACHINENAME:" + Environment.MachineName);
                objWriter.WriteLine("LOCATION:  " + location);
                objWriter.WriteLine("ERROR MESSAGE (USER):");
                objWriter.WriteLine(errorMsg + "\n");
                objWriter.WriteLine("ERROR MESSAGE (DEVELOPER):");
                objWriter.WriteLine(error_dev + "\n");
                objWriter.WriteLine("MORE INFO:" + addInfo + "\n");
                objWriter.Close();

                if (enviarCorreo)
                {
                    StringBuilder html = new StringBuilder();
                    html.AppendLine("<p>");
                    html.AppendLine("<b>Fecha Error:</b>"+DateTime.Now+"<br/>");
                    html.AppendLine("<b>Usuario Windows/ Usuario Dominio:</b>" + Environment.UserName + "/" + Environment.UserDomainName + "<br/>");
                    html.AppendLine("<b>Nombre de Maquina:</b>" + Environment.MachineName + "<br/>");
                    html.AppendLine("<b>Ubicación:</b>" + location + "<br/>");
                    html.AppendLine("<b>Error Mensaje (Usuario):</b><br/>");
                    html.AppendLine(errorMsg+ "<br/>");
                    html.AppendLine("<b>Error Mensaje (Usuario):</b><br/>");
                    html.AppendLine(error_dev + "<br/>");
                    html.AppendLine("<b>Mas Información:</b>" + addInfo + "<br/>");
                    html.AppendLine("</p>");
                    Interfaz i = new Interfaz();
                    i.EnviarCorreo("E-tractomaq Notificacion Log Errores", html.ToString(), Tools.Configuraciones.UsuarioEmail, Tools.Configuraciones.ListaCorreosANotificarExcepciones);
                }
            }
            catch (Exception)
            {

            }
        }
        public static void WriteLog(string msg, string addInfo = "",bool enviarCorreo=false)
        {
            try
            {
                System.IO.StreamWriter objWriter = null;
                try
                {
                    objWriter = new System.IO.StreamWriter(CustomDirectory + "eFacturaTraceLog.txt", true);
                }
                catch (Exception)
                {
                    objWriter = new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ErrorLog.txt", true);
                }
                objWriter.WriteLine("***************************************************************************************");
                objWriter.WriteLine("DATE:  " + DateTime.Now);
                objWriter.WriteLine("MESSAGE:" + msg);
                if (!String.IsNullOrEmpty(addInfo)) {
                    objWriter.WriteLine("MORE INFO:" + addInfo + "\n");
                }
                objWriter.Close();

                if (enviarCorreo)
                {
                    StringBuilder html = new StringBuilder();
                    html.AppendLine("<p>");
                    html.AppendLine("<b>Fecha Log:</b>" + DateTime.Now + "<br/>");
                    html.AppendLine("<b>Mensaje:</b>" + msg + "<br/>");                    
                    html.AppendLine("<b>Mas Información:</b>" + addInfo + "<br/>");
                    html.AppendLine("</p>");
                    Interfaz i = new Interfaz();
                    i.EnviarCorreo("E-tractomaq Notificacion Log", html.ToString(), Tools.Configuraciones.UsuarioEmail, Tools.Configuraciones.ListaCorreosANotificarExcepciones);
                }
            }
            catch (Exception) { }
        }
        public static void WriteErrorLog(Exception ex, string addInfo = "",bool enviarCorreo=false)
        {
            WriteErrorLog(ex.Message, ex.Source, ex.ToString(),addInfo,enviarCorreo);
        }
    }
}
