using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHR.Data.Abstraction;
using SGHR.Data.Models;

namespace SGHRWeb.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly IReservaService _reservaService;
        private readonly ICategoriaHabitacionService _categoriaService;

        public ReservasController(IReservaService reservaService, ICategoriaHabitacionService categoriaService)
        {
            _reservaService = reservaService;
            _categoriaService = categoriaService;
        }

        // GET: /Reservas
        public async Task<IActionResult> Index()
        {
            var result = await _reservaService.GetAllReservasAsync();
            var lista = result.Data as List<Reserva> ?? new List<Reserva>();
            return View(lista);
        }

        // GET: /Reservas/Create
        public async Task<IActionResult> Create()
        {
            await CargarCategoriasViewBag();
            return View(new Reserva { FechaEntrada = DateTime.Today.AddDays(1), FechaSalida = DateTime.Today.AddDays(2) });
        }

        // POST: /Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasViewBag();
                return View(reserva);
            }

            var result = await _reservaService.CreateReservaAsync(reserva);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await CargarCategoriasViewBag();
                return View(reserva);
            }

            TempData["Success"] = $"Reserva creada. Número: {reserva.NumeroReserva}";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Reservas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _reservaService.GetReservaByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data as Reserva);
        }

        // GET: /Reservas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _reservaService.GetReservaByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            await CargarCategoriasViewBag();
            return View(result.Data as Reserva);
        }

        // POST: /Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasViewBag();
                return View(reserva);
            }

            var result = await _reservaService.UpdateReservaAsync(id, reserva);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await CargarCategoriasViewBag();
                return View(reserva);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Reservas/Cancelar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id, string? observacion)
        {
            var result = await _reservaService.CancelarReservaAsync(id, observacion);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCategoriasViewBag()
        {
            var result = await _categoriaService.GetAllCategoriasAsync();
            ViewBag.Categorias = result.Data as List<CategoriaHabitacion> ?? new List<CategoriaHabitacion>();
        }
    }
}
