using MageritHealth.Filters;
using MageritHealth.Models;
using MageritHealth.Models.ViewModels;
using MageritHealth.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MageritHealth.Controllers
{
    [AuthorizeUsers(Policy = "DoctorOnly")]
    public class DoctorController : Controller
    {
        private readonly IUsuariosRepository usuariosRepository;

        public DoctorController(IUsuariosRepository usuariosRepository)
        {
            this.usuariosRepository = usuariosRepository;
        }


        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Consulta()
        {
            return View();
        }

        public IActionResult Agenda()
        {
            return View();
        }

        public async Task<IActionResult> Perfil()
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario user = await this.usuariosRepository.GetUsuarioByIdAsync(idUsuario);
            return View(user);
        }

        public async Task<IActionResult> EditarPerfil()
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario usuario = await this.usuariosRepository.GetUsuarioByIdAsync(idUsuario);
            EditPacienteViewModel viewModel = new EditPacienteViewModel()
            {
                Telefono = usuario.Telefono,
                Email = usuario.Email
            };
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(EditPacienteViewModel viewModel)
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid)
            {
                Usuario usuario = await this.usuariosRepository.GetUsuarioByIdAsync(idUsuario);
                EditPacienteViewModel model = new EditPacienteViewModel()
                {
                    Telefono = usuario.Telefono,
                    Email = usuario.Email
                };
                return View(model);
            }

            if (viewModel.Password != null)
            {
                await this.usuariosRepository.UpdateDatosUsuarioAsync(idUsuario, viewModel.Telefono, viewModel.Email, viewModel.Password);
                return RedirectToAction("Perfil", "Doctor");
            }
            else
            {
                await this.usuariosRepository.UpdateDatosUsuarioAsync(idUsuario, viewModel.Telefono, viewModel.Email, null);
                return RedirectToAction("Perfil", "Doctor");
            }
        }
    }
}
