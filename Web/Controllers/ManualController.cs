using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class ManualController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ManualController> _logger;

        public ManualController(IConfiguration configuration, ILogger<ManualController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Abre el Manual de Usuario (PDF) ubicado en el servidor de archivos
        // (RutasArchivosIris:RutaManuales). Toma el PDF más reciente de la carpeta, de modo que el
        // enlace del menú sigue funcionando aunque cambie el nombre/revisión del archivo; se
        // devuelve inline para que el navegador lo muestre en una pestaña nueva.
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var ruta = _configuration["RutasArchivosIris:RutaManuales"];
                if (string.IsNullOrWhiteSpace(ruta) || !Directory.Exists(ruta))
                {
                    _logger.LogWarning("Manual: carpeta no disponible ({Ruta})", ruta);
                    return NotFound("El manual de usuario no está disponible en este momento.");
                }

                var pdf = new DirectoryInfo(ruta)
                    .GetFiles("*.pdf")
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (pdf == null)
                {
                    _logger.LogWarning("Manual: no se encontró ningún PDF en {Ruta}", ruta);
                    return NotFound("No se encontró el manual de usuario.");
                }

                var stream = new FileStream(pdf.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                // inline => el navegador lo muestra en lugar de descargarlo.
                Response.Headers["Content-Disposition"] = "inline; filename=\"manual-usuario.pdf\"";
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al abrir el manual de usuario");
                return NotFound("No fue posible abrir el manual de usuario.");
            }
        }
    }
}
