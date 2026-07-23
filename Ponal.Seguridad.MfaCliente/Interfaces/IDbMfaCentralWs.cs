
using Ponal.Seguridad.MfaCliente.Modelos;
using Ponal.Seguridad.MfaCliente.Modelos.DtoMfa;


namespace Ponal.Seguridad.MfaCliente.Interfaces //
{
    public interface IDbMfaCentralWs
    {
        Task<MfaWsResponse<DtoMfaState>> StateAsync(long identificacion, string usuario);
        // Task<DtoResultado<DtoMfaTrustClearReq>> TrustClearUserAsync(long identificacion, string usuario);

        Task<MfaWsResponse<int>> TrustClearUserAsync(long identificacion, string usuario, string ip);

        Task<MfaWsResponse<DtoMfaEnrollStartResp>> EnrollStartAsync(long identificacion, string usuario);

        Task<MfaWsResponse<DtoMfaEnrrollConfirmResp>> EnrollConfirmAsync(long identificacion, string usuario, string enrollToken, string code, string ip);

        Task<MfaWsResponse<DtoMfaVerifyResp>> VerifyAsync(long identificacion, string usuario, string code, bool rememberDevice, string? deviceId, string ip);

        Task<MfaWsResponse<int>> IsTrustedAsync(long identificacion, string usuario, string deviceId);

        Task<MfaWsResponse<int>> ResetAsync(long identificacion, string usuario, string ip);
        Task<MfaWsResponse<int>> ChangeMfaAsync(long identificacion, string usuario, int? V_Estado2Fa, string ip);
        
        Task<MfaWsResponse<int>> ResetRequestAsync(long identificacion, string usuario, string ip);
        
        Task<MfaWsResponse<DtoMfaResetConfirmResp>> ResetConfirmAsync(long identificacion, string usuario, string code, string ip);
        
        Task<MfaWsResponse<int>> ResetExecuteAsync(long identificacion, string usuario, string ip);
    }
}
