using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGHR.Data.Models;

namespace SGHR.Data.Context
{
    public class SGHRContext : DbContext
    {
        public SGHRContext(DbContextOptions<SGHRContext> options) : base(options)
        {
        }

        public DbSet<Habitacion> Habitaciones { get; set; }

        public DbSet<Recepcionista> Recepcionistas { get; set; }

        public DbSet<LogAccion> LogAcciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recepcionista>()
                .HasIndex(r => r.IdEmpleado)
                .IsUnique();

            modelBuilder.Entity<Recepcionista>()
                .HasIndex(r => r.EmailCorporativo)
                .IsUnique();
        }
    }
}