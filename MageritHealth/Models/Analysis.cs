using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("ANALYSES")]
    public class Analysis
    {
        [Key]
        public int AnalysisId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string AnalysisStatus { get; set; }
        public string Notes { get; set; }
    }
}
