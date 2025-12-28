using Comun.Areas.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;
using System.Security.Claims;

namespace Web.Controllers
{
    [Route("api/cuenta")]
    [ApiController]
    public class CuentaApiController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbConsultasPIP _iDbConsultasPIP;

        public CuentaApiController(
            IHttpContextAccessor httpContext,
            IDbAdministracion iDbAdministracion,
            IDbConsultasPIP iDbConsultasPIP)
        {
            _httpContext = httpContext;
            _iDbAdministracion = iDbAdministracion;
            _iDbConsultasPIP = iDbConsultasPIP;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DtoCredenciales loginUsuario)
        {
            if (!ModelState.IsValid)
                return BadRequest("Credenciales inválidas");

            // 🔁 reutilizas EXACTAMENTE la lógica que ya tienes
            var respuestaOud = await _iDbConsultasPIP.ObtenerOudAsync(loginUsuario);
            if (!respuestaOud.Respuesta)
                return Unauthorized("Usuario o contraseña incorrectos");

            var ip = _httpContext.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            _httpContext.HttpContext.Session.SetString("IpMaquina", ip);

            var usuario = await _iDbAdministracion.P_GetValidaUser(loginUsuario.UsuarioEmpresarial, ip);

            if (usuario.Data.Identificacion == 0)
                return Unauthorized("Usuario no registrado");

            if (usuario.Data.Bloqueado == 1)
                return Forbid("Usuario bloqueado");

            if (usuario.Data.DtoUserRoles.Count == 0)
                return Forbid("Usuario sin roles");

            // claims (idénticos a los actuales)
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Data.Usuario),
            new Claim("Identificacion", usuario.Data.Identificacion.ToString())
        };

            foreach (var rol in usuario.Data.DtoUserRoles)
                claims.Add(new Claim(ClaimTypes.Role, rol.IdRol.ToString()));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContext.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Ok(new
            {
                success = true,
                usuario = usuario.Data.Usuario
            });
        }
    }

}
