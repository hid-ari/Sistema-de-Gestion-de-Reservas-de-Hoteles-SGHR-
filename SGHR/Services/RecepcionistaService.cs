using Microsoft.EntityFrameworkCore;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Helpers;
using SGHR.Data.Models;
using System.Security.Cryptography;
using System.Text;

namespace SGHR.Data.Services
{
    public class RecepcionistaService
    {
        private readonly SGHRContext _context;

        public RecepcionistaService(SGHRContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> CrearRecepcionistaAsync(
            string nombreCompleto,
            string idEmpleado,
            string turno,
            string emailCorporativo,
            string password,
            string rolSolicitado,
            string adminId)
        {

            if (!PasswordValidator.EsPasswordFuerte(password))
                return OperationResult.Failure("La contraseña debe tener mínimo 8 caracteres, mayúsculas, números y símbolos.");

            if (!emailCorporativo.EndsWith("@hotel.com"))
                return OperationResult.Failure("El email debe pertenecer al dominio del hotel.");

            if (rolSolicitado == "Administrador")
                return OperationResult.Failure("No se permite autoasignarse permisos de administrador.");

            var existe = await _context.Recepcionistas
                .AnyAsync(r => r.IdEmpleado == idEmpleado);

            if (existe)
                return OperationResult.Failure("El ID de empleado ya existe.");

            var recepcionista = new Recepcionista
            {
                NombreCompleto = nombreCompleto,
                IdEmpleado = idEmpleado,
                Turno = turno,
                EmailCorporativo = emailCorporativo,
                Rol = "Recepcionista",
                PasswordHash = HashPassword(password),
                EsActivo = true
            };

            _context.Recepcionistas.Add(recepcionista);

            await RegistrarLog(adminId, $"Creó recepcionista {idEmpleado}");

            await _context.SaveChangesAsync();

            return OperationResult.Success("Recepcionista registrado correctamente.", recepcionista);
        }

        private async Task RegistrarLog(string idEmpleado, string accion)
        {
            var log = new LogAccion
            {
                IdEmpleado = idEmpleado,
                Accion = accion
            };

            _context.LogAcciones.Add(log);
            await _context.SaveChangesAsync();
        }

        private string HashPassword(string password)
        {
            using SHA256 sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}