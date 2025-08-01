using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.AplicacionDTO
{
    public class Criminalidad_DTO
    {
        #region criminalidadDTO
        public decimal ID_UNIDAD { get; set; }
        public decimal ID_ZONA { get; set; }
        public decimal ID_CUADRANTE { get; set; }
        public long IDENTIFICACION_INFORMA { get; set; }
        public string CELULAR { get; set; }
        public decimal ID_TIPO_SERVICIO { get; set; }
        public decimal ID_CLASE { get; set; }
        public string NOMBRE_CLASE { get; set; }
        public decimal ID_TIEMPO_DELITO_STR { get; set; }
        public decimal CANTIDAD_INTEGRANTE { get; set; }
        public decimal ID_FUENTE { get; set; }
        //public FormFile FOTO_EXPENDIO { get; set; }
        public string CARACTERISTICAS_GENERALES { get; set; }
        #endregion
        #region  LAS VARIABLES NUEVAS PARA GUARDAR FOTO DE LOS EXPENDIOS
        public string servidor { get; set; }
        public string tipo_doc { get; set; }
        public string name_file { get; set; }
        public string ruta { get; set; }
        #endregion

        #region integranteDTO
        public string IntegranteDTO { get; set; }

        #endregion

        #region delitoDTO

        public string[] ARRAY_DELITO { get; set; }
        public List<Delito_DTO> delito_DTO { get; set; }

        public decimal DELITO_PRINCIPAL { get; set; }

        #endregion

        #region delitoDTO
        public string CODIGO_DANE { get; set; }
        public string CODIGO_ESTACION { get; set; }
        public string CUADRANTE_RURAL { get; set; }
        public string MUNICIPIO { get; set; }
        public string BARRIO { get; set; }
        public string DIRECCION { get; set; }
        public string LATITUD { get; set; }
        public string LONGITUD { get; set; }
        public string CUADRANTE { get; set; }
        public string RADIO_ACCION { get; set; }
        public string CODIGO_SIEDCO_CUADRANTE { get; set; }

        #endregion

        public int IDENTIFICACION_CREACION { get; set; }
        public string MAQUINA_CREACION { get; set; }

        public decimal CONSECUTIVO_CODIGO { get; set; }
        public string CODIGO { get; set; }
        public string SIGLA_UNIDAD { get; set; }
        public string CRIMINALIDAD_ID { get; set; }
        public DateTime FECHA_CREACION { get; set; }
        public int ID_ESPECIALIDAD { get; set; }
        public decimal CLASIFICACION_NARCOTRAFICO { get; set; }
        public decimal MODALIDAD_EXPENDIO { get; set; }
        public int ENTORNO_AFECTADO { get; set; }
        public string NOMBRE_ENTORNO_AFECTADO { get; set; }
        public string CELULAR_INTEGRANTE { get; set; }
        public string DIRECCION_INTEGRANTE { get; set; }
        public string STR_ESTADO { get; set; }
        public string ID_CRIMINALIDAD_FOTOS { get; set; }

    }
}
