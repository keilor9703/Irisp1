using Comun.Areas.Clientes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion.General;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.Clientes;
using Negocio.Interfaz.General;
using System.Security.Claims;

namespace Web.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    [Authorize(Roles = "1,2,3")]

    public class ClienteController : Controller
    {
        #region Propiedades
        private readonly IDbClientes _IDbClientes;
        private readonly IDbAdministracion _iDbAdministracion;
        private readonly IDbDominios _IDbDominios;
        #endregion

        #region Constructor
        public ClienteController(IDbClientes iDbClientes, IDbAdministracion iDbAdministracion, IDbDominios iDbDominios)
        {
            _IDbClientes = iDbClientes;
            _iDbAdministracion = iDbAdministracion;
            _IDbDominios = iDbDominios;
        }
        #endregion

        public async Task<IActionResult> Cliente()
        {
            var Auditoria = await _iDbAdministracion.P_InsAuditoria(Convert.ToInt64(User.FindFirstValue("Identificacion")), "Cliente", "Ingreso Módulo", "0", HttpContext.Session.GetString("IpMaquina"));
            //ViewBag.IdDto = new SelectList((await _IDbDominios.F_GetDepartamentos()).Data.ToList().OrderBy(x => x.DESCRIPCION).ToList(), "Codigo", "DESCRIPCION");
            ViewBag.IdDto = new SelectList((await _IDbDominios.F_GetDepartamentos(10)).Data.ToList().OrderBy(x => x.Descripcion).ToList(), "IdDominio", "Descripcion");
            ViewBag.IdGenero = new SelectList((await _IDbDominios.F_GetDominios(2)).Data.ToList().OrderBy(x => x.Descripcion).ToList(), "IdDominio", "Descripcion");
            ViewBag.ddlUnidad = new SelectList((await _IDbDominios.F_GetUnidadesPoliciales(1)).Data.ToList().OrderBy(x => x.Descripcion).ToList(), "Descripcion2", "Descripcion");
            ViewBag.ddlDependencia = new SelectList((await _IDbDominios.F_GetDependencias("")).ToString(), "IdDominio", "Descripcion");

            return View();
        }

        #region Métodos de Consulta        
        public async Task<IActionResult> F_GetKardex(Int64 V_Identificacion)
        {
            var Resultado = await _IDbClientes.F_GetKardex(V_Identificacion);
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
        public async Task<IActionResult> P_InsUdpKardex(DtoKardex Obj)
        {
            if (Obj.Identificacion == 0)
                return Json(new { success = false, message = "Debe registrar Identificación, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Apellidos))
                return Json(new { success = false, message = "Debe registar los Apellidos, revise" });

            if (string.IsNullOrWhiteSpace(Obj.FechaNace))
                return Json(new { success = false, message = "Debe registar fecha de nacimiento, revise" });

            if (Obj.IdDto == 0)
                return Json(new { success = false, message = "Debe seleccionar Departamento, revise" });

            if (Obj.IdLugar == 0)
                return Json(new { success = false, message = "Debe seleccionar Municipio, revise" });

            if (Obj.IdGenero == 0)
                return Json(new { success = false, message = "Debe Seleccionar genero, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Direccion))
                return Json(new { success = false, message = "Debe registrar la direccion, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Unidad))
                return Json(new { success = false, message = "Debe seleccionar unidad policial, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Dependencia))
                return Json(new { success = false, message = "Debe seleccionar dependencia, revise" });

            if (string.IsNullOrWhiteSpace(Obj.Observaciones))
                return Json(new { success = false, message = "Debe registrar las observaciones, revise" });

            try
            {
                var V_Usuario = Convert.ToInt32(User.FindFirstValue("Identificacion"));
                var V_Maquina = HttpContext.Session.GetString("IpMaquina");
                var result = await _IDbClientes.P_InsUdpKardex(Obj, V_Usuario, V_Maquina);
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





        public async Task<JsonResult> F_GetMunicipios(int V_Id)
        {
            try
            {
                if (V_Id <= 0)
                {
                    return Json(new { success = false, message = "El ID del lugar geográfico es inválido." });
                }

                // Llamada a la capa de negocio para obtener los municipios
                var municipios = await _IDbDominios.F_GetMunicipios(V_Id);

                if (municipios != null && municipios.IdRespuesta == 1)
                {
                    // Ajuste aquí: usar IdDominio en lugar de BarriosId
                    var municipiosSelectList = municipios.Data.Select(m => new SelectListItem
                    {
                        Value = m.IdDominio.ToString(),
                        Text = m.Descripcion
                    }).ToList();

                    return Json(new { success = true, datos = municipiosSelectList });
                }
                else
                {
                    return Json(new { success = false, message = "No se encontraron municipios para el lugar especificado." });
                }
            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, message = $"Error al obtener los municipios: {ex.Message}" });
            }
        }

        public async Task<JsonResult> F_GetDependencias(string V_SiglaPapa)
        {
            try
            {                

                if (string.IsNullOrWhiteSpace(V_SiglaPapa))
                    return Json(new { success = false, message = "La dependencia es inválida" });

                // Llamada a la capa de negocio para obtener los municipios
                var municipios = await _IDbDominios.F_GetDependencias(V_SiglaPapa);

                if (municipios != null && municipios.IdRespuesta == 1)
                {
                    // Ajuste aquí: usar IdDominio en lugar de BarriosId
                    var dependenciasSelectList = municipios.Data.Select(m => new SelectListItem
                    {
                        Value = m.IdDominio.ToString(),
                        Text = m.Descripcion
                    }).ToList();

                    return Json(new { success = true, datos = dependenciasSelectList });
                }
                else
                {
                    return Json(new { success = false, message = "No se encontraron dependencias para el lugar especificado." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = $"Error al obtener los municipios: {ex.Message}" });
            }
        }
        //{
        //    var result = await _IDbDominios.F_GetMunicipios(V_Id);
        //    if (result.IdRespuesta > 0)
        //    {
        //        return Json(new { success = true, data = result.Data, message = result.Mensaje });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, data = result.Data, message = result.Mensaje });
        //    }
        //}


        #endregion
    }

}






















