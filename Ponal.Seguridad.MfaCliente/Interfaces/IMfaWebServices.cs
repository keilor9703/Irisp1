
using Ponal.Seguridad.MfaCliente.Modelos;
using Ponal.Seguridad.MfaCliente.Modelos.DtoMfa;

public interface IMfaWebServices
{
    Task<MfaWsResponse<string>> TokenSistemaAsync(DtoTokenSistemaReq req);
    Task<MfaWsResponse<DtoMfaState>> StateAsync(long identificacion, string usuario, string bearer);

    Task<MfaWsResponse<DtoMfaEnrollStartResp>> EnrollStartAsync(DtoMfaEnrollStartReq req, string bearer);
    Task<MfaWsResponse<DtoMfaEnrrollConfirmResp>> EnrollConfirmAsync(DtoMfaEnrollConfirmReq req, string bearer);

    Task<MfaWsResponse<DtoMfaVerifyResp>> VerifyAsync(DtoMfaVerifyReq req, string bearer);

    Task<MfaWsResponse<int>> IsTrustedAsync(long identificacion, string usuario, string deviceId, string bearer);
    Task<MfaWsResponse<int>> ResetAsync(DtoMfaResetReq req, string bearer);
    Task<MfaWsResponse<int>> ChangeMfaAsync(DtoMfaResetReq req, string bearer);

    Task<MfaWsResponse<int>> ResetRequestAsync(DtoMfaResetRequestReq req, string bearer);

    Task<MfaWsResponse<DtoMfaResetConfirmResp>> ResetConfirmAsync(DtoMfaResetConfirmReq req, string bearer);

    Task<MfaWsResponse<int>> ResetExecuteAsync(DtoMfaResetExecuteReq req, string bearer);

    Task<MfaWsResponse<int>> TrustClearUserAsync(DtoMfaTrustClearReq req, string bearer);





}
