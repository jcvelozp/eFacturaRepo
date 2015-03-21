using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities.Reportes.Clases
{
   public class GuiaRemision:Plantilla
    {
       private DataSet.DS_FacturacionElectronica.GuiaRemisionDataTable dtGuia = new DataSet.DS_FacturacionElectronica.GuiaRemisionDataTable();
       private DataSet.DS_FacturacionElectronicaTableAdapters.GuiaRemisionTableAdapter taGuia = new DataSet.DS_FacturacionElectronicaTableAdapters.GuiaRemisionTableAdapter();
       public GuiaRemision(int secuencia, string id_cliente)
           : base(secuencia, id_cliente)
        {
            
            this.NombreDocumento = "GuiaRemision";            
            this.DataSourceName = "DS_GuiaRemision";
            this.ReportFileNameRDLC = "ride_guia_remision";
            if (Cabecera != null) {
                //this.taDet.FillBySecuenciaCab(dtDet, this.Cabecera.SECUENCIA);

                this.taDoc.FillByIdCabecera(dtDoc, this.Cabecera.SECUENCIA);
               this.dtGuia= this.taGuia.GetDataGuiaRemision(Convert.ToDecimal(Cabecera.SECUENCIA));
                foreach (var _item in dtGuia)
                {
                    if (dtDoc.Rows.Count > 0)
                    {
                        _item.BARCODE = imageToByteArray(GenerarBarcode(dtDoc[0].clave_acceso));
                    }
                /*TRANSPORTISTA*/
                AgregarParametro("trans_nombre", Cabecera.TRANSPORTE);
                AgregarParametro("trans_placa", Cabecera.PLACA);
                AgregarParametro("trans_punto_partida", Cabecera.DIRECCION_PARTIDA);
                AgregarParametro("trans_fecha_inicio", Cabecera.FECHA_INI_TRAS);
                AgregarParametro("trans_fecha_fin", Cabecera.FECHA_FIN_TRAS);
                AgregarParametro("trans_identificacion", Cabecera.RUC_TRANSPORTE);
                /*DETALLE DE DOCUMENTO*/
                AgregarParametro("det_autorizacion", Cabecera.AUT_COMP_MODIFICA);
                AgregarParametro("det_motivo", _item.motivoTraslado);
                AgregarParametro("det_destino", _item.dirDestinatario);
                AgregarParametro("det_identificacion", _item.idDestinatario);
                AgregarParametro("det_razon_social", _item.razonSocialDest);
                AgregarParametro("det_doc_aduanero", _item.docAduanero);
                AgregarParametro("det_cod_est_destino", _item.codEstabDest);
                AgregarParametro("det_ruta", _item.ruta);
                AgregarParametro("det_comprobante", _item.numSustento);
                AgregarParametro("det_fecha_emision_comp", _item.FechaSustento);
                }
            }
           this.DataSource = dtGuia;
        }
    }
}
