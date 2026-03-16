using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface ICitasRepository
    {
        Task<Cita> GetCitaByIdAsync(int idCita);
        Task<List<Cita>> GetAllCitas();
        Task<List<Cita>> GetListaCitasByIdPacienteAsync(int idPaciente, bool activas);

        Task<List<Cita>> GetAllCitasByIdPaciente(int idPaciente);

        Task<List<Cita>> GetListaCitasByIdDoctorAsync(int idDoctor, bool activas);

        Task InsertCitaAsync(int idPaciente, int idDoctor, string motivo, DateTime fechaHora);

        Task UpdateCitaAsync(int idCita, int idDoctor, DateTime fechaHora);

        Task DeleteLogicoCitaAsync(int idCita);
        Task<Cita> GetProximaCitaAsync(int idPaciente);
        Task<List<Cita>> GetHistorialCitasPacienteAsync(int idPaciente, int limit);
    }
}