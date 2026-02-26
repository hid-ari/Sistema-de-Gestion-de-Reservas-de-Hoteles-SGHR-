using System.ComponentModel.DataAnnotations;


namespace SGHR.Data.Models
{
    public class Habitacion
    {
        public Guid Id { get; set; }
        [Required]
        public string? Numero { get; set; }
        public int TipoHabitacionId { get; set; }
        public decimal Precio { get; set; }
        public int Piso { get; set; }
        public EstadoHabitacion Estado { get; set; }
        public string? Descripcion { get; set; }
        public bool TieneMiniBar { get; set; }
    }

    public enum EstadoHabitacion
    {
        Disponible,
        Ocupada,
        EnMantenimiento,
        Limpieza
    }
}
