namespace SGHRWeb.Auth
{
    /// <summary>
    /// Almacén de usuarios en memoria para demo (SRS §3.2).
    /// En producción esto se reemplaza por EF Core + hash de contraseñas (BCrypt/Argon2).
    /// </summary>
    public class UserStore
    {
        public record UserRecord(
            string Id,
            string Nombre,
            string Apellido,
            string Email,
            string Telefono,
            string PasswordHash,   // SHA-256 en hex
            string Role            // "Admin" | "Recepcionista" | "Cliente"
        );

        private readonly List<UserRecord> _users = new()
        {
            // Cuenta de administrador predeterminada para demo
            new UserRecord(
                Guid.NewGuid().ToString(),
                "Admin",
                "SGHR",
                "admin@sghr.com",
                "8091234567",
                HashPassword("Admin123"),
                "Admin"
            )
        };

        public UserRecord? FindByEmail(string email) =>
            _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public bool EmailExists(string email) => FindByEmail(email) is not null;

        public UserRecord? ValidateCredentials(string email, string password)
        {
            var user = FindByEmail(email);
            if (user is null) return null;
            return user.PasswordHash == HashPassword(password) ? user : null;
        }

        public UserRecord Register(string nombre, string apellido, string email,
                                   string telefono, string password, string role = "Cliente")
        {
            var user = new UserRecord(
                Guid.NewGuid().ToString(),
                nombre, apellido, email, telefono,
                HashPassword(password),
                role
            );
            _users.Add(user);
            return user;
        }

        /// <summary>Hash SHA-256 simple para demo. NO usar en producción.</summary>
        public static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            return Convert.ToHexString(sha.ComputeHash(bytes));
        }
    }
}
