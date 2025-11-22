using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Gestion.General;
using Negocio.Gestion.Integrantes;
using Negocio.Interfaz.Admin;
using Negocio.Interfaz.General;
using Negocio.Interfaz.Integrantes;

namespace Web.Areas.Integrantes.Controllers
{


    [Area("Integrantes")]
    [Authorize(Roles = "1,2,3,4,5,6,7,8,11")]
    public class BuscarIntegController : Controller
    {

        private readonly IConfiguration _iConfiguration;
        private readonly IDbAdministracion _iDbAdministracion;
       private readonly IDbBuscarIntegrantes _iDbBuscarIntegrantes;
        private readonly IDbDominios _iDbDominios;



        public BuscarIntegController(IConfiguration iConfiguration, IDbAdministracion dbAdministracion, 
            IDbBuscarIntegrantes dbBuscarIntegrantes,
            IDbDominios iDbDominios)
        {

            _iConfiguration = iConfiguration;
            _iDbAdministracion = dbAdministracion;
            _iDbBuscarIntegrantes = dbBuscarIntegrantes;
            _iDbDominios = iDbDominios;
        }



        public IActionResult BuscarIntegrantes()
        {
            return View();
        }





        [HttpGet]
        public async Task<IActionResult> F_GetIntegrantesPorId(long V_Identificacion)
        {
            var resultado = await _iDbBuscarIntegrantes.F_GetIntegrantesPorId(V_Identificacion);

            if (resultado.IdRespuesta > 0)
            {
                return Json(new { success = true, data = resultado.Data });
            }
            else
            {
                return Json(new { success = false, message = resultado.Mensaje });
            }
        }

    }
}
