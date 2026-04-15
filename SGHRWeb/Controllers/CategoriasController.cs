using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGHR.Data.Abstraction;
using SGHR.Data.Models;

namespace SGHRWeb.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaHabitacionService _categoriaService;

        public CategoriasController(ICategoriaHabitacionService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: /Categorias
        public async Task<IActionResult> Index()
        {
            var result = await _categoriaService.GetAllCategoriasAsync();
            var lista = result.Data as List<CategoriaHabitacion> ?? new List<CategoriaHabitacion>();
            return View(lista);
        }

        // GET: /Categorias/Create
        public IActionResult Create() => View(new CategoriaHabitacion());

        // POST: /Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaHabitacion categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            var result = await _categoriaService.CreateCategoriaAsync(categoria);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(categoria);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categorias/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoriaService.GetCategoriaByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data as CategoriaHabitacion);
        }

        // POST: /Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaHabitacion categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            var result = await _categoriaService.UpdateCategoriaAsync(id, categoria);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(categoria);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Categorias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoriaService.DeleteCategoriaAsync(id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
