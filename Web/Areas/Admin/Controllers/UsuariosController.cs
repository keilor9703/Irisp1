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
        #endregion
        #region Constructor
        public UsuariosController(IDbAdministracion iDbAdministracion)
        {
            _iDbAdministracion = iDbAdministracion;
        }
        #endregion

        public async Task<IActionResult> VwUsuarios()
        {
            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Ingreso Módulo", "Ingreso módulo Administracion/Usuarios", Convert.ToInt64(User.FindFirstValue("Identificacion")), HttpContext.Session.GetString("IpMaquina"));
            ViewBag.ddlRol = new SelectList((await _iDbAdministracion.F_GetRoles()).Data.ToList().OrderBy(x => x.DESCRIPCION).ToList(), "IDROL", "DESCRIPCION");
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
            }
        }
        public async Task<IActionResult> P_InsUdpUsuarios(DtoUsuario obj)
        {
            if (obj.Identificacion == 0)
                return Json(new { success = false, message = "Debe registrar Identificación, revise" });

            try
            {
                //bool esSuperUsuario = User.IsInRole("1");

                // Si no es SuperUsuario, no se cambia MFA (ignorar lo que venga)
                //int? estado2FaPermitido = esSuperUsuario ? (int?)obj.Estado2Fa : null;

                var Resultado = await _iDbAdministracion.P_InsUdpUsuarios(
                    obj.Identificacion,
                    obj.Bloqueado,
                    obj.Estado2Fa,              // <-- nullable
                    obj.Usuario,
                    Convert.ToInt64(User.FindFirstValue("Identificacion")),
                    HttpContext.Session.GetString("IpMaquina")
                );

                if (Resultado.IdRespuesta > 0)
                    return Json(new { success = true, data = Resultado.Data, message = Resultado.Mensaje });

                return Json(new { success = false, data = Resultado.Data, message = Resultado.Mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
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
                return Json(new { success = false, data = 0, message = "Error: no es posible guardar, revise " + ex });
            }
        }
        #endregion
    }
}
