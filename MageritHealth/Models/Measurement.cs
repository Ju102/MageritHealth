using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("MEASUREMENTS")]
    public class Measurement
    {
        [Key]
        public int MeasurementId { get; set; }
        public int MeasurementTypeId { get; set; }
        public decimal MeasurementValue { get; set; }
        public int AnalysisId { get; set; }
    }
}
