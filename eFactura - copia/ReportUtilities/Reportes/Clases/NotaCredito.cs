using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportUtilities.Core;
using ReportUtilities.Reportes.Clases;
namespace ReportUtilities
{
    public class NotaCredito:Plantilla
    {
        public NotaCredito(int secuencia, string id_cliente)
            : base(secuencia, id_cliente)
        {
           this.NombreDocumento = "NotaCredito";
           this.DataSourceName = "DS_Det_Documento";
           this.ReportFileNameRDLC = "ride_nota_credito";
           if (Cabecera != null)
           {
               this.taDet.FillBySecuenciaCab(dtDet, this.Cabecera.SECUENCIA);
               this.taDoc.FillByIdCabecera(dtDoc, this.Cabecera.SECUENCIA);
               foreach (var _item in dtDet)
               {
                   if (_item.BASE_SIN_IVA > 0)
                   {
                       _item.SUBTOTAL_0 = _item.BASE_SIN_IVA + _item.DESCUENTO;
                   }
                   else
                   {
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
               Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter("comprobante_modificar", Cabecera.NUM_COMP_MODIFICA));
               Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter("fecha_modifica", Cabecera.FEC_COMP_MODIFICA));
               Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter("motivo_modifica", Cabecera.MOTIVO));
           }
           this.DataSource = dtDet;
        }
    }
}
