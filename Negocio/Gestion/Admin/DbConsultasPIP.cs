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
using System.Threading.Tasks;

namespace Negocio.Gestion.Admin
{
    public class DbConsultasPIP : IDbConsultasPIP
    {
        private readonly IPipWebServices _iPipWebServices;
        private readonly CredencialesPipOptions _credenciales;
        private readonly ILogger<DbConsultasPIP> _logger;

        public DbConsultasPIP(
            IPipWebServices pipWebServices,
            IOptions<CredencialesPipOptions> opciones,
            ILogger<DbConsultasPIP> logger)
        {
            _iPipWebServices = pipWebServices;
            _credenciales = opciones.Value;
            _logger = logger;
        }

        public async Task<DtoRespuesta<string>> ObtenerTokenAsync()
        {
            const string operacion = "ObtenerTokenAsync";

            try
            {
                var resp = await _iPipWebServices.ObtenerTokenSeviciosAsync(new DtoUsuarioPip
                {
                    Usuario = _credenciales.Usuario,
                    Clave = _credenciales.Clave
                });

                if (!resp.Estado || string.IsNullOrWhiteSpace(resp.Respuesta))
                {
                    _logger.LogWarning(
                        "{Operacion} | Token no válido | Estado={Estado} | Codigo={Codigo} | Mensaje={Mensaje}",
                        operacion, resp.Estado, resp.Codigo, resp.Mensaje);
                }

                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Operacion} | Excepción obteniendo token PIP", operacion);

                return new DtoRespuesta<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error obteniendo token PIP: {ex.Message}",
                    Respuesta = string.Empty
                };
            }
        }

        public async Task<DtoRespuesta<bool>> ObtenerOudAsync(DtoCredenciales credenciales)
        {
            const string operacion = "ObtenerOudAsync";

            try
            {
                var token = await ObtenerTokenAsync();
                if (!token.Estado || string.IsNullOrWhiteSpace(token.Respuesta))
                {
                    _logger.LogWarning(
                        "{Operacion} | No fue posible obtener token | MensajeToken={MensajeToken}",
                        operacion, token.Mensaje);

                    return new DtoRespuesta<bool>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = "No fue posible obtener token para validar OUD.",
                        Respuesta = false
                    };
                }

                return await _iPipWebServices.ObtenerOudSeviciosAsync(credenciales, token.Respuesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Operacion} | Excepción validando OUD | Usuario={Usuario}",
                    operacion, credenciales?.UsuarioEmpresarial);

                return new DtoRespuesta<bool>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error validando OUD: {ex.Message}",
                    Respuesta = false
                };
            }
        }

        public async Task<DtoRespuesta<DtoFuncionariosPIP>> ObtenerDatosFuncionarioIdAsync(long identificacion)
        {
            const string operacion = "ObtenerDatosFuncionarioIdAsync";

            try
            {
                var token = await ObtenerTokenAsync();
                if (!token.Estado || string.IsNullOrWhiteSpace(token.Respuesta))
                {
                    _logger.LogWarning(
                        "{Operacion} | No fue posible obtener token | Identificacion={Identificacion} | MensajeToken={MensajeToken}",
                        operacion, identificacion, token.Mensaje);

                    return new DtoRespuesta<DtoFuncionariosPIP>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = "No fue posible obtener token para consultar datos del funcionario.",
                        Respuesta = new DtoFuncionariosPIP()
                    };
                }

                var resp = await _iPipWebServices.ObtenerFuncionariosIdSeviciosAsync(identificacion, token.Respuesta);

                if (!resp.Estado)
                {
                    _logger.LogWarning(
                        "{Operacion} | Respuesta no exitosa | Identificacion={Identificacion} | Codigo={Codigo} | Mensaje={Mensaje}",
                        operacion, identificacion, resp.Codigo, resp.Mensaje);
                }

                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Operacion} | Excepción consultando funcionario | Identificacion={Identificacion}",
                    operacion, identificacion);

                return new DtoRespuesta<DtoFuncionariosPIP>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error consultando funcionario: {ex.Message}",
                    Respuesta = new DtoFuncionariosPIP()
                };
            }
        }

        public async Task<DtoRespuesta<string>> ObtenerFotoFuncinarioAsync(long identificacion)
        {
            const string operacion = "ObtenerFotoFuncinarioAsync";

            try
            {
                var token = await ObtenerTokenAsync();
                if (!token.Estado || string.IsNullOrWhiteSpace(token.Respuesta))
                {
                    _logger.LogWarning(
                        "{Operacion} | No fue posible obtener token | Identificacion={Identificacion} | MensajeToken={MensajeToken}",
                        operacion, identificacion, token.Mensaje);

                    return new DtoRespuesta<string>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = "No fue posible obtener token para consultar la foto del funcionario.",
                        Respuesta = string.Empty
                    };
                }

                var resp = await _iPipWebServices.ObtenerFotoFuncionarioSeviciosAsync(identificacion, token.Respuesta);

                if (!resp.Estado)
                {
                    _logger.LogWarning(
                        "{Operacion} | Respuesta no exitosa | Identificacion={Identificacion} | Codigo={Codigo} | Mensaje={Mensaje}",
                        operacion, identificacion, resp.Codigo, resp.Mensaje);
                }

                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Operacion} | Excepción consultando foto funcionario | Identificacion={Identificacion}",
                    operacion, identificacion);

                return new DtoRespuesta<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error consultando foto del funcionario: {ex.Message}",
                    Respuesta = string.Empty
                };
            }
        }

        public async Task<DtoRespuesta<List<DtoCarrusel>>> ObtenerCarruselAsync()
        {
            const string operacion = "ObtenerCarruselAsync";

            try
            {
                var token = await ObtenerTokenAsync();
                if (!token.Estado || string.IsNullOrWhiteSpace(token.Respuesta))
                {
                    _logger.LogWarning(
                        "{Operacion} | No fue posible obtener token para carrusel | MensajeToken={MensajeToken}",
                        operacion, token.Mensaje);

                    return new DtoRespuesta<List<DtoCarrusel>>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = "No fue posible obtener token para el carrusel.",
                        Respuesta = new List<DtoCarrusel>()
                    };
                }

                var resp = await _iPipWebServices.ObtenerCarruselSeviciosAsync(token.Respuesta);

                if (!resp.Estado)
                {
                    _logger.LogWarning(
                        "{Operacion} | Respuesta no exitosa carrusel | Codigo={Codigo} | Mensaje={Mensaje}",
                        operacion, resp.Codigo, resp.Mensaje);
                }

                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Operacion} | Excepción consultando carrusel", operacion);

                return new DtoRespuesta<List<DtoCarrusel>>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error consultando carrusel: {ex.Message}",
                    Respuesta = new List<DtoCarrusel>()
                };
            }
        }
    }
}
