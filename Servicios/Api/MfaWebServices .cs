using Comun.DtoMfa;
using Comun.General;
using Servicios.ApiInterfaz;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class MfaWebServices : IMfaWebServices
{
    private readonly HttpClient _http;
    private readonly ApiGatewayUrl _urls;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MfaWebServices(HttpClient httpClient, ApiGatewayUrl apiGatewayUrl)
    {
        _http = httpClient;
        _urls = apiGatewayUrl;
    }

    private static async Task<T> ReadAsAsync<T>(HttpResponseMessage resp)
    {
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}. Body inicia con: {body.Substring(0, Math.Min(200, body.Length))}");

        try
        {
            return JsonSerializer.Deserialize<T>(body, _jsonOpts)!;
        }
        catch (Exception ex)
        {
            throw new Exception($"Respuesta no es JSON válido. Body inicia con: {body.Substring(0, Math.Min(200, body.Length))}", ex);
        }
    }

    private static HttpRequestMessage BuildJsonRequest(HttpMethod method, string url, object? body, string? bearer)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(bearer))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return req;
    }

    public async Task<DtoResultado<string>> TokenSistemaAsync(DtoTokenSistemaReq req)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaToken!, req, bearer: null);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<string>>(resp);
    }

    public async Task<DtoResultado<DtoMfaState>> StateAsync(long identificacion, string usuario, string bearer)
    {
        var url = $"{_urls.MfaState}?identificacion={identificacion}&usuario={Uri.EscapeDataString(usuario)}";
        using var httpReq = BuildJsonRequest(HttpMethod.Get, url, body: null, bearer: bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<DtoMfaState>>(resp);
    }

    public async Task<DtoResultado<DtoMfaEnrollStartResp>> EnrollStartAsync(DtoMfaEnrollStartReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaEnrollStart!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<DtoMfaEnrollStartResp>>(resp);
    }

    public async Task<DtoResultado<DtoMfaEnrrollConfirmResp>> EnrollConfirmAsync(DtoMfaEnrollConfirmReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaEnrollConfirm!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<DtoMfaEnrrollConfirmResp>>(resp);
    }

    public async Task<DtoResultado<DtoMfaVerifyResp>> VerifyAsync(DtoMfaVerifyReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaVerify!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<DtoMfaVerifyResp>>(resp);
    }

    public async Task<DtoResultado<int>> IsTrustedAsync(long identificacion, string usuario, string deviceId, string bearer)
    {
        var url = $"{_urls.MfaIsTrusted}?identificacion={identificacion}&usuario={Uri.EscapeDataString(usuario)}&deviceId={Uri.EscapeDataString(deviceId)}";
        using var httpReq = BuildJsonRequest(HttpMethod.Get, url, body: null, bearer: bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<int>>(resp);
    }

    public async Task<DtoResultado<int>> ResetAsync(DtoMfaResetReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaReset!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<int>>(resp);
    }


    public async Task<DtoResultado<int>> ChangeMfaAsync(DtoMfaResetReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaChangeMfa!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<int>>(resp);
    }

    public async Task<DtoResultado<int>> TrustClearUserAsync(DtoMfaTrustClearReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaTrustCleanUser!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<int>>(resp);
    }


    public async Task<DtoResultado<int>> ResetRequestAsync(DtoMfaResetRequestReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaResetRequest!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<int>>(resp);
    }

    public async Task<DtoResultado<DtoMfaResetConfirmResp>> ResetConfirmAsync(DtoMfaResetConfirmReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaResetConfirm!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<DtoMfaResetConfirmResp>>(resp);
    }

    public async Task<DtoResultado<int>> ResetExecuteAsync(DtoMfaResetExecuteReq req, string bearer)
    {
        using var httpReq = BuildJsonRequest(HttpMethod.Post, _urls.MfaResetExecute!, req, bearer);
        using var resp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        return await ReadAsAsync<DtoResultado<int>>(resp);
    }

}
