using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    public class Doctor : User
    {
        public int SpecialtyId { get; set; }
        public string LicenseNumber { get; set; }
    }
}
