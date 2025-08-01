using Comun.Areas.Admin;
using Comun.General;

namespace Servicios.ApiInterfaz
{
    public interface IApiWebOud
    {
        Task<DtoRespuesta<bool>> ObtenerOudSeviciosAsync(DtoCredenciales _credenciales, string token);
    }
}
