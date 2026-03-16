using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface IPrescripcionesRepository
    {
        Task<List<Medicamento>> GetListaMedicamentosAsync();
        Task<List<Medicamento>> GetListaMedicamentosBySearch(string nombre);
        Task<List<Prescripcion>> GetListaPrescripcionesByIdPacienteAsync(int idPaciente);
        Task<List<Prescripcion>> GetListaPrescripcionesByIdCitaAsync(int idCita);
        Task<Prescripcion> GetPrescripcionByIdAsync(int idPrescripcion);

        Task InsertMedicamento(Medicamento medicamento);
        Task InsertPrescripcionAsync(int idCita, int idMedicamento, string instrucciones, DateOnly fechaInicio, DateOnly fechaFin);

        Task UpdatePrescripcionAsync(int idPrescripcion, int idMedicamento, string instrucciones, DateOnly fechaInicio, DateOnly fechaFin);

        Task UpdateMedicamentoAsync(int idMedicamento, Medicamento medicamento);

        Task DeleteLogicoMedicamentoAsync(int idMedicamento);

        Task DeleteLogicoPrescripcionAsync(int idPrescripcion);

    }
}
