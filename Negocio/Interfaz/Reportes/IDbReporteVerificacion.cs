using Comun.Areas.Reportes;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Reportes
{
    public interface IDbReporteVerificacion
    {
        Task<DtoResultado<List<DtoReporteVerificacion>>> F_GetReporteVerificacion(int? Anio, string RolesUsuario, long CodigoUnidad);
    }
}
