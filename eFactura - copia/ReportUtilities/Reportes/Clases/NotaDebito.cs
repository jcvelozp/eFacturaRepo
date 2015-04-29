using ReportUtilities.Reportes.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities
{
    public class NotaDebito:Plantilla
    {
        public NotaDebito(int secuencia, string id_cliente)
            : base(secuencia, id_cliente)
        {
           this.NombreDocumento = "NotaDebito";
           this.DataSourceName = "DS_Det_Documento";
           this.ReportFileNameRDLC = "ride_nota_debito";
           if (Cabecera != null)
           {
               this.taDet.FillBySecuenciaCab(dtDet, this.Cabecera.SECUENCIA);
               this.taDoc.FillByIdCabecera(dtDoc, this.Cabecera.SECUENCIA);
               foreach (var _item in dtDet)
               {
                   if (dtDoc.Rows.Count > 0)
                   {
                       _item.BARCODE = imageToByteArray(GenerarBarcode(dtDoc[0].clave_acceso));
                   }
               }
               Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter("comprobante_modificar", Cabecera.NUM_COMP_MODIFICA));
               Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter("fecha_modifica", Cabecera.FEC_COMP_MODIFICA));
               Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter("motivo_modifica", Cabecera.MOTIVO));
           }
           this.DataSource = dtDet;
        }
    }
}
