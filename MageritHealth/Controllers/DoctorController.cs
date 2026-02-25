using Microsoft.AspNetCore.Mvc;

namespace MageritHealth.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Consulta()
        {
            return View();
        }
    }
}
