namespace Comun.General
{
    public class ApiGatewayUrl
    {
        public ApiGatewayUrl(string url)
        {
            Url = url;
            Token = $"{url}api/Cuenta/Token";
            Oud = $"{url}api/Oud/LoginOud";

            GetUsuarioPorIdentificacion = $"{url}api/Icahu/FuncionarioPorIdentificacion";
            ImagenFuncionario = $"{url}api/Icahu/ImagenFuncionarioB64";
            GetCarruselImagenesPonal = $"{url}api/Psi/CarruselImagenesPonal";
        }

        public readonly string Url;
        public readonly string Token;
        public readonly string Oud;

        public readonly string GetUsuarioPorIdentificacion;
        public readonly string ImagenFuncionario;
        public readonly string GetCarruselImagenesPonal;

    }
}
