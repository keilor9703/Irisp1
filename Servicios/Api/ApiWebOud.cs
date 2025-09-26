using Comun.Areas.Admin;
using Comun.General;
using Servicios.ApiInterfaz;
using System.Text;
using System.Text.Json;

namespace Servicios.Api
{
    /// <summary>
    /// servicio que conecta con el endpoint de oud
    /// </summary>
    public class ApiWebOud : IApiWebOud
    {
        private readonly ApiGatewayUrl _apiGatewayUrl;
        private readonly HttpClient _httpClient;

        public ApiWebOud(HttpClient httpClient, ApiGatewayUrl apiGatewayUrl)
        {
            _httpClient = httpClient;
            _apiGatewayUrl = apiGatewayUrl;
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

    }
}
