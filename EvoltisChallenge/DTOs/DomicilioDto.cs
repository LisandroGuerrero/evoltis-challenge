using System.ComponentModel.DataAnnotations;

namespace EvoltisChallenge.DTOs
{
    public class DomicilioDto
    {
        [Required(ErrorMessage = "La calle es requerida")]
        [StringLength(100, ErrorMessage = "La calle no puede exceder 100 caracteres")]
        public string Calle { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número es requerido")]
        [StringLength(20, ErrorMessage = "El número no puede exceder 20 caracteres")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La provincia es requerida")]
        [StringLength(50, ErrorMessage = "La provincia no puede exceder 50 caracteres")]
        public string Provincia { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es requerida")]
        [StringLength(50, ErrorMessage = "La ciudad no puede exceder 50 caracteres")]
        public string Ciudad { get; set; } = string.Empty;
    }

    public class DomicilioResponseDto
    {
        public int ID { get; set; }
        public string Calle { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}