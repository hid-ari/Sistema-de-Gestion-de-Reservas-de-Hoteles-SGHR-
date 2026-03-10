namespace SGHR.Data.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Email { get; set; }
        public required string Telefono { get; set; }
        public required string Contrasena { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
