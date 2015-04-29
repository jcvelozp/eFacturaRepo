using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReportUtilities.Core;
using ReportUtilities.Ftp;
using ReportUtilities.DataSet.DS_FacturacionElectronicaTableAdapters;
using ReportUtilities.Tools;
namespace ReportUtilities
{
    public partial class FrmConfiguraciones : Form
    {
        private tipo_documentoTableAdapter ta = new tipo_documentoTableAdapter();
        private tipo_emisionTableAdapter taEmision = new tipo_emisionTableAdapter();
        private ambienteTableAdapter taAmbiente = new ambienteTableAdapter();
        private Interfaz interfaz = new Interfaz();

        public FrmConfiguraciones()
        {            
            InitializeComponent();
            cbxTipoDocumento.DisplayMember = "NOMBRE";
            cbxTipoDocumento.ValueMember = "CODIGO";
            cbxTipoDocumento.DataSource = ta.GetData();

            cbAmbiente.DisplayMember = "descripcion";
            cbAmbiente.ValueMember = "codigo";
            cbAmbiente.DataSource = taAmbiente.GetData();

            cbTipoEmision.DisplayMember = "DESCRIPCION";
            cbTipoEmision.ValueMember = "CODIGO";
            cbTipoEmision.DataSource = taEmision.GetData();
        }

        private void btnGenerarPDF_Click(object sender, EventArgs e)
        {
            this.interfaz.GenerarRIDE(txtCedula.Text,Convert.ToInt32(txtSecuencia.Text),cbxTipoDocumento.SelectedValue.ToString(), txtNombreRIDE.Text,true);
        }

        private void btnSubirArchivoRIDE_Click(object sender, EventArgs e)
        {
            this.interfaz.SubirArchivosAlServidor(txtCedula.Text, cbxTipoDocumento.SelectedValue.ToString(), txtNumDoc.Text);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            switch (folderBrowserDialog1.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    txtRepositorioLocal.Text = folderBrowserDialog1.SelectedPath;
                    break;
            }
        }

        private void FrmTest_Load(object sender, EventArgs e)
        {
            CargarConfiguraciones();

            textBox1.Text=Configuraciones.Get("x");
            textBox2.Text=Configuraciones.Get("y");
            textBox3.Text=Configuraciones.Get("x2");
            textBox4.Text=Configuraciones.Get("y2");

            /*ReportUtilities.Reportes.Formatos.FormatoFactura factura = new Reportes.Formatos.FormatoFactura();
            pgFactura.SelectedObject = factura;*/

            /*Configuracion plantilla RIDE*/
            txtNotificacionRIDETituloCorreo.Text = Configuraciones.PruebaTitulo;
            txtNotificacionRIDEDescripcion.Text = Configuraciones.PruebaDescripcion;
            txtNotificacionRIDEInformacion.Text = Configuraciones.PruebaInfoEmpresa;
            txtNotificacionRIDEDireccion.Text = Configuraciones.PruebaDireccion;
            txtNotificacionRIDEPara.Text = Configuraciones.CorreoAEnviar;
            txtNotificacionRIDETitulo.Text = Configuraciones.TituloAEnviar;
            txtNotificacionRIDEMensaje.Text = Configuraciones.MensajeAEnviar;

            /*Configuracion plantilla Cambio*/
            txtNotificacionCambioTituloCorreo.Text = Configuraciones.PruebaTituloCambio;
            txtNotificacionCambioDescripcion1.Text = Configuraciones.PruebaDescripcionCambioConfirmacion;
            txtNotificacionCambioDescripcion2.Text = Configuraciones.PruebaDescripcionCambio;
            txtNotificacionCambioInformacion.Text = Configuraciones.PruebaInfoEmpresaCambio;
            txtNotificacionCambioDireccion.Text = Configuraciones.PruebaDireccionCambio;
            txtNotificacionCambioPara.Text = Configuraciones.CorreoAEnviarCambio;
            txtNotificacionCambioTitulo.Text = Configuraciones.TituloAEnviarCambio;
            txtNotificacionCambioMensaje.Text = Configuraciones.MensajeAEnviarCambio;

            /*Configuracion plantilla Registro*/
            txtNotificacionRegistroTituloCorreo.Text = Configuraciones.PruebaTituloRegistro;
            txtNotificacionRegistroDescripcion.Text = Configuraciones.PruebaDescripcionRegistro;
            txtNotificacionRegistroInformacion.Text = Configuraciones.PruebaInfoEmpresaRegistro;
            txtNotificacionRegistroDireccion.Text = Configuraciones.PruebaDireccionRegistro;
            txtNotificacionRegistroPara.Text = Configuraciones.CorreoAEnviarRegistro;
            txtNotificacionRegistroTitulo.Text = Configuraciones.TituloAEnviarRegistro;
            txtNotificacionRegistroMensaje.Text = Configuraciones.MensajeAEnviarRegistro;

            /*Notificaciones errores*/
            txtCorreosErrores.Text = Configuraciones.CorreosANotificarExcepciones;
            chbModoServicio.Checked = Configuraciones.ModoServicio;
            chbNotificarExcepciones.Checked = Configuraciones.EnviarCorreoEnExcepciones;
        }



