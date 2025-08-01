using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Irisp1.Controllers
{
    [Area("Irisp1")]
    [Authorize(Roles = "1,2")]
    public class ConsultaController : Controller
    {
        public IActionResult Consulta()
        {
            return View();
        }
    }
}
