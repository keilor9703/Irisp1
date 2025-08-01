using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Irisp1.Controllers
{
    public class SeguimientoController : Controller
    {
        [Area("Irisp1")]
        [Authorize(Roles = "1,2")]
        public IActionResult Seguimiento()
        {
            return View();
        }
    }
}
