using System.ComponentModel.DataAnnotations;

public class Cliente
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } // Id del usuario autenticado (Identity)

    [Required]
    public string Nombre { get; set; }

    [Required]
    public string Apellido { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string Telefono { get; set; }
}