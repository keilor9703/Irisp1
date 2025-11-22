using Comun.Areas.Integrantes;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Integrantes
{
    public interface IDbBuscarIntegrantes
    {

        Task<DtoResultado<List<DtoDatosBasicos>>> F_GetIntegrantesPorId(long identificacion);
    }
}
