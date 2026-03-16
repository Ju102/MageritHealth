using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("INFO_CLINICA_PACIENTES")]
    public class InfoClinicaPaciente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdInfoClinica { get; set; }

        public int IdPaciente { get; set; }

        public string GrupoSanguineo { get; set; }

        public decimal PesoActual { get; set; }

        public decimal PesoNacimiento { get; set; }

        public string ContactoEmergenciaNombre { get; set; }

        public string ContactoEmergenciaTelefono { get; set; }

        public DateTime FechaActualizacion { get; set; }

        // Relacion 1 a 1 con Usuario (Paciente)
        [ForeignKey("IdPaciente")]
        public virtual Usuario Paciente { get; set; }
    }
}
