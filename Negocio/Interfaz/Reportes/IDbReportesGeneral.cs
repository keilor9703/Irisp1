using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.Areas.Reportes;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Reportes
{
    public interface IDbReportesGeneral
    {


        Task<DtoResultado<List<DtoAnio>>> F_GetAniosIrisP1();


        Task<DtoResultado<List<DtoGeneralIrisp>>> F_GetReporteGeneral(string roles, Int32? unidad, int anio);
    }
}
