using Comun.General;

namespace Negocio.Interfaz.Admin
{
    public interface IGestionToken
    {
        Task<DtoRespuesta<string>> ObtenerTokenAsync();
    }
}
