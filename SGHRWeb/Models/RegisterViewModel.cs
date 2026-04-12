using System.ComponentModel.DataAnnotations;

namespace SGHRWeb.Models
{
    /// <summary>
    /// Extiende UserRegistration con Apellido y Teléfono para el SRS §3.2.
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Ingresa un número de teléfono válido.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Debe contener al menos una mayúscula, una minúscula y un número.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
