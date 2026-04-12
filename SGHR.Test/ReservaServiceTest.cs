using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;
using SGHR.Data.Services;

namespace SGHR.Test
{
    /// <summary>
    /// Tests del servicio de Gestión de Reservas.
    /// Trazabilidad: SRS §3.1 – Requerimientos Funcionales y Validaciones.
    /// </summary>
    public class ReservaServiceTest
    {
        private readonly IReservaService _reservaService;
        private readonly SGHRContext _context;

        public ReservaServiceTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // BD aislada por instancia
                .Options;

            _context = new SGHRContext(options);
            _reservaService = new ReservaService(_context);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Crea una reserva válida con fechas futuras.</summary>
        private static Reserva CrearReservaValida(int categoriaId = 1, int clienteId = 1) =>
            new Reserva
            {
                ClienteId = clienteId,
                CategoriaHabitacionId = categoriaId,
                FechaEntrada = DateTime.UtcNow.AddDays(1),
                FechaSalida = DateTime.UtcNow.AddDays(3),
                NumeroHuespedes = 2
            };

        // ──────────────────────────────────────────────────────────────────────
        // GET ALL
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF – Obtener todas las reservas del sistema.</summary>
        [Fact]
        public async Task GetAllReservas_ShouldSucceed()
        {
            // Arrange
            var r1 = CrearReservaValida(categoriaId: 1, clienteId: 1);
            var r2 = CrearReservaValida(categoriaId: 2, clienteId: 2);
            r1.NumeroReserva = Guid.NewGuid();
            r2.NumeroReserva = Guid.NewGuid();
            await _context.Reservas.AddRangeAsync(r1, r2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reservaService.GetAllReservasAsync();

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            var lista = result.Data as List<Reserva>;
            Assert.NotNull(lista);
            Assert.Equal(2, lista.Count);
        }

        // ──────────────────────────────────────────────────────────────────────
        // GET BY ID
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF – Obtener reserva existente por Id.</summary>
        [Fact]
        public async Task GetReservaById_ShouldSucceed_WhenFound()
        {
            // Arrange
            var reserva = CrearReservaValida();
            reserva.Id = 100;
            reserva.NumeroReserva = Guid.NewGuid();
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reservaService.GetReservaByIdAsync(100);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        /// <summary>RF – Error al buscar reserva inexistente.</summary>
        [Fact]
        public async Task GetReservaById_ShouldFail_WhenNotFound()
        {
            // Act
            var result = await _reservaService.GetReservaByIdAsync(9999);
            string expectedMessage = "Reserva no encontrada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Validación ID – Falla cuando el Id es cero o negativo.</summary>
        [Fact]
        public async Task GetReservaById_ShouldFail_WhenIdEsCero()
        {
            // Act
            var result = await _reservaService.GetReservaByIdAsync(0);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
        }

        // ──────────────────────────────────────────────────────────────────────
        // GET BY CLIENTE ID – §3.5 RF1, §3.2 RF4
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF – Consultar historial de reservas de un cliente.</summary>
        [Fact]
        public async Task GetReservasByClienteId_ShouldSucceed_WhenClienteTieneReservas()
        {
            // Arrange
            var r1 = CrearReservaValida(clienteId: 5);
            var r2 = CrearReservaValida(clienteId: 5);
            r1.NumeroReserva = Guid.NewGuid();
            r2.NumeroReserva = Guid.NewGuid();
            r2.FechaEntrada = DateTime.UtcNow.AddDays(10);
            r2.FechaSalida = DateTime.UtcNow.AddDays(12);
            await _context.Reservas.AddRangeAsync(r1, r2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reservaService.GetReservasByClienteIdAsync(5);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            var lista = result.Data as List<Reserva>;
            Assert.NotNull(lista);
            Assert.Equal(2, lista.Count);
        }

        /// <summary>§3.2 RF6 – Mensaje informativo cuando el cliente no tiene reservas.</summary>
        [Fact]
        public async Task GetReservasByClienteId_ShouldReturnEmptyMessage_WhenSinReservas()
        {
            // Act
            var result = await _reservaService.GetReservasByClienteIdAsync(999);
            string expectedMessage = "El cliente no tiene reservas registradas.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess); // success=true con lista vacía
            Assert.Equal(expectedMessage, result.Message);
        }

        // ──────────────────────────────────────────────────────────────────────
        // CREATE – RF1, RF2, RF3, RF6, Val.1-5
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF1, RF2, RF3, RF6 – Crear reserva válida con número único y estado Confirmada.</summary>
        [Fact]
        public async Task CreateReserva_ShouldSucceed_WhenEsValida()
        {
            // Arrange
            var reserva = CrearReservaValida();

            // Act
            var result = await _reservaService.CreateReservaAsync(reserva);
            string expectedMessage = "Reserva creada exitosamente.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
            Assert.NotNull(result.Data);
        }

        /// <summary>RF3 – El número de reserva generado debe ser único (Guid no vacío).</summary>
        [Fact]
        public async Task CreateReserva_ShouldGenerarNumeroUnicoDeReserva()
        {
            // Arrange
            var reserva = CrearReservaValida();

            // Act
            await _reservaService.CreateReservaAsync(reserva);

            // Assert – NumeroReserva asignado automáticamente y no vacío
            Assert.NotEqual(Guid.Empty, reserva.NumeroReserva);
        }

        /// <summary>RF6 – El estado inicial de la reserva creada debe ser Confirmada.</summary>
        [Fact]
        public async Task CreateReserva_ShouldEstablecer_EstadoConfirmada()
        {
            // Arrange
            var reserva = CrearReservaValida();

            // Act
            await _reservaService.CreateReservaAsync(reserva);

            // Assert
            Assert.Equal(EstadoReserva.Confirmada, reserva.Estado);
        }

        /// <summary>Validación nulo – Error claro cuando se pasa null.</summary>
        [Fact]
        public async Task CreateReserva_ShouldFail_WhenEsNula()
        {
            // Arrange
            Reserva reserva = null!;

            // Act
            var result = await _reservaService.CreateReservaAsync(reserva);
            string expectedMessage = "La reserva no puede ser nula.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
            Assert.Null(result.Data);
        }

        /// <summary>Val.1 – Fecha de entrada pasada debe ser rechazada.</summary>
        [Fact]
        public async Task CreateReserva_ShouldFail_WhenFechaEntradaEsPasada()
        {
            // Arrange
            var reserva = CrearReservaValida();
            reserva.FechaEntrada = DateTime.UtcNow.AddDays(-2); // pasado
            reserva.FechaSalida = DateTime.UtcNow.AddDays(1);

            // Act
            var result = await _reservaService.CreateReservaAsync(reserva);
            string expectedMessage = "La fecha de entrada no puede ser anterior a la fecha actual.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.1 – Fecha de salida anterior o igual a entrada debe ser rechazada.</summary>
        [Fact]
        public async Task CreateReserva_ShouldFail_WhenFechaSalidaAntesDeEntrada()
        {
            // Arrange
            var reserva = CrearReservaValida();
            reserva.FechaEntrada = DateTime.UtcNow.AddDays(5);
            reserva.FechaSalida = DateTime.UtcNow.AddDays(3); // antes de entrada

            // Act
            var result = await _reservaService.CreateReservaAsync(reserva);
            string expectedMessage = "La fecha de salida debe ser posterior a la fecha de entrada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.4 – Número de huéspedes igual a cero debe ser rechazado.</summary>
        [Fact]
        public async Task CreateReserva_ShouldFail_WhenNumeroHuespedesEsCero()
        {
            // Arrange
            var reserva = CrearReservaValida();
            reserva.NumeroHuespedes = 0;

            // Act
            var result = await _reservaService.CreateReservaAsync(reserva);
            string expectedMessage = "El número de huéspedes debe ser mayor que cero.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.2, Val.3, RF2 – No se puede reservar si hay conflicto de fechas en la misma categoría.</summary>
        [Fact]
        public async Task CreateReserva_ShouldFail_WhenHayConflictoDeDisponibilidad()
        {
            // Arrange – insertar reserva existente que ocupa esas fechas
            var reservaExistente = new Reserva
            {
                Id = 200,
                NumeroReserva = Guid.NewGuid(),
                ClienteId = 1,
                CategoriaHabitacionId = 1,
                FechaEntrada = DateTime.UtcNow.AddDays(2),
                FechaSalida = DateTime.UtcNow.AddDays(5),
                NumeroHuespedes = 2,
                Estado = EstadoReserva.Confirmada
            };
            await _context.Reservas.AddAsync(reservaExistente);
            await _context.SaveChangesAsync();

            // Nueva reserva que solapa fechas en la misma categoría
            var nuevaReserva = new Reserva
            {
                ClienteId = 2,
                CategoriaHabitacionId = 1, // misma categoría
                FechaEntrada = DateTime.UtcNow.AddDays(3), // solapamiento
                FechaSalida = DateTime.UtcNow.AddDays(6),
                NumeroHuespedes = 1
            };

            // Act
            var result = await _reservaService.CreateReservaAsync(nuevaReserva);
            string expectedMessage = "No hay disponibilidad para la categoría seleccionada en las fechas indicadas.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.2, RF2 – Reservas en categorías distintas no generan conflicto.</summary>
        [Fact]
        public async Task CreateReserva_ShouldSucceed_WhenCategoriasDiferentes()
        {
            // Arrange – reserva existente en categoría 1
            var reservaExistente = new Reserva
            {
                Id = 300,
                NumeroReserva = Guid.NewGuid(),
                ClienteId = 1,
                CategoriaHabitacionId = 1,
                FechaEntrada = DateTime.UtcNow.AddDays(2),
                FechaSalida = DateTime.UtcNow.AddDays(5),
                NumeroHuespedes = 2,
                Estado = EstadoReserva.Confirmada
            };
            await _context.Reservas.AddAsync(reservaExistente);
            await _context.SaveChangesAsync();

            // Nueva reserva en categoría 2 (no conflicto)
            var nuevaReserva = new Reserva
            {
                ClienteId = 2,
                CategoriaHabitacionId = 2, // categoría diferente
                FechaEntrada = DateTime.UtcNow.AddDays(3),
                FechaSalida = DateTime.UtcNow.AddDays(6),
                NumeroHuespedes = 1
            };

            // Act
            var result = await _reservaService.CreateReservaAsync(nuevaReserva);

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
        }

        // ──────────────────────────────────────────────────────────────────────
        // UPDATE
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Modificar reserva existente con datos válidos.</summary>
        [Fact]
        public async Task UpdateReserva_ShouldSucceed_WhenEsValida()
        {
            // Arrange
            var reserva = new Reserva
            {
                Id = 400,
                NumeroReserva = Guid.NewGuid(),
                ClienteId = 1,
                CategoriaHabitacionId = 1,
                FechaEntrada = DateTime.UtcNow.AddDays(5),
                FechaSalida = DateTime.UtcNow.AddDays(8),
                NumeroHuespedes = 2,
                Estado = EstadoReserva.Confirmada
            };
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();

            var reservaActualizada = new Reserva
            {
                CategoriaHabitacionId = 1,
                FechaEntrada = DateTime.UtcNow.AddDays(6),
                FechaSalida = DateTime.UtcNow.AddDays(9),
                NumeroHuespedes = 3
            };

            // Act
            var result = await _reservaService.UpdateReservaAsync(400, reservaActualizada);
            string expectedMessage = "Reserva actualizada exitosamente.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Error al actualizar una reserva que no existe.</summary>
        [Fact]
        public async Task UpdateReserva_ShouldFail_WhenNotFound()
        {
            // Arrange
            var reservaActualizada = new Reserva
            {
                CategoriaHabitacionId = 1,
                FechaEntrada = DateTime.UtcNow.AddDays(1),
                FechaSalida = DateTime.UtcNow.AddDays(3),
                NumeroHuespedes = 2
            };

            // Act
            var result = await _reservaService.UpdateReservaAsync(9999, reservaActualizada);
            string expectedMessage = "Reserva no encontrada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        // ──────────────────────────────────────────────────────────────────────
        // CANCELAR – RF5, RF6, RF7, Val.6
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>RF5, RF6, RF7 – Cancelar reserva activa con observación de auditoría.</summary>
        [Fact]
        public async Task CancelarReserva_ShouldSucceed_WhenEsValida()
        {
            // Arrange
            var reserva = new Reserva
            {
                Id = 500,
                NumeroReserva = Guid.NewGuid(),
                ClienteId = 1,
                CategoriaHabitacionId = 1,
                FechaEntrada = DateTime.UtcNow.AddDays(10),
                FechaSalida = DateTime.UtcNow.AddDays(12),
                NumeroHuespedes = 2,
                Estado = EstadoReserva.Confirmada
            };
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reservaService.CancelarReservaAsync(500, "El cliente solicitó cancelación.");
            string expectedMessage = "Reserva cancelada exitosamente.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);

            // RF7 – verificar que se guardó la observación (auditoría)
            var reservaCancelada = result.Data as Reserva;
            Assert.NotNull(reservaCancelada);
            Assert.Equal(EstadoReserva.Cancelada, reservaCancelada.Estado);
            Assert.Equal("El cliente solicitó cancelación.", reservaCancelada.ObservacionCancelacion);
        }

        /// <summary>RF5 – Error al cancelar una reserva que no existe.</summary>
        [Fact]
        public async Task CancelarReserva_ShouldFail_WhenNotFound()
        {
            // Act
            var result = await _reservaService.CancelarReservaAsync(9999, null);
            string expectedMessage = "Reserva no encontrada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        /// <summary>Val.6 – No se puede cancelar una reserva ya cancelada.</summary>
        [Fact]
        public async Task CancelarReserva_ShouldFail_WhenYaEstaCancelada()
        {
            // Arrange – insertar reserva ya cancelada
            var reserva = new Reserva
            {
                Id = 600,
                NumeroReserva = Guid.NewGuid(),
                ClienteId = 1,
                CategoriaHabitacionId = 1,
                FechaEntrada = DateTime.UtcNow.AddDays(15),
                FechaSalida = DateTime.UtcNow.AddDays(17),
                NumeroHuespedes = 1,
                Estado = EstadoReserva.Cancelada,
                ObservacionCancelacion = "Cancelada previamente."
            };
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reservaService.CancelarReservaAsync(600, "Intento duplicado.");
            string expectedMessage = "La reserva ya se encuentra cancelada.";

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