        #region Administrar Configuraciones

        public void CargarConfiguraciones()
        {
            txtDireccionFTP.Text = Configuraciones.HostFTP;
            txtUsuarioFTP.Text = Configuraciones.UsuarioFTP;
            txtPasswordFTP.Text = Configuraciones.PasswordFTP;
            txtRutaDestino.Text = Configuraciones.RutaRepositorioFTP;
            txtRepositorioLocal.Text = Configuraciones.RutaRepositorioLocal;
            txtPuertoFTP.Text = Configuraciones.PuertoFTP;

            txtUsuarioEmail.Text = Configuraciones.UsuarioEmail;
            txtPasswordEmail.Text = Configuraciones.PasswordEmail;
            txtHostEmail.Text = Configuraciones.HostEmail;
            txtPuertoEmail.Text = Configuraciones.PuertoEmail;
            chbHabilitarSSL.Checked = Convert.ToBoolean(Configuraciones.HabilitarSSL);
            txtNombreEmailEmisor.Text = Configuraciones.NombreEmailEmisor;

            cbAmbiente.SelectedValue = Configuraciones.Ambiente;
            cbTipoEmision.SelectedValue = Configuraciones.TipoEmision;
        }

        public void GuardarConfiguraciones()
        {
            Configuraciones.HostFTP = txtDireccionFTP.Text;
            Configuraciones.UsuarioFTP = txtUsuarioFTP.Text;
            Configuraciones.PasswordFTP = txtPasswordFTP.Text;
            Configuraciones.RutaRepositorioFTP = txtRutaDestino.Text;
            Configuraciones.RutaRepositorioLocal = txtRepositorioLocal.Text;
            Configuraciones.PuertoFTP = txtPuertoFTP.Text;

            Configuraciones.UsuarioEmail = txtUsuarioEmail.Text;
            Configuraciones.PasswordEmail = txtPasswordEmail.Text;
            Configuraciones.HostEmail = txtHostEmail.Text;
            Configuraciones.PuertoEmail = txtPuertoEmail.Text;
            Configuraciones.HabilitarSSL = chbHabilitarSSL.Checked.ToString();
            Configuraciones.NombreEmailEmisor = txtNombreEmailEmisor.Text;

            Configuraciones.Ambiente=Convert.ToDecimal(cbAmbiente.SelectedValue);
            Configuraciones.TipoEmision=Convert.ToDecimal(cbTipoEmision.SelectedValue);

            /*Configuraciones.PruebaTitulo = textBox5.Text;
             Configuraciones.PruebaDescripcion=textBox6.Text;
            Configuraciones.PruebaInfoEmpresa=textBox7.Text;
            Configuraciones.PruebaDireccion = textBox8.Text;

            Configuraciones.CorreoAEnviar = txtFrom.Text;
            Configuraciones.TituloAEnviar=txtSubject.Text;
            Configuraciones.MensajeAEnviar=txtBody.Text;*/


            /*Configuracion plantilla RIDE*/
            Configuraciones.PruebaTitulo = txtNotificacionRIDETituloCorreo.Text;
            Configuraciones.PruebaDescripcion = txtNotificacionRIDEDescripcion.Text;
            Configuraciones.PruebaInfoEmpresa = txtNotificacionRIDEInformacion.Text;
            Configuraciones.PruebaDireccion = txtNotificacionRIDEDireccion.Text;
            Configuraciones.CorreoAEnviar = txtNotificacionRIDEPara.Text;
            Configuraciones.TituloAEnviar = txtNotificacionRIDETitulo.Text;
            Configuraciones.MensajeAEnviar=txtNotificacionRIDEMensaje.Text;

            /*Configuracion plantilla Cambio*/
            Configuraciones.PruebaTituloCambio = txtNotificacionCambioTituloCorreo.Text;
            Configuraciones.PruebaDescripcionCambioConfirmacion = txtNotificacionCambioDescripcion1.Text;
            Configuraciones.PruebaDescripcionCambio = txtNotificacionCambioDescripcion2.Text;
            Configuraciones.PruebaInfoEmpresaCambio = txtNotificacionCambioInformacion.Text;
            Configuraciones.PruebaDireccionCambio = txtNotificacionCambioDireccion.Text;
            Configuraciones.CorreoAEnviarCambio = txtNotificacionCambioPara.Text;
            Configuraciones.TituloAEnviarCambio=txtNotificacionCambioTitulo.Text;
            Configuraciones.MensajeAEnviarCambio=txtNotificacionCambioMensaje.Text;

            /*Configuracion plantilla Registro*/
            Configuraciones.PruebaTituloRegistro=txtNotificacionRegistroTituloCorreo.Text;
            Configuraciones.PruebaDescripcionRegistro =txtNotificacionRegistroDescripcion.Text ;
            Configuraciones.PruebaInfoEmpresaRegistro=txtNotificacionRegistroInformacion.Text;
            Configuraciones.PruebaDireccionRegistro = txtNotificacionRegistroDireccion.Text;
            Configuraciones.CorreoAEnviarRegistro = txtNotificacionRegistroPara.Text;
            Configuraciones.TituloAEnviarRegistro = txtNotificacionRegistroTitulo.Text;
            Configuraciones.MensajeAEnviarRegistro = txtNotificacionRegistroMensaje.Text;

            Configuraciones.CorreosANotificarExcepciones = txtCorreosErrores.Text;
            Configuraciones.ModoServicio=chbModoServicio.Checked;
            Configuraciones.EnviarCorreoEnExcepciones=chbNotificarExcepciones.Checked;
        }

