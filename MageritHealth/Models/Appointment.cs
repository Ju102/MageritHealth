using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("APPOINTMENTS")]
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; } // Scheduled, Ongoing, Completed, Cancelled, ...
        public string Reason { get; set; }
        public string DoctorNotes { get; set; }

    }
}
