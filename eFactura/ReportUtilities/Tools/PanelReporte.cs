using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities.Tools
{
    public class PanelReporte
    {
        private Posicion pPanel = new Posicion();
        private Posicion pMargen = new Posicion();
        [Browsable(false)]
        public string Name { get; set; }

        [Browsable(true)]                         // this property should be visible
        [ReadOnly(false)]                          // but just read only
        [Description("Ancho del Panel")]             // sample hint1
        [Category("Dimension del Panel")]                   // Category that I want
        [DisplayName("Ancho")]       // I want to say more, than just DisplayInt
        public float Ancho
        {
            get
            {
                return Convert.ToSingle(Configuraciones.Get(this.Name + ".Ancho"));
            }
            set
            {
                Configuraciones.Set(this.Name + ".Ancho", value.ToString());
            }
        }

        [Browsable(true)]                         // this property should be visible
        [ReadOnly(false)]                          // but just read only
        [Description("Alto del Panel")]             // sample hint1
        [Category("Dimension del Panel")]                   // Category that I want
        [DisplayName("Alto")]       // I want to say more, than just DisplayInt
        public float Alto
        {
            get
            {
                return Convert.ToSingle(Configuraciones.Get(this.Name + ".Alto"));
            }
            set
            {
                Configuraciones.Set(this.Name + ".Alto", value.ToString());
            }
        }

        [Browsable(true)]                         // this property should be visible
        [ReadOnly(false)]                          // but just read only
        [Description("Borde del Panel")]             // sample hint1
        [Category("Dimension del Panel")]                   // Category that I want
        [DisplayName("Borde")]       // I want to say more, than just DisplayInt
        public float Borde
        {
            get
            {
                return Convert.ToSingle(Configuraciones.Get(this.Name + ".Borde"));
            }
            set
            {
                Configuraciones.Set(this.Name + ".Borde", value.ToString());
            }
        }

        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Posicion X y Y del Panel en el Documento")]
        [Category("Posicion")]
        [DisplayName("Posicion del Panel")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion PosicionPanel
        {
            get
            {
                pPanel.SaveInDataBase = true;
                pPanel.Name = this.Name + ".PosicionPanel";
                return pPanel;
            }
        }
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Margen de etiquetas con el Panel")]
        [Category("Posicion")]
        [DisplayName("Margen Etiquetas")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Margen
        {
            get
            {
                pMargen.SaveInDataBase = true;
                pMargen.Name = this.Name+".PosicionMargen";
                return pMargen;
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        [Description("Posicion X y Y del Margen en el Documento")]
        [Category("Posicion")]
        [DisplayName("Posicion Real del Margen")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion MargenReal
        {
            get
            {
                Posicion temp = new Posicion();
                temp.SaveInDataBase = false;
                temp.X = Margen.X + PosicionPanel.X;
                temp.Y = -Margen.Y + PosicionPanel.Y+this.Alto;
                return temp;
            }
        }
    }
}
