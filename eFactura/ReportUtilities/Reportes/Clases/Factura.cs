using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportUtilities.Core;
using ReportUtilities.DataSet;
using ReportUtilities.DataSet.DS_FacturacionElectronicaTableAdapters;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using ReportUtilities.Properties;
using System.Diagnostics;
using ReportUtilities.Tools;
using ReportUtilities.Reportes.Formatos;
using ReportUtilities.Reportes.Clases;

namespace ReportUtilities
{
    public class Factura : Plantilla
    {

        public Factura(int secuencia, string id_cliente)
            : base(secuencia, id_cliente)
        {
            
            this.NombreDocumento = "Factura";            
            this.DataSourceName = "DS_Det_Documento";
            //this.ReportFileNameRDLC = "ride_factura";
            this.ReportFileNameRDLC = "ride_factura_new";
            if (Cabecera != null) {
                this.taDet.FillBySecuenciaCab(dtDet, this.Cabecera.SECUENCIA);
                this.taDoc.FillByIdCabecera(dtDoc, this.Cabecera.SECUENCIA);
                this.taInfoAdicional.FillBySecuencia(dtInfoAdicional, this.Cabecera.SECUENCIA);
                foreach (var _item in dtDet)
                {
                    if (_item.BASE_SIN_IVA > 0)
                    {
                        _item.SUBTOTAL_0 = _item.BASE_SIN_IVA + _item.DESCUENTO;
                    }
                    else {
                        _item.SUBTOTAL_0 = 0;
                    }
                    if (_item.BASE_IMP_IVA > 0) 
                    {
                        _item.SUBTOTAL_12 = _item.BASE_IMP_IVA + _item.DESCUENTO;
                    }
                    else
                    {
                        _item.SUBTOTAL_12 = 0;
                    }
                    if (dtDoc.Rows.Count > 0)
                    {
                        _item.BARCODE = imageToByteArray(GenerarBarcode(dtDoc[0].clave_acceso));
                    }
                }
            }
            this.DataSource = dtDet;
            this.MultipleDataSource.Add("DS_Det_Documento", dtDet);
            this.MultipleDataSource.Add("DS_Info_Adicional", dtInfoAdicional);
        }

