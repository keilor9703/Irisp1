using Comun.Areas.Admin;
using Comun.General;

namespace Negocio.Interfaz.Admin
{
    public interface IGestionOUD
    {
        Task<DtoRespuesta<bool>> ObtenerOudAsync(DtoCredenciales _credenciales);
    }
}
