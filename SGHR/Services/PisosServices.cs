using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.IBase;
using SGHR.Data.Models;



using Microsoft.EntityFrameworkCore;
using SGHR.Data.IBase;

namespace SGHR.Data.Services
{
    public class PisosServices : IbaseService<Pisos>
    {
        private readonly SGHRContext _context;

        public PisosServices(SGHRContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> Save(Pisos entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                _context.Pisos.Add(entity);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Data = entity;
                result.Message = "Piso guardado correctamente";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error al guardar el piso: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> Update(Pisos entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                if (entity.PisosId == 0)
                {
                    result.IsSuccess = false;
                    result.Message = "El ID del piso no puede ser cero";
                    return result;
                }

                _context.Pisos.Update(entity);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Piso actualizado correctamente";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error al actualizar el piso: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> Delete(Pisos entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                if (entity.PisosId == 0)
                {
                    result.IsSuccess = false;
                    result.Message = "El piso no existe";
                    return result;
                }

                _context.Pisos.Remove(entity);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Piso eliminado correctamente";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error al eliminar el piso: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();

            try
            {
                var pisos = await _context.Pisos.ToListAsync();

                result.Data = pisos;
                result.IsSuccess = true;
                result.Message = "Pisos obtenidos correctamente";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error al obtener los pisos: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            OperationResult result = new OperationResult();

            try
            {
                var piso = await _context.Pisos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PisosId == id);

                if (piso == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Piso no encontrado";
                    return result;
                }

                result.Data = piso;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Error al obtener el piso: {ex.Message}";
            }

            return result;
        }
    }
}
