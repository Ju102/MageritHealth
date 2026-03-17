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

            // Indicar que la relación con Credenciales es 1:1
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Credencial)
                .WithOne(c => c.Usuario)
                .HasForeignKey<Credencial>(c => c.IdUsuario);

            // Para las mediciones (Glucosa, Peso, etc.)
            modelBuilder.Entity<Medicion>()
                .Property(m => m.ValorMedicion)
                .HasPrecision(10, 2);

            // Para los tipos de medicion (Rangos)
            modelBuilder.Entity<TipoMedicion>()
                .Property(tm => tm.ValorMaximo).HasPrecision(10, 2);
            modelBuilder.Entity<TipoMedicion>()
                .Property(tm => tm.ValorMinimo).HasPrecision(10, 2);

            // Para la info clínica (Peso)
            modelBuilder.Entity<InfoClinicaPaciente>()
                .Property(i => i.PesoActual).HasPrecision(5, 2);

            // Indicar que el Dni es unico
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Dni)
                .IsUnique();

            // Indicar que el email es unico
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Indicar que la relación con InfoClinicaPaciente es 1:1
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.InfoClinica)
                .WithOne(i => i.Paciente) // Asumiendo que pusiste "public virtual Usuario Paciente { get; set; }" en InfoClinicaPaciente
                .HasForeignKey<InfoClinicaPaciente>(i => i.IdPaciente);
        }
    }
}
