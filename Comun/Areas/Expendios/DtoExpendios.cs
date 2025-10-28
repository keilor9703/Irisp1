using System;
using System.Text.Json.Serialization;

namespace Comun.Areas.Expendios
{
    public class DtoExpendios
    {
        [JsonPropertyName("AnoIrisp1")]
        public Int32 AnoIrisp1 { get; set; }
        
        [JsonPropertyName("IdEstado")]
        public Int32 IdEstado { get; set; }

        [JsonPropertyName("CriminalidadDirecId")]
        public string CriminalidadDirecId { get; set; }

        [JsonPropertyName("Unidad")]
        public string Unidad { get; set; }

        [JsonPropertyName("Sigla")]
        public string Sigla { get; set; }

        [JsonPropertyName("Region")]
        public string Region { get; set; }

        [JsonPropertyName("Barrio")]
        public string Barrio { get; set; }

        [JsonPropertyName("Direccion")]
        public string Direccion { get; set; }

        [JsonPropertyName("Latitud")]
        public string Latitud { get; set; }

        [JsonPropertyName("Longitud")]
        public string Longitud { get; set; }

        [JsonPropertyName("Cuadrante")]
        public string Cuadrante { get; set; }

        [JsonPropertyName("Categoria")]
        public string Categoria { get; set; }

        [JsonPropertyName("OtraCategoria")]
        public string OtraCategoria { get; set; }

        [JsonPropertyName("CodigoMored")]
        public string CodigoMored { get; set; }

        [JsonPropertyName("NombreMored")]
        public string NombreMored { get; set; }

        [JsonPropertyName("Nunc")]
        public string Nunc { get; set; }

        [JsonPropertyName("Siedco")]
        public string Siedco { get; set; }

        [JsonPropertyName("Vigente")]
        public int Vigente { get; set; }

        [JsonPropertyName("FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonPropertyName("IdentificacionCreacion")]
        public long IdentificacionCreacion { get; set; }

        [JsonPropertyName("MaquinaCreacion")]
        public string MaquinaCreacion { get; set; }

        [JsonPropertyName("FechaModifica")]
        public DateTime? FechaModifica { get; set; }

        [JsonPropertyName("IdentificacionModifica")]
        public long? IdentificacionModifica { get; set; }

        [JsonPropertyName("MaquinaModifica")]
        public string MaquinaModifica { get; set; }

        [JsonPropertyName("Municipio")]
        public string Municipio { get; set; }

        [JsonPropertyName("UnidadInforma")]
        public int UnidadInforma { get; set; }

        [JsonPropertyName("Zona")]
        public string Zona { get; set; }

        [JsonPropertyName("Clase")]
        public string Clase { get; set; }

        [JsonPropertyName("Expendio")]
        public string Expendio { get; set; }

        [JsonPropertyName("Estado")]
        public string Estado { get; set; }

        [JsonPropertyName("Fuente")]
        public string Fuente { get; set; }

        [JsonPropertyName("FechaInicioExistencia")]
        public DateTime FechaInicioExistencia { get; set; }

        [JsonPropertyName("CaracteristicasGenerales")]
        public string CaracteristicasGenerales { get; set; }

        [JsonPropertyName("Erradicado")]
        public int? Erradicado { get; set; }

        [JsonPropertyName("Codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("ConsecutivoCodigo")]
        public int ConsecutivoCodigo { get; set; }

        [JsonPropertyName("SiglaUnidadInforma")]
        public string SiglaUnidadInforma { get; set; }

        [JsonPropertyName("Observacion")]
        public string Observacion { get; set; }

        [JsonPropertyName("UnidadInformaDescripcion")]
        public string UnidadInformaDescripcion { get; set; }
        
        [JsonPropertyName("RegionDescripcion")]
        public string RegionDescripcion { get; set; }
    }
}
