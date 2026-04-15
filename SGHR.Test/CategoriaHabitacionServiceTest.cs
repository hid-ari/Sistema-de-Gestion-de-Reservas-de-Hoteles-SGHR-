using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;
using SGHR.Data.Services;

namespace SGHR.Test
{
    /// <summary>
    /// Tests del servicio de Gestión de Categorías de Habitaciones.
    /// Trazabilidad: SRS §3.3 – Requerimientos Funcionales y Validaciones.
    /// </summary>
    public class CategoriaHabitacionServiceTest
    {
        private readonly ICategoriaHabitacionService _categoriaService;
        private readonly SGHRContext _context;

        public CategoriaHabitacionServiceTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // BD aislada por test
                .Options;

            _context = new SGHRContext(options);
            _categoriaService = new CategoriaHabitacionService(_context);
        }

        // ──────────────────────────────────────────────────────────────────────
        // GET ALL – RF1
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF1 – El sistema debe mostrar el listado de categorías disponibles.</summary>
        [Fact]
        public async Task GetAllCategorias_ShouldSucceed_WhenCategoriasExisten()
        {
            // Arrange
            await _context.CategoriasHabitacion.AddRangeAsync(
                new CategoriaHabitacion { Id = 1, Nombre = "Estándar", TarifaPorNoche = 80m, Activa = true },
                new CategoriaHabitacion { Id = 2, Nombre = "Suite", TarifaPorNoche = 200m, Activa = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaService.GetAllCategoriasAsync();

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            var lista = result.Data as List<CategoriaHabitacion>;
            Assert.NotNull(lista);
            Assert.Equal(2, lista.Count);
        }

        // ──────────────────────────────────────────────────────────────────────
        // GET BY ID – RF1, RF5
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF1 – Obtener categoría existente por Id.</summary>
        [Fact]
        public async Task GetCategoriaById_ShouldSucceed_WhenFound()
        {
            // Arrange
            var categoria = new CategoriaHabitacion { Id = 10, Nombre = "Deluxe", TarifaPorNoche = 150m, Activa = true };
            await _context.CategoriasHabitacion.AddAsync(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaService.GetCategoriaByIdAsync(10);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        /// <summary>RF5 – Error claro cuando la categoría no existe.</summary>
        [Fact]
        public async Task GetCategoriaById_ShouldFail_WhenNotFound()
        {
            // Act
            var result = await _categoriaService.GetCategoriaByIdAsync(9999);
            string expectedMessage = "Categoría no encontrada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Validación de ID – Debe fallar cuando el ID no es positivo.</summary>
        [Fact]
        public async Task GetCategoriaById_ShouldFail_WhenIdEsCero()
        {
            // Act
            var result = await _categoriaService.GetCategoriaByIdAsync(0);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
        }

        // ──────────────────────────────────────────────────────────────────────
        // CREATE – RF2, RF5, Val.1, Val.2
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF2 – Crear categoría válida con nombre, descripción, tarifa y características.</summary>
        [Fact]
        public async Task CreateCategoria_ShouldSucceed_WhenEsValida()
        {
            // Arrange
            var categoria = new CategoriaHabitacion
            {
                Nombre = "Junior Suite",
                Descripcion = "Habitación amplia con sala de estar",
                TarifaPorNoche = 175m,
                Caracteristicas = "Vista al mar, jacuzzi, minibar",
                Activa = true
            };

            // Act
            var result = await _categoriaService.CreateCategoriaAsync(categoria);
            string expectedMessage = "Categoría creada exitosamente.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
            Assert.NotNull(result.Data);
        }

        /// <summary>RF5 – Error claro cuando se pasa un objeto nulo.</summary>
        [Fact]
        public async Task CreateCategoria_ShouldFail_WhenEsNula()
        {
            // Arrange
            CategoriaHabitacion categoria = null!;

            // Act
            var result = await _categoriaService.CreateCategoriaAsync(categoria);
            string expectedMessage = "La categoría no puede ser nula.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
            Assert.Null(result.Data);
        }

        /// <summary>Val.1 – Nombre duplicado debe ser rechazado.</summary>
        [Fact]
        public async Task CreateCategoria_ShouldFail_WhenNombreEsDuplicado()
        {
            // Arrange – Insertar categoría existente
            await _context.CategoriasHabitacion.AddAsync(
                new CategoriaHabitacion { Id = 20, Nombre = "Presidencial", TarifaPorNoche = 500m, Activa = true }
            );
            await _context.SaveChangesAsync();

            var nuevaCategoria = new CategoriaHabitacion
            {
                Nombre = "Presidencial", // nombre duplicado
                TarifaPorNoche = 600m,
                Activa = true
            };

            // Act
            var result = await _categoriaService.CreateCategoriaAsync(nuevaCategoria);
            string expectedMessage = "Ya existe una categoría con ese nombre.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.2 – Tarifa negativa o cero debe ser rechazada.</summary>
        [Fact]
        public async Task CreateCategoria_ShouldFail_WhenTarifaEsNegativa()
        {
            // Arrange
            var categoria = new CategoriaHabitacion
            {
                Nombre = "Económica",
                TarifaPorNoche = -50m, // inválida
                Activa = true
            };

            // Act
            var result = await _categoriaService.CreateCategoriaAsync(categoria);
            string expectedMessage = "La tarifa por noche debe ser un valor positivo.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.2 – Tarifa igual a cero también debe ser rechazada.</summary>
        [Fact]
        public async Task CreateCategoria_ShouldFail_WhenTarifaEsCero()
        {
            // Arrange
            var categoria = new CategoriaHabitacion
            {
                Nombre = "Gratuita",
                TarifaPorNoche = 0m,
                Activa = true
            };

            // Act
            var result = await _categoriaService.CreateCategoriaAsync(categoria);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
        }

        // ──────────────────────────────────────────────────────────────────────
        // UPDATE – RF3, RF5, Val.1, Val.2
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF3 – Actualizar categoría existente con datos válidos.</summary>
        [Fact]
        public async Task UpdateCategoria_ShouldSucceed_WhenEsValida()
        {
            // Arrange
            var categoria = new CategoriaHabitacion { Id = 30, Nombre = "Familiar", TarifaPorNoche = 120m, Activa = true };
            await _context.CategoriasHabitacion.AddAsync(categoria);
            await _context.SaveChangesAsync();

            var categoriaActualizada = new CategoriaHabitacion
            {
                Nombre = "Familiar Plus",
                Descripcion = "Habitación familiar ampliada",
                TarifaPorNoche = 145m,
                Caracteristicas = "2 camas dobles, balcón",
                Activa = true
            };

            // Act
            var result = await _categoriaService.UpdateCategoriaAsync(30, categoriaActualizada);
            string expectedMessage = "Categoría actualizada exitosamente.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>RF5 – Error al actualizar una categoría que no existe.</summary>
        [Fact]
        public async Task UpdateCategoria_ShouldFail_WhenNotFound()
        {
            // Arrange
            var categoriaActualizada = new CategoriaHabitacion
            {
                Nombre = "Inexistente",
                TarifaPorNoche = 100m,
                Activa = true
            };

            // Act
            var result = await _categoriaService.UpdateCategoriaAsync(9999, categoriaActualizada);
            string expectedMessage = "Categoría no encontrada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Validación ID – Falla cuando el ID es 0 en Update.</summary>
        [Fact]
        public async Task UpdateCategoria_ShouldFail_WhenIdEsCero()
        {
            // Arrange
            var categoriaActualizada = new CategoriaHabitacion { Nombre = "X", TarifaPorNoche = 100m };

            // Act
            var result = await _categoriaService.UpdateCategoriaAsync(0, categoriaActualizada);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
        }

        // ──────────────────────────────────────────────────────────────────────
        // DELETE – RF4, RF6, Val.4
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF4 – Eliminar categoría sin habitaciones asociadas.</summary>
        [Fact]
        public async Task DeleteCategoria_ShouldSucceed_WhenNoTieneHabitaciones()
        {
            // Arrange
            var categoria = new CategoriaHabitacion { Id = 40, Nombre = "Penthouse", TarifaPorNoche = 800m, Activa = true };
            await _context.CategoriasHabitacion.AddAsync(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaService.DeleteCategoriaAsync(40);
            string expectedMessage = "Categoría eliminada exitosamente.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>RF6 – Error al eliminar categoría que no existe.</summary>
        [Fact]
        public async Task DeleteCategoria_ShouldFail_WhenNotFound()
        {
            // Act
            var result = await _categoriaService.DeleteCategoriaAsync(9999);
            string expectedMessage = "Categoría no encontrada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.4 / RF6 – No se puede eliminar una categoría con habitaciones asociadas.</summary>
        [Fact]
        public async Task DeleteCategoria_ShouldFail_WhenTieneHabitacionesAsociadas()
        {
            // Arrange – crear categoría y una habitación que la referencia
            var categoria = new CategoriaHabitacion { Id = 50, Nombre = "Superior", TarifaPorNoche = 250m, Activa = true };
            await _context.CategoriasHabitacion.AddAsync(categoria);

            // TipoHabitacionId representa la categoría (clave foránea lógica en el modelo actual)
            var habitacion = new Habitacion
            {
                Id = Guid.NewGuid(),
                Numero = "201",
                TipoHabitacionId = 50, // mismo Id de la categoría
                Precio = 250m,
                Estado = EstadoHabitacion.Disponible
            };
            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaService.DeleteCategoriaAsync(50);
            string expectedMessage = "No se puede eliminar la categoría porque tiene habitaciones asociadas.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
