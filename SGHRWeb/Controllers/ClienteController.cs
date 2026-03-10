using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGHR.Data.Context;
using SGHR.Data.Models;
using SGHRWeb.Models;

namespace SGHRWeb.Controllers
{
    public class ClienteController : Controller
    {
        private readonly SGHRDbContext _context;

        public ClienteController(SGHRDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroClienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el email ya existe
                var emailExiste = await _context.Clientes
                    .AnyAsync(c => c.Email.ToLower() == model.Email.ToLower());

                if (emailExiste)
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado en el sistema");
                    return View(model);
                }

                // Crear nuevo cliente
                var cliente = new Cliente
                {
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Email = model.Email,
                    Telefono = model.Telefono,
                    Contrasena = model.Contrasena, // En producción, debe hashearse
                    FechaRegistro = DateTime.Now
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registrado exitosamente";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
