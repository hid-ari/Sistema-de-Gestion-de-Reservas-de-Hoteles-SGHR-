using System.ComponentModel.DataAnnotations;

namespace SGHR.Data.Models
{
    /// <summary>
    /// Representa una categoría de habitación del hotel.
    /// SRS §3.3 – Gestión de Categorías de Habitaciones.
    /// </summary>
    public class CategoriaHabitacion
    {
        public int Id { get; set; }

        /// <summary>Nombre único de la categoría (ej. "Suite", "Estándar").</summary>
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Descripción general de la categoría.</summary>
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>Tarifa base por noche. Debe ser mayor que 0 (SRS Val.2).</summary>
        [Required]
        public decimal TarifaPorNoche { get; set; }

        /// <summary>Características relevantes: camas, vista, amenities, etc.</summary>
        [MaxLength(500)]
        public string Caracteristicas { get; set; } = string.Empty;

        /// <summary>Indica si la categoría está activa en el sistema.</summary>
        public bool Activa { get; set; } = true;
    }
}
