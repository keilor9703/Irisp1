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


namespace Servicios.Api
{
    public class PipWebServices: IPipWebServices
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
            var resultado = JsonSerializer.Deserialize<DtoRespuesta<DtoFuncionariosPIP>>(contenido, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return resultado!;
        }


    }
}
