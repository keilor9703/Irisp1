namespace Ponal.Seguridad.MfaCliente.Modelos
{
    public class ApiGatewayUrls
    {
        public ApiGatewayUrls(string url)
        {
            Url = url;
            Token = $"{url}api/Cuenta/Token";
            Oud = $"{url}api/Oud/LoginOud";

            GetUsuarioPorIdentificacion = $"{url}api/Icahu/FuncionarioPorIdentificacion";
            ImagenFuncionario = $"{url}api/Icahu/ImagenFuncionarioB64";
            GetCarruselImagenesPonal = $"{url}api/Psi/CarruselImagenesPonal";
        }

        // ✅ Nuevo ctor overload para MFA (opcional)
        public ApiGatewayUrls(string url, string mfaBase) : this(url)
        {
            MfaBase = mfaBase;  
            MfaToken = $"{MfaBase}api/Token/TokenSistema";

            MfaState = $"{MfaBase}api/ApiMfa/State";
            MfaEnrollStart = $"{MfaBase}api/ApiMfa/EnrollStart";
            MfaEnrollConfirm = $"{MfaBase}api/ApiMfa/EnrollConfirm";
            MfaVerify = $"{MfaBase}api/ApiMfa/Verify";
            MfaIsTrusted = $"{MfaBase}api/ApiMfa/IsTrusted";
            MfaSaveTrusted = $"{MfaBase}api/ApiMfa/SaveTrusted";
            MfaReset = $"{MfaBase}api/ApiMfa/Reset";
            MfaChangeMfa = $"{MfaBase}api/ApiMfa/Admin/ChangeMfa";
            MfaResetRequest = $"{MfaBase}api/ApiMfa/ResetRequest";
            MfaResetConfirm = $"{MfaBase}api/ApiMfa/ResetConfirm";
            MfaResetExecute = $"{MfaBase}api/ApiMfa/ResetExecute";
            MfaTrustCleanUser = $"{MfaBase}api/ApiMfa/Admin/TrustClearUser";
        }


        public readonly string Url;
        public readonly string Token;
        public readonly string Oud;
        public readonly string GetUsuarioPorIdentificacion;
        public readonly string ImagenFuncionario;
        public readonly string GetCarruselImagenesPonal;

        // ✅ MFA
        public readonly string? MfaBase;
        public readonly string? MfaToken;
        public readonly string? MfaState;
        public readonly string? MfaEnrollStart;
        public readonly string? MfaEnrollConfirm;//////
        public readonly string? MfaVerify;
        public readonly string? MfaIsTrusted;
        public readonly string? MfaSaveTrusted;
        public readonly string? MfaReset;
        public readonly string? MfaChangeMfa;
        public readonly string? MfaResetRequest;
        public readonly string? MfaResetConfirm;
        public readonly string? MfaResetExecute;
        public readonly string? MfaTrustCleanUser;
    }
}
