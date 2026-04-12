using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;

namespace SGHR.Data.Services
{
    /// <summary>
    /// Servicio de Gestión de Reservas.
    /// SRS §3.1 – Requerimientos Funcionales y Validaciones.
    /// </summary>
    public sealed class ReservaService : IReservaService
    {
        private readonly SGHRContext _context;

        public ReservaService(SGHRContext context)
        {
            _context = context;
        }

        // Obtener todas las reservas
        public async Task<OperationResult> GetAllReservasAsync()
        {
            var reservas = await _context.Reservas
                .AsNoTracking()
                .ToListAsync();

            return new OperationResult { IsSuccess = true, Data = reservas };
        }

        // Obtener reserva por Id
        public async Task<OperationResult> GetReservaByIdAsync(int id)
        {
            if (id <= 0)
                return new OperationResult { IsSuccess = false, Message = "El ID debe ser un valor positivo." };

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
                return new OperationResult { IsSuccess = false, Message = "Reserva no encontrada." };

            return new OperationResult { IsSuccess = true, Data = reserva };
        }

        // Historial de reservas por cliente (SRS §3.5 RF1 / §3.2 RF4)
        public async Task<OperationResult> GetReservasByClienteIdAsync(int clienteId)
        {
            if (clienteId <= 0)
                return new OperationResult { IsSuccess = false, Message = "El ID del cliente debe ser un valor positivo." };

            var reservas = await _context.Reservas
                .AsNoTracking()
                .Where(r => r.ClienteId == clienteId)
                .ToListAsync();

            if (!reservas.Any())
                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "El cliente no tiene reservas registradas.",
                    Data = reservas
                };

            return new OperationResult { IsSuccess = true, Data = reservas };
        }

        // RF1, RF2, RF3, RF6 – Crear y confirmar una nueva reserva
        public async Task<OperationResult> CreateReservaAsync(Reserva reserva)
        {
            try
            {
                if (reserva == null)
                    return new OperationResult { IsSuccess = false, Message = "La reserva no puede ser nula." };

                // Val.1 – Fecha de entrada no puede ser anterior a la fecha actual
                if (reserva.FechaEntrada.Date < DateTime.UtcNow.Date)
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "La fecha de entrada no puede ser anterior a la fecha actual."
                    };

                // Val.1 – Fecha de salida debe ser posterior a la fecha de entrada
                if (reserva.FechaSalida <= reserva.FechaEntrada)
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "La fecha de salida debe ser posterior a la fecha de entrada."
                    };

                // Val.4 – Número de huéspedes debe ser mayor que 0
                if (reserva.NumeroHuespedes <= 0)
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "El número de huéspedes debe ser mayor que cero."
                    };

                // Val.5 – El cliente debe ser válido
                if (reserva.ClienteId <= 0)
                    return new OperationResult { IsSuccess = false, Message = "El ID del cliente no es válido." };

                // Val.2, Val.3 – Verificar disponibilidad: no conflicto de fechas en la misma categoría
                // (estados activos: Confirmada, EnEspera, CheckIn)
                bool hayConflicto = await _context.Reservas.AnyAsync(r =>
                    r.CategoriaHabitacionId == reserva.CategoriaHabitacionId &&
                    r.Estado != EstadoReserva.Cancelada &&
                    r.Estado != EstadoReserva.CheckOut &&
                    r.FechaEntrada < reserva.FechaSalida &&
                    r.FechaSalida > reserva.FechaEntrada);

                if (hayConflicto)
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "No hay disponibilidad para la categoría seleccionada en las fechas indicadas."
                    };

                // RF3 – Generar número único de reserva
                reserva.NumeroReserva = Guid.NewGuid();

                // RF6 – Estado inicial: Confirmada
                reserva.Estado = EstadoReserva.Confirmada;
                reserva.FechaCreacion = DateTime.UtcNow;

                await _context.Reservas.AddAsync(reserva);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Reserva creada exitosamente.",
                    Data = reserva
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error inesperado al crear la reserva: {ex.Message}"
                };
            }
        }

        // Modificar datos de una reserva existente
        public async Task<OperationResult> UpdateReservaAsync(int id, Reserva reserva)
        {
            if (id <= 0)
                return new OperationResult { IsSuccess = false, Message = "El ID debe ser un valor positivo." };

            if (reserva == null)
                return new OperationResult { IsSuccess = false, Message = "La reserva no puede ser nula." };

            var existingReserva = await _context.Reservas.FindAsync(id);
            if (existingReserva == null)
                return new OperationResult { IsSuccess = false, Message = "Reserva no encontrada." };

            if (existingReserva.Estado == EstadoReserva.Cancelada)
                return new OperationResult { IsSuccess = false, Message = "No se puede modificar una reserva cancelada." };

            // Val.1 – Validar fechas al actualizar
            if (reserva.FechaEntrada.Date < DateTime.UtcNow.Date)
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "La fecha de entrada no puede ser anterior a la fecha actual."
                };

            if (reserva.FechaSalida <= reserva.FechaEntrada)
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "La fecha de salida debe ser posterior a la fecha de entrada."
                };

            if (reserva.NumeroHuespedes <= 0)
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "El número de huéspedes debe ser mayor que cero."
                };

            existingReserva.CategoriaHabitacionId = reserva.CategoriaHabitacionId;
            existingReserva.FechaEntrada = reserva.FechaEntrada;
            existingReserva.FechaSalida = reserva.FechaSalida;
            existingReserva.NumeroHuespedes = reserva.NumeroHuespedes;

            await _context.SaveChangesAsync();

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Reserva actualizada exitosamente.",
                Data = existingReserva
            };
        }

        // RF5, RF6, RF7 – Cancelar una reserva con registro de auditoría
        // Val.6 – Validar que no esté ya cancelada
        public async Task<OperationResult> CancelarReservaAsync(int id, string? observacion)
        {
            if (id <= 0)
                return new OperationResult { IsSuccess = false, Message = "El ID debe ser un valor positivo." };

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
                return new OperationResult { IsSuccess = false, Message = "Reserva no encontrada." };

            // Val.6 – No cancelar una reserva ya cancelada
            if (reserva.Estado == EstadoReserva.Cancelada)
                return new OperationResult { IsSuccess = false, Message = "La reserva ya se encuentra cancelada." };

            // RF7 – Registrar observación para auditoría
            reserva.Estado = EstadoReserva.Cancelada;
            reserva.ObservacionCancelacion = observacion ?? "Cancelada por el sistema.";

            await _context.SaveChangesAsync();

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Reserva cancelada exitosamente.",
                Data = reserva
            };
        }
    }
}
