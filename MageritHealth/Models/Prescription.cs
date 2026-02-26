using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("PRESCRIPTIONS")]
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string MedicationId { get; set; }
        public string Dosage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
