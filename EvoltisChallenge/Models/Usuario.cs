using System.ComponentModel.DataAnnotations;

namespace EvoltisChallenge.Models
{
    public class Usuario
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public Domicilio? Domicilio { get; set; }
    }
}
