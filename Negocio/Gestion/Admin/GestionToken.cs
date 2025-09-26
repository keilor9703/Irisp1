using Comun.Areas.Admin;
using Comun.General;
using Negocio.Interfaz.Admin;
using Servicios.ApiInterfaz;

namespace Negocio.Gestion.Admin
{
    public class GestionToken : IGestionToken
    {
        private readonly IApiWebToken _apiWebToken;

        public GestionToken(IApiWebToken apiWebToken)
        {
            _apiWebToken = apiWebToken;
        }

        public async Task<DtoRespuesta<string>> ObtenerTokenAsync()
        {
            //var usuarioMsObtenido = _context!.GepadUsuarios!.AsNoTracking().FirstOrDefault(s => s.Vigente == true && s.IdTipoUsuario == 1);

            return await _apiWebToken.ObtenerTokenSeviciosAsync(new DtoUsuarioPip
            {
                //claveEmpresarial = usuarioMsObtenido!.UsuarioEmpresarial,
                //usuarioEmpresarial = usuarioMsObtenido!.UsuarioEmpresarial  

                Usuario = "USRSW.GEPAD",
                Clave = "Gepad2030*"
            });
        }
    }
}
