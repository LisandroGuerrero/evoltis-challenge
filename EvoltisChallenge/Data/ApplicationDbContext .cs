using EvoltisChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace EvoltisChallenge.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Domicilio> Domicilios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.ID);
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.FechaCreacion).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Domicilio>(entity =>
            {
                entity.HasKey(d => d.ID);
                entity.Property(d => d.Calle).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Numero).IsRequired().HasMaxLength(20);
                entity.Property(d => d.Provincia).IsRequired().HasMaxLength(50);
                entity.Property(d => d.Ciudad).IsRequired().HasMaxLength(50);
                entity.Property(d => d.FechaCreacion).IsRequired();

                entity.HasOne(d => d.Usuario)
                      .WithOne(u => u.Domicilio)
                      .HasForeignKey<Domicilio>(d => d.UsuarioID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(d => d.UsuarioID).IsUnique();
            });
        }
    }
}
