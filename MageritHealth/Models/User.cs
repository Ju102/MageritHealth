using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;

namespace MageritHealth.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Email { get; set; }
        public string Pass { get; set; }
        public string UserRole { get; set; }

        public string FirstName { get; set; }
        public string LastName1 { get; set; }
        public string? LastName2 { get; set; }
        public string Dni { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string UserAddress { get; set; }

        public int? SpecialtyId { get; set; }
        public string? LicenseNumber { get; set; }

        public string? InsuranceNumber { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
