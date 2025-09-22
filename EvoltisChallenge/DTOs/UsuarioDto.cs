using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EvoltisChallenge.DTOs
{
    public class UsuarioDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        public DomicilioDto? Domicilio { get; set; }
    }

    public class UpdateUsuarioDto
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        public DomicilioDto? Domicilio { get; set; }
    }

    public class UsuarioResponseDto
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DomicilioResponseDto? Domicilio { get; set; }
    }

    public class SearchUsuarioDto
    {
        public string? Nombre { get; set; }
        public string? Provincia { get; set; }
        public string? Ciudad { get; set; }
    }
}
