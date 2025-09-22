using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvoltisChallenge.Models
{
    public class Domicilio
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(100)]
        public string Calle { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Provincia { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Ciudad { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}
