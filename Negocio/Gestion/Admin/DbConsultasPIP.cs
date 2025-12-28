using Comun.Areas.Admin;
using Comun.Areas.Admin.Comun.Areas.Admin;
using Comun.Enumeraciones;
using Comun.General;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Negocio.Interfaz.Admin;
using Servicios.ApiInterfaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion.Admin
{
    public class DbConsultasPIP: IDbConsultasPIP
    {

        private readonly IPipWebServices _iPipWebServices;
        private readonly CredencialesPipOptions _credenciales;
        private readonly ILogger _logger;

        public DbConsultasPIP(IPipWebServices pipWebServices, IOptions<CredencialesPipOptions> opciones, ILogger<DbAdministracion> logger)
                             
        {
            _iPipWebServices = pipWebServices;
            _credenciales = opciones.Value;
            _logger = logger;


        }

        public async Task<DtoRespuesta<string>> ObtenerTokenAsync()
        {

            return await _iPipWebServices.ObtenerTokenSeviciosAsync(new DtoUsuarioPip
            {
                Usuario = _credenciales.Usuario,
                Clave = _credenciales.Clave

            });
        }

        public async Task<DtoRespuesta<bool>> ObtenerOudAsync(DtoCredenciales _credenciales)
        {
            
            var token = await ObtenerTokenAsync();
            return await _iPipWebServices.ObtenerOudSeviciosAsync(_credenciales, token.Respuesta);
        }

        public async Task<DtoRespuesta<DtoFuncionariosPIP>> ObtenerDatosFuncionarioIdAsync(long identificacion)
        {
            var token = await ObtenerTokenAsync();
            return await _iPipWebServices.ObtenerFuncionariosIdSeviciosAsync(identificacion, token.Respuesta);
        }

        //public async Task<DtoRespuesta<DtoFuncionariosPIP>> ObtenerCarruselImgAsync()
        //{
        //    var token = await ObtenerTokenAsync();
        //    return await _iPipWebServices.ObtenerCarruselImgSeviciosAsync(identificacion, token.Respuesta);
        //}


        public async Task<DtoRespuesta<string>> ObtenerFotoFuncinarioAsync(long identificacion)
        {
            var token = await ObtenerTokenAsync();
            return await _iPipWebServices.ObtenerFotoFuncionarioSeviciosAsync(identificacion, token.Respuesta);
        }

        public async Task<DtoRespuesta<List<DtoCarrusel>>> ObtenerCarruselAsync()
        {
            var token = await ObtenerTokenAsync();
            if (!token.Estado || string.IsNullOrWhiteSpace(token.Respuesta))
            {
                return new DtoRespuesta<List<DtoCarrusel>>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = "No fue posible obtener token para el carrusel.",
                    Respuesta = new List<DtoCarrusel>()
                };
            }

            return await _iPipWebServices.ObtenerCarruselSeviciosAsync(token.Respuesta);
        }


    }
}
