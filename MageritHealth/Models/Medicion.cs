using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace MageritHealth.Models
{
    [Table("MEDICIONES")]
    public class Medicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdMedicion { get; set; }

        public int IdTipoMedicion { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ValorMedicion { get; set; }

        public int? IdAnalitica { get; set; }

        public int? IdCita { get; set; }

        // Relaciones con otras tablas
        [ForeignKey("IdTipoMedicion")]
        public virtual TipoMedicion TipoMedicion { get; set; }
        [ForeignKey("IdAnalitica")]
        public virtual Analitica Analitica { get; set; }
        [ForeignKey("IdCita")]
        public virtual Cita Cita { get; set; }
    }
}
