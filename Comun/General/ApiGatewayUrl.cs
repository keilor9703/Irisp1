namespace Comun.General
{
    public class ApiGatewayUrl
    {
        public ApiGatewayUrl(string url)
        {
            Url = url;
            Token = $"{url}api/Cuenta/Token";
            Oud = $"{url}api/Oud/LoginOud";
            GetValidaUser = $"{url}api/GetValidaUser";
            GetUsuarioPorIdentificacion = $"{url}api/Icahu/FuncionarioPorIdentificacion";
            GetCarruselImagenesPonal = $"{url}api/Psi/CarruselImagenesPonal";
        }

        public readonly string Url;
        public readonly string Token;
        public readonly string Oud;
        public readonly string GetValidaUser;
        public readonly string GetUsuarioPorIdentificacion;
        public readonly string GetCarruselImagenesPonal;

    }
}
