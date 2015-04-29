using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities.Reportes.Clases
{
    public class Retencion:Plantilla
    {
        public Retencion(int secuencia, string id_cliente)
            : base(secuencia, id_cliente)
        {

            this.NombreDocumento = "Retencion";            
            this.DataSourceName = "DS_Retenciones";
            this.ReportFileNameRDLC = "ride_retencion";
            if (Cabecera != null) {
                this.taRetenciones.FillBySecuenciaCab(dtRetenciones, this.Cabecera.SECUENCIA);
                this.taDoc.FillByIdCabecera(dtDoc, this.Cabecera.SECUENCIA);
                foreach (var _item in dtRetenciones)
                {
                    if (dtDoc.Rows.Count > 0)
                    {
                        _item.BARCODE = imageToByteArray(GenerarBarcode(dtDoc[0].clave_acceso));
                    }
                }
                this.Parametros.Add(new Microsoft.Reporting.WinForms.ReportParameter("ejercicio_fiscal", GetEjercicioFiscal(Cabecera.FECHA)));
            }
            this.DataSource = dtRetenciones;
        }

        public string GetEjercicioFiscal(string val) {
            string e = "";
            var d = Convert.ToDateTime(val);
            var m = d.Month;
            var y = d.Year;

            if (m.ToString().Length == 0) {
                e+="00-";
            }
            else if (m.ToString().Length == 1)
            {
                e += "0" + m + "-";
            }
            else {
                e += m;
            }
            e += y;
            return e;
        }
    }
}
