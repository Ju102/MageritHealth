using MageritHealth.Models;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Data
{
    public class MageritHealthDbContext : DbContext
    {
        public MageritHealthDbContext(DbContextOptions<MageritHealthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cuando se recupera cada registro, decide si crea, de User, un objeto Admin, Doctor o Patient, según el campo UserRole.
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserRole")
                .HasValue<Patient>("Patient")
                .HasValue<Doctor>("Doctor")
                .HasValue<Admin>("Admin");

            
            modelBuilder.Entity<Appointment>()
                .HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne<Patient>()
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Appointment> Appointments { get; set; }



        public DbSet<Prescription> Prescriptions { get; set; }

    }
}
