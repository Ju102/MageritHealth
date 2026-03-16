using MageritHealth.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MageritHealth.Controllers
{
    [AuthorizeUsers(Policy = "DoctorOnly")]
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
