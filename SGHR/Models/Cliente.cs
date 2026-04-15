using System.ComponentModel.DataAnnotations;

namespace SGHR.Data.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // Id del usuario autenticado (Identity)

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Telefono { get; set; } = string.Empty;
    }
}
