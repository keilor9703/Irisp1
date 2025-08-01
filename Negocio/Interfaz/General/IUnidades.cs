using Comun.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.General
{
    public interface IUnidades
    {
        public Task<DtoResultado<List<UnidadesDTO>>> ConsultarUnidades();
    }
}
