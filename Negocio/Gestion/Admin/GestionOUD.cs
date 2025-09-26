using Comun.Areas.Admin;
using Comun.General;
using Negocio.Interfaz.Admin;
using Servicios.ApiInterfaz;

namespace Negocio.Gestion.Admin
{
    public class GestionOUD : IGestionOUD
    {
        private readonly IApiWebOud _apiWebOud;
        private readonly IGestionToken _GestionToken;

        public GestionOUD(IApiWebOud apiWebOud, IGestionToken gestionToken)
        {
            _apiWebOud = apiWebOud;
            _GestionToken = gestionToken;
        }

        public async Task<DtoRespuesta<bool>> ObtenerOudAsync(DtoCredenciales _credenciales)
        {
            var token = await _GestionToken.ObtenerTokenAsync();
            return await _apiWebOud.ObtenerOudSeviciosAsync(_credenciales, token.Respuesta);
        }

    }
}
