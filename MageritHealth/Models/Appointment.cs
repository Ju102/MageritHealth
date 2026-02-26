using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MageritHealth.Models
{
    [Table("APPOINTMENTS")]
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Room { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; } // Scheduled, Ongoing, Completed, Cancelled, ...
        public string Reason { get; set; }
        public string DoctorNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
