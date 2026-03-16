using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;

namespace SGHR.Data.Services
{
    public sealed class HabitacionService : IHabitacionesService
    {
        private readonly SGHRContext _context;
        public HabitacionService(SGHRContext context)
        {
            _context = context;
        }
        public async Task<OperationResult> CreateHabitacionAsync(Habitacion habitacion)
        {
            if (habitacion == null)
            {
                return new OperationResult { IsSuccess = false, Message = "La habitación no puede ser nula." };
            }

            habitacion.Id = Guid.NewGuid();
            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();
            return new OperationResult { Data = habitacion, IsSuccess = true, Message = "Habitación creada exitosamente." };

        }

        public async Task<OperationResult> DeleteHabitacionAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new OperationResult { IsSuccess = false, Message = "El ID no puede ser vacío." };
            }
            Habitacion? habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null)
            {
                return new OperationResult { IsSuccess = false, Message = "Habitación no encontrada." };
            }
            
            if (habitacion.Estado == EstadoHabitacion.Reservada)
            {
                return new OperationResult { IsSuccess = false, Message = "No se puede eliminar una habitación reservada." };
            }
            _context.Habitaciones.Remove(habitacion);
            await _context.SaveChangesAsync();
            return new OperationResult { IsSuccess = true, Message = "Habitación eliminada exitosamente." };
        }

        public async Task<OperationResult> GetAllHabitacionesAsync()
        {
            var habitaciones = await _context.Habitaciones.AsNoTracking().ToListAsync();
            return new OperationResult { IsSuccess = true, Data = habitaciones};
        }

        public async Task<OperationResult> GetHabitacionByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new OperationResult { IsSuccess = false, Message = "El ID no puede ser vacío." };
            }
            
            Habitacion? habitacion = await _context.Habitaciones.FindAsync(id);

            if (habitacion == null)
            {
                return new OperationResult { IsSuccess = false, Message = "Habitación no encontrada." };
            }

            return new OperationResult { IsSuccess = true, Data = habitacion };
        }

        public async Task<OperationResult> UpdateHabitacionAsync(Guid id, Habitacion habitacion)
        {
            if (id == Guid.Empty)
            {
                return new OperationResult { IsSuccess = false, Message = "El ID no puede ser vacío." };
            }

            if (habitacion == null)
            {
                return new OperationResult { IsSuccess = false, Message = "La habitación no puede ser nula." };
            }

            Habitacion? existingHabitacion = await _context.Habitaciones.FindAsync(id);
            if (existingHabitacion == null)
            {
                return new OperationResult { IsSuccess = false, Message = "Habitación no encontrada." };
            }

            existingHabitacion.Numero = habitacion.Numero;
            existingHabitacion.TipoHabitacionId = habitacion.TipoHabitacionId;
            existingHabitacion.Precio = habitacion.Precio;
            existingHabitacion.Piso = habitacion.Piso;
            existingHabitacion.Estado = habitacion.Estado;
            existingHabitacion.Descripcion = habitacion.Descripcion;
            existingHabitacion.TieneMiniBar = habitacion.TieneMiniBar;
            
            await _context.SaveChangesAsync();
            
            return new OperationResult { IsSuccess = true, Message = "Habitación actualizada exitosamente.", Data = habitacion };
        }
    }
}
