using System.ComponentModel.DataAnnotations;

namespace SGHR.Data.Models
{
    /// <summary>
    /// Representa una reserva de habitación en el hotel.
    /// SRS §3.1 – Gestión de Reservas.
    /// </summary>
    public class Reserva
    {
        public int Id { get; set; }

        /// <summary>
        /// Número único generado automáticamente al confirmar la reserva (SRS RF3).
        /// </summary>
        public Guid NumeroReserva { get; set; }

        /// <summary>Id del cliente que realiza la reserva (SRS Val.5).</summary>
        [Required]
        public int ClienteId { get; set; }

        /// <summary>Categoría de habitación solicitada (SRS RF1).</summary>
        [Required]
        public int CategoriaHabitacionId { get; set; }

        /// <summary>Fecha de entrada — no puede ser anterior al día actual (SRS Val.1).</summary>
        [Required]
        public DateTime FechaEntrada { get; set; }

        /// <summary>Fecha de salida — debe ser posterior a FechaEntrada (SRS Val.1).</summary>
        [Required]
        public DateTime FechaSalida { get; set; }

        /// <summary>
        /// Número de huéspedes — no puede exceder la capacidad de la categoría (SRS Val.4).
        /// </summary>
        [Required]
        public int NumeroHuespedes { get; set; }

        /// <summary>Estado actual de la reserva (SRS RF6).</summary>
        public EstadoReserva Estado { get; set; } = EstadoReserva.Confirmada;

        /// <summary>Fecha y hora en que se creó la reserva.</summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>Observación o motivo en caso de cancelación (SRS RF7).</summary>
        [MaxLength(500)]
        public string? ObservacionCancelacion { get; set; }
    }

    /// <summary>
    /// Estados posibles de una reserva según el SRS §3.1 RF6.
    /// </summary>
    public enum EstadoReserva
    {
        Confirmada,
        Cancelada,
        EnEspera,
        CheckIn,
        CheckOut
    }
}
