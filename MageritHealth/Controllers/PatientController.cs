using Microsoft.AspNetCore.Mvc;

namespace MageritHealth.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
