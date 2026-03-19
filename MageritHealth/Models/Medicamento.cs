using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("MEDICAMENTOS")]
    public class Medicamento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdMedicamento { get; set; }

        public string NombreComercial { get; set; }

        public string PrincipioActivo { get; set; }

        public string Concentracion { get; set; }

        public string Formato { get; set; }

        public string Fabricante { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        // RELACIONES: Un medicamento puede estar en muchas prescripciones
        public virtual ICollection<Prescripcion> Prescripciones { get; set; }
    }
}
