using System.Text.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class SeguimientoIrisDto
    {
        
             
        [JsonPropertyName("AnoIrisp1")]
        public Int32 AnoIrisp1 { get; set; }

        public int CriminalidadId { get; set; }
        public int IdResponsable { get; set; }
        public string? IdEstado { get; set; }
        public string EstadoDescripcion { get; set; }
        public string IdEstadoExistencia { get; set; }
        public string? EstadoExistenciaDescripcion { get; set; }
        public string? Codigo { get; set; }
        public string? IdUnidadResponsable { get; set; }
        public string? UnidadResponsable { get; set; }
        public string? IdUnidad { get; set; }
        public string? Unidad { get; set; }
        public string? Dependencia { get; set; }
        public string? Municipio { get; set; }
        public string? FechaInicioExistencia { get; set; }
        public string? IdClase { get; set; }
        public string? Clase { get; set; }
        public string? NombreClase { get; set; }
        public string? CantidadIntegrantes { get; set; }
        public string? CaracteristicasGenerales { get; set; }
        public string? DescripcionTramite { get; set; }
        public string? IdZona { get; set; }
        public string? Zona { get; set; }
        public string? TipoServicio { get; set; }
        public string? IdFuente { get; set; }
        public string? Fuente { get; set; }
        public string? FechaCreacion { get; set; }
        public string? IdentificacionInforma { get; set; }
        public string? Celular { get; set; }
        public string? IdTipoServicio { get; set; }
        public string? IdCuadrante { get; set; }
        public string? Vigente { get; set; }
        public string? MaquinaCrea  { get; set; }
        public string? SiglaUnidad  { get; set; }
        public string? Cuadrante    { get; set; }
        public string? IdentificacionCrea { get; set; }
        public string? FechaModifica { get; set; }
        public string? IdentificacionModifica { get; set; }
        public string? MaquinaModifica { get; set; }
        public string? ConsecutivoCodigo { get; set; }
        public string? DependCuadrante { get; set; }
        public string? Estacioncuadrante { get; set; }
        public string? Nivel1cuadrante { get; set; }
        public string? CelularCuadrante { get; set; }
        public string? IdTipoResultado { get; set; }
        public string? DescTipoResultado { get; set; }
        public string? NumeroResultado { get; set; }
        public string? EstadoResultados { get; set; }
        //public string? FechaAsignacionVerificacionExistencia { get; set; }
    }

}
