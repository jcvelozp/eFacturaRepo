using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportUtilities.Tools
{
    public class Posicion
    {
        private float x;
        private float y;
        private bool saveInDataBase = true;
        [Browsable(false)]
        public string Name { get; set; }
        [Browsable(false)]
        public bool SaveInDataBase
        {
            get
            {
                return saveInDataBase;
            }
            set
            {
                saveInDataBase = value;
            }
        }
        public float X
        {
            get
            {
                if (SaveInDataBase)
                {
                    x = Convert.ToSingle(Configuraciones.Get(this.Name + ".X"));
                }
                return x;
            }
            set
            {
                if (SaveInDataBase)
                {
                    Configuraciones.Set(this.Name + ".X", value.ToString());
                }

                x = value;
            }
        }
        public float Y
        {
            get
            {
                if (SaveInDataBase)
                {
                    y = Convert.ToSingle(Configuraciones.Get(this.Name + ".Y"));
                }
                return y;
            }
            set
            {
                if (SaveInDataBase)
                {
                    Configuraciones.Set(this.Name + ".Y", value.ToString());
                }
                y = value;
            }
        }

    }
}
