using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2")]
    public class MapaController : Controller
    {
        private object _iDbMapaIris;

        [HttpPost]
        public async Task<IActionResult> ConsultarAnioSeguimiento(string _anioMapa)
        {
            if (string.IsNullOrWhiteSpace(_anioMapa))
            {
                return BadRequest(new { msg = "el año es requerido.", ok = false });
            }

            try
            {
                var resultados = await _iDbMapaIris.ConsultarMapaIris(_anioMapa);

                if (resultados == null || !resultados.Any())
                {
                    return NotFound(new { msg = "No se encontraron iris para el año seleccionado.", ok = false });
                }

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Error interno del servidor", ok = false, error = ex.Message });
            //}
        }
    }
}
