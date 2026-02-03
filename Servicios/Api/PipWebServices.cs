using Comun.Areas.Admin;
using Comun.Enumeraciones;
using Comun.General;
using Servicios.ApiInterfaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Comun.Areas.Admin.Comun.Areas.Admin;



namespace Servicios.Api
{
    public class PipWebServices : IPipWebServices
    {

        private readonly ApiGatewayUrl _apiGatewayUrl;
        private readonly HttpClient _httpClient;


        public PipWebServices(HttpClient httpClient, ApiGatewayUrl apiGatewayUrl)
        {
            _httpClient = httpClient;
            _apiGatewayUrl = apiGatewayUrl;

        }


        public async Task<DtoRespuesta<string>> ObtenerTokenSeviciosAsync(DtoUsuarioPip _usuarioMs)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(_usuarioMs),
                Encoding.UTF8,
                "application/json"
            );
            var request = await _httpClient.PostAsync(_apiGatewayUrl.Token, content);

            var contenido = JsonSerializer.Deserialize<DtoRespuesta<string>>(
                await request.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    //no distingue entre mayúsculas y minúsculas durante la deserialización
                    PropertyNameCaseInsensitive = true
                });
            return contenido!;
        }

        public async Task<DtoRespuesta<bool>> ObtenerOudSeviciosAsync(DtoCredenciales _credenciales, string token)
        {

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + token);

            var content = new StringContent(
                JsonSerializer.Serialize(_credenciales),
                Encoding.UTF8,
                "application/json"
            );
            var request = await _httpClient.PostAsync(_apiGatewayUrl.Oud, content);

            var contenido = JsonSerializer.Deserialize<DtoRespuesta<bool>>(
                await request.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    //no distingue entre mayúsculas y minúsculas durante la deserialización
                    PropertyNameCaseInsensitive = true
                });

            return contenido!;
        }


        public async Task<DtoRespuesta<DtoFuncionariosPIP>> ObtenerFuncionariosIdSeviciosAsync(long identificacion, string token)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");

            var url = $"{_apiGatewayUrl.GetUsuarioPorIdentificacion}?_identificacion={identificacion}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new DtoRespuesta<DtoFuncionariosPIP>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error al consultar el servicio: {response.StatusCode}",
                    Respuesta = null!
                };
            }

            var contenido = await response.Content.ReadAsStringAsync();

            try
            {
                var resultado = JsonSerializer.Deserialize<DtoRespuesta<DtoFuncionariosPIP>>(contenido, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (resultado == null)
                {
                    return new DtoRespuesta<DtoFuncionariosPIP>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = "La respuesta del servicio no pudo ser deserializada.",
                        Respuesta = null!
                    };
                }

                return resultado;
            }
            catch (JsonException ex)
            {
                return new DtoRespuesta<DtoFuncionariosPIP>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error al deserializar la respuesta JSON: {ex.Message}",
                    Respuesta = null!
                };
            }
            catch (Exception ex)
            {
                return new DtoRespuesta<DtoFuncionariosPIP>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error inesperado al procesar la respuesta: {ex.Message}",
                    Respuesta = null!
                };
            }
        }

        public async Task<DtoRespuesta<string>> ObtenerFotoFuncionarioSeviciosAsync(long identificacion, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");

                var url = $"{_apiGatewayUrl.ImagenFuncionario}?_identificacion={identificacion}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new DtoRespuesta<string>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = $"Error al consumir el servicio de imagen: {response.StatusCode}",
                        Respuesta = null!
                    };
                }

                var json = await response.Content.ReadAsStringAsync();

                var resultado = JsonSerializer.Deserialize<DtoRespuesta<string>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (resultado == null || string.IsNullOrWhiteSpace(resultado.Respuesta))
                {
                    return new DtoRespuesta<string>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = "El servicio no devolvió imagen.",
                        Respuesta = null!
                    };
                }

                return resultado;
            }
            catch (JsonException ex)
            {
                return new DtoRespuesta<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error deserializando la imagen: {ex.Message}",
                    Respuesta = null!
                };
            }
            catch (Exception ex)
            {
                return new DtoRespuesta<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error general al obtener la imagen: {ex.Message}",
                    Respuesta = null!
                };
            }
        }


        public async Task<DtoRespuesta<List<DtoCarrusel>>> ObtenerCarruselSeviciosAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");

                // OJO: ya es URL completa según tu ApiGatewayUrl
                var url = _apiGatewayUrl.GetCarruselImagenesPonal;

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new DtoRespuesta<List<DtoCarrusel>>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = $"Error al consumir el servicio de Carrusel: {response.StatusCode}",
                        Respuesta = new List<DtoCarrusel>()
                    };
                }

                var json = await response.Content.ReadAsStringAsync();

                var resultado = JsonSerializer.Deserialize<DtoRespuesta<List<DtoCarrusel>>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (resultado == null || resultado.Respuesta == null || resultado.Respuesta.Count == 0)
                {
                    return new DtoRespuesta<List<DtoCarrusel>>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Estado = false,
                        Mensaje = "El servicio no devolvió imágenes de carrusel.",
                        Respuesta = new List<DtoCarrusel>()
                    };
                }

                return resultado;
            }
            catch (JsonException ex)
            {
                return new DtoRespuesta<List<DtoCarrusel>>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error deserializando el carrusel: {ex.Message}",
                    Respuesta = new List<DtoCarrusel>()
                };
            }
            catch (Exception ex)
            {
                return new DtoRespuesta<List<DtoCarrusel>>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Estado = false,
                    Mensaje = $"Error general al obtener el carrusel: {ex.Message}",
                    Respuesta = new List<DtoCarrusel>()
                };
            }
        }



    }
}
