using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Comun.Areas.Admin
{
    /// <summary>
    /// clase para validar credenciales del logueo
    /// </summary>
    public class DtoCredenciales
    {
        private string _username = string.Empty;

        [Required, Display(Name = "Usuario empresarial")]
        [StringLength(30, ErrorMessage = "El {0} debe tener entre 5 y 30 caracteres.", MinimumLength = 5)]
        [JsonPropertyName("usuario")]
        public string UsuarioEmpresarial { get => _username; set { _username = value.Trim().ToLower(); } }


        [Required, Display(Name = "Contraseña")]
        [StringLength(50, ErrorMessage = "La {0} debe tener entre 8 y 50 caracteres.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [JsonPropertyName("contrasena")]
        public string ClaveEmpresarial { get; set; } = string.Empty;
    }
}
