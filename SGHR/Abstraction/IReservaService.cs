using SGHR.Data.Base;
using SGHR.Data.Models;

namespace SGHR.Data.Abstraction
{
    /// <summary>
    /// Contrato del servicio de Gestión de Reservas.
    /// SRS §3.1 – Requerimientos Funcionales.
    /// </summary>
    public interface IReservaService
    {
        /// <summary>Obtiene todas las reservas registradas en el sistema.</summary>
        Task<OperationResult> GetAllReservasAsync();

        /// <summary>Obtiene una reserva por su identificador interno.</summary>
        Task<OperationResult> GetReservaByIdAsync(int id);

        /// <summary>RF historial – Obtiene todas las reservas asociadas a un cliente.</summary>
        Task<OperationResult> GetReservasByClienteIdAsync(int clienteId);

        /// <summary>
        /// RF1, RF2, RF3, RF6 – Crea y confirma una nueva reserva, validando
        /// disponibilidad, fechas y número de huéspedes.
        /// </summary>
        Task<OperationResult> CreateReservaAsync(Reserva reserva);

        /// <summary>Modifica los datos de una reserva existente.</summary>
        Task<OperationResult> UpdateReservaAsync(int id, Reserva reserva);

        /// <summary>
        /// RF5, RF6, RF7 – Cancela una reserva, validando las políticas
        /// y registrando la observación para auditoría.
        /// </summary>
        Task<OperationResult> CancelarReservaAsync(int id, string? observacion);
    }
}
