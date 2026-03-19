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
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Prescripcion> Prescripciones { get; set; }
        public DbSet<TipoMedicion> TiposMedicion { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURACIÓN DE USUARIO ---
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Dni).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                // Relación 1:Especialidad (Evitar borrado si hay doctores)
                entity.HasOne(u => u.Especialidad)
                    .WithMany(e => e.Doctores)
                    .HasForeignKey(u => u.IdEspecialidad)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación 1:1 con Credenciales
                entity.HasOne(u => u.Credencial)
                    .WithOne(c => c.Usuario)
                    .HasForeignKey<Credencial>(c => c.IdUsuario);

                // Relación 1:1 con Info Clínica
                entity.HasOne(u => u.InfoClinica)
                    .WithOne(i => i.Paciente)
                    .HasForeignKey<InfoClinicaPaciente>(i => i.IdPaciente);
            });

            // --- CONFIGURACIÓN DE CITA (Combinada) ---
            modelBuilder.Entity<Cita>(entity =>
            {
                // Relación con Paciente
                entity.HasOne(c => c.Paciente)
                    .WithMany(u => u.CitasComoPaciente)
                    .HasForeignKey(c => c.IdPaciente)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Doctor
                entity.HasOne(c => c.Doctor)
                    .WithMany(u => u.CitasComoDoctor)
                    .HasForeignKey(c => c.IdDoctor)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Analíticas
                entity.HasMany(c => c.Analiticas)
                    .WithOne(a => a.Cita)
                    .HasForeignKey(a => a.IdCita)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- CONFIGURACIÓN DE PRECISIONES DECIMALES ---
            modelBuilder.Entity<Medicion>()
                .Property(m => m.ValorMedicion).HasPrecision(10, 2);

            modelBuilder.Entity<TipoMedicion>(entity =>
            {
                entity.Property(tm => tm.ValorMaximo).HasPrecision(10, 2);
                entity.Property(tm => tm.ValorMinimo).HasPrecision(10, 2);
            });

            modelBuilder.Entity<InfoClinicaPaciente>()
                .Property(i => i.PesoActual).HasPrecision(5, 2);

            // --- CONFIGURACIÓN DE ANALÍTICA ---
            modelBuilder.Entity<Analitica>(entity =>
            {
                // Mapeo explícito de la clave foránea desde la tabla dependiente
                entity.HasOne(a => a.Cita)
                      .WithMany(c => c.Analiticas) // Asumiendo que en Cita tienes public ICollection<Analitica> Analiticas { get; set; }
                      .HasForeignKey(a => a.IdCita)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}