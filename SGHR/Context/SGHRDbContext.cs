using Microsoft.EntityFrameworkCore;
using SGHR.Data.Models;

namespace SGHR.Data.Context
{
    public class SGHRDbContext : DbContext
    {
        public SGHRDbContext(DbContextOptions<SGHRDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(25);
                entity.Property(e => e.Contrasena).IsRequired().HasMaxLength(30);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}
