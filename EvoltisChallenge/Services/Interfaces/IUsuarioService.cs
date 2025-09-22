using EvoltisChallenge.DTOs;

namespace EvoltisChallenge.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<UsuarioResponseDto>> SearchAsync(SearchUsuarioDto searchDto);
        Task<UsuarioResponseDto> CreateAsync(UsuarioDto createDto);
        Task<UsuarioResponseDto> UpdateAsync(int id, UpdateUsuarioDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}
