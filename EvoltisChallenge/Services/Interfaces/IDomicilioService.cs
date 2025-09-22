using EvoltisChallenge.DTOs;

namespace EvoltisChallenge.Services.Interfaces
{
    public interface IDomicilioService
    {
        Task<DomicilioResponseDto?> GetByUsuarioIdAsync(int usuarioId);
        Task<DomicilioResponseDto> CreateAsync(int usuarioId, DomicilioDto domicilioDto);
        Task<DomicilioResponseDto> UpdateAsync(int usuarioId, DomicilioDto domicilioDto);
        Task<bool> DeleteAsync(int usuarioId);
        void ValidarDomicilioCompleto(DomicilioDto domicilioDto);
    }
}
