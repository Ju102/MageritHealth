using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class PacienteRegisterViewModel : UserRegisterViewModel
    {
        public string Rol { get; set; } = "paciente";

        [Required(ErrorMessage = "El número de asegurado es obligatorio.")]
        public string NumeroAsegurado { get; set; }
    }
}
