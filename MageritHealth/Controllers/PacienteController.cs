using MageritHealth.Filters;
using MageritHealth.Models;
using MageritHealth.Models.ViewModels;
using MageritHealth.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MageritHealth.Controllers
{
    [AuthorizeUsers(Policy = "PacienteOnly")]
    public class PacienteController : Controller
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly ICitasRepository citasRepository;
        private readonly IInfoClinicaRepository infoClinicaRepository;
        private readonly IPrescripcionesRepository prescripcionesRepository;
        private readonly IAnaliticasRepository analiticasRepository;

        public PacienteController(IUsuariosRepository usuariosRepository,
            ICitasRepository citasRepository, IInfoClinicaRepository infoClinicaRepository,
            IPrescripcionesRepository prescripcionesRepository, IAnaliticasRepository analiticasRepository)
        {
            this.usuariosRepository = usuariosRepository;
            this.citasRepository = citasRepository;
            this.infoClinicaRepository = infoClinicaRepository;
            this.prescripcionesRepository = prescripcionesRepository;
            this.analiticasRepository = analiticasRepository;
        }

        public async Task<IActionResult> Dashboard()
        {
            int idUsuarioLogueado = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            string numeroAsegurado = HttpContext.User.FindFirstValue("InsuranceNumber");
            List<Cita> historialCitas = await this.citasRepository.GetHistorialCitasPacienteAsync(idUsuarioLogueado, 3);
            Cita proximaCita = await this.citasRepository.GetProximaCitaAsync(idUsuarioLogueado);
            List<Prescripcion> medicacionActiva = await this.prescripcionesRepository.GetListaPrescripcionesByIdPacienteAsync(idUsuarioLogueado);
            InfoClinicaPaciente info = await this.infoClinicaRepository.GetInfoClinicaPacienteByIdPacienteAsync(idUsuarioLogueado);
            List<AntecedenteMedico> antecedentes = await this.infoClinicaRepository.GetListaAntecedentesMedicosByIdPacienteAsync(idUsuarioLogueado);

            PerfilVital perfil = new PerfilVital()
            {
                NombreContacto = info.ContactoEmergenciaNombre,
                TelefonoContacto = info.ContactoEmergenciaTelefono,
                Peso = info.PesoActual,
                TipoSangre = info.GrupoSanguineo,
                Altura = 100
            };

            PacienteDashboardViewModel viewmodel = new PacienteDashboardViewModel()
            {
                Nombre = HttpContext.User.Identity.Name,
                NumeroAsegurado = numeroAsegurado,
                ProximaCita = proximaCita,
                UltimasTresCitas = historialCitas,
                MedicacionActiva = medicacionActiva,
                Perfil = perfil
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> SolicitarCita()
        {
            // Necesitamos pasarle a la vista la lista de especialidades para el primer 
            // Esto asume que tienes un método GetEspecialidadesAsync() en tu repositorio
            var especialidades = await this.usuariosRepository.GetListaEspecialidadesAsync();

            // SelectList es perfecto para los <select> de HTML
            ViewBag.Especialidades = new SelectList(especialidades, "IdEspecialidad", "NombreEspecialidad");

            return View();
        }

        public async Task<JsonResult> GetDoctoresPorEspecialidad(int idEspecialidad)
        {
            // Buscamos solo a los médicos de esa especialidad concreta
            var doctores = await this.usuariosRepository.GetListaDoctoresByEspecialidadAsync(idEspecialidad);

            // Transformamos la lista en el formato que espera el JavaScript (value, text)
            var doctoresFiltrados = doctores.Select(d => new
            {
                value = d.IdUsuario,
                text = $"Dr/a. {d.Nombre} {d.Apellido1}"
            });

            return Json(doctoresFiltrados);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SolicitarCita(SolicitarCitaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Si falla, hay que recargar el desplegable de especialidades antes de devolver la vista
                var especialidades = await this.usuariosRepository.GetListaEspecialidadesAsync();
                ViewBag.Especialidades = new SelectList(especialidades, "IdEspecialidad", "NombreEspecialidad");
                return View(model);
            }

            // A. Sacamos el ID del paciente directamente de su sesión (¡Seguridad máxima!)
            string idPacienteString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int idPaciente = int.Parse(idPacienteString);

            // B. Magia con las fechas: Unimos la Fecha (ej. 20/06/2024) y la Hora (ej. 09:30)
            TimeSpan horaCita = TimeSpan.Parse(model.Hora);
            DateTime fechaHoraFinal = model.Fecha.Date.Add(horaCita);

            // C. Preparamos el objeto Cita para la base de datos
            Cita nuevaCita = new Cita
            {
                IdPaciente = idPaciente,
                IdDoctor = model.IdDoctor,
                FechaHora = fechaHoraFinal,
                Motivo = model.Motivo,
                Estado = "programada",
                Activa = true,
                FechaCreacion = DateTime.Now
            };

            // D. Guardamos usando el repositorio
            await this.citasRepository.InsertCitaAsync(idPaciente, model.IdDoctor, model.Motivo, fechaHoraFinal);

            // E. Mensaje de éxito para el Dashboard
            TempData["MensajeExito"] = "Tu cita ha sido solicitada correctamente.";

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Perfil()
        {
            Usuario usuario = await this.usuariosRepository.GetUsuarioByIdAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            return View(usuario);
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
                return RedirectToAction("Perfil", "Paciente");
            }
            else
            {
                await this.usuariosRepository.UpdateDatosUsuarioAsync(idUsuario, viewModel.Telefono, viewModel.Email, null);
                return RedirectToAction("Perfil", "Paciente");
            }
        }

        public async Task<IActionResult> Citas()
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<Cita> citas = await this.citasRepository.GetAllCitasByIdPaciente(idUsuario);
            return View(citas);
        }
    }
}