        #endregion

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarConfiguraciones();
        }

        private void chbHabilitarSSL_CheckedChanged(object sender, EventArgs e)
        {
            if (chbHabilitarSSL.Checked)
            {
                chbHabilitarSSL.Text = "Si";
            }
            else {
                chbHabilitarSSL.Text = "No";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Factura fx=new Factura(Convert.ToInt32(txtSecuencia.Text),txtCedula.Text);
            PanelReporte pos = new PanelReporte();
            pos.PosicionPanel.X=float.Parse(textBox1.Text);
            pos.PosicionPanel.Y= float.Parse(textBox2.Text);
            fx.GenerarPDF("Prueba", pos);//, , float.Parse(textBox3.Text), float.Parse(textBox4.Text));

            Configuraciones.Set("x", textBox1.Text);
            Configuraciones.Set("y", textBox2.Text);
            Configuraciones.Set("x2", textBox3.Text);
            Configuraciones.Set("y2", textBox4.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Interfaz i = new Interfaz();
            List<string> correos=new List<string>();
            correos.Add(txtFrom.Text);
            string mensaje = txtBody.Text;
            i.EnviarCorreo(txtSubject.Text, mensaje, txtUsuarioEmail.Text, correos);
        }

        private void cbxTipoDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtSecuencia_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtSecuencia.Text))
                {
                    var taDoc = new DataSet.DS_FacturacionElectronicaTableAdapters.cab_documentoTableAdapter();
                    var dt = taDoc.GetDataBySecuencia(Convert.ToInt32(txtSecuencia.Text), cbxTipoDocumento.SelectedValue.ToString());
                    if (dt.Count > 0)
                    {
                        var dr = dt[0];
                        txtCedula.Text = dr.ID_CLIENTE;
                        txtNumDoc.Text = dr.NUM_DOC;
                    }
                }
                else 
                {
                    txtCedula.Clear();
                    txtNumDoc.Clear();                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnNotificacionCambio_Click(object sender, EventArgs e)
        {
            Interfaz i = new Interfaz();
            List<string> correos = new List<string>();
            correos.Add(txtNotificacionCambioPara.Text);

            string mensaje = txtNotificacionCambioMensaje.Text;
            mensaje = mensaje.Replace("#CLIENTE#", "Cliente de Prueba Email");
            mensaje = mensaje.Replace("#TITULO#", txtNotificacionCambioTituloCorreo.Text);
            mensaje = mensaje.Replace("#CONTENIDO#", txtNotificacionCambioDescripcion1.Text);
            mensaje = mensaje.Replace("#INFO_EMPRESA#", txtNotificacionCambioInformacion.Text);
            mensaje = mensaje.Replace("#DIRECCION#", txtNotificacionCambioDireccion.Text);
            mensaje = mensaje.Replace("#FECHA_HORA#", DateTime.Now.ToString());
            mensaje = mensaje.Replace("#PASSWORD#", txtPasswordNuevo.Text);
            mensaje = mensaje.Replace("#URL#", txtURLConfirmacion.Text);
            i.EnviarCorreo(txtNotificacionCambioTituloCorreo.Text, mensaje, txtUsuarioEmail.Text, correos);
        }

        private void btnNotificacionRIDE_Click(object sender, EventArgs e)
        {
            Interfaz i = new Interfaz();
            List<string> correos = new List<string>();
            correos.Add(txtNotificacionRIDEPara.Text);

            string mensaje = txtNotificacionRIDEMensaje.Text;
            mensaje = mensaje.Replace("#CLIENTE#", "Cliente de Prueba Email");
            mensaje = mensaje.Replace("#TITULO#", txtNotificacionRIDETituloCorreo.Text);
            mensaje = mensaje.Replace("#CONTENIDO#", txtNotificacionRIDEDescripcion.Text);
            mensaje = mensaje.Replace("#INFO_EMPRESA#", txtNotificacionRIDEInformacion.Text);
            mensaje = mensaje.Replace("#DIRECCION#", txtNotificacionRIDEDireccion.Text);
            mensaje = mensaje.Replace("#FECHA_HORA#", DateTime.Now.ToString());
            i.EnviarCorreo(txtNotificacionRIDETituloCorreo.Text, mensaje, txtUsuarioEmail.Text, correos);
        }

        private void btnNotificacionRegistro_Click(object sender, EventArgs e)
        {
            Interfaz i = new Interfaz();
            List<string> correos = new List<string>();
            correos.Add(txtNotificacionRegistroPara.Text);

            string mensaje = txtNotificacionRegistroMensaje.Text;
            mensaje = mensaje.Replace("#CLIENTE#", "Cliente de Prueba Email");
            mensaje = mensaje.Replace("#TITULO#", txtNotificacionRegistroTituloCorreo.Text);
            mensaje = mensaje.Replace("#CONTENIDO#", txtNotificacionRegistroDescripcion.Text);
            mensaje = mensaje.Replace("#INFO_EMPRESA#", txtNotificacionRegistroInformacion.Text);
            mensaje = mensaje.Replace("#DIRECCION#", txtNotificacionRegistroDireccion.Text);
            mensaje = mensaje.Replace("#FECHA_HORA#", DateTime.Now.ToString());
            mensaje = mensaje.Replace("#USUARIO#", txtUsuarioPruebaRegistro.Text);
            mensaje = mensaje.Replace("#PASSWORD#", txtPasswordPruebaRegistro.Text);
            i.EnviarCorreo(txtNotificacionRegistroTituloCorreo.Text, mensaje, txtUsuarioEmail.Text, correos);
        }

        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void chbNotificarExcepciones_CheckedChanged(object sender, EventArgs e)
        {
            txtCorreosErrores.Enabled = chbNotificarExcepciones.Checked;
        }

        private void txtNumDoc_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNombreRIDE.Text))
            {
                txtNombreRIDE.Text = txtNumDoc.Text;
            }
        }


    }
}
