using Comun.Areas.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Gestion.Admin;
using Negocio.Interfaz.Admin;
using System.Data;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDbAdministracion _DbAdministracion;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDbConsultasPIP _iDbConsultasPIP;
        private readonly ILogger<HomeController> _logger;
        public HomeController(IDbAdministracion iDbAdministracion, IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger, IDbConsultasPIP iDbConsultasPIP)
        {
            _DbAdministracion = iDbAdministracion;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _iDbConsultasPIP = iDbConsultasPIP;
            
        }
   
        public async Task<IActionResult> Index()
        {
            try
            {
                var carruselResp = await _iDbConsultasPIP.ObtenerCarruselAsync();

                if (!carruselResp.Estado || carruselResp.Respuesta == null || carruselResp.Respuesta.Count == 0)
                    throw new Exception("Carrusel vacío o no disponible");

                var slidersView = carruselResp.Respuesta.Select( x => new DtoSlider
                {
                    IMAGENES_CONSECUTIVO = x.consecutivo,
                    FILENAME = x.fileName,
                    RUTA = $"data:{(string.IsNullOrWhiteSpace(x.contentType) ? "image/jpeg" : x.contentType)};base64,{x.foto}"
                }).ToList();

                return View(slidersView);
            }
            catch
            {
            
                var slidersView = new List<DtoSlider>
        {
            new DtoSlider
            {
                IMAGENES_CONSECUTIVO = 19957,
                FILENAME = "ARTE4_polired.jpg",
                RUTA = "~/img/Carrusel/19957.jpg"
            }
        };
                return View(slidersView);
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
       

    }
}
