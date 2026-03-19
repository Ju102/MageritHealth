using MageritHealth.Data;
using MageritHealth.Models;
using MageritHealth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Repositories
{
    public class CitasRepository : ICitasRepository
    {
        private readonly MageritHealthDbContext context;

        public CitasRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }


        /* VIEWS: Paciente-Dashboard */
        public async Task<Cita> GetProximaCitaAsync(int idPaciente)
        {
            return await this.context.Citas
                .Include(c => c.Doctor)
                    .ThenInclude(d => d.Especialidad)
                .Where(c => c.IdPaciente == idPaciente && c.Activa == true)
                .OrderBy(c => c.FechaHora)
                .FirstOrDefaultAsync();
        }

        /* VIEWS: Paciente-Dashboard */
        public async Task<List<Cita>> GetHistorialCitasPacienteAsync(int idPaciente, int? limit)
        {
            var query = this.context.Citas
                .Include(c => c.Doctor)
                .Where(c => c.IdPaciente == idPaciente && c.Estado == "completada")
                .OrderByDescending(c => c.FechaHora);

            if (limit.HasValue)
            {
                return await query.Take(limit.Value).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        /* VIEWS: Paciente-Citas */
        public async Task<List<Cita>> GetAllCitasByIdPacienteAsync(int idPaciente)
        {
            return await this.context.Citas.Include(c => c.Doctor)
                .ThenInclude(d => d.Especialidad).Where(c => c.IdPaciente == idPaciente).ToListAsync();
        }

        public async Task<int> GetRecuentoCitasActivasAsync()
        {
            return await this.context.Citas.CountAsync(c => c.Activa == true);
        }

        public async Task InsertCitaAsync(int idPaciente, int idDoctor, string motivo, DateTime fechaHora)
        {
            int maxId = await this.context.Citas.MaxAsync(c => (int?)c.IdCita) ?? 0;
            Cita cita = new Cita()
            {
                IdCita = maxId + 1,
                IdPaciente = idPaciente,
                IdDoctor = idDoctor,
                Motivo = motivo,
                FechaHora = fechaHora,
                Estado = "programada",
                FechaCreacion = DateTime.Now,
                Activa = true,
            };

            await this.context.Citas.AddAsync(cita);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateCitaAsync(int idCita, string estado)
        {
            Cita cita = await this.context.Citas.FirstOrDefaultAsync(c => c.IdCita == idCita);
            if (cita != null)
            {
                cita.Estado = estado;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Cita>> GetCitasByDoctorYFechaAsync(int idDoctor, DateTime fecha)
        {
            return await this.context.Citas.Include(c => c.Paciente).Where(c => c.IdDoctor == idDoctor && c.FechaHora.Date == fecha.Date).ToListAsync();
        }

        public async Task<List<Cita>> GetAllCitasByIdPacienteAndIdDoctorAsync(int idPaciente, int idDoctor)
        {
            return await this.context.Citas.Include(c => c.Paciente).Where(c => c.IdPaciente == idPaciente && c.IdDoctor == idDoctor).ToListAsync();
        }

        public async Task DeleteLogicoCitaAsync(int idCita)
        {
            Cita cita = await this.context.Citas.FirstOrDefaultAsync(c => c.IdCita == idCita);
            if (cita != null)
            {
                cita.Estado = "cancelada";
                cita.Activa = false;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Cita>> GetListaCitasByIdPacienteAsync(int idPaciente, bool activas)
        {
            return await this.context.Citas.Where(c => c.IdPaciente == idPaciente && c.Activa == activas).OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<List<Cita>> GetListaCitasByIdDoctorAsync(int idDoctor, bool activas)
        {
            return await this.context.Citas.Where(c => c.IdDoctor == idDoctor && c.Activa == activas).OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<Cita> GetCitaByIdAsync(int idCita)
        {
            return await context.Citas
                .Include(c => c.Doctor)
                    .ThenInclude(d => d.Especialidad)
                    .Include(c => c.Paciente)
                .Include(c => c.Prescripciones)
                    .ThenInclude(p => p.Medicamento) // CRÍTICO: Si no lo incluyes, prescripcion.Medicamento será null
                .FirstOrDefaultAsync(c => c.IdCita == idCita);
        }

        public async Task<List<Cita>> GetAllCitasAsync()
        {
            return await this.context.Citas.Include(c => c.Doctor).Include(c => c.Paciente).OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task FinalizarCitaAsync(int idCita, string notasDoctor)
        {
            Cita cita = await this.context.Citas.FirstOrDefaultAsync(c => c.IdCita == idCita);
            if (cita != null)
            {
                cita.Estado = "completada";
                cita.Notas = notasDoctor;
                cita.Activa = false;
                await this.context.SaveChangesAsync();
            }
        }
        public async Task<List<Cita>> GetProximasCitasAsync()
        {
            return await this.context.Citas.Include(c => c.Doctor).Include(c => c.Paciente).Where(c => c.Activa == true && c.FechaHora > DateTime.Now).
                OrderBy(c => c.FechaHora).ToListAsync();

        }

        public async Task<Cita> GetUltimaCitaAsync(int idPaciente)
        {
            return await this.context.Citas
                .Include(c => c.Doctor)
                    .ThenInclude(d => d.Especialidad)
                .Where(c => c.IdPaciente == idPaciente && c.Estado == "completada")
                .OrderByDescending(c => c.FechaHora)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Cita>> GetCitasHoyByIdDoctorAsync(int idDoctor)
        {
            return await this.context.Citas.Include(c => c.Paciente).Where(c => c.IdDoctor == idDoctor && c.Estado != "cancelada" && c.FechaHora.Date == DateTime.Today)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();
        }

        public async Task<List<string>> GetHorasDisponiblesDoctorAsync(int idDoctor, DateTime fechaElegida)
        {
            var citasOcupadas = await this.context.Citas
                .Where(c => c.IdDoctor == idDoctor
                         && c.FechaHora.Date == fechaElegida.Date
                         && c.Estado != "cancelada")
                .Select(c => c.FechaHora.ToString("HH:mm"))
                .ToListAsync();

            List<string> todasLasHoras = new List<string>
            {
                "09:00", "09:30", "10:00", "10:30",
                "11:00", "11:30", "12:00", "12:30",
                "13:00", "13:30", "14:00", "14:30"
            };

            if (fechaElegida.Date == DateTime.Now.Date)
            {
                string horaActual = DateTime.Now.ToString("HH:mm");
                todasLasHoras = todasLasHoras.Where(h => string.Compare(h, horaActual) > 0).ToList();
            }

            var horasDisponibles = todasLasHoras.Except(citasOcupadas).ToList();

            return horasDisponibles;
        }

    }
}
