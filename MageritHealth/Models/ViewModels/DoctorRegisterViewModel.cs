using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class DoctorRegisterViewModel : UserRegisterViewModel
    {
        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        public int Especialidad { get; set; }

        [Required(ErrorMessage = "El número de colegiado es obligatorio.")]
        public string NumeroColegiado { get; set; }
    }
}
