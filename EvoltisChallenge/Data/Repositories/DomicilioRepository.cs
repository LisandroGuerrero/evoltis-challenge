using EvoltisChallenge.Data.Repositories.Interfaces;
using EvoltisChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace EvoltisChallenge.Data.Repositories
{
    public class DomicilioRepository : Repository<Domicilio>, IDomicilioRepository
    {
        public DomicilioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Domicilio> CreateAsync(Domicilio domicilio)
        {
            domicilio.FechaCreacion = DateTime.UtcNow;
            return await AddAsync(domicilio);
        }

        public async Task<bool> DeleteByUsuarioIdAsync(int usuarioId)
        {
            var domicilio = await GetByUsuarioIdAsync(usuarioId);
            if (domicilio == null)
                return false;

            _dbSet.Remove(domicilio);
            await SaveChangesAsync();
            return true;
        }

        public async Task<Domicilio?> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet.Include(d => d.Usuario).FirstOrDefaultAsync(d => d.UsuarioID == usuarioId);
        }

        public async Task<Domicilio> UpdateAsync(Domicilio domicilio)
        {
            return await base.UpdateAsync(domicilio);
        }
    }
}
