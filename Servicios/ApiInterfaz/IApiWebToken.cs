using Comun.Areas.Admin;
using Comun.General;

namespace Servicios.ApiInterfaz
{
    public interface IApiWebToken
    {
        Task<DtoRespuesta<string>> ObtenerTokenSeviciosAsync(DtoUsuarioPip _usuarioMs);
    }
}
