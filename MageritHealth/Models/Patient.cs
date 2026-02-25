using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    public class Patient : User
    {
        public string InsuranceNumber { get; set; }
    }
}
