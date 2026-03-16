using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("TIPOS_MEDICIONES")]
    public class TipoMedicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTipoMedicion { get; set; }

        public string NombreMedicion { get; set; }

        public string UnidadMedicion { get; set; }

        [Column("ValorMaximo", TypeName = "decimal(10,2)")]
        public decimal ValorMaximo { get; set; }

        [Column("ValorMinimo", TypeName = "decimal(10,2)")]
        public decimal ValorMinimo { get; set; }

        public bool Activo { get; set; }

        // RELACIONES: Un tipo de medición tiene muchas mediciones registradas
        public virtual ICollection<Medicion> Mediciones { get; set; }
    }
}
