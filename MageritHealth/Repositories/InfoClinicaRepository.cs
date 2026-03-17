using MageritHealth.Data;
using MageritHealth.Models;
using MageritHealth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Repositories
{
    public class InfoClinicaRepository : IInfoClinicaRepository
    {
        private readonly MageritHealthDbContext context;

        public InfoClinicaRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task DeleteLogicoAntecedenteMedicoAsync(int idAntecedente)
        {
            AntecedenteMedico antecedenteMedico = await this.context.AntecedentesMedicos.FirstOrDefaultAsync(a => a.IdAntecedente == idAntecedente);
            if (antecedenteMedico != null)
            {
                antecedenteMedico.Activo = false;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<InfoClinicaPaciente> GetInfoClinicaPacienteByIdPacienteAsync(int idPaciente)
        {
            return await this.context.InfoClinicaPacientes.FirstOrDefaultAsync(i => i.IdPaciente == idPaciente);
        }

        public async Task<List<AntecedenteMedico>> GetListaAntecedentesMedicosByIdPacienteAsync(int idPaciente)
        {
            return await this.context.AntecedentesMedicos.Where(a => a.IdPaciente == idPaciente && a.Activo).ToListAsync();
        }

        public async Task InsertAntecedenteMedicoAsync(int idPaciente, string tipo, string nombre, string severidad, DateTime fecha, string notas)
        {
            int maxId = await this.context.AntecedentesMedicos.MaxAsync(a => (int?)a.IdAntecedente) ?? 0;
            AntecedenteMedico antecedente = new AntecedenteMedico()
            {
                IdAntecedente = maxId + 1,
                IdPaciente = idPaciente,
                Tipo = tipo,
                Nombre = nombre,
                Severidad = severidad,
                FechaDiagnostico = fecha,
                Notas = notas,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            await this.context.AntecedentesMedicos.AddAsync(antecedente);
            await this.context.SaveChangesAsync();
        }

        public async Task InsertInfoClinicaPacienteAsync(int idPaciente, string grupo, decimal pesoNac, string nombreContacto, string telefContacto)
        {
            int maxId = await this.context.InfoClinicaPacientes.MaxAsync(i => (int?)i.IdInfoClinica) ?? 0;
            InfoClinicaPaciente infoClinicaPaciente = new InfoClinicaPaciente()
            {
                IdInfoClinica = maxId + 1,
                IdPaciente = idPaciente,
                GrupoSanguineo = grupo,
                ContactoEmergenciaNombre = nombreContacto,
                ContactoEmergenciaTelefono = telefContacto,
            };

            await this.context.InfoClinicaPacientes.AddAsync(infoClinicaPaciente);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAntecedenteMedicoAsync(int idAntecedente, string tipo, string nombre, string severidad, DateTime fecha, string notas)
        {
            AntecedenteMedico antecedente = await this.context.AntecedentesMedicos.FirstOrDefaultAsync(a => a.IdAntecedente == idAntecedente);
            if (antecedente != null)
            {
                antecedente.Tipo = tipo;
                antecedente.Nombre = nombre;
                antecedente.Severidad = severidad;
                antecedente.FechaDiagnostico = fecha;
                antecedente.Notas = notas;

                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdateInfoClinicaPacienteByIdInfoClinica(int idInfoClinica, string grupo, decimal pesoNac, string nombreContacto, string telefContacto)
        {
            InfoClinicaPaciente infoClinicaPaciente = await this.context.InfoClinicaPacientes.FirstOrDefaultAsync(i => i.IdInfoClinica == idInfoClinica);

            if (infoClinicaPaciente != null)
            {
                infoClinicaPaciente.GrupoSanguineo = grupo;
                infoClinicaPaciente.ContactoEmergenciaNombre = nombreContacto;
                infoClinicaPaciente.ContactoEmergenciaTelefono = telefContacto;

                await this.context.SaveChangesAsync();
            }
        }
    }
}
