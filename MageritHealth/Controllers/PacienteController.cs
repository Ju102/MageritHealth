using MageritHealth.Filters;
using MageritHealth.Models;
using MageritHealth.Models.ViewModels;
using MageritHealth.Repositories.Interfaces;
using MageritHealth.Services.Interfaces;
using MageritHealth.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        private readonly IExportService exportService;

        public PacienteController(IUsuariosRepository usuariosRepository,
            ICitasRepository citasRepository, IInfoClinicaRepository infoClinicaRepository,
            IPrescripcionesRepository prescripcionesRepository, IAnaliticasRepository analiticasRepository, IExportService exportService)
        {
            this.usuariosRepository = usuariosRepository;
            this.citasRepository = citasRepository;
            this.infoClinicaRepository = infoClinicaRepository;
            this.prescripcionesRepository = prescripcionesRepository;
            this.analiticasRepository = analiticasRepository;
            this.exportService = exportService;
        }

        public async Task<IActionResult> Dashboard()
        {
            int idUsuarioLogueado = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            string numeroAsegurado = HttpContext.User.FindFirstValue("InsuranceNumber");
            List<Cita> historialCitas = await this.citasRepository.GetHistorialCitasPacienteAsync(idUsuarioLogueado, 3);
            Cita proximaCita = await this.citasRepository.GetProximaCitaAsync(idUsuarioLogueado);
            List<Prescripcion> todasPrescripciones = await this.prescripcionesRepository.GetListaPrescripcionesByIdPacienteAsync(idUsuarioLogueado);
            List<Prescripcion> medicacionActiva = todasPrescripciones.Where(p => p.Activa).ToList();
            InfoClinicaPaciente info = await this.infoClinicaRepository.GetInfoClinicaPacienteByIdPacienteAsync(idUsuarioLogueado);
            List<AntecedenteMedico> antecedentes = await this.infoClinicaRepository.GetListaAntecedentesMedicosByIdPacienteAsync(idUsuarioLogueado);

            PerfilVital perfil = new PerfilVital();

            if (info != null)
            {
                perfil.NombreContacto = info.ContactoEmergenciaNombre ?? "Sin asignar";
                perfil.TelefonoContacto = info.ContactoEmergenciaTelefono ?? "--";
                perfil.Peso = info.PesoActual ?? 0;
                perfil.TipoSangre = info.GrupoSanguineo ?? "Desconocido";
                perfil.Altura = 100;
            }
            else
            {
                perfil.NombreContacto = "Sin asignar";
                perfil.TelefonoContacto = "--";
                perfil.Peso = 0;
                perfil.TipoSangre = "Desconocido";
                perfil.Altura = 0;
            }

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

        public async Task<IActionResult> GetHorasDisponibles(int idDoctor, string fecha)
        {
            // Validar que la fecha sea correcta
            if (DateTime.TryParse(fecha, out DateTime fechaParsed))
            {
                List<string> horasLibres = await this.citasRepository.GetHorasDisponiblesDoctorAsync(idDoctor, fechaParsed);

                // Devolvemos el array en formato JSON para que jQuery lo entienda
                // Ej: ["09:00", "10:30", "14:00"]
                return Json(horasLibres);
            }

            // Si la fecha es inválida, devolvemos un array vacío
            return Json(new List<string>());
        }

        public async Task<JsonResult> GetDoctoresPorEspecialidad(int idEspecialidad)
        {
            // Buscamos solo a los médicos de esa especialidad concreta
            var doctores = await this.usuariosRepository.GetListaDoctoresByEspecialidadAsync(idEspecialidad);

            // Transformamos la lista en el formato que espera el JavaScript (value, text)
            var doctoresFiltrados = doctores.Select(d => new
            {
                value = d.IdUsuario,
                text = $"Dr/a. {d.Nombre} {d.Apellido1} {d.Apellido2}"
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
            List<Cita> citas = await this.citasRepository.GetAllCitasByIdPacienteAsync(idUsuario);
            return View(citas);
        }

        public async Task<IActionResult> Salud()
        {
            MiSaludVitalViewModel viewModel = new MiSaludVitalViewModel();

            int idUsuario = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            int idUltimaAnalitica;
            viewModel.Analiticas = await this.analiticasRepository.GetListaAnaliticasByIdUsuarioAsync(idUsuario);
            if (viewModel.Analiticas != null && viewModel.Analiticas.Count != 0)
            {
                idUltimaAnalitica = viewModel.Analiticas.OrderByDescending(a => a.FechaAnalitica).FirstOrDefault().IdAnalitica;
                List<Medicion> mediciones = await this.analiticasRepository.GetListaMedicionesByIdAnaliticaAsync(idUltimaAnalitica);
                foreach (Medicion medicion in mediciones)
                {
                    MedicionResumen resumen = new MedicionResumen();
                    resumen.Nombre = medicion.TipoMedicion.NombreMedicion;
                    resumen.Unidad = medicion.TipoMedicion.UnidadMedicion;
                    resumen.Valor = medicion.ValorMedicion;
                    resumen.Minimo = medicion.TipoMedicion.ValorMinimo;
                    resumen.Maximo = medicion.TipoMedicion.ValorMaximo;
                    resumen.Fecha = medicion.Analitica.FechaAnalitica;
                    viewModel.Mediciones.Add(resumen);
                }
            }
            else { }

            List<AntecedenteMedico> antecedentes = await this.infoClinicaRepository.GetListaAntecedentesMedicosByIdPacienteAsync(idUsuario);
            antecedentes = antecedentes.Where(a => a.Tipo.ToLower() != "alergia").ToList();

            viewModel.AntecedentesMedicos = antecedentes;

            return View(viewModel);

        }

        public async Task<IActionResult> Tratamientos()
        {
            // 1. Obtener el ID del paciente logueado
            int idUsuarioLogueado = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            List<Prescripcion> todasPrescripciones = await this.prescripcionesRepository.GetListaPrescripcionesByIdPacienteAsync(idUsuarioLogueado);

            List<AntecedenteMedico> antecedentes = await this.infoClinicaRepository.GetListaAntecedentesMedicosByIdPacienteAsync(idUsuarioLogueado);

            TratamientosViewModel viewmodel = new TratamientosViewModel()
            {
                // Medicación activa: La fecha de fin es mayor a hoy, o es null (tratamiento crónico)
                MedicacionActiva = todasPrescripciones
                    .Where(p => p.Activa == true)
                    .OrderByDescending(p => p.FechaInicio)
                    .ToList(),

                // Historial: La fecha de fin ya pasó y esta !Activa
                HistorialMedicacion = todasPrescripciones
                    .Where(p => p.Activa == false)
                    .OrderByDescending(p => p.FechaFin)
                    .ToList(),

                // Alergias: Filtramos los antecedentes donde el Tipo sea 'alergia'
                Alergias = antecedentes
                    .Where(a => a.Tipo.ToLower() == "alergia")
                    .ToList()
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> DetallesCita(int idcita)
        {
            Cita cita = await this.citasRepository.GetCitaByIdAsync(idcita);
            return View(cita);
        }

        [HttpPost]
        public async Task<IActionResult> CancelarCita(int idcita)
        {
            await this.citasRepository.DeleteLogicoCitaAsync(idcita);
            return RedirectToAction("Citas");
        }

        public async Task<IActionResult> ResultadosAnalitica(int id)
        {
            byte[] pdfBytes = await exportService.GenerarInformeAnaliticaPdfAsync(id);

            // Si por algún motivo falla (ej. no existe la analítica), devolvemos un 404
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("No se pudo generar el informe de la analítica. Es posible que aún no haya resultados.");
            }

            // Devolvemos el archivo PDF para que el navegador lo descargue
            string fileName = $"Resultados_Analitica_{id}_{DateTime.Now:ddMMyyyy}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> DetallesRecetaPdf(int id)
        {
            // Ojo: aquí pasamos el idCita, tal como definiste en tu servicio
            byte[] pdfBytes = await exportService.GenerarRecetasPorCitaPdfAsync(id);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("No se encontraron prescripciones médicas asociadas a esta cita.");
            }

            string fileName = $"Plan_Medicacion_{id}_{DateTime.Now:ddMMyyyy}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
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
    }
}
