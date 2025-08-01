using Comun.Areas.Clientes;
using Comun.Areas.Mod_Uno;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Clientes;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Modulo1;
using System.Security.Claims;

namespace Web.Areas.Mod_Uno.Controllers
{
    [Area("Mod_Uno")]
    [Authorize(Roles = "1,2,3")]

    public class RegistrosController : Controller
    {
        #region Propiedades
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbDominios _IDbDominios;
        private readonly IDbClientes _IDbClientes;
        private readonly IDbRegistro _IDbRegistro;
        #endregion

        #region Constructor

        public RegistrosController(IDbClientes iDbClientes, IDbAdministracion iDbAdministracion, IDbDominios iDbDominios, IDbRegistro iDbRegistro)
        {
            _iDbAdministracion = iDbAdministracion;
            _IDbClientes = iDbClientes;
            _IDbDominios = iDbDominios;
            _IDbRegistro = iDbRegistro;
        }

        #endregion
        public async Task<IActionResult> RegistrosIris()

        {
            ViewBag.IdUnidad = new SelectList((await _IDbDominios.F_GetDepartamentos(10)).Data.ToList().OrderBy(x => x.Descripcion).ToList(), "IdDominio", "Descripcion");

            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Cliente", "Ingreso Módulo Iris ", "0", HttpContext.Session.GetString("IpMaquina"));
            var Funcionario = User.FindFirstValue("Funcionario");
            var IpMaquina = HttpContext.Session.GetString("IpMaquina");

            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            return View();
        }
        //public async Task<IActionResult> F_GetDatos(long V_Identificacion)
        //{
        //    var retorno = await _iDbAdministracion.F_GetSilerSuperior();

        //    if (retorno != null)
        //    {
        //        return Json(new { success = true, data = retorno.Data });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, data = retorno.Data });
        //    }
        //}


        #region Métodos de Inserción y Actualización
        public async Task<IActionResult> P_InsUdpCriminalidadIris(CriminalidadDTO Obj)
        {
            if (Obj.IdentificacionInforma == 0)
                return Json(new { success = false, message = "Debe registrar Identificación, revise" });

            if (Obj.IdUnidad == 0)
                return Json(new { success = false, message = "Debe registrar Unidad, revise" });

            if (Obj.IdZona == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Celular))
                return Json(new { success = false, message = "Debe regsitrar numero de celular, revise" });

            if (Obj.IdTipoServicio == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            if (Obj.IdCuadrante == 0)
                return Json(new { success = false, message = "Debe seleccionar Cuadrante, revise" });

            if (Obj.IdClase == 0)
                return Json(new { success = false, message = "Debe seleccionar Clase, revise" });

            if (string.IsNullOrWhiteSpace(Obj.NombreClase))
                return Json(new { success = false, message = "Debe registar los Apellidos, revise" });

            if (string.IsNullOrWhiteSpace(Obj.FechaInicioExistencia))
                return Json(new { success = false, message = "Debe registar fecha de Inicio de existencia, revise" });

            if (Obj.CantidadIntegrante == 0)
                return Json(new { success = false, message = "Debe Seleccionar cantidad de integrantes, revise" });

            if (string.IsNullOrWhiteSpace(Obj.CaracteristicasGenerales))
                return Json(new { success = false, message = "Debe registrar las caracteristicas, revise" });

            if (Obj.Vigente == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Codigo))
                return Json(new { success = false, message = "Debe seleccionar unidad policial, oiga revise" });

            if (Obj.IdEstado == 0)
                return Json(new { success = false, message = "Debe registrar Estado, revise" });

            if (Obj.IdFuente == 0)
                return Json(new { success = false, message = "Debe registrar fuente, revise" });

            if (Obj.IdEstadoExistencia == 0)
                return Json(new { success = false, message = "Debe registrar Estado de existencia, revise" });

            if (string.IsNullOrWhiteSpace(Obj.DescripcionTramite))
                return Json(new { success = false, message = "Debe seleccionar descripcion del tramite, revise" });

            if (Obj.EntornoAfectado == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            if (Obj.IdTiempoDelito == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            if (Obj.Clasificacion == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            if (Obj.ModalidadExpendio == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Origen))
                return Json(new { success = false, message = "Debe seleccionar descripcion del tramite, revise" });

            if (string.IsNullOrWhiteSpace(Obj.NombreEntornoAfectado))
                return Json(new { success = false, message = "Debe seleccionar descripcion del tramite, revise" });

            if (Obj.EspecialidadAportaInfo == 0)
                return Json(new { success = false, message = "Debe registrar Zona, revise" });

            try
            {
                var V_Usuario = Convert.ToInt32(User.FindFirstValue("Identificacion"));
                var V_Maquina = HttpContext.Session.GetString("IpMaquina");
                var result = await _IDbRegistro.P_InsUdpCriminalidadIris(Obj, V_Usuario, V_Maquina);
                if (result.IdRespuesta > 0)
                {
                    return Json(new
                    {
                        success = true,
                        data = result.Data,
                        message = result.Mensaje
                    });
                }
                else
                {
                    return Json(new { success = false, data = result.Data, message = result.Mensaje });
                }

            }

            catch (Exception ex)
            {
                return Json(new { success = false, data = 0, message = "Error:  No es posible guardar, revise " + ex });
            }
        }

        #endregion

    }
}

