using MageritHealth.Filters;
using MageritHealth.Helpers;
using MageritHealth.Models;
using MageritHealth.Models.ViewModels;
using MageritHealth.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MageritHealth.Controllers
{
    [AuthorizeUsers(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IUsuariosRepository usersRepository;

        public AdminController(IUsuariosRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        #region Registro
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                string passwordGenerada = ToolsHelper.GenerateRandomPassword();
                string emailGenerado = ToolsHelper.GenerateEmailCorporativo(model.Nombre, model.Apellido1, model.Apellido2);
                Usuario admin = new Usuario
                {
                    Nombre = model.Nombre,
                    Apellido1 = model.Apellido1,
                    Apellido2 = model.Apellido2,
                    Dni = model.Dni,
                    FechaNacimiento = model.FechaNacimiento,
                    Telefono = model.Telefono,
                    Genero = model.Genero.ToLower(),
                    Direccion = model.Direccion,
                    Email = emailGenerado,
                    Rol = "admin",
                    Pass = passwordGenerada,

                };
                try
                {
                    await this.usersRepository.InsertUsuarioAsync(admin, passwordGenerada);
                    TempData["mensaje"] = "Usuario Administrador añadido con éxito.";
                    return RedirectToAction("Dashboard");
                }
                catch (Exception e)
                {
                    TempData["mensaje"] = "Ha ocurrido un error inesperado. Vuelve a intentarlo de nuevo más tarde." + e.Message;
                    return RedirectToAction("Dashboard");
                }

            }
        }

        public async Task<IActionResult> RegisterDoctor()
        {
            List<Especialidad> especialidades = await this.usersRepository.GetListaEspecialidadesAsync();
            ViewData["especialidades"] = especialidades;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(DoctorRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                List<Especialidad> especialidades = await this.usersRepository.GetListaEspecialidadesAsync();
                ViewData["especialidades"] = especialidades;
                return View(model);
            }
            else
            {
                string emailGenerado = ToolsHelper.GenerateEmailCorporativo(model.Nombre, model.Apellido1, model.Apellido2);
                string passwordGenerada = ToolsHelper.GenerateRandomPassword();
                Usuario doctor = new Usuario()
                {
                    Nombre = model.Nombre,
                    Apellido1 = model.Apellido1,
                    Apellido2 = model.Apellido2,
                    Dni = model.Dni,
                    FechaNacimiento = model.FechaNacimiento,
                    Telefono = model.Telefono,
                    Genero = model.Genero.ToLower(),
                    Direccion = model.Direccion,
                    Email = emailGenerado,
                    Rol = "doctor",
                    Pass = passwordGenerada,
                    IdEspecialidad = model.Especialidad,
                    NumeroColegiado = model.NumeroColegiado
                };

                await this.usersRepository.InsertUsuarioAsync(doctor, passwordGenerada);

                return RedirectToAction("Dashboard");
            }
        }

        public IActionResult RegisterPaciente()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPaciente(PacienteRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                string emailGenerado = ToolsHelper.GenerateEmailCorporativo(model.Nombre, model.Apellido1, model.Apellido2);
                string passwordGenerada = ToolsHelper.GenerateRandomPassword();
                Usuario paciente = new Usuario()
                {
                    Nombre = model.Nombre,
                    Apellido1 = model.Apellido1,
                    Apellido2 = model.Apellido2,
                    Dni = model.Dni,
                    FechaNacimiento = model.FechaNacimiento,
                    Telefono = model.Telefono,
                    Genero = model.Genero.ToLower(),
                    Direccion = model.Direccion,
                    Email = emailGenerado,
                    Rol = "paciente",
                    Pass = passwordGenerada,
                    NumeroAsegurado = model.NumeroAsegurado
                };
                await this.usersRepository.InsertUsuarioAsync(paciente, passwordGenerada);
                return RedirectToAction("Dashboard");
            }
        }

        #endregion
    }
}
