using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;

namespace SGHR.Data.Services
{
    public sealed class RegistroService : IRegistroService
    {
        private readonly SGHRContext _context;

        public RegistroService(SGHRContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> CreateRegistroAsync(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                {
                    return new OperationResult { IsSuccess = false, Message = "El cliente no puede ser nulo." };
                }

                await _context.Clientes.AddAsync(cliente);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Cliente creado exitosamente.",
                    Data = cliente
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error inesperado al crear el cliente: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult> DeleteRegistroAsync(int id)
        {
            if (id <= 0)
            {
                return new OperationResult { IsSuccess = false, Message = "El ID no puede ser cero." };
            }

            Cliente? cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return new OperationResult { IsSuccess = false, Message = "Cliente no encontrado." };
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return new OperationResult { IsSuccess = true, Message = "Cliente eliminado exitosamente." };
        }

        public async Task<OperationResult> GetAllRegistrosAsync()
        {
            var clientes = await _context.Clientes.AsNoTracking().ToListAsync();
            return new OperationResult { IsSuccess = true, Data = clientes };
        }

        public async Task<OperationResult> GetRegistroByIdAsync(int id)
        {
            if (id <= 0)
            {
                return new OperationResult { IsSuccess = false, Message = "El ID no puede ser cero." };
            }

            Cliente? cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return new OperationResult { IsSuccess = false, Message = "Cliente no encontrado." };
            }

            return new OperationResult { IsSuccess = true, Data = cliente };
        }

        public async Task<OperationResult> UpdateRegistroAsync(int id, Cliente cliente)
        {
            if (id <= 0)
            {
                return new OperationResult { IsSuccess = false, Message = "El ID no puede ser cero." };
            }

            if (cliente == null)
            {
                return new OperationResult { IsSuccess = false, Message = "El cliente no puede ser nulo." };
            }

            Cliente? existingCliente = await _context.Clientes.FindAsync(id);
            if (existingCliente == null)
            {
                return new OperationResult { IsSuccess = false, Message = "Cliente no encontrado." };
            }

            existingCliente.UserId = cliente.UserId;
            existingCliente.Nombre = cliente.Nombre;
            existingCliente.Apellido = cliente.Apellido;
            existingCliente.Email = cliente.Email;
            existingCliente.Telefono = cliente.Telefono;

            await _context.SaveChangesAsync();

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Cliente actualizado exitosamente.",
                Data = cliente
            };
        }
    }
}
