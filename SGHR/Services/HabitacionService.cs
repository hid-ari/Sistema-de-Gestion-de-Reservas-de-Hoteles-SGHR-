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
            OperationResult result = new OperationResult();
            if (habitacion == null)
            {
                result.IsSuccess = false;
                result.Message = "La habitación no puede ser nula.";
                return result;
            }

            habitacion.Id = Guid.NewGuid();
            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();
            result.IsSuccess = true;
            result.Message = "Habitación creada exitosamente.";
            result.Data = habitacion;
            return result;

        }

        public async Task<OperationResult> DeleteHabitacionAsync(Guid id)
        {
            OperationResult result = new OperationResult();
            if (id == Guid.Empty)
            {
                result.IsSuccess = false;
                result.Message = "El ID no puede ser vacío.";
                return result;
            }
            Habitacion? habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null)
            {
                result.IsSuccess = false;
                result.Message = "Habitación no encontrada.";
                return result;
            }
            
            if (habitacion.Estado == EstadoHabitacion.Reservada)
            {
                result.IsSuccess = false;
                result.Message = "No se puede eliminar una habitación reservada.";
                return result;
            }

            result.IsSuccess = true;
            result.Message = "Habitación eliminada exitosamente.";
            return result;

        }

        public async Task<OperationResult> GetAllHabitacionesAsync()
        {
            OperationResult result = new OperationResult();
            result.Data = await _context.Habitaciones.ToListAsync();
            return result;
        }

        public async Task<OperationResult> GetHabitacionByIdAsync(Guid id)
        {
            OperationResult result = new OperationResult();
            if (id == Guid.Empty)
            {
                result.IsSuccess = false;
                result.Message = "El ID no puede ser vacío.";
                return result;
            }
            
            Habitacion? habitacion = await _context.Habitaciones.FindAsync(id);

            if (habitacion == null)
            {
                result.IsSuccess = false;
                result.Message = "Habitación no encontrada.";
                return result;
            }

            result.IsSuccess = true;
            result.Data = habitacion;
            return result;

        }

        public async Task<OperationResult> UpdateHabitacionAsync(Guid id, Habitacion habitacion)
        {
            OperationResult result = new OperationResult();
            if (id == Guid.Empty)
            {
                result.IsSuccess = false;
                result.Message = "El ID no puede ser vacío.";
                return result;
            }

            if (habitacion == null)
            {
                result.IsSuccess = false;
                result.Message = "La habitación no puede ser nula.";
                return result;
            }

            Habitacion? existingHabitacion = await _context.Habitaciones.FindAsync(id);
            if (existingHabitacion == null)
            {
                result.IsSuccess = false;
                result.Message = "Habitación no encontrada.";
                return result;
            }

            existingHabitacion.Numero = habitacion.Numero;
            existingHabitacion.TipoHabitacionId = habitacion.TipoHabitacionId;
            existingHabitacion.Precio = habitacion.Precio;
            existingHabitacion.Piso = habitacion.Piso;
            existingHabitacion.Estado = habitacion.Estado;
            existingHabitacion.Descripcion = habitacion.Descripcion;
            existingHabitacion.TieneMiniBar = habitacion.TieneMiniBar;
            
            await _context.SaveChangesAsync();
            
            result.IsSuccess = true;
            result.Message = "Habitación actualizada exitosamente.";
            result.Data = habitacion;
            return result;

        }
    }
}
