using Comun.Areas.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.Admin;
using System.Security.Claims;

namespace Web.Areas.Admin.Controllers
{


    [Area("Admin")]
    [Authorize(Roles = "1,2")]//Super Usuario -- Admnistrador
    public class UsuariosController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IConfiguration _iConfiguration;
        private readonly ILogger<UsuariosController> _logger;
        #endregion
        #region Constructor
        public UsuariosController(IDbAdministracion iDbAdministracion, IConfiguration iConfiguration, ILogger<UsuariosController> logger)
        {
            _iDbAdministracion = iDbAdministracion;
            _iConfiguration = iConfiguration;
            _logger = logger;
        }
        #endregion

        public async Task<IActionResult> VwUsuarios()
        {
            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Administracion/Usuarios", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
            ViewBag.ddlRol = new SelectList((await _iDbAdministracion.F_GetRoles()).Data.ToList().OrderBy(x => x.DESCRIPCION).ToList(), "IDROL", "DESCRIPCION");
            // Solo se muestran los controles de 2FA cuando el sistema tiene MFA habilitado
            // (mismo flag que respeta el login), evitando exponer acciones MFA que no aplican.
            ViewBag.MfaEnabled = _iConfiguration.GetValue<bool>("MfaCentral:Enabled");
            return View();
        }

        #region Métodos de Consulta        
        public async Task<IActionResult> F_GetUserRoles(Int64 V_Identificacion)
        {
            var Resultado = await _iDbAdministracion.F_GetUserRoles(V_Identificacion);
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }




        public async Task<IActionResult> F_GetEstadoMfa(Int64 V_Identificacion, string V_Usuario)
        {
            var Resultado = await _iDbAdministracion.F_GetEstadoMfa(V_Identificacion, V_Usuario);
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }



        //public async Task<IActionResult> F_TrustClearUserAsync(long identificacion, string usuario)
        //{
        //    var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        //    var usuarioAudita = Convert.ToInt64(User.FindFirstValue("Identificacion"));

        //    var resp = await _iDbAdministracion.F_TrustClearUserAsync(identificacion, usuario, ip, usuarioAudita);

        //    if (resp.CodigoExito != 1)
        //        return Json(new { ok = false, msg = resp.Mensaje ?? "No fue posible revocar dispositivos." });

        //    // Opcional: si el usuario afectado es el mismo que está en esta sesión, borra cookie local
        //    var identClaim = User.FindFirstValue("Identificacion");
        //    if (!string.IsNullOrEmpty(identClaim) && long.TryParse(identClaim, out var identAdmin))
        //    {
        //        if (identAdmin == identificacion) { }
        //            //Response.Cookies.Delete(CookieTrusted);
        //    }

        //    return Json(new { ok = true, msg = "Dispositivos confiables revocados." });
        //}



        public async Task<IActionResult> F_GetListUsuarios()
        {
            var Resultado = await _iDbAdministracion.F_GetListUsuarios();
            if (Resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
            }
            else
            {
                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
        }
        #endregion

        #region Métodos de Inserción y Actualización

        /// Insertar un rol a un usuario en el formulario de Asignar Roles

        public async Task<IActionResult> P_InsRoles(DtoInsUserRoles obj)
        {
            if (obj.IdUsuario == 0)
                return Json(new { success = false, message = "Debe seleccionar usuario, revise" });

            if (obj.IdRol == 0)
                return Json(new { success = false, message = "Debe seleccionar rol, revise" });

            if (string.IsNullOrWhiteSpace(obj.Justificacion))
                return Json(new { success = false, message = "Debe registrar justificación, revise" });

            if (obj.FechaFin == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
                return Json(new { success = false, message = "Debe registrar fecha de caducidad del rol, revise" });

            try
            {
                var Resultado = await _iDbAdministracion.P_InsRolesUser(obj, Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
                if (Resultado.IdRespuesta > 0)
                {
                    return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
                }
                else
                {
                    return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en P_InsRoles");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }
        }
        public async Task<IActionResult> P_InsUdpUsuarios(DtoUsuario obj)
        {
            if (obj.Identificacion == 0)
                return Json(new { success = false, message = "Debe registrar Identificación, revise" });

            try
            {
                

                var Resultado = await _iDbAdministracion.P_InsUdpUsuarios(
                    obj.Identificacion,
                    obj.Bloqueado,
                    obj.Estado2Fa,   
                    obj.Usuario,
                    Convert.ToInt64(User.FindFirstValue("Identificacion")),
                    HttpContext.Session.GetString("IpMaquina"),
                    obj.LimpiarDispConfiable
                );

                if (Resultado.IdRespuesta > 0)
                    return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });

                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en P_InsUdpUsuarios");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }
        }

        #endregion

        #region Métodos de Eliminación

        /// Eliminar un rol de la grilla de roles asignados a un usuario
        public async Task<IActionResult> P_DelRoles(DtoInsUserRoles obj)
        {
            if (obj.IdUserRol == 0)
                return Json(new { success = false, message = "Debe seleccionar rol, revise" });

            if (string.IsNullOrWhiteSpace(obj.Justificacion))
                return Json(new { success = false, message = "Debe registrar justificación, revise" });

            try
            {
                var Resultado = await _iDbAdministracion.P_DelRoles(obj, Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
                if (Resultado.IdRespuesta > 0)
                {
                    return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });
                }
                else
                {
                    return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en P_DelRoles");
                return Json(new { success = false, data = 0, message = "Error: no fue posible guardar, intente nuevamente." });
            }
        }
        #endregion
    }
}
