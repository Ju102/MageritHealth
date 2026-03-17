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

        public async Task UpdateCitaAsync(int idCita, int idDoctor, DateTime fechaHora)
        {
            Cita cita = await this.context.Citas.FirstOrDefaultAsync(c => c.IdCita == idCita);
            if (cita != null)
            {
                cita.IdDoctor = idDoctor;
                cita.FechaHora = fechaHora;

                await this.context.SaveChangesAsync();
            }
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
            return await this.context.Citas
                .Include(c => c.Doctor) // Carga el doctor
                    .ThenInclude(d => d.Especialidad) // Carga la especialidad del doctor
                .Include(c => c.Paciente) // Por si acaso también lo necesitas
                .FirstOrDefaultAsync(c => c.IdCita == idCita);
        }

        public async Task<List<Cita>> GetAllCitas()
        {
            return await this.context.Citas.OrderByDescending(c => c.FechaHora).ToListAsync();
        }

        public async Task<Cita> GetProximaCitaAsync(int idPaciente)
        {
            return await this.context.Citas
                .Include(c => c.Doctor)
                    .ThenInclude(d => d.Especialidad)
                .Where(c => c.IdPaciente == idPaciente && c.Activa == true && c.FechaHora > DateTime.Now)
                .OrderBy(c => c.FechaHora)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Cita>> GetHistorialCitasPacienteAsync(int idPaciente, int limit)
        {
            return await this.context.Citas
                .Include(c => c.Doctor)
                .Where(c => c.IdPaciente == idPaciente && c.Estado == "completada")
                .OrderByDescending(c => c.FechaHora)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Cita>> GetAllCitasByIdPaciente(int idPaciente)
        {
            return await this.context.Citas.Include(c => c.Doctor)
                .ThenInclude(d => d.Especialidad).Where(c => c.IdPaciente == idPaciente).ToListAsync();
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
    }
}
