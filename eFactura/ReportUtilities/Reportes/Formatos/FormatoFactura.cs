using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportUtilities.Tools;
using ReportUtilities.Core;
using System.ComponentModel;
namespace ReportUtilities.Reportes.Formatos
{
    public class FormatoFactura
    {
        #region Vars
        private PanelReporte panelInforFactura = new PanelReporte();
        private PanelReporte panelInfoEmpresa = new PanelReporte();
        private PanelReporte panelInfoCliente = new PanelReporte();
        private Posicion titulo = new Posicion();
        private Posicion ruc = new Posicion();
        private Posicion num = new Posicion();
        private Posicion autorizacion = new Posicion();
        private Posicion autorizacionCodigo = new Posicion();
        private Posicion codigoBarra = new Posicion();
        private Posicion fecha = new Posicion();
        private Posicion ambiente = new Posicion();
        private Posicion emision = new Posicion();
        private Posicion etiquetaClaveAcceso = new Posicion();
        private Posicion logo = new Posicion();  
        #endregion

        #region Informacion Factura

        [Browsable(true)]                         // this property should be visible
        [ReadOnly(false)]                          // but just read only
        [Description("Sirve para que las etiquetas utilicen la posicion del Panel como posicion principal")]             // sample hint1
        [Category("Informacion Factura")]                   // Category that I want
        [DisplayName("Utilizar Margen del Panel de Factura")]       // I want to say more, than just DisplayInt
        public bool UtilizarMargenPanelFactura
        {
            get
            {
                return Convert.ToBoolean(Configuraciones.Get("UtilizarMargenPanelFactura"));
            }
            set
            {
                Configuraciones.Set("UtilizarMargenPanelFactura", value.ToString());
            }
        }

