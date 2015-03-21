using iTextSharp.text.pdf;
using ReportUtilities.Core;
using ReportUtilities.DataSet;
using ReportUtilities.DataSet.DS_FacturacionElectronicaTableAdapters;
using ReportUtilities.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities.Reportes.Clases
{
    public class Plantilla : ReportMaster
    {
        #region Vars
        public DS_FacturacionElectronica ds = new DS_FacturacionElectronica();
        public DS_FacturacionElectronica.cab_documentoDataTable dtCab = new DS_FacturacionElectronica.cab_documentoDataTable();
        public DS_FacturacionElectronica.det_documentoDataTable dtDet = new DS_FacturacionElectronica.det_documentoDataTable();
        public DS_FacturacionElectronica.info_empresaDataTable dtEmpresa = new DS_FacturacionElectronica.info_empresaDataTable();
        public DS_FacturacionElectronica.documentosDataTable dtDoc = new DS_FacturacionElectronica.documentosDataTable();
        public DS_FacturacionElectronica.ambienteDataTable dtAmb = new DS_FacturacionElectronica.ambienteDataTable();
        public DS_FacturacionElectronica.tipo_emisionDataTable dtTipoEmision = new DS_FacturacionElectronica.tipo_emisionDataTable();
        public DS_FacturacionElectronica.usuarioDataTable dtUsuario = new DS_FacturacionElectronica.usuarioDataTable();
        public DS_FacturacionElectronica.det_retencionDataTable dtRetenciones = new DS_FacturacionElectronica.det_retencionDataTable();
        public DS_FacturacionElectronica.info_adicionalDataTable dtInfoAdicional = new DS_FacturacionElectronica.info_adicionalDataTable();

        public DS_FacturacionElectronica.info_empresaRow empresa = null;
        public cab_documentoTableAdapter taCab = new cab_documentoTableAdapter();
        public det_documentoTableAdapter taDet = new det_documentoTableAdapter();
        public info_empresaTableAdapter taEmp = new info_empresaTableAdapter();
        public documentosTableAdapter taDoc = new documentosTableAdapter();
        public ambienteTableAdapter taAmb = new ambienteTableAdapter();
        public tipo_emisionTableAdapter taTipoEmision = new tipo_emisionTableAdapter();
        public usuarioTableAdapter taUsuario = new usuarioTableAdapter();
        public det_retencionTableAdapter taRetenciones = new det_retencionTableAdapter();
        public info_adicionalTableAdapter taInfoAdicional = new info_adicionalTableAdapter();

        public string NombreDocumento = "Plantilla";
        public DS_FacturacionElectronica.cab_documentoRow Cabecera { get; set; }
        List<Microsoft.Reporting.WinForms.ReportParameter> parameters = null;
        public List<Microsoft.Reporting.WinForms.ReportParameter> Parametros {
            get {
                if (parameters == null) {
                    parameters = GetParameters();
                }
                return parameters;
            }
        } 
        #endregion

        public Plantilla(int secuencia, string id_cliente)
        {
            this.ReportProject = "ReportUtilities";
            this.TypeStream = this.GetType();
            this.taCab.FillByClienteID(dtCab, id_cliente,secuencia);
            this.taEmp.Fill(dtEmpresa);
            dtAmb = taAmb.GetDataByCodigo(Convert.ToInt32(Configuraciones.Ambiente));
            dtTipoEmision = taTipoEmision.GetDataByCodigo(Convert.ToInt32(Configuraciones.TipoEmision));
            dtUsuario = taUsuario.GetDataByCedula(id_cliente);
            if (dtEmpresa.Count > 0)
            {
                this.empresa = dtEmpresa[0];
            }
            if (dtCab.Count > 0) {
                this.Cabecera = dtCab[0];
                this.PathExportFile = ReportUtilities.Tools.Configuraciones.RutaRepositorioLocal + "/" + id_cliente + "/" + dtCab[0].TIPO_DOC;
            }
        }

        public List<Microsoft.Reporting.WinForms.ReportParameter> GetParameters()
        {
            parameters= new List<Microsoft.Reporting.WinForms.ReportParameter>();
            if (dtCab.Rows.Count > 0)
            {
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("ruc", empresa.ruc));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("no_documento", dtCab[0].ESTABLECIMIENTO + "-" + dtCab[0].PTO_EMISION + "-" + dtCab[0].NUM_DOC));

                string no_autorizacion = "";
                string fecha_autorizacion = "";
                string clave_acceso = "";

                if (dtDoc.Count > 0) {
                    no_autorizacion = dtDoc[0].num_autoriza;
                    fecha_autorizacion = dtDoc[0].fecha_autoriza;
                    clave_acceso=dtDoc[0].clave_acceso;
                }
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("no_autorizacion",no_autorizacion));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("fecha_autorizacion", fecha_autorizacion));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("clave_acceso",clave_acceso));

                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("ambiente", dtAmb[0].descripcion));
                
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("emision", "NORMAL"));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("identificacion", dtCab[0].ID_CLIENTE));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("razon_social", dtCab[0].RAZONS_CLIENTE));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("fecha_emision", dtCab[0].FECHA));
                var comprobanteModifica = "";
                if (!dtCab[0].IsNUM_COMP_MODIFICANull())
                {
                    comprobanteModifica=dtCab[0].NUM_COMP_MODIFICA;
                }
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("guia_remision", comprobanteModifica));                
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("direccion_matriz", empresa.direccion));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("direccion_sucursal", ""));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("no_contribuyente", empresa.contEspecial));
                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("obligado_contabilidad", empresa.ObligaConta));

                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("direccion_cliente", dtCab[0].DIRECCION_CLIENTE));
                if (dtUsuario.Count > 0)
                {
                    parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("email_cliente", dtUsuario[0].email));
                }
                else
                {
                    parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("email_cliente", "<sin correo>"));
                }

                parameters.Add(new Microsoft.Reporting.WinForms.ReportParameter("telefono_cliente", dtCab[0].TELEFONO));


            }
            return parameters;
        }

        public bool OpenFile(string filename)
        {
            try
            {
                CerrarPDF(filename);
                AbrirPDF(filename);
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return false;
        }

        public bool CerrarPDF(string filename)
        {
            //Cerrar Archivo de PDF
            string pdfPath = Path.Combine(this.PathExportFile, filename + ".pdf");
            Process[] processes = Process.GetProcessesByName("AcroRd32");
            var process = processes.FirstOrDefault(p => p.MainWindowTitle.Contains(filename));
            if (process != null)
            {
                process.Kill();
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            return false;
        }

        public void AbrirPDF(string filename)
        {
            string pdfPath = Path.Combine(this.PathExportFile, filename + ".pdf");
            Process.Start(pdfPath);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            if (imageIn != null) {
                MemoryStream ms = new MemoryStream();
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            return new byte []{0};
        }

        public System.Drawing.Bitmap GenerarBarcode(string prodCode)
        {
            if (prodCode.Length > 0)
            {
                Barcode128 code128 = new Barcode128();
                code128.CodeType = Barcode.CODE128;
                code128.ChecksumText = true;
                code128.GenerateChecksum = true;
                code128.StartStopText = true;
                code128.Code = prodCode;
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(code128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White));
                return bm;
            }
            return null;
        }

        public System.Drawing.Bitmap CreateImage(string path)
        {
            System.Drawing.Bitmap img = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(path, true);
            return img;
        }

        public void AgregarParametro(string key, string value) {
            Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter(key, value));
        }

    }
}
