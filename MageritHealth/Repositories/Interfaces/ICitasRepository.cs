using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface ICitasRepository
    {
        Task<Cita> GetCitaByIdAsync(int idCita);
        Task<List<Cita>> GetAllCitasAsync();
        Task<List<Cita>> GetListaCitasByIdPacienteAsync(int idPaciente, bool activas);

        Task<List<Cita>> GetAllCitasByIdPacienteAsync(int idPaciente);

        Task<List<Cita>> GetProximasCitasAsync();
        Task<int> GetRecuentoCitasActivasAsync();

        Task<List<Cita>> GetListaCitasByIdDoctorAsync(int idDoctor, bool activas);
        Task<List<Cita>> GetAllCitasByIdPacienteAndIdDoctorAsync(int idPaciente, int idDoctor);

        Task InsertCitaAsync(int idPaciente, int idDoctor, string motivo, DateTime fechaHora);

        Task UpdateCitaAsync(int idCita, string estado);

        Task DeleteLogicoCitaAsync(int idCita);
        Task<Cita> GetProximaCitaAsync(int idPaciente);
        Task<List<Cita>> GetHistorialCitasPacienteAsync(int idPaciente, int? limit);
        Task<Cita> GetUltimaCitaAsync(int idPaciente);
        Task<List<Cita>> GetCitasHoyByIdDoctorAsync(int idDoctor);

        Task<List<Cita>> GetCitasByDoctorYFechaAsync(int idDoctor, DateTime fecha);

        Task<List<string>> GetHorasDisponiblesDoctorAsync(int idDoctor, DateTime fechaElegida);

        Task FinalizarCitaAsync(int idCita, string notasDoctor);
    }
}