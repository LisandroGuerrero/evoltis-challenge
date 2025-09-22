using EvoltisChallenge.Models;

namespace EvoltisChallenge.Data.Repositories.Interfaces
{
    public interface IDomicilioRepository
    {
        Task<Domicilio?> GetByUsuarioIdAsync(int usuarioId);
        Task<Domicilio> CreateAsync(Domicilio domicilio);
        Task<Domicilio> UpdateAsync(Domicilio domicilio);
        Task<bool> DeleteByUsuarioIdAsync(int usuarioId);
    }
}