        #region iTextRIDE
        public void DrawPanelFactura(Document document, PdfWriter writer, FormatoFactura panel)
        {
            ReportUtilities.DataSet.DS_FacturacionElectronica.cab_documentoRow row = this.dtCab[0];

            PdfContentByte cb = writer.DirectContent;
            cb.RoundRectangle(panel.PanelFactura.PosicionPanel.X, panel.PanelFactura.PosicionPanel.Y, panel.PanelFactura.Ancho, panel.PanelFactura.Alto, panel.PanelFactura.Borde);
            ColumnText ct = new ColumnText(cb);
            AddText(ct, NombreDocumento, panel.PanelFactura.MargenReal.X + panel.Titulo.X, panel.PanelFactura.MargenReal.Y + panel.Titulo.Y, true, false, 18);
            Write(ct);
            AddText(ct, "RUC: ", panel.PanelFactura.MargenReal.X + panel.Ruc.X, panel.PanelFactura.MargenReal.Y - panel.Ruc.Y, true);
            AddText(ct, row.ID_CLIENTE, panel.PanelFactura.MargenReal.X, panel.PanelFactura.MargenReal.Y - panel.Ruc.Y);
            Write(ct);
            AddText(ct, "Nº: ", panel.PanelFactura.MargenReal.X + panel.Numero.X, panel.PanelFactura.MargenReal.Y - panel.Numero.Y, true);
            AddText(ct, row.PTO_EMISION + " - " + row.NUM_DOC, panel.PanelFactura.MargenReal.X + panel.Numero.X, panel.PanelFactura.MargenReal.Y - panel.Numero.Y);
            Write(ct);
            AddText(ct, "Número de Autorización: ", panel.PanelFactura.MargenReal.X + panel.Autorizacion.X, panel.PanelFactura.MargenReal.Y - panel.Autorizacion.Y, true, false);
            Write(ct);
            AddText(ct, dtDoc[0].num_autoriza, panel.PanelFactura.MargenReal.X + panel.AutorizacionCodigo.X, panel.PanelFactura.MargenReal.Y - panel.AutorizacionCodigo.Y, false, false);
            Write(ct);
            AddText(ct, "Fecha de Autorización: ", panel.PanelFactura.MargenReal.X + panel.Fecha.X, panel.PanelFactura.MargenReal.Y - panel.Fecha.Y, true);
            AddText(ct, DateTime.Now.ToShortDateString(), panel.PanelFactura.MargenReal.X + panel.Fecha.X, panel.PanelFactura.MargenReal.Y - panel.Fecha.Y);
            Write(ct);
            AddText(ct, "Ambiente: ", panel.PanelFactura.MargenReal.X + panel.Ambiente.X, panel.PanelFactura.MargenReal.Y - panel.Ambiente.Y, true);
            AddText(ct, "Desarrollo", panel.PanelFactura.MargenReal.X + panel.Ambiente.X, panel.PanelFactura.MargenReal.Y - panel.Ambiente.Y);
            Write(ct);
            AddText(ct, "Emision: ", panel.PanelFactura.MargenReal.X + panel.Emision.X, panel.PanelFactura.MargenReal.Y - panel.Emision.Y, true);
            AddText(ct, "Normal", panel.PanelFactura.MargenReal.X + panel.Emision.X, panel.PanelFactura.MargenReal.Y - panel.Emision.Y);
            Write(ct);
            AddText(ct, "CLAVE DE ACCESO", panel.PanelFactura.MargenReal.X + panel.EtiquetaClaveAcceso.X, panel.PanelFactura.MargenReal.Y - panel.EtiquetaClaveAcceso.Y, true, false, 10);
            Write(ct);
            Image gif = Image.GetInstance(imageToByteArray(GenerarBarcode(dtDoc[0].num_autoriza)));
            gif.ScalePercent(80f);
            gif.SetAbsolutePosition(panel.PanelFactura.MargenReal.X + panel.CodigoBarra.X, panel.PanelFactura.MargenReal.Y - panel.CodigoBarra.Y);
            document.Add(gif);
            Write(ct);
            /*Draw image Logo*/
            Image logo = Image.GetInstance(imageToByteArray(CreateImage(panel.RutaLogoEmpresa)));
            logo.ScalePercent(panel.LogoEmpresaEscala);
            logo.SetAbsolutePosition(panel.LogoEmpresa.X, panel.LogoEmpresa.Y);
            document.Add(logo);
            Write(ct);
            /*Panel Informacion Empresa*/
            cb.RoundRectangle(panel.PanelInfoEmpresa.PosicionPanel.X, panel.PanelInfoEmpresa.PosicionPanel.Y, panel.PanelInfoEmpresa.Ancho, panel.PanelInfoEmpresa.Alto, panel.PanelInfoEmpresa.Borde);
            AddText(ct, "SERVICIO DE RENTAS INTERNAS", panel.PanelInfoEmpresa.MargenReal.X + panel.TituloInfoEmpresa.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.TituloInfoEmpresa.Y, true, false, 10);
            Write(ct);
            AddText(ct, "Dirección Matriz: ", panel.PanelInfoEmpresa.MargenReal.X + panel.DireccionMatrizEmpresa.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.DireccionMatrizEmpresa.Y, true);
            AddText(ct, "xxxxxxxxxxxx", panel.PanelInfoEmpresa.MargenReal.X + panel.DireccionMatrizEmpresa.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.DireccionMatrizEmpresa.Y);
            Write(ct);
            AddText(ct, "Dirección Sucursal: ", panel.PanelInfoEmpresa.MargenReal.X + panel.DireccionSucursalEmpresa.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.DireccionSucursalEmpresa.Y, true);
            AddText(ct, "xxxxxxxxxxxx", panel.PanelInfoEmpresa.MargenReal.X + panel.DireccionSucursalEmpresa.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.DireccionSucursalEmpresa.Y);
            Write(ct);
            AddText(ct, "Contribuyente Especial Nro: ", panel.PanelInfoEmpresa.MargenReal.X + panel.NumeroContribuyente.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.NumeroContribuyente.Y, true);
            AddText(ct, "123456", panel.PanelInfoEmpresa.MargenReal.X + panel.NumeroContribuyente.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.NumeroContribuyente.Y);
            Write(ct);
            AddText(ct, "OBLIGADO A LLEVAR CONTABILIDAD: ", panel.PanelInfoEmpresa.MargenReal.X + panel.ObligadoALlevarContabilidad.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.ObligadoALlevarContabilidad.Y, true);
            AddText(ct, "Si", panel.PanelInfoEmpresa.MargenReal.X + panel.ObligadoALlevarContabilidad.X, panel.PanelInfoEmpresa.MargenReal.Y - panel.ObligadoALlevarContabilidad.Y);
            Write(ct);

            /*Panel informacion Cliente*/
            cb.RoundRectangle(panel.PanelInfoCliente.PosicionPanel.X, panel.PanelInfoCliente.PosicionPanel.Y, panel.PanelInfoCliente.Ancho, panel.PanelInfoCliente.Alto, panel.PanelInfoCliente.Borde);
            AddText(ct, "Razón Social/ Nombre y Apellidos: ", panel.PanelInfoCliente.MargenReal.X + panel.RazonSocialONombreCompleto.X, panel.PanelInfoCliente.MargenReal.Y - panel.RazonSocialONombreCompleto.Y, true);
            AddText(ct, "xxxxxxxxxxxx", panel.PanelInfoCliente.MargenReal.X + panel.RazonSocialONombreCompleto.X, panel.PanelInfoCliente.MargenReal.Y - panel.RazonSocialONombreCompleto.Y);
            Write(ct);
            AddText(ct, "Identificación: ", panel.PanelInfoCliente.MargenReal.X + panel.Identificacion.X, panel.PanelInfoCliente.MargenReal.Y - panel.Identificacion.Y, true);
            AddText(ct, "xxxxxxxxxxxx", panel.PanelInfoCliente.MargenReal.X + panel.Identificacion.X, panel.PanelInfoCliente.MargenReal.Y - panel.Identificacion.Y);
            Write(ct);
            AddText(ct, "Fecha Emisión: ", panel.PanelInfoCliente.MargenReal.X + panel.FechaEmision.X, panel.PanelInfoCliente.MargenReal.Y - panel.FechaEmision.Y, true);
            AddText(ct, "10/10/2014", panel.PanelInfoCliente.MargenReal.X + panel.FechaEmision.X, panel.PanelInfoCliente.MargenReal.Y - panel.FechaEmision.Y);
            Write(ct);
            AddText(ct, "Guia Remisión: ", panel.PanelInfoCliente.MargenReal.X + panel.GuiaRemision.X, panel.PanelInfoCliente.MargenReal.Y - panel.GuiaRemision.Y, true);
            AddText(ct, "xxxxxxxxxxxxx", panel.PanelInfoCliente.MargenReal.X + panel.GuiaRemision.X, panel.PanelInfoCliente.MargenReal.Y - panel.GuiaRemision.Y);
            Write(ct);
            cb.Stroke();
            //DrawPanelDetalle(document,writer);
        }

