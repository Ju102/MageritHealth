using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("ANTECEDENTES_MEDICOS")]
    public class AntecedenteMedico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAntecedente { get; set; }

        public int IdPaciente { get; set; }

        public string Tipo { get; set; }

        public string Nombre { get; set; }

        public string? Severidad { get; set; }

        public DateTime? FechaDiagnostico { get; set; }
        public string? Notas { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }

        // Relacion 1 a N con Usuario (Paciente)
        [ForeignKey("IdPaciente")]
        public virtual Usuario Paciente { get; set; }
    }
}
