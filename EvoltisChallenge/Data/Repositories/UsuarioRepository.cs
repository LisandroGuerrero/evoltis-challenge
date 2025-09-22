using EvoltisChallenge.Data;
using EvoltisChallenge.Data.Repositories;
using EvoltisChallenge.Data.Repositories.Interfaces;
using EvoltisChallenge.DTOs;
using EvoltisChallenge.Models;
using Microsoft.EntityFrameworkCore;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(u => u.Domicilio).FirstOrDefaultAsync(u => u.ID == id);
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        usuario.FechaCreacion = DateTime.UtcNow;

        if (usuario.Domicilio != null)
            usuario.Domicilio.FechaCreacion = DateTime.UtcNow;

        return await AddAsync(usuario);
    }
    public async Task<bool> EmailExistsAsync(string email, int? excluirIdUsuario = null)
    {
        var query = _dbSet.Where(u => u.Email == email);

        if (excluirIdUsuario.HasValue)
        {
            query = query.Where(u => u.ID != excluirIdUsuario.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet.Include(u => u.Domicilio).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<Usuario>> SearchAsync(SearchUsuarioDto searchDto)
    {
        var query = _dbSet.Include(u => u.Domicilio).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.Nombre))
            query = query.Where(u => u.Nombre.Contains(searchDto.Nombre));
        if (!string.IsNullOrWhiteSpace(searchDto.Provincia))
            query = query.Where(u => u.Domicilio != null && u.Domicilio.Provincia.Contains(searchDto.Provincia));
        if (!string.IsNullOrWhiteSpace(searchDto.Ciudad))
            query = query.Where(u => u.Domicilio != null && u.Domicilio.Ciudad.Contains(searchDto.Ciudad));

        return await query.ToListAsync();
    }
    public async Task<Usuario> UpdateAsync(Usuario usuario)
    {
        return await base.UpdateAsync(usuario);
    }
}