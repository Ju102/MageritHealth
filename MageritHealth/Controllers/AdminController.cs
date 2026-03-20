using MageritHealth.Filters;
using MageritHealth.Helpers;
using MageritHealth.Models;
using MageritHealth.Models.ViewModels;
using MageritHealth.Repositories.Interfaces;
using MageritHealth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MageritHealth.Controllers
{
    [AuthorizeUsers(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IUsuariosRepository usersRepository;
        private readonly ICitasRepository citasRepository;
        private readonly IPrescripcionesRepository prescripcionesRepository;
        private readonly IAnaliticasRepository analiticasRepository;
        private readonly IInfoClinicaRepository infoClinicaRepository;
        private readonly IEmailingService emailingService;

        public AdminController(IUsuariosRepository usersRepository, ICitasRepository citasRepository,
            IPrescripcionesRepository prescripcionesRepository, IAnaliticasRepository analiticasRepository, IInfoClinicaRepository infoClinicaRepository, IEmailingService emailingService)
        {
            this.usersRepository = usersRepository;
            this.citasRepository = citasRepository;
            this.prescripcionesRepository = prescripcionesRepository;
            this.analiticasRepository = analiticasRepository;
            this.infoClinicaRepository = infoClinicaRepository;
            this.emailingService = emailingService;
        }

        public async Task<IActionResult> Dashboard()
        {
            int recuentoPacientesActivos = await this.usersRepository.GetRecuentoUsuariosByRolAsync("paciente");
            int recuentoDoctoresActivos = await this.usersRepository.GetRecuentoUsuariosByRolAsync("doctor");
            int citasProximas = await this.citasRepository.GetRecuentoCitasActivasAsync();
            int analiticasProximas = await this.analiticasRepository.GetRecuentoProximasAnaliticasAsync();
            List<Cita> proximasCitas = await this.citasRepository.GetProximasCitasAsync();

            AdminDashboardViewModel viewModel = new AdminDashboardViewModel()
            {
                TotalPacientes = recuentoPacientesActivos,
                TotalDoctoresActivos = recuentoDoctoresActivos,
                CitasProgramadas = citasProximas,
                AnaliticasPendientes = analiticasProximas,
                ProximasCitas = proximasCitas
            };
            return View(viewModel);
        }

        public async Task<IActionResult> MiPerfil()
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario user = await this.usersRepository.GetUsuarioByIdAsync(idUsuario);
            return View(user);
        }

        public async Task<IActionResult> EditarPerfil()
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            Usuario usuario = await this.usersRepository.GetUsuarioByIdAsync(idUsuario);
            EditUsuarioViewModel viewModel = new EditUsuarioViewModel()
            {
                Telefono = usuario.Telefono,
                Email = usuario.Email
            };
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(EditUsuarioViewModel viewModel)
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid)
            {
                Usuario usuario = await this.usersRepository.GetUsuarioByIdAsync(idUsuario);
                EditUsuarioViewModel model = new EditUsuarioViewModel()
                {
                    Telefono = usuario.Telefono,
                    Email = usuario.Email
                };
                return View(model);
            }

            if (viewModel.Password != null)
            {
                await this.usersRepository.UpdateDatosUsuarioAsync(idUsuario, viewModel.Telefono, viewModel.Email, viewModel.Password);
                return RedirectToAction("MiPerfil", "Admin");
            }
            else
            {
                await this.usersRepository.UpdateDatosUsuarioAsync(idUsuario, viewModel.Telefono, viewModel.Email, null);
                return RedirectToAction("MiPerfil", "Admin");
            }
        }

        public IActionResult Usuarios()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Usuarios(string dni, string rol)
        {
            List<Usuario> usuarios = await this.usersRepository.GetUsuariosByDniYRolAsync(dni, rol);
            return View(usuarios);
        }

        public async Task<IActionResult> DetallesUsuario(int id)
        {
            ViewData["especialidades"] = await this.usersRepository.GetListaEspecialidadesAsync();
            Usuario usuario = await this.usersRepository.GetUsuarioByIdAsync(id);
            return View(usuario);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditarUsuario(Usuario us)
        {
            await this.usersRepository.UpdateUsuarioAsync(us);
            return RedirectToAction("Usuarios");
        }

        public async Task<IActionResult> DesactivarUsuario(int id)
        {
            await this.usersRepository.DisableUsuarioAsync(id);
            return RedirectToAction("Usuarios");
        }

        public async Task<IActionResult> ActivarUsuario(int id)
        {
            await this.usersRepository.EnableUsuarioAsync(id);
            return RedirectToAction("Usuarios");
        }

        public async Task<IActionResult> Especialidades()
        {
            List<Especialidad> especialidades = await this.usersRepository.GetListaEspecialidadesAsync();
            return View(especialidades);
        }

        public async Task<IActionResult> EliminarEspecialidad(int idespecialidad)
        {
            await this.usersRepository.DeleteEspecialidadAsync(idespecialidad);
            return RedirectToAction("Especialidades");
        }

        public async Task<IActionResult> CrearEspecialidad(string especialidad)
        {
            await this.usersRepository.InsertEspecialidadAsync(especialidad);
            return RedirectToAction("Especialidades");
        }

        public async Task<IActionResult> Medicamentos()
        {
            List<Medicamento> medicamentos = await this.prescripcionesRepository.GetListaMedicamentosAsync();
            return View(medicamentos);
        }

        public async Task<IActionResult> CrearMedicamento(string nombrecomercial, string principioactivo, string concentracion, string formato, string fabricante)
        {
            Medicamento medicamento = new Medicamento()
            {
                NombreComercial = nombrecomercial,
                PrincipioActivo = principioactivo,
                Concentracion = concentracion,
                Formato = formato,
                Fabricante = fabricante
            };
            await this.prescripcionesRepository.InsertMedicamentoAsync(medicamento);
            return RedirectToAction("Medicamentos");
        }

        public async Task<IActionResult> EliminarMedicamento(int idmedicamento)
        {
            await this.prescripcionesRepository.DeleteLogicoMedicamentoAsync(idmedicamento);
            return RedirectToAction("Medicamentos");
        }

        public async Task<IActionResult> Mediciones()
        {
            List<TipoMedicion> tipos = await this.analiticasRepository.GetListaTiposMedicionAsync();
            return View(tipos);
        }

        public async Task<IActionResult> CrearTipoMedicion(string nombre, string unidad, decimal minimo, decimal maximo)
        {
            TipoMedicion tipo = new TipoMedicion()
            {
                NombreMedicion = nombre,
                UnidadMedicion = unidad,
                ValorMinimo = minimo,
                ValorMaximo = maximo,
                Activo = true,
            };

            await this.analiticasRepository.InsertTipoMedicionAsync(tipo);
            return RedirectToAction("Mediciones");
        }

        public async Task<IActionResult> EliminarTipoMedicion(int idtipomedicion)
        {
            await this.analiticasRepository.DeleteLogicoTipoMedicionAsync(idtipomedicion);
            return RedirectToAction("Mediciones");
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
                await this.usersRepository.InsertUsuarioAsync(admin, passwordGenerada);
                await this.emailingService.SendEmailBienvenidaAsync(emailGenerado, passwordGenerada, model.Nombre, "admin");
                return RedirectToAction("Dashboard");
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
                await this.emailingService.SendEmailBienvenidaAsync(emailGenerado, passwordGenerada, model.Nombre, "doctor");
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
                Usuario usuario = await this.usersRepository.GetUsuarioByEmailAsync(emailGenerado);
                await this.infoClinicaRepository.InsertInfoClinicaPacienteAsync(usuario.IdUsuario, model.GrupoSanguineo,
                        model.PesoActual, model.ContactoEmergenciaNombre, model.ContactoEmergenciaTelefono);

                await this.emailingService.SendEmailBienvenidaAsync(emailGenerado, passwordGenerada, model.Nombre, "paciente");
                return RedirectToAction("Dashboard");
            }
        }

        #endregion
    }
}
