using MageritHealth.Models;
using MageritHealth.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MageritHealth.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsersRepository repo;

        public AccountController(UsersRepository repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId.HasValue)
            {
                return RedirectToAction("Index", "Patient");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await this.repo.GetUserByEmailAsync(model.LoginEmail, model.LoginPassword);

            if (user == null)
            {
                ModelState.AddModelError("", "Usuario o password incorrectos.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", user.UserRole);

            if (user.UserRole == "Patient")
            {
                return RedirectToAction("Index", "Patient");
            }
            else if (user.UserRole == "Doctor")
            {
                return RedirectToAction("Index", "Doctor");
            }
            else if (user.UserRole == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            } else
            {
                ModelState.AddModelError("", "Ha habido un problema al intentar iniciar sesión. Inténtelo de nuevo más tarde.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
