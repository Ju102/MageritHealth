using MageritHealth.Models;
using MageritHealth.Models.ViewModels;
using MageritHealth.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MageritHealth.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuariosRepository repo;

        public AccountController(IUsuariosRepository repo)
        {
            this.repo = repo;
        }

        #region Login y Logout
        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Usuario user = await this.repo.LoginUsuarioAsync(model.LoginEmail, model.LoginPassword);

            if (user == null)
            {
                ModelState.AddModelError("", "Las credenciales no son correctas. Inténtalo de nuevo.");
                return View(model);
            }
            else
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name,
                    ClaimTypes.Role);

                Claim claimName = new Claim(ClaimTypes.Name, user.Nombre);
                identity.AddClaim(claimName);

                string apellido = user.Apellido2 != null ? user.Apellido1 + " " + user.Apellido2 : user.Apellido1;

                Claim claimSurname = new Claim("FullSurname", apellido);
                identity.AddClaim(claimSurname);

                Claim claimRole = new Claim(ClaimTypes.Role, user.Rol);
                identity.AddClaim(claimRole);

                Claim claimId = new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString());
                identity.AddClaim(claimId);

                if (user.NumeroAsegurado != null)
                {
                    Claim claimInsuranceNumber = new Claim("InsuranceNumber", user.NumeroAsegurado);
                    identity.AddClaim(claimInsuranceNumber);
                }

                if (user.NumeroColegiado != null)
                {
                    Claim claimLicenseNumber = new Claim("LicenseNumber", user.NumeroColegiado);
                    identity.AddClaim(claimLicenseNumber);
                }

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (user.Rol == "paciente")
                {
                    return RedirectToAction("Dashboard", "Paciente");
                }
                else if (user.Rol == "doctor")
                {
                    return RedirectToAction("Dashboard", "Doctor");
                }
                else if (user.Rol == "admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Ha ocurrido un error inesperado. Vuelve a intentarlo más tarde.");
                    return View(model);
                }
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        #endregion

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
