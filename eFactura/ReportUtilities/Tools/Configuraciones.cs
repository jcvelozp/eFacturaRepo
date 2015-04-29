using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportUtilities.DataSet.DS_FacturacionElectronicaTableAdapters;
using ReportUtilities.DataSet;

namespace ReportUtilities.Tools
{
    public class Configuraciones
    {

        #region DB functions
        private static configuracionesTableAdapter ta = new configuracionesTableAdapter();

        public static String Get(string clave)
        {
            try
            {
                return ta.GetValue(clave);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Logs.WriteErrorLog(ex, "Clave:" + clave, true);
                throw;
            }            
        }

        public static void Set(string clave, string valor)
        {
            if (ta.UpdateValue(valor, clave) == 0)
            {
                ta.Insert(clave, valor);
            }
        }
        #endregion

        #region Propiedades Repositorio Local FTP

        public static string RutaRepositorioLocal
        {
            get
            {
                return Get("RutaRepositorioLocal");
            }
            set
            {
                Set("RutaRepositorioLocal", value);
            }
        }

        public static string HostFTP
        {
            get
            {
                return Get("HostFTP");
            }
            set
            {
                Set("HostFTP", value);
            }
        }

        public static string UsuarioFTP
        {
            get
            {
                return Get("UsuarioFTP");
            }
            set
            {
                Set("UsuarioFTP", value);
            }
        }

        public static string PasswordFTP
        {
            get
            {
                return Get("PasswordFTP");
            }
            set
            {
                Set("PasswordFTP", value);
            }
        }

        public static string RutaRepositorioFTP
        {
            get
            {
                return Get("RutaRepositorioFTP");
            }
            set
            {
                Set("RutaRepositorioFTP", value);
            }
        }

        public static string PuertoFTP
        {
            get
            {
                return Get("PuertoFTP");
            }
            set
            {
                Set("PuertoFTP", value);
            }
        }

        #endregion

        #region Propiedades Email

        public static string NombreEmailEmisor
        {
            get
            {
                return Get("NombreEmailEmisor");
            }
            set
            {
                Set("NombreEmailEmisor", value);
            }
        }

        public static string HostEmail
        {
            get
            {
                return Get("HostEmail");
            }
            set
            {
                Set("HostEmail", value);
            }
        }

        public static string UsuarioEmail
        {
            get
            {
                return Get("UsuarioEmail");
            }
            set
            {
                Set("UsuarioEmail", value);
            }
        }

        public static string PasswordEmail
        {
            get
            {
                return Get("PasswordEmail");
            }
            set
            {
                Set("PasswordEmail", value);
            }
        }

        public static string PuertoEmail
        {
            get
            {
                return Get("PuertoEmail");
            }
            set
            {
                Set("PuertoEmail", value);
            }
        }

        public static string HabilitarSSL
        {
            get
            {
                return Get("HabilitarSSL");
            }
            set
            {
                Set("HabilitarSSL", value);
            }
        }

        /*Correo Secundario*/

        public static string HostEmail2
        {
            get
            {
                return Get("HostEmail2");
            }
            set
            {
                Set("HostEmail2", value);
            }
        }

        public static string UsuarioEmail2
        {
            get
            {
                return Get("UsuarioEmail2");
            }
            set
            {
                Set("UsuarioEmail2", value);
            }
        }

        public static string PasswordEmail2
        {
            get
            {
                return Get("PasswordEmail2");
            }
            set
            {
                Set("PasswordEmail2", value);
            }
        }

        public static string PuertoEmail2
        {
            get
            {
                return Get("PuertoEmail2");
            }
            set
            {
                Set("PuertoEmail2", value);
            }
        }

        public static string HabilitarSSL2
        {
            get
            {
                return Get("HabilitarSSL2");
            }
            set
            {
                Set("HabilitarSSL2", value);
            }
        }

        #endregion

        #region Propiedades Sistema

        public static decimal TipoEmision
        {
            get
            {
                var t = Convert.ToDecimal(Get("TipoEmision"));
                if (t == 0)
                {
                    TipoEmision = 1;
                }
                return t;
            }
            set
            {
                Set("TipoEmision", value.ToString());
            }
        }

        public static decimal Ambiente
        {
            get
            {
                var a = Convert.ToDecimal(Get("Ambiente"));
                if (a == 0)
                {
                    Ambiente = 1;
                }
                return a;
            }
            set
            {
                Set("Ambiente", value.ToString());
            }
        }

        #endregion


        #region PlantillaCorreo
        public static string CorreoAEnviar {
            get {
                return Get("CorreoAEnviar");
            }
            set {
                Set("CorreoAEnviar", value);
            }
        }
        public static string TituloAEnviar {
            get {
                return Get("TituloAEnviar");
            }
            set {
                Set("TituloAEnviar", value);
            }
        }
        public static string MensajeAEnviar {
            get {
                return Get("MensajeAEnviar");
            }
            set {
                Set("MensajeAEnviar", value);
            }
        }

