using Comun.Areas.Admin;
using Comun.General;
using Servicios.ApiInterfaz;
using System.Text;
using System.Text.Json;

namespace Servicios.Api
{
    /// <summary>
    /// servicio que conecta con el endpoint de token
    /// </summary>
    public class ApiWebToken : IApiWebToken
    {
        private readonly ApiGatewayUrl _apiGatewayUrl;
        private readonly HttpClient _httpClient;

        public ApiWebToken(HttpClient httpClient, ApiGatewayUrl apiGatewayUrl)
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
    }
}
