using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportUtilities.DataSet.DS_FacturacionElectronicaTableAdapters;
using ReportUtilities.DataSet;
using System.Data;


namespace ReportUtilities
{
    public class Validacion
    {
        private DS_FacturacionElectronica.validacionDataTable dt = new DS_FacturacionElectronica.validacionDataTable();
        private validacionTableAdapter ta = new validacionTableAdapter();

        public bool GenerarActivacion(string referencia_id, string email) {
            bool flag = false;
            try
            {
                DataView dv = ta.GetData().DefaultView;
                dv.RowFilter = "valores='" + referencia_id + "' and tipo='ACTIVACION_CUENTA'";
                bool enviarNotificacion = false;
                string codigo = "";
                if (dv.Count == 0)
                {
                    codigo = GenerarCodigo();
                    int result = ta.Insert(-1, DateTime.Now, codigo, "ACTIVACION_CUENTA", DateTime.Now, 1, referencia_id);
                    if (result > 0)
                    {
                        enviarNotificacion = true;
                        flag = true;
                    }
                }
                else {
                    dv.RowFilter = "valores='" + referencia_id + "' and tipo='ACTIVACION_CUENTA' and estado=1";
                    if (dv.Count == 1) {
                        enviarNotificacion = true;
                        codigo = dv[0]["codigo_secreto"].ToString();
                    }
                }
                if (enviarNotificacion) {
                        Email e = new Email();
                        e.toAddress.Add(email);
                        e.from = "angar0202@gmail.com";
                        e.password = "090153391";
                        e.subject = "Activacion cuenta E-Invoice";
                        e.body = @"Para activar su cuenta al Portal web porfavor ingrese en el siguiente link: http://localhost:90/VirtualInvoice/Validacion/" + codigo;
                        e.Send();                        
                }
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return flag;
        }


        private string GenerarCodigo()
        {
            string[] letras = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string[] codigo = new string[48];
            string result = "";
            for (int i = 0; i < codigo.Length; i++)
            {
                int n = new Random().Next(0, 10);
                System.Threading.Thread.Sleep(100);
                int l = new Random().Next(0, letras.Length);
                System.Threading.Thread.Sleep(100);
                int t = new Random().Next(0, 3);
                switch (t)
                {
                    case 0:
                        codigo[i] = "" + n;
                        break;
                    case 1:
                        codigo[i] = "" + letras[l];
                        break;
                    case 2:
                        codigo[i] = "" + letras[l].ToUpper();
                        break;
                }
            }
            result = String.Join("", codigo);
            return result;
        }

    }
}
