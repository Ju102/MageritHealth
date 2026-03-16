using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface IInfoClinicaRepository
    {
        Task<InfoClinicaPaciente> GetInfoClinicaPacienteByIdPacienteAsync(int idPaciente);
        Task InsertInfoClinicaPacienteAsync(int idPaciente, string grupo, decimal pesoNac, string nombreContacto, string telefContacto);

        Task UpdateInfoClinicaPacienteByIdInfoClinica(int idInfoClinica, string grupo, decimal pesoNac, string nombreContacto, string telefContacto);

        Task<List<AntecedenteMedico>> GetListaAntecedentesMedicosByIdPacienteAsync(int idPaciente);
        Task InsertAntecedenteMedicoAsync(int idPaciente, string tipo, string nombre, string severidad, DateTime fecha, string notas);
        Task DeleteLogicoAntecedenteMedicoAsync(int idAntecedente);
        Task UpdateAntecedenteMedicoAsync(int idAntecedente, string tipo, string nombre, string severidad, DateTime fecha, string notas);
    }
}
