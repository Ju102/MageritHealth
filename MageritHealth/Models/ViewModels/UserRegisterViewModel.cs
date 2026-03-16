using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "No puede superar los 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        public string Apellido1 { get; set; }

        public string? Apellido2 { get; set; }

        [Required(ErrorMessage = "El DNI/Pasaporte es obligatorio")]
        [RegularExpression(@"^[0-9A-Z]+$", ErrorMessage = "Formato de DNI inválido")]
        public string Dni { get; set; }

        [Required]
        public DateOnly FechaNacimiento { get; set; }

        [Required]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; }

        [Required]
        public string Genero { get; set; }

        [Required]
        public string Direccion { get; set; }
    }
}