using MageritHealth.Helpers;
using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface IUsuariosRepository
    {
        Task<Usuario> LoginUsuarioAsync(string email, string password);

        Task<Usuario> GetUsuarioByIdAsync(int id);
        Task<List<Usuario>> GetListaUsuariosByRolAsync(string rol);

        Task<List<Usuario>> GetListaPacientesByIdDoctorAsync(int idDoctor);

        Task<List<Usuario>> GetListaDoctoresByEspecialidadAsync(int especialidad);

        Task InsertUsuarioAsync(Usuario user, string password);

        Task UpdateUsuarioAsync(Usuario changedUser);

        Task UpdatePasswordUsuarioAsync(int idUsuario, string oldPassword, string newPassword);

        Task ResetPasswordUsuarioAsync(int idUsuario, string newPassword);

        Task EnableUsuarioAsync(int idUsuario);

        Task DisableUsuarioAsync(int idUsuario);

        Task<List<Especialidad>> GetListaEspecialidadesAsync();

        Task<Especialidad> GetEspecialidadByIdAsync(int idEspecialidad);

        Task InsertEspecialidadAsync(string especialidad);
        Task DeleteEspecialidadAsync(int idEspecialidad);

        Task UpdateDatosUsuarioAsync(int idUsuario, string telefono, string email, string password);

    }
}
