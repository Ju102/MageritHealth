using MageritHealth.Models;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Data
{
    public class MageritHealthDbContext : DbContext
    {
        public MageritHealthDbContext(DbContextOptions<MageritHealthDbContext> options)
            : base(options) { }

        public DbSet<Analitica> Analiticas { get; set; }
        public DbSet<AntecedenteMedico> AntecedentesMedicos { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Credencial> Credenciales { get; set; }
        public DbSet<DoctorPaciente> DoctoresPacientes { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<InfoClinicaPaciente> InfoClinicaPacientes { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Medicion> Mediciones { get; set; }
        public DbSet<Prescripcion> Prescripciones { get; set; }
        public DbSet<TipoMedicion> TiposMedicion { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Evitar borrado en cascada en Citas (Ruta Paciente)
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(u => u.CitasComoPaciente)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // Evitar borrado en cascada en Citas (Ruta Doctor)
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Doctor)
                .WithMany(u => u.CitasComoDoctor)
                .HasForeignKey(c => c.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Evitar borrado de especialidades si tienen doctores
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Especialidad)
                .WithMany(e => e.Doctores)
                .HasForeignKey(u => u.IdEspecialidad)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorPaciente>()
            .HasOne(dp => dp.Paciente)
            .WithMany(u => u.MedicosAsignados)
            .HasForeignKey(dp => dp.IdPaciente)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorPaciente>()
                .HasOne(dp => dp.Doctor)
                .WithMany(u => u.PacientesAsignados)
                .HasForeignKey(dp => dp.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
