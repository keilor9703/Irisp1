using Comun.Areas.Irisp1;
using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Irisp1
{
   public interface IDbVerificacionIris
    {


        public Task<DtoResultado<List<DtoIrisp1>>> F_GetAniosIrisP1();
        public Task<DtoResultado<List<DtoIrispCriminalidad>>> F_GetInfoGrillas(Int32 V_Anio);
        public Task<DtoResultado<List<DtoTareasIris>>> F_GetTareas(string V_ResponsableId);


    }
}
