using MageritHealth.Data;
using MageritHealth.Models;
using MageritHealth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Repositories
{
    public class AnaliticasRepository : IAnaliticasRepository
    {
        private readonly MageritHealthDbContext context;

        public AnaliticasRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task DeleteLogicoTipoMedicionAsync(int idTipo)
        {
            TipoMedicion tipo = await this.context.TiposMedicion.FirstOrDefaultAsync(tm => tm.IdTipoMedicion == idTipo);

            if (tipo != null)
            {
                tipo.Activo = false;
                await this.context.SaveChangesAsync();
            }
        }


        public async Task<TipoMedicion> FindTipoMedicionByIdAsync(int idTipo)
        {
            return await this.context.TiposMedicion.FirstOrDefaultAsync(tm => tm.IdTipoMedicion == idTipo);
        }

        public async Task<Analitica> GetAnaliticaByIdAsync(int idAnalitica)
        {
            return await this.context.Analiticas.FirstOrDefaultAsync(a => a.IdAnalitica == idAnalitica);
        }

        public async Task<Analitica> GetAnaliticaByIdCitaAsync(int idCita)
        {
            return await this.context.Analiticas.FirstOrDefaultAsync(a => a.IdCita == idCita);
        }

        public async Task<List<Analitica>> GetListaAnaliticasByIdUsuarioAsync(int idUsuario)
        {
            return await this.context.Analiticas.Where(a => a.Cita.IdPaciente == idUsuario).ToListAsync();
        }

        public async Task<List<Medicion>> GetListaMedicionesByIdAnaliticaAsync(int idAnalitica)
        {
            return await this.context.Mediciones
                .Include(m => m.TipoMedicion)
                .Include(m => m.Analitica)
                .Where(m => m.IdAnalitica == idAnalitica)
                .ToListAsync();
        }

        public async Task<List<TipoMedicion>> GetListaTiposMedicionAsync()
        {
            return await this.context.TiposMedicion.Where(tm => tm.Activo).ToListAsync();
        }

        public async Task InsertAnaliticaAsync(int idCita, DateTime fecha, string? notas)
        {
            int maxId = await this.context.Analiticas.MaxAsync(a => (int?)a.IdAnalitica) ?? 0;
            Analitica analitica = new Analitica()
            {
                IdAnalitica = maxId + 1,
                IdCita = idCita,
                FechaAnalitica = fecha,
                Estado = "programada",
                Notas = notas
            };

            await this.context.Analiticas.AddAsync(analitica);
            await this.context.SaveChangesAsync();
        }

        public async Task InsertMedicionesToAnaliticaAsync(int idAnalitica, List<Medicion> mediciones)
        {
            Analitica analitica = await this.context.Analiticas.FirstOrDefaultAsync(a => a.IdAnalitica == idAnalitica);
            if (analitica == null) return;

            int currentMaxId = await this.context.Mediciones.MaxAsync(m => (int?)m.IdMedicion) ?? 0;

            foreach (Medicion med in mediciones)
            {
                currentMaxId++;
                med.IdMedicion = currentMaxId;
                med.IdAnalitica = idAnalitica;

                await this.context.Mediciones.AddAsync(med);
            }

            analitica.Estado = "completada";
            await this.context.SaveChangesAsync();
        }

        public async Task InsertTipoMedicionAsync(TipoMedicion tipo)
        {
            int maxId = await this.context.TiposMedicion.MaxAsync(t => (int?)t.IdTipoMedicion) ?? 0;
            tipo.IdTipoMedicion = maxId + 1;
            await this.context.TiposMedicion.AddAsync(tipo);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAnaliticaAsync(int idAnalitica, Analitica editAnalitica)
        {
            Analitica analitica = await this.context.Analiticas.FirstOrDefaultAsync(a => a.IdAnalitica == idAnalitica);

            if (analitica != null)
            {
                analitica.FechaAnalitica = editAnalitica.FechaAnalitica;
                analitica.Estado = editAnalitica.Estado;
                analitica.Notas = editAnalitica.Notas;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateMedicionesAnaliticaAsync(int idAnalitica, List<Medicion> medicionesEditadas)
        {
            // Traemos TODAS las mediciones de esta analítica de una
            var medicionesActuales = await this.context.Mediciones
                .Where(m => m.IdAnalitica == idAnalitica)
                .ToListAsync();

            foreach (var medEdit in medicionesEditadas)
            {
                var medicionBD = medicionesActuales.FirstOrDefault(m => m.IdMedicion == medEdit.IdMedicion);
                if (medicionBD != null)
                {
                    medicionBD.ValorMedicion = medEdit.ValorMedicion;
                }
            }

            await this.context.SaveChangesAsync();
        }

        public async Task UpdateTipoMedicionAsync(int idTipo, TipoMedicion tipo)
        {
            TipoMedicion tipoMedicion = await this.context.TiposMedicion.FirstOrDefaultAsync(tm => tm.IdTipoMedicion == idTipo);
            if (tipoMedicion != null)
            {
                tipoMedicion.NombreMedicion = tipo.NombreMedicion;
                tipoMedicion.UnidadMedicion = tipo.UnidadMedicion;
                tipoMedicion.ValorMaximo = tipo.ValorMaximo;
                tipoMedicion.ValorMinimo = tipo.ValorMinimo;
                await this.context.SaveChangesAsync();
            }
        }
    }
}
