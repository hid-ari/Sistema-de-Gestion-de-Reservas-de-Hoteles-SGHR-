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
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<CategoriaHabitacion> CategoriasHabitacion { get; set; }
    }
}
