using MageritHealth.Data;
using MageritHealth.Models;
using MageritHealth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Repositories
{
    public class PrescripcionesRepository : IPrescripcionesRepository
    {
        private readonly MageritHealthDbContext context;

        public PrescripcionesRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task DeleteLogicoMedicamentoAsync(int idMedicamento)
        {
            Medicamento medicamento = await this.context.Medicamentos.FirstOrDefaultAsync(m => m.IdMedicamento == idMedicamento);
            if (medicamento != null)
            {
                medicamento.Activo = false;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task DeleteLogicoPrescripcionAsync(int idPrescripcion)
        {
            Prescripcion prescripcion = await this.context.Prescripciones.FirstOrDefaultAsync(p => p.IdPrescripcion == idPrescripcion);
            if (prescripcion != null)
            {
                prescripcion.Activa = false;
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Medicamento>> GetListaMedicamentosAsync()
        {
            return await this.context.Medicamentos.Where(m => m.Activo).ToListAsync();
        }

        public async Task<List<Medicamento>> GetListaMedicamentosBySearch(string nombre)
        {
            return await this.context.Medicamentos.Where(m => m.Activo && (m.NombreComercial.Contains(nombre) || m.PrincipioActivo.Contains(nombre))).ToListAsync();
        }

        public async Task<List<Prescripcion>> GetListaPrescripcionesByIdCitaAsync(int idCita)
        {
            return await this.context.Prescripciones
                .Where(p => p.IdCita == idCita && p.Activa)
                .ToListAsync();
        }

        public async Task<List<Prescripcion>> GetListaPrescripcionesByIdPacienteAsync(int idPaciente)
        {
            return await this.context.Prescripciones
                .Where(p => p.Cita.IdPaciente == idPaciente && p.Activa).Include(p => p.Medicamento)
                .ToListAsync();
        }

        public async Task<Prescripcion> GetPrescripcionByIdAsync(int idPrescripcion)
        {
            return await this.context.Prescripciones.FirstOrDefaultAsync(p => p.IdPrescripcion == idPrescripcion);
        }

        public async Task InsertMedicamento(Medicamento medicamento)
        {
            int maxId = await this.context.Medicamentos.MaxAsync(m => (int?)m.IdMedicamento) ?? 0;
            medicamento.IdMedicamento = maxId + 1;

            await this.context.Medicamentos.AddAsync(medicamento);
            await this.context.SaveChangesAsync();
        }

        public async Task InsertPrescripcionAsync(int idCita, int idMedicamento, string instrucciones, DateOnly fechaInicio, DateOnly fechaFin)
        {
            int maxId = await this.context.Prescripciones.MaxAsync(p => (int?)p.IdPrescripcion) ?? 0;
            Prescripcion prescripcion = new Prescripcion()
            {
                IdPrescripcion = maxId + 1,
                IdCita = idCita,
                IdMedicamento = idMedicamento,
                Instrucciones = instrucciones,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                FechaCreacion = DateTime.Now,
                Activa = true
            };

            await this.context.Prescripciones.AddAsync(prescripcion);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateMedicamentoAsync(int idMedicamento, Medicamento editMedicamento)
        {
            Medicamento medicamento = await this.context.Medicamentos.FirstOrDefaultAsync(m => m.IdMedicamento == idMedicamento);
            if (medicamento != null)
            {
                medicamento.NombreComercial = editMedicamento.NombreComercial;
                medicamento.PrincipioActivo = editMedicamento.PrincipioActivo;
                medicamento.Concentracion = editMedicamento.Concentracion;
                medicamento.Formato = editMedicamento.Formato;
                medicamento.Fabricante = editMedicamento.Fabricante;
                this.context.Medicamentos.Update(medicamento);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task UpdatePrescripcionAsync(int idPrescripcion, int idMedicamento, string instrucciones, DateOnly fechaInicio, DateOnly fechaFin)
        {
            Prescripcion prescripcion = await this.context.Prescripciones.FirstOrDefaultAsync(p => p.IdPrescripcion == idPrescripcion);

            if (prescripcion != null)
            {
                prescripcion.IdMedicamento = idMedicamento;
                prescripcion.Instrucciones = instrucciones;
                prescripcion.FechaInicio = fechaInicio;
                prescripcion.FechaFin = fechaFin;

                await this.context.SaveChangesAsync();
            }
        }
    }
}
