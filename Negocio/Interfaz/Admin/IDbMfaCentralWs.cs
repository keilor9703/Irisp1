using Comun.DtoMfa;
using Comun.General;

namespace Negocio.Interfaz.Admin
{
    public interface IDbMfaCentralWs
    {
        Task<DtoResultado<DtoMfaState>> StateAsync(long identificacion, string usuario);
        // Task<DtoResultado<DtoMfaTrustClearReq>> TrustClearUserAsync(long identificacion, string usuario);

        Task<DtoResultado<int>> TrustClearUserAsync(long identificacion, string usuario, string ip, long usuarioAudita);

        Task<DtoResultado<DtoMfaEnrollStartResp>> EnrollStartAsync(long identificacion, string usuario);

        Task<DtoResultado<DtoMfaEnrrollConfirmResp>> EnrollConfirmAsync(long identificacion, string usuario, string enrollToken, string code, string ip, long userAudit);

        Task<DtoResultado<DtoMfaVerifyResp>> VerifyAsync(long identificacion, string usuario, string code, bool rememberDevice, string? deviceId, string ip, long userAudit);

        Task<DtoResultado<int>> IsTrustedAsync(long identificacion, string usuario, string deviceId);

        Task<DtoResultado<int>> ResetAsync(long identificacion, string usuario, string ip, long userAudit);
        Task<DtoResultado<int>> ChangeMfaAsync(long identificacion, string usuario, int? V_Estado2Fa, string ip, long userAudit);
        
        Task<DtoResultado<int>> ResetRequestAsync(long identificacion, string usuario, string ip, long userAudit);
        
        Task<DtoResultado<DtoMfaResetConfirmResp>> ResetConfirmAsync(long identificacion, string usuario, string code, string ip, long userAudit);
        
        Task<DtoResultado<int>> ResetExecuteAsync(long identificacion, string usuario, string ip, long userAudit);
    }
}