        public void DrawPanelDetalle(Document document, PdfWriter writer)
        {
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            //fix the absolute width of the table
            table.LockedWidth = true;

            //relative col widths in proportions - 1/3 and 2/3
            float[] widths = new float[] { 2f, 4f, 6f };
            table.SetWidths(widths);
            table.HorizontalAlignment = 0;

            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            /*PdfPCell cell = new PdfPCell(new Phrase("Header spanning 3 columns"));
            cell.Colspan = 3;
            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
            table.AddCell(cell);
            table.AddCell("Col 1 Row 1");
            table.AddCell("Col 2 Row 1");
            table.AddCell("Col 3 Row 1");
            table.AddCell("Col 1 Row 2");
            table.AddCell("Col 2 Row 2");
            table.AddCell("Col 3 Row 2");

            table.
            

            document.Add(table);*/
        }

        public void GenerarPDF(string filename, PanelReporte pos)
        {

            CerrarPDF(filename);
            //Genera PDF
            System.IO.FileStream fs = new FileStream(this.PathExportFile + "/" + filename + ".pdf", FileMode.Create);
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.AddTitle("Prueba de Documento de Factura");
            document.Open();
            DrawPanelFactura(document, writer, new FormatoFactura());
            document.Close();
            writer.Close();
            fs.Close();
            //System.Windows.Forms.MessageBox.Show("Se a generado el PDF correctamente");          
            AbrirPDF(filename);
        }

        private void AddText(ColumnText ct, String value, float x, float y, bool isTitle = false, bool isSameLine = true, float size = 8)
        {
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, size);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, size);
            Font font = normalFont;
            Phrase txt = new Phrase();
            if (isTitle)
            {
                font = boldFont;
            }
            txt.Add(new Chunk(value, font));
            ct.SetSimpleColumn(txt, x, y, 580, 317, 15, Element.ALIGN_LEFT);
            if (!isSameLine)
            {
                ct.Go();
            }
        }

        public void Write(ColumnText ct)
        {
            ct.Go();
        }
        #endregion
        
    }
}
