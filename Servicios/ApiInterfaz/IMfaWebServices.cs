using Comun.DtoMfa;
using Comun.General;

public interface IMfaWebServices
{
    Task<DtoResultado<string>> TokenSistemaAsync(DtoTokenSistemaReq req);
    Task<DtoResultado<DtoMfaState>> StateAsync(long identificacion, string usuario, string bearer);

    Task<DtoResultado<DtoMfaEnrollStartResp>> EnrollStartAsync(DtoMfaEnrollStartReq req, string bearer);
    Task<DtoResultado<bool>> EnrollConfirmAsync(DtoMfaEnrollConfirmReq req, string bearer);

    Task<DtoResultado<DtoMfaVerifyResp>> VerifyAsync(DtoMfaVerifyReq req, string bearer);

    Task<DtoResultado<int>> IsTrustedAsync(long identificacion, string usuario, string deviceId, string bearer);
    Task<DtoResultado<int>> ResetAsync(DtoMfaResetReq req, string bearer);
    Task<DtoResultado<int>> ChangeMfaAsync(DtoMfaResetReq req, string bearer);

    Task<DtoResultado<int>> ResetRequestAsync(DtoMfaResetRequestReq req, string bearer);

    Task<DtoResultado<DtoMfaResetConfirmResp>> ResetConfirmAsync(DtoMfaResetConfirmReq req, string bearer);

    Task<DtoResultado<int>> ResetExecuteAsync(DtoMfaResetExecuteReq req, string bearer);


  

}
