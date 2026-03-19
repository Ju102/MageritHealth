using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("NOTIFICACIONES")]
    public class Notificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdNotificacion { get; set; }

        public int IdUsuario { get; set; }

        public string Titulo { get; set; }

        public string Mensaje { get; set; }

        public string Tipo { get; set; } // 'info', 'alerta', 'cita', 'analitica', 'receta'

        public string? EnlaceAccion { get; set; }

        public bool Leido { get; set; }

        public DateTime FechaCreacion { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}