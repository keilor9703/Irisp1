using Comun.Areas.Admin;
using Gepad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaz.Admin;
using System.Data;
using System.Diagnostics;

namespace Gepad.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDbAdministracion _DbAdministracion;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> _logger;
        public HomeController(IDbAdministracion iDbAdministracion, IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger)
        {
            _DbAdministracion = iDbAdministracion;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var ImagenesSlider = await _DbAdministracion.F_GetSilerSuperior();
                var SlidersView = new List<DtoSlider>();

                foreach (var item in ImagenesSlider.Data)
                {
                    string ruta = ConsultarRuta(Convert.ToInt32(item.IMAGENES_CONSECUTIVO));
                    string ruta1 = "";

                    if (ruta == null || ruta == "")
                    {
                        ruta1 = F_GetImagenes(Convert.ToInt32(item.IMAGENES_CONSECUTIVO));
                    }
                    else
                    {
                        ruta1 = ruta;
                    }
                    var SliderView = new DtoSlider
                    {
                        CONSECUTIVO = item.CONSECUTIVO,
                        IMAGENES_CONSECUTIVO = item.IMAGENES_CONSECUTIVO,
                        URL = item.URL,
                        FILENAME = item.FILENAME,
                        ORDEN = item.ORDEN,
                        RUTA = ruta1
                    };
                    SlidersView.Add(SliderView);
                }
                return View(SlidersView);

            }
            catch (Exception )
            {
                var SlidersView = new List<DtoSlider>();
                var SliderView = new DtoSlider
                {
                    IMAGENES_CONSECUTIVO = "19957",
                    FILENAME = "ARTE4_polired.jpg",
                    RUTA = "~/img/Carrusel/19957.jpg"
                };
                SlidersView.Add(SliderView);
                return View(SlidersView);
            }
            //Immplementar con microservicio
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string ConsultarRuta(int Consecutivo)
        {
            // verificar si la imagen existe en una carpeta
            string[] formatos = new[] { ".tiff", ".ief", ".gif", ".jpg", ".png" };
            string ruta = "";
            string extensionArchivo = "";
            bool existe = false;
            string Resultado = "";
            foreach (string formato in formatos)
            {

                string webRootPath = _webHostEnvironment.WebRootPath;
                string contentRootPath = _webHostEnvironment.ContentRootPath;

                ruta = Path.Combine(webRootPath, "img/Carrusel/") + Consecutivo.ToString() + formato;
                //or path = Path.Combine(contentRootPath , "wwwroot" ,"CSS" );

                if (System.IO.File.Exists(ruta))
                {
                    existe = true;
                    extensionArchivo = formato;
                    break;
                }
            }
            // si existe devolverla
            if (existe)
            {
                return "~/img/Carrusel/" + Consecutivo.ToString() + extensionArchivo;
            }
            else
            {
                return Resultado;
            }
        }
        public string F_GetImagenes(int Consecutivo)
        {
            bool existe = false;
            string ruta = "";
            string extensionArchivo = "";
            string Resultado = "";

            DataTable dsImagen = _DbAdministracion.F_GetImagenes(Consecutivo);


            if (dsImagen != null)
            {
                // validar que haya un resultado
                if (dsImagen.Rows.Count == 1)
                {
                    DataRow fila = dsImagen.Rows[0];
                    string ContentType = fila[1].ToString();
                    string FileName = fila[2].ToString();
                    byte[] Foto = (byte[])fila[3];
                    // obtener la extension
                    if (ContentType.Equals("image/tiff"))
                        extensionArchivo = ".tiff";
                    else if (ContentType.Equals("image/ief"))
                        extensionArchivo = ".ief";
                    else if (ContentType.Equals("image/gif"))
                        extensionArchivo = ".gif";
                    else if (ContentType.Equals("image/jpg") | ContentType.Equals("image/jpeg"))
                        extensionArchivo = ".jpg";
                    else if (ContentType.Equals("image/png"))
                        extensionArchivo = ".png";

                    // guardar la imagen en la carpeta
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string contentRootPath = _webHostEnvironment.ContentRootPath;

                    ruta = Path.Combine(webRootPath, "img/Carrusel/") + Consecutivo.ToString() + extensionArchivo;
                    System.IO.File.WriteAllBytes(ruta, Foto);
                    existe = true;
                }
                else
                {
                    existe = false;
                }

            }
            // si existe devolverla
            if (existe)
            {
                return "~/img/Carrusel/" + Consecutivo.ToString() + extensionArchivo;
            }
            else
            {
                return Resultado;
            }
        }

    }
}
