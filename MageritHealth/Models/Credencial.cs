using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("CREDENCIALES")]
    public class Credencial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCredencial { get; set; }

        public int IdUsuario { get; set; }

        public byte[] PasswordHash { get; set; }

        [MaxLength(50)]
        public string Salt { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}
