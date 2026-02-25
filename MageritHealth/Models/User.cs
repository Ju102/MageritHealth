using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("USERS")]
    public abstract class User
    {
        public int UserId { get; set; }

        public string Email { get; set; }
        public string Pass { get; set; }
        public string UserRole { get; set; }

        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        public string Dni { get; set; }

        public DateOnly BirthDate { get; set; }

        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string UserAddress { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
