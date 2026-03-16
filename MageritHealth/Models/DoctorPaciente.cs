using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("DOCTORES_PACIENTES")]
    public class DoctorPaciente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAsignacion { get; set; }

        public int IdPaciente { get; set; }

        public int IdDoctor { get; set; }

        public DateTime FechaAsignacion { get; set; }

        [ForeignKey("IdPaciente")]
        public virtual Usuario Paciente { get; set; }

        [ForeignKey("IdDoctor")]
        public virtual Usuario Doctor { get; set; }
    }
}
