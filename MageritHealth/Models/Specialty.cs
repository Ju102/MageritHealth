using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("SPECIALTIES")]
    public class Specialty
    {
        [Key]
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
    }
}
