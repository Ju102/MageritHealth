using Microsoft.AspNetCore.Mvc;

namespace MageritHealth.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
