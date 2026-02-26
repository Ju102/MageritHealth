using MageritHealth.Models;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Data
{
    public class MageritHealthDbContext : DbContext
    {
        public MageritHealthDbContext(DbContextOptions<MageritHealthDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Prescription> Prescriptions { get; set; }

        public DbSet<Medication> Medications { get; set; }

        public DbSet<Analysis> Analyses { get; set; }

        public DbSet<Measurement> Measurements { get; set; }

        public DbSet<Specialty> Specialties { get; set; }
    }
}