        public static string PruebaTitulo {
            get {
                return Get("PruebaTitulo");
            }
            set {
                Set("PruebaTitulo",value);
            }
        }

        public static string PruebaDescripcion {
            get {
                return Get("PruebaDescripcion");
            }
            set {
                Set("PruebaDescripcion",value);
            }
        }

        public static string PruebaInfoEmpresa
        {
            get
            {
                return Get("PruebaInfoEmpresa");
            }
            set
            {
                Set("PruebaInfoEmpresa", value);
            }
        }
        public static string PruebaDireccion
        {
            get
            {
                return Get("PruebaDireccion");
            }
            set
            {
                Set("PruebaDireccion", value);
            }
        }



        #endregion

        /*Notificacion de Registro*/
        #region PlantillaCorreoRegistro
        public static string CorreoAEnviarRegistro
        {
            get
            {
                return Get("CorreoAEnviarRegistro");
            }
            set
            {
                Set("CorreoAEnviarRegistro", value);
            }
        }
        public static string TituloAEnviarRegistro
        {
            get
            {
                return Get("TituloAEnviarRegistro");
            }
            set
            {
                Set("TituloAEnviarRegistro", value);
            }
        }
        public static string MensajeAEnviarRegistro
        {
            get
            {
                return Get("MensajeAEnviarRegistro");
            }
            set
            {
                Set("MensajeAEnviarRegistro", value);
            }
        }

        public static string PruebaTituloRegistro
        {
            get
            {
                return Get("PruebaTituloRegistro");
            }
            set
            {
                Set("PruebaTituloRegistro", value);
            }
        }

        public static string PruebaDescripcionRegistro
        {
            get
            {
                return Get("PruebaDescripcionRegistro");
            }
            set
            {
                Set("PruebaDescripcionRegistro", value);
            }
        }

        public static string PruebaInfoEmpresaRegistro
        {
            get
            {
                return Get("PruebaInfoEmpresaRegistro");
            }
            set
            {
                Set("PruebaInfoEmpresaRegistro", value);
            }
        }
        public static string PruebaDireccionRegistro
        {
            get
            {
                return Get("PruebaDireccionRegistro");
            }
            set
            {
                Set("PruebaDireccionRegistro", value);
            }
        }
        #endregion

        /*Notificacion de Cambio de Correo*/
        #region PlantillaCorreoCambio
        public static string CorreoAEnviarCambio
        {
            get
            {
                return Get("CorreoAEnviarCambio");
            }
            set
            {
                Set("CorreoAEnviarCambio", value);
            }
        }
        public static string TituloAEnviarCambio
        {
            get
            {
                return Get("TituloAEnviarCambio");
            }
            set
            {
                Set("TituloAEnviarCambio", value);
            }
        }
        public static string MensajeAEnviarCambio
        {
            get
            {
                return Get("MensajeAEnviarCambio");
            }
            set
            {
                Set("MensajeAEnviarCambio", value);
            }
        }

        public static string PruebaTituloCambio
        {
            get
            {
                return Get("PruebaTituloCambio");
            }
            set
            {
                Set("PruebaTituloCambio", value);
            }
        }

        public static string PruebaDescripcionCambio
        {
            get
            {
                return Get("PruebaDescripcionCambio");
            }
            set
            {
                Set("PruebaDescripcionCambio", value);
            }
        }

        public static string PruebaDescripcionCambioConfirmacion
        {
            get
            {
                return Get("PruebaDescripcionCambioConfirmacion");
            }
            set
            {
                Set("PruebaDescripcionCambioConfirmacion", value);
            }
        }

        public static string PruebaInfoEmpresaCambio
        {
            get
            {
                return Get("PruebaInfoEmpresaCambio");
            }
            set
            {
                Set("PruebaInfoEmpresaCambio", value);
            }
        }
        public static string PruebaDireccionCambio
        {
            get
            {
                return Get("PruebaDireccionCambio");
            }
            set
            {
                Set("PruebaDireccionCambio", value);
            }
        }
        #endregion

        public static bool ModoServicio
        {
            get
            {
                return Convert.ToBoolean(Get("ModoServicio"));
            }
            set {
                Set("ModoServicio", value.ToString());
            }
        }

        public static bool EnviarCorreoEnExcepciones
        {
            get
            {
                return Convert.ToBoolean(Get("EnviarCorreoEnExcepciones"));
            }
            set
            {
                Set("EnviarCorreoEnExcepciones", value.ToString());
            }
        }

        public static string CorreosANotificarExcepciones
        {
            get
            {
                return Get("CorreosANotificarExcepciones");
            }
            set
            {
                Set("CorreosANotificarExcepciones", value);
            }
        }

        public static List<string> ListaCorreosANotificarExcepciones
        {
            get
            {
                var correos = CorreosANotificarExcepciones.Split('|');
                List<string> temp = new List<string>();
                foreach (var item in correos)
                {
                    temp.Add(item);
                }
                return temp;
            }
        }

    }
}
