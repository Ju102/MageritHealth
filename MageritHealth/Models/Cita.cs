using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("CITAS")]
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCita { get; set; }

        public int IdPaciente { get; set; }

        public int IdDoctor { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaHora { get; set; }

        public string? Notas { get; set; }

        public string Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activa { get; set; }

        [ForeignKey("IdPaciente")]
        public virtual Usuario Paciente { get; set; }
        [ForeignKey("IdDoctor")]
        public virtual Usuario Doctor { get; set; }

        public virtual ICollection<Analitica> Analiticas { get; set; }
        public virtual ICollection<Prescripcion> Prescripciones { get; set; }

    }
}
