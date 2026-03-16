using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface IAnaliticasRepository
    {
        Task<List<Medicion>> GetListaMedicionesByIdAnaliticaAsync(int idAnalitica);

        Task<List<Analitica>> GetListaAnaliticasByIdUsuarioAsync(int idUsuario);
        Task<Analitica> GetAnaliticaByIdCitaAsync(int idCita);
        Task<Analitica> GetAnaliticaByIdAsync(int idAnalitica);

        // Programar la analítica
        Task InsertAnaliticaAsync(int idCita, DateTime fecha, string? notas);

        // Añadir las mediciones de una analítica
        Task InsertMedicionesToAnaliticaAsync(int idAnalitica, List<Medicion> mediciones);

        Task UpdateAnaliticaAsync(int idAnalitica, Analitica analitica);

        // Cambiar alguna medición de una analítica
        Task UpdateMedicionesAnaliticaAsync(int idAnalitica, List<Medicion> mediciones);

        Task InsertTipoMedicionAsync(TipoMedicion tipo);
        Task UpdateTipoMedicionAsync(int idTipo, TipoMedicion tipo);
        Task<List<TipoMedicion>> GetListaTiposMedicionAsync();
        Task<TipoMedicion> FindTipoMedicionByIdAsync(int idTipo);
        Task DeleteLogicoTipoMedicionAsync(int idTipo);
    }
}
