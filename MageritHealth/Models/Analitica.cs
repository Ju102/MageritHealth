using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("ANALITICAS")]
    public class Analitica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAnalitica { get; set; }

        public int IdCita { get; set; }

        public DateTime FechaAnalitica { get; set; }

        public string Estado { get; set; }

        public string? Notas { get; set; }

        [ForeignKey("IdCita")]
        public virtual Cita Cita { get; set; }
        public virtual ICollection<Medicion> Mediciones { get; set; }
    }
}
