using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Irisp1
{
    public class SeguimientoDto
    {
        public DateTime? FechaAsignacionVerificacionExistencia;

        public string CriminalidadId { get; set; }

        // VARCHAR2(40)
        public string IdResponsable { get; set; }

        // NUMBER
        public int? IdEstado { get; set; }

        // VARCHAR2(3000)
        public string EstadoDescripcion { get; set; }

        // NUMBER
        public int? IdEstadoExistencia { get; set; }

        // VARCHAR2(3000)
        public string EstadoExistenciaDescripcion { get; set; }

        // VARCHAR2(255)
        public string Codigo { get; set; }

        // NUMBER
        public int? IdUnidadResponsable { get; set; }

        // VARCHAR2(120)
        public string UnidadResponsable { get; set; }

        // NUMBER (mandatory en la tabla)
        public int? IdUnidad { get; set; }

        // VARCHAR2(120)
        public string Unidad { get; set; }

        // VARCHAR2(113)
        public string Dependencia { get; set; }

        // VARCHAR2(60)
        public string Municipio { get; set; }

        // DATE
        public DateTime? FechaInicioExistencia { get; set; }

        // NUMBER
        public int? IdClase { get; set; }

        // VARCHAR2(3000)
        public string Clase { get; set; }

        // VARCHAR2(255)
        public string NombreClase { get; set; }

        // NUMBER
        public int? CantidadIntegrantes { get; set; }

        // NVARCHAR2(2000)
        public string CaracteristicasGenerales { get; set; }

        // NVARCHAR2(255)
        public string DescripcionTramite { get; set; }

        // NUMBER
        public int? IdZona { get; set; }

        // VARCHAR2(3000)
        public string Zona { get; set; }

        // VARCHAR2(3000)
        public string TipoServicio { get; set; }

        // NUMBER
        public int? IdFuente { get; set; }

        // VARCHAR2(3000)
        public string Fuente { get; set; }

        // DATE
        public DateTime? FechaCreacion { get; set; }

        // NUMBER(12)
        public long? IdentificacionInforma { get; set; }

        // VARCHAR2(100)
        public string Celular { get; set; }

        // NUMBER
        public int? IdTipoServicio { get; set; }

        // NUMBER
        public int? IdCuadrante { get; set; }

        // NUMBER(1)
        public short? Vigente { get; set; }

        // VARCHAR2(100)
        public string MaquinaCrea { get; set; }

        // NUMBER(12)
        public long? IdentificacionCrea { get; set; }

        // DATE
        public DateTime? FechaModifica { get; set; }

        // NUMBER(12)
        public long? IdentificacionModifica { get; set; }

        // VARCHAR2(100)
        public string MaquinaModifica { get; set; }

        // NUMBER
        public int? ConsecutivoCodigo { get; set; }

        // VARCHAR2(10)
        public string SiglaUnidad { get; set; }

        // VARCHAR2(50)
        public string Cuadrante { get; set; }

        // VARCHAR2(100)
        public string DependCuadrante { get; set; }

        // VARCHAR2(100)
        public string EstacionCuadrante { get; set; }

        // VARCHAR2(100)
        public string Nivel1Cuadrante { get; set; }

        // VARCHAR2(20)
        public string CelularCuadrante { get; set; }

        // NUMBER
        public int? IdTipoResultado { get; set; }

        // VARCHAR2(3000)
        public string DescTipoResultado { get; set; }

        // VARCHAR2(30)
        public string NumeroResultado { get; set; }

        // VARCHAR2(96)
        public string EstadoResultados { get; set; }
               
    }
}
