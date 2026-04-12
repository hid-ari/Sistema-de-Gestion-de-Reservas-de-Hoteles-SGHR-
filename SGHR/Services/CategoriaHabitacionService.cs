using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;

namespace SGHR.Data.Services
{
    /// <summary>
    /// Servicio de Gestión de Categorías de Habitaciones.
    /// SRS §3.3 – Requerimientos Funcionales y Validaciones.
    /// </summary>
    public sealed class CategoriaHabitacionService : ICategoriaHabitacionService
    {
        private readonly SGHRContext _context;

        public CategoriaHabitacionService(SGHRContext context)
        {
            _context = context;
        }

        // RF1 – Ver listado de todas las categorías
        public async Task<OperationResult> GetAllCategoriasAsync()
        {
            var categorias = await _context.CategoriasHabitacion
                .AsNoTracking()
                .ToListAsync();

            return new OperationResult { IsSuccess = true, Data = categorias };
        }

        // RF1 – Obtener categoría por Id
        public async Task<OperationResult> GetCategoriaByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult { IsSuccess = false, Message = "El ID debe ser un valor positivo." };

            var categoria = await _context.CategoriasHabitacion.FindAsync(id);
            if (categoria == null)
                return new OperationResult { IsSuccess = false, Message = "Categoría no encontrada." };

            return new OperationResult { IsSuccess = true, Data = categoria };
        }

        // RF2 – Crear nueva categoría
        // Val.1: nombre no duplicado | Val.2: tarifa positiva | RF5: mensajes de error claros
        public async Task<OperationResult> CreateCategoriaAsync(CategoriaHabitacion categoria)
        {
            try
            {
                if (categoria == null)
                    return new OperationResult { IsSuccess = false, Message = "La categoría no puede ser nula." };

                if (string.IsNullOrWhiteSpace(categoria.Nombre))
                    return new OperationResult { IsSuccess = false, Message = "El nombre de la categoría es obligatorio." };

                // Val.2 – Tarifa por noche debe ser un número positivo
                if (categoria.TarifaPorNoche <= 0)
                    return new OperationResult { IsSuccess = false, Message = "La tarifa por noche debe ser un valor positivo." };

                // Val.1 – Nombre no duplicado en el sistema
                bool nombreDuplicado = await _context.CategoriasHabitacion
                    .AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower());

                if (nombreDuplicado)
                    return new OperationResult { IsSuccess = false, Message = "Ya existe una categoría con ese nombre." };

                await _context.CategoriasHabitacion.AddAsync(categoria);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Categoría creada exitosamente.",
                    Data = categoria
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error inesperado al crear la categoría: {ex.Message}"
                };
            }
        }

        // RF3 – Actualizar categoría existente
        // RF5: mensajes de error | Val.1: nombre no duplicado | Val.2: tarifa positiva
        public async Task<OperationResult> UpdateCategoriaAsync(int id, CategoriaHabitacion categoria)
        {
            if (id <= 0)
                return new OperationResult { IsSuccess = false, Message = "El ID debe ser un valor positivo." };

            if (categoria == null)
                return new OperationResult { IsSuccess = false, Message = "La categoría no puede ser nula." };

            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return new OperationResult { IsSuccess = false, Message = "El nombre de la categoría es obligatorio." };

            // Val.2 – Tarifa positiva
            if (categoria.TarifaPorNoche <= 0)
                return new OperationResult { IsSuccess = false, Message = "La tarifa por noche debe ser un valor positivo." };

            var existingCategoria = await _context.CategoriasHabitacion.FindAsync(id);
            if (existingCategoria == null)
                return new OperationResult { IsSuccess = false, Message = "Categoría no encontrada." };

            // Val.1 – Nombre no duplicado (excluye la misma categoría)
            bool nombreDuplicado = await _context.CategoriasHabitacion
                .AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower() && c.Id != id);

            if (nombreDuplicado)
                return new OperationResult { IsSuccess = false, Message = "Ya existe una categoría con ese nombre." };

            existingCategoria.Nombre = categoria.Nombre;
            existingCategoria.Descripcion = categoria.Descripcion;
            existingCategoria.TarifaPorNoche = categoria.TarifaPorNoche;
            existingCategoria.Caracteristicas = categoria.Caracteristicas;
            existingCategoria.Activa = categoria.Activa;

            await _context.SaveChangesAsync();

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Categoría actualizada exitosamente.",
                Data = existingCategoria
            };
        }

        // RF4 – Eliminar categoría (solo si no tiene habitaciones asociadas)
        // Val.4 / RF6: no eliminar si tiene habitaciones asignadas
        public async Task<OperationResult> DeleteCategoriaAsync(int id)
        {
            if (id <= 0)
                return new OperationResult { IsSuccess = false, Message = "El ID debe ser un valor positivo." };

            var categoria = await _context.CategoriasHabitacion.FindAsync(id);
            if (categoria == null)
                return new OperationResult { IsSuccess = false, Message = "Categoría no encontrada." };

            // Val.4 – No eliminar si hay habitaciones asociadas (por TipoHabitacionId)
            bool tieneHabitaciones = await _context.Habitaciones
                .AnyAsync(h => h.TipoHabitacionId == id);

            if (tieneHabitaciones)
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "No se puede eliminar la categoría porque tiene habitaciones asociadas."
                };

            _context.CategoriasHabitacion.Remove(categoria);
            await _context.SaveChangesAsync();

            return new OperationResult { IsSuccess = true, Message = "Categoría eliminada exitosamente." };
        }
    }
}
