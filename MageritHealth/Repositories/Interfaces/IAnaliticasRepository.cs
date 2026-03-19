using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface IAnaliticasRepository
    {
        Task<List<Medicion>> GetListaMedicionesByIdAnaliticaAsync(int idAnalitica);
        Task<List<Analitica>> GetAllAnaliticasAsync();
        Task<List<Analitica>> GetListaAnaliticasByIdUsuarioAsync(int idUsuario);
        Task<List<Analitica>> GetAnaliticasByIdCitaAsync(int idCita);
        Task<Analitica> GetAnaliticaByIdAsync(int idAnalitica);

        Task<int> GetRecuentoProximasAnaliticasAsync();
        Task<List<Analitica>> GetAnaliticaByIdDoctorAsync(int idDoctor);

        Task InsertAnaliticaAsync(int idCita, DateTime fecha, string? notas);

        Task InsertMedicionesToAnaliticaAsync(int idAnalitica, List<Medicion> mediciones);

        Task UpdateAnaliticaAsync(int idAnalitica, Analitica analitica);

        Task UpdateAnaliticaRealizada(int idAnalitica);

        Task UpdateAnaliticaCompletada(int idAnalitica);

        Task UpdateMedicionesAnaliticaAsync(int idAnalitica, List<Medicion> mediciones);

        Task InsertTipoMedicionAsync(TipoMedicion tipo);
        Task UpdateTipoMedicionAsync(int idTipo, TipoMedicion tipo);
        Task<List<TipoMedicion>> GetListaTiposMedicionAsync();
        Task<TipoMedicion> FindTipoMedicionByIdAsync(int idTipo);
        Task DeleteLogicoTipoMedicionAsync(int idTipo);

    }
}
