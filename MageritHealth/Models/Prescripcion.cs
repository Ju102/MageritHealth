using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("PRESCRIPCIONES")]
    public class Prescripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPrescripcion { get; set; }

        public int IdCita { get; set; }

        public int IdMedicamento { get; set; }

        public string Instrucciones { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activa { get; set; }

        [ForeignKey("IdCita")]
        public virtual Cita Cita { get; set; }
        [ForeignKey("IdMedicamento")]
        public virtual Medicamento Medicamento { get; set; }
    }
}
