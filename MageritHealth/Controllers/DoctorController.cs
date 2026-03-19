using MageritHealth.Filters;
using MageritHealth.Models;
using MageritHealth.Models.ViewModels;
using MageritHealth.Repositories.Interfaces;
using MageritHealth.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MageritHealth.Controllers
{
    [AuthorizeUsers(Policy = "DoctorOnly")]
    public class DoctorController : Controller
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly ICitasRepository citasRepository;
        private readonly IPrescripcionesRepository prescripcionesRepository;
        private readonly IAnaliticasRepository analiticasRepository;
        private readonly IInfoClinicaRepository infoClinicaRepository;
        private readonly IExportService exportService;


        public DoctorController(IUsuariosRepository usuariosRepository, ICitasRepository citasRepository,
            IPrescripcionesRepository prescripcionesRepository, IAnaliticasRepository analiticasRepository,
            IInfoClinicaRepository infoClinicaRepository, IExportService exportService)
        {
            this.usuariosRepository = usuariosRepository;
            this.citasRepository = citasRepository;
            this.prescripcionesRepository = prescripcionesRepository;
            this.analiticasRepository = analiticasRepository;
            this.exportService = exportService;
            this.infoClinicaRepository = infoClinicaRepository;
        }


        public async Task<IActionResult> Dashboard()
        {
            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<Cita> citasHoy = await this.citasRepository.GetCitasHoyByIdDoctorAsync(idUsuario);
            List<Analitica> analiticasPendientes = await this.analiticasRepository.GetListaAnaliticasByIdUsuarioAsync(idUsuario);
            List<Prescripcion> prescripciones = await this.prescripcionesRepository.GetListaPrescripcionesHoyByIdDoctorAsync(idUsuario);

            DoctorDashboardViewModel viewModel = new DoctorDashboardViewModel()
            {
                TotalCitasHoy = citasHoy.Count,
                AnaliticasPendientes = analiticasPendientes.Count,
                PrescripcionesRecientes = prescripciones.Count,
                AgendaHoy = citasHoy,
                UltimosPacientes = null
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Consulta(int id)
        {
            Cita citaActual = await this.citasRepository.GetCitaByIdAsync(id);
            await this.citasRepository.UpdateCitaAsync(id, "progreso");

            Usuario paciente = citaActual.Paciente;

            DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);
            int edad = hoy.Year - paciente.FechaNacimiento.Year;

            // Ajuste si aún no ha cumplido años este año
            if (paciente.FechaNacimiento > hoy.AddYears(-edad))
            {
                edad--;
            }

            List<AntecedenteMedico> antecedentes = await this.infoClinicaRepository.GetListaAntecedentesMedicosByIdPacienteAsync(paciente.IdUsuario);
            List<Prescripcion> recetas = await this.prescripcionesRepository.GetListaPrescripcionesByIdPacienteAsync(paciente.IdUsuario);

            List<Cita> historial = await this.citasRepository.GetHistorialCitasPacienteAsync(paciente.IdUsuario, 3);
            var historialCompletado = historial.Where(c => c.Estado.ToLower() == "completada").ToList();

            List<Analitica> analiticasCita = await this.analiticasRepository.GetAnaliticasByIdCitaAsync(id);

            ConsultaViewModel viewModel = new ConsultaViewModel
            {
                IdCita = citaActual.IdCita,
                MotivoCita = citaActual.Motivo,
                NotasDoctor = citaActual.Notas,

                IdPaciente = paciente.IdUsuario,
                NombrePaciente = paciente.Nombre,
                ApellidoPaciente = paciente.Apellido1,
                DniPaciente = paciente.Dni,
                TelefonoPaciente = paciente.Telefono,
                NumeroAsegurado = paciente.NumeroAsegurado ?? "Sin Seguro",
                Genero = paciente.Genero,
                Edad = edad,

                Antecedentes = antecedentes,
                RecetasActivas = recetas,
                HistorialCitasCompletadas = historialCompletado,
                AnaliticasSolicitadasHoy = analiticasCita
            };

            ViewBag.Medicamentos = await this.prescripcionesRepository.GetListaMedicamentosAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarConsulta(ConsultaViewModel model)
        {
            await this.citasRepository.FinalizarCitaAsync(model.IdCita, model.NotasDoctor);

            return RedirectToAction("Agenda");
        }

        [HttpPost]
        public async Task<IActionResult> AñadirAntecedente(ConsultaViewModel model)
        {
            await this.infoClinicaRepository.InsertAntecedenteMedicoAsync(model.IdPaciente, model.TipoAntecedente, model.NombreAntecedente, model.SeveridadAntecedente, model.FechaDiagnostico, model.NotasAntecedente);
            return RedirectToAction("Consulta", new { id = model.IdCita });
        }

        [HttpPost]
        public async Task<IActionResult> AñadirReceta(ConsultaViewModel model)
        {
            await this.prescripcionesRepository.InsertPrescripcionAsync(model.IdCita, model.NuevoMedicamentoId, model.InstruccionesReceta, DateOnly.FromDateTime(DateTime.Now), model.FechaFinReceta);

            return RedirectToAction("Consulta", new { id = model.IdCita });
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarAnalitica(ConsultaViewModel model)
        {
            await this.analiticasRepository.InsertAnaliticaAsync(model.IdCita, model.FechaAnalitica, model.InstruccionesAnalitica);

            return RedirectToAction("Consulta", new { id = model.IdCita });
        }

        public async Task<IActionResult> Agenda()
        {
            DateTime fechaConsulta = DateTime.Today;

            int idDoctor = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var citasDelDia = await citasRepository.GetCitasByDoctorYFechaAsync(idDoctor, fechaConsulta);

            var viewModel = new DoctorAgendaViewModel
            {
                FechaDada = DateTime.Today,
                Citas = citasDelDia,

                TotalCitas = citasDelDia.Count,
                CitasCompletadas = citasDelDia.Count(c => c.Estado.ToLower() == "completada"),
                CitasEnCurso = citasDelDia.Count(c => c.Estado.ToLower() == "progreso"),
                CitasPendientes = citasDelDia.Count(c => c.Estado.ToLower() == "programada"),
                CitasCanceladas = citasDelDia.Count(c => c.Estado.ToLower() == "cancelada")
            };

            return View(viewModel);
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
                Usuario usuario = await this.usuariosRepository.GetUsuarioByIdAsync(idUsuario);
                EditUsuarioViewModel model = new EditUsuarioViewModel()
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

        public async Task<IActionResult> Analiticas()
        {
            int idDoctor = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<Analitica> analiticas = await this.analiticasRepository.GetAnaliticaByIdDoctorAsync(idDoctor);
            return View(analiticas);
        }

        public async Task<IActionResult> CambiarEstadoAnalitica(int id)
        {
            await this.analiticasRepository.UpdateAnaliticaRealizada(id);
            return RedirectToAction("Analiticas");
        }

        public async Task<IActionResult> CompletarAnalitica(int idanalitica)
        {
            Analitica analitica = await this.analiticasRepository.GetAnaliticaByIdAsync(idanalitica);

            CompletarAnaliticaViewModel viewModel = new CompletarAnaliticaViewModel
            {
                IdAnalitica = analitica.IdAnalitica,
                EstadoActual = analitica.Estado,
                NombrePaciente = $"{analitica.Cita.Paciente.Nombre} {analitica.Cita.Paciente.Apellido1}",
                DniPaciente = analitica.Cita.Paciente.Dni,
                FechaSolicitud = analitica.FechaAnalitica,
                NombreDoctor = $"Dr/a. {analitica.Cita.Doctor.Nombre} {analitica.Cita.Doctor.Apellido1}",
                MotivoCita = analitica.Cita.Motivo,
                NotasDoctor = analitica.Notas
            };

            ViewBag.TiposMedicion = await this.analiticasRepository.GetListaTiposMedicionAsync();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CompletarAnalitica(CompletarAnaliticaViewModel model)
        {
            var analitica = await this.analiticasRepository.GetAnaliticaByIdAsync(model.IdAnalitica);

            analitica.Notas = model.NotasDoctor;
            analitica.Estado = "completada";

            List<Medicion> nuevasMediciones = new List<Medicion>();

            for (int i = 0; i < model.TiposMedicion.Count; i++)
            {
                if (model.TiposMedicion[i] > 0)
                {
                    nuevasMediciones.Add(new Medicion
                    {
                        IdAnalitica = model.IdAnalitica,
                        IdTipoMedicion = model.TiposMedicion[i],
                        ValorMedicion = model.Valores[i]
                    });
                }

                await this.analiticasRepository.InsertMedicionesToAnaliticaAsync(analitica.IdAnalitica, nuevasMediciones);
                await this.analiticasRepository.UpdateAnaliticaAsync(analitica.IdAnalitica, analitica);

                return RedirectToAction("Analiticas");
            }

            ViewBag.TiposMedicion = await this.analiticasRepository.GetListaTiposMedicionAsync();
            return View(model);
        }

        public async Task<IActionResult> DetallesCitaPdf(int id)
        {
            byte[] pdfBytes = await exportService.GenerarInformeCitaPdfAsync(id);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("No se encontraron resultados para esta cita.");
            }

            string fileName = $"Detalles_Cita_{id}_{DateTime.Now:ddMMyyyy}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> DetallesAnaliticaPdf(int id)
        {
            byte[] pdfBytes = await exportService.GenerarInformeAnaliticaPdfAsync(id);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("No se pudo generar el informe de la analítica. Es posible que aún no haya resultados.");
            }

            string fileName = $"Resultados_Analitica_{id}_{DateTime.Now:ddMMyyyy}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }


    }
}
