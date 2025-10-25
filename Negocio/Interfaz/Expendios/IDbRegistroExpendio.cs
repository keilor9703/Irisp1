using Comun.Areas.Expendios;
using Comun.Areas.Integrantes;
using Comun.Areas.Irisp1;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Expendios
{
    public interface IDbRegistroExpendio
    {

        Task<DtoResultado<List<DtoExpendios>>> F_GetAniosIrisP1();
        Task<DtoResultado<List<DtoExpendios>>> F_GetInfoGrillas(Int32 V_Anio);

        Task<DtoResultado<List<DtoIntegrantes>>> F_GetIntegrantes(string V_CriminalidadId);
        Task<DtoResultado<List<DtoDelitosIris>>> F_GetDelitosIris(string V_CriminalidadId);
        Task<DtoResultado<List<DtoInfoAdicional>>> F_GetBitacora(string V_CriminalidadId);
        Task<DtoResultado<List<DtoResultadosExpendio>>> F_GetResultados(string V_CriminalidadId);



    }
}