        [Browsable(true)]                         // this property should be visible
        [ReadOnly(false)]                          // but just read only
        [Description("Establece las posiciones X y Y del panel dentro del Documento PDF, si la propiedad para utilizar margen esta 'Activa' las etiquetas utilizan esta posicion + el valor del Margen")]             // sample hint1
        [Category("Informacion Factura")]                   // Category that I want
        [DisplayName("Panel de Informacion de Facturas")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PanelReporte PanelFactura
        {
            get
            {
                panelInforFactura.Name = "PanelFactura";
                return panelInforFactura;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo que identifica el documento")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Titulo")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Titulo
        {
            get
            {
                titulo.Name = "Factura.Titulo";
                return titulo;
            }
        }
        [Browsable(true)]                         // this property should be visible        
        [Description("RUC de la empresa que genera la factura")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("RUC del Cliente")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Ruc
        {
            get
            {
                ruc.Name = "Factura.Ruc";
                return ruc;
            }
        }
        [Browsable(true)]                         // this property should be visible        
        [Description("Secuencia o Numero de Factura")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Numero de Factura")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Numero
        {
            get
            {
                num.Name = "Factura.Numero";
                return num;
            }
        }
        [Description("Numero de Autorizacion generado por el proceso de RIDE")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Etiqueta Numero de Autorizacion")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Autorizacion
        {
            get
            {
                autorizacion.Name = "Factura.Autorizacion";
                return autorizacion;
            }
        }
        [Description("Numero de Autorizacion generado por el proceso de RIDE")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Etiqueta Numero de Autorizacion(Codigo)")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion AutorizacionCodigo
        {
            get
            {
                autorizacionCodigo.Name = "Factura.AutorizacionCodigo";
                return autorizacionCodigo;
            }
        }

        [Description("Fecha de generacion de la Factura")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Fecha de Factura")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Fecha
        {
            get
            {
                fecha.Name = "Factura.Fecha";
                return fecha;
            }
        }
        [Description("Ambiente en que se esta trabajando")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Ambiente")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Ambiente
        {
            get
            {
                ambiente.Name = "Factura.Ambiente";
                return ambiente;
            }
        }
        [Description("Tipo de emision del documento")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Tipo de Emision")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Emision
        {
            get
            {
                emision.Name = "Factura.Emision";
                return emision;
            }
        }
        [Description("Codigo de Barra del numero de autorizacion")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Codigo de Barra")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion CodigoBarra
        {
            get
            {
                codigoBarra.Name = "Factura.CodigoBarra";
                return codigoBarra;
            }
        }

        [Description("Etiqueta del titulo que contiene el codigo de la clave de Acceso")]             // sample hint1
        [Category("Propiedades Panel de Factura")]                   // Category that I want
        [DisplayName("Etiqueta Clave de Acceso")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion EtiquetaClaveAcceso
        {
            get
            {
                etiquetaClaveAcceso.Name = "Factura.EtiquetaClaveAcceso";
                return etiquetaClaveAcceso;
            }
        }
        #endregion

        #region Logo Empresa
        /*LOGO DE LA EMPRESA*/
        [Description("Seleccione la ruta donde se encuentra la imagen para el logo de la empresa")]             // sample hint1
        [Category("Panel del Logo de la Empresa")]                   // Category that I want
        [DisplayName("Logo Empresa Ruta Archivo")]       // I want to say more, than just DisplayInt
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string RutaLogoEmpresa
        {
            get
            {
                return Tools.Configuraciones.Get("RutaLogoEmpresa");
            }
            set
            {
                Tools.Configuraciones.Set("RutaLogoEmpresa", value);
            }
        }

        [Description("Posiciones X y Y de la imagen dentro del Documento")]             // sample hint1
        [Category("Panel del Logo de la Empresa")]                   // Category that I want
        [DisplayName("Logo Empresa Posicion Imagen")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion LogoEmpresa
        {
            get
            {
                logo.Name = "Factura.LogoEmpresa";
                return logo;
            }
        }

        [Description("Escala o tamaño de la imagen")]             // sample hint1
        [Category("Panel del Logo de la Empresa")]                   // Category that I want
        [DisplayName("Logo Empresa Tamano")]       // I want to say more, than just DisplayInt
        [EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public float LogoEmpresaEscala
        {
            get
            {
                return Convert.ToSingle(Tools.Configuraciones.Get("LogoEmpresaEscala"));
            }
            set
            {
                Tools.Configuraciones.Set("LogoEmpresaEscala", value.ToString());
            }
        }
        #endregion

        #region Informacion Empresa
        [Browsable(true)]                         // this property should be visible
        [ReadOnly(false)]                          // but just read only
        [Description("Establece las posiciones X y Y del panel dentro del Documento PDF, si la propiedad para utilizar margen esta 'Activa' las etiquetas utilizan esta posicion + el valor del Margen")]             // sample hint1
        [Category("Informacion Empresa")]                   // Category that I want
        [DisplayName("Panel de Informacion de Facturas")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PanelReporte PanelInfoEmpresa
        {
            get {
                panelInfoEmpresa.Name = "Factura.PanelEmpresa";
                return panelInfoEmpresa;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Empresa")]                   // Category that I want
        [DisplayName("Titulo")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion TituloInfoEmpresa
        {
            get
            {
                titulo.Name = "Factura.TituloInfoEmpresa";
                return titulo;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Empresa")]                   // Category that I want
        [DisplayName("Direccion Matriz")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion DireccionMatrizEmpresa
        {
            get
            {
                titulo.Name = "Factura.DireccionMatrizEmpresa";
                return titulo;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Empresa")]                   // Category that I want
        [DisplayName("Direccion Sucursal")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion DireccionSucursalEmpresa
        {
            get
            {
                titulo.Name = "Factura.DireccionSucursalEmpresa";
                return titulo;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Empresa")]                   // Category that I want
        [DisplayName("Numero de Contribuyente Especial")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion NumeroContribuyente
        {
            get
            {
                titulo.Name = "Factura.NumeroContribuyente";
                return titulo;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Empresa")]                   // Category that I want
        [DisplayName("Obligado a llevar Contabilidad")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion ObligadoALlevarContabilidad
        {
            get
            {
                titulo.Name = "Factura.ObligadoALlevarContabilidad";
                return titulo;
            }
        }

        #endregion

        #region Informacion Cliente
        [Browsable(true)]                         // this property should be visible
        [ReadOnly(false)]                          // but just read only
        [Description("Establece las posiciones X y Y del panel dentro del Documento PDF, si la propiedad para utilizar margen esta 'Activa' las etiquetas utilizan esta posicion + el valor del Margen")]             // sample hint1
        [Category("Informacion Cliente")]                   // Category that I want
        [DisplayName("Panel de Informacion de Cliente")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PanelReporte PanelInfoCliente
        {
            get
            {
                panelInfoCliente.Name = "Factura.PanelInfoCliente";
                return panelInfoCliente;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Cliente")]             // sample hint1
        [Category("Propiedades Panel de Informacion Cliente")]                   // Category that I want
        [DisplayName("Razon Social/ Nombre y Apellido")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion RazonSocialONombreCompleto
        {
            get
            {
                titulo.Name = "Factura.RazonSocialONombreCompleto";
                return titulo;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Cliente")]                   // Category that I want
        [DisplayName("Identificacion")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion Identificacion
        {
            get
            {
                titulo.Name = "Factura.Identificacion";
                return titulo;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Cliente")]                   // Category that I want
        [DisplayName("Fecha Emision")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion FechaEmision
        {
            get
            {
                titulo.Name = "Factura.FechaEmision";
                return titulo;
            }
        }

        [Browsable(true)]                         // this property should be visible        
        [Description("Titulo del panel de Informacion de la Empresa")]             // sample hint1
        [Category("Propiedades Panel de Informacion Cliente")]                   // Category that I want
        [DisplayName("GuiaRemision")]       // I want to say more, than just DisplayInt
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Posicion GuiaRemision
        {
            get
            {
                titulo.Name = "Factura.GuiaRemision";
                return titulo;
            }
        }
        #endregion


    }
}
