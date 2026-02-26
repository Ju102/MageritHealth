using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("MEDICATIONS")]
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }
        public string CommercialName { get; set; }
        public string GenericName { get; set; }
        public string MedicationFormat { get; set; }
        public string MedicationAdministration { get; set; }
        public string Concentration { get; set; }
    }
}
