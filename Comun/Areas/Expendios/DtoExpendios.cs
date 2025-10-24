using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.Areas.Expendios
{
    public class DtoExpendios
    {

        [JsonPropertyName("AnoIrisp1")]
        public Int32 AnoIrisp1 { get; set; }




        public string CriminalidadDirecId { get; set; }  // varchar2(40), mandatory
        public string Unidad { get; set; }               // varchar2(4000), optional
        public string Sigla { get; set; }                // varchar2(4000), optional
        public string Region { get; set; }               // varchar2(4000), optional
        public string Barrio { get; set; }               // varchar2(40), mandatory
        public string Direccion { get; set; }            // varchar2(40), mandatory
        public string Latitud { get; set; }              // varchar2(255), mandatory
        public string Longitud { get; set; }             // varchar2(255), mandatory
        public string Cuadrante { get; set; }            // varchar2(50), optional
        public string Categoria { get; set; }            // nvarchar2(2000), optional
        public string OtraCategoria { get; set; }        // varchar2(40), optional
        public string CodigoMored { get; set; }          // varchar2(50), optional
        public string NombreMored { get; set; }          // varchar2(40), optional
        public string Nunc { get; set; }                 // varchar2(50), optional
        public string Siedco { get; set; }               // varchar2(40), optional
        public int Vigente { get; set; }                 // number(1), mandatory
        public DateTime FechaCreacion { get; set; }      // date, mandatory
        public long IdentificacionCreacion { get; set; } // number(12), mandatory
        public string MaquinaCreacion { get; set; }      // varchar2(100), mandatory
        public DateTime? FechaModifica { get; set; }     // date, optional
        public long? IdentificacionModifica { get; set; }// number(12), optional
        public string MaquinaModifica { get; set; }      // varchar2(100), optional
        public string Municipio { get; set; }            // varchar2(255), mandatory
        public int UnidadInforma { get; set; }           // number, mandatory
        public string Zona { get; set; }                 // nvarchar2(2000), optional
        public string Clase { get; set; }                // nvarchar2(2000), optional
        public string Expendio { get; set; }             // nvarchar2(2000), optional
        public string Estado { get; set; }               // nvarchar2(2000), optional
        public string Fuente { get; set; }               // nvarchar2(2000), optional
        public DateTime FechaInicioExistencia { get; set; } // date, mandatory
        public string CaracteristicasGenerales { get; set; } // nvarchar2(255), optional
        public int? Erradicado { get; set; }             // number(1), optional
        public string Codigo { get; set; }               // varchar2(25), mandatory
        public int ConsecutivoCodigo { get; set; }       // number, mandatory
        public string SiglaPapa { get; set; }            // varchar2(10), mandatory
        public string Observacion { get; set; }          // varchar2(255), optional
    }
}



