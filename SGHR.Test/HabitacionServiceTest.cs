using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;
using SGHR.Data.Services;

namespace SGHR.Test
{
    public class HabitacionServiceTest
    {
        private readonly IHabitacionesService _habitacionService;
        private readonly SGHRContext _context;
        public HabitacionServiceTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: "SGHR_Test")
                .Options;

            _context = new SGHRContext(options);

            _habitacionService = new HabitacionService(_context);
        }
        [Fact]
        public async Task GetHabitacionById_ShouldFail_WhenNotFound()
        {
            Guid id = Guid.NewGuid();
            //Arrange
            var result = await _habitacionService.GetHabitacionByIdAsync(id);
            string expectedMessage = "Habitación no encontrada.";
            //Assert
            Assert.NotNull(result);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task GetHabitacionById_ShouldFail_WhenIdIsNull()
        {
            //Arrange
            Guid id = Guid.Empty;
            //Act
            var result = await _habitacionService.GetHabitacionByIdAsync(id);
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public async Task CreateHabitacion_ShouldFail_WhenIsNull()
        {
            //Arrange
            Habitacion habitacion = null;
            //Act
            var result = await _habitacionService.CreateHabitacionAsync(habitacion);
            string expectedMessage = "La habitación no puede ser nula.";
            //Assert
            Assert.NotNull(result);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task CreateHabitacion_ShouldSucceed_WhenIsValid()
        {
            //Arrange
            Habitacion habitacion = new Habitacion
            {
                Numero = "101",
                TipoHabitacionId = 1,
                Precio = 100.00m,
                Estado = EstadoHabitacion.Disponible,
                Descripcion = "Habitación estándar con vista al mar",
                TieneMiniBar = true
            };
            //Act
            var result = await _habitacionService.CreateHabitacionAsync(habitacion);
            var expectedMessage = "Habitación creada exitosamente.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task DeleteHabitacion_ShouldFail_WhenIdIsNull()
        {
            //Arrange
            Guid id = Guid.Empty;
            //Act
            var result = await _habitacionService.DeleteHabitacionAsync(id);
            string expectedMessage = "El ID no puede ser vacío.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task DeleteHabitacion_ShouldFail_WhenNotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            //Act
            var result = await _habitacionService.DeleteHabitacionAsync(id);
            string expectedMessage = "Habitación no encontrada.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task DeleteHabitacion_ShouldFail_WhenIsReservada()
        {
            //Arrange
            Habitacion habitacion = new Habitacion
            {
                Id = Guid.NewGuid(),
                Numero = "102",
                TipoHabitacionId = 1,
                Precio = 150.00m,
                Estado = EstadoHabitacion.Reservada,
                Descripcion = "Habitación deluxe con vista al mar",
                TieneMiniBar = true
            };
            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();
            //Act
            var result = await _habitacionService.DeleteHabitacionAsync(habitacion.Id);
            string expectedMessage = "No se puede eliminar una habitación reservada.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task DeleteHabitacion_ShouldSucceed_WhenIsValid()
        {
            //Arrange
            Habitacion habitacion = new Habitacion
            {
                Id = Guid.NewGuid(),
                Numero = "103",
                TipoHabitacionId = 1,
                Precio = 200.00m,
                Estado = EstadoHabitacion.Disponible,
                Descripcion = "Suite con vista al mar",
                TieneMiniBar = true
            };
            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();
            //Act
            var result = await _habitacionService.DeleteHabitacionAsync(habitacion.Id);
            string expectedMessage = "Habitación eliminada exitosamente.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task GetAllHabitaciones_ShouldSucceed()
        {
            //Arrange
            Habitacion habitacion1 = new Habitacion
            {
                Id = Guid.NewGuid(),
                Numero = "104",
                TipoHabitacionId = 1,
                Precio = 120.00m,
                Estado = EstadoHabitacion.Disponible,
                Descripcion = "Habitación estándar con vista a la ciudad",
                TieneMiniBar = false
            };
            Habitacion habitacion2 = new Habitacion
            {
                Id = Guid.NewGuid(),
                Numero = "105",
                TipoHabitacionId = 1,
                Precio = 180.00m,
                Estado = EstadoHabitacion.Ocupada,
                Descripcion = "Habitación deluxe con vista a la ciudad",
                TieneMiniBar = true
            };
            await _context.Habitaciones.AddRangeAsync(habitacion1, habitacion2);
            await _context.SaveChangesAsync();
            //Act
            var result = await _habitacionService.GetAllHabitacionesAsync();
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.NotNull(result.Data);
            Assert.IsAssignableFrom<List<Habitacion>>(result.Data);
            var habitacionesList = result.Data as List<Habitacion>;
            Assert.Contains(habitacionesList, h => h.Id == habitacion1.Id);
            Assert.Contains(habitacionesList, h => h.Id == habitacion2.Id);
        }
        [Fact]
        public async Task GetHabitacionById_ShouldSucceed_WhenFound()
        {
            //Arrange
            Habitacion habitacion = new Habitacion
            {
                Id = Guid.NewGuid(),
                Numero = "106",
                TipoHabitacionId = 1,
                Precio = 130.00m,
                Estado = EstadoHabitacion.Disponible,
                Descripcion = "Habitación estándar con vista al jardín",
                TieneMiniBar = false
            };
            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();
            //Act
            var result = await _habitacionService.GetHabitacionByIdAsync(habitacion.Id);
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }
        [Fact]
        public async Task UpdateHabitacion_ShouldFail_WhenIdIsNull()
        {
            //Arrange
            Guid id = Guid.Empty;
            Habitacion habitacion = new Habitacion
            {
                Numero = "107",
                TipoHabitacionId = 1,
                Precio = 140.00m,
                Estado = EstadoHabitacion.Disponible,
                Descripcion = "Habitación estándar con vista a la piscina",
                TieneMiniBar = false
            };
            //Act
            var result = await _habitacionService.UpdateHabitacionAsync(id, habitacion);
            string expectedMessage = "El ID no puede ser vacío.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateHabitacion_ShouldFail_WhenNotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            Habitacion habitacion = new Habitacion
            {
                Numero = "108",
                TipoHabitacionId = 1,
                Precio = 150.00m,
                Estado = EstadoHabitacion.Disponible,
                Descripcion = "Habitación estándar con vista a la montaña",
                TieneMiniBar = false
            };
            //Act
            var result = await _habitacionService.UpdateHabitacionAsync(id, habitacion);
            string expectedMessage = "Habitación no encontrada.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
        [Fact]
        public async Task UpdateHabitacion_ShouldSucceed_WhenIsValid()
        {
            //Arrange
            Habitacion habitacion = new Habitacion
            {
                Id = Guid.NewGuid(),
                Numero = "109",
                TipoHabitacionId = 1,
                Precio = 160.00m,
                Estado = EstadoHabitacion.Disponible,
                Descripcion = "Habitación estándar con vista al parque",
                TieneMiniBar = false
            };
            await _context.Habitaciones.AddAsync(habitacion);
            await _context.SaveChangesAsync();
            Habitacion updatedHabitacion = new Habitacion
            {
                Numero = "109A",
                TipoHabitacionId = 1,
                Precio = 170.00m,
                Estado = EstadoHabitacion.Ocupada,
                Descripcion = "Habitación estándar con vista al parque y balcón",
                TieneMiniBar = true
            };
            //Act
            var result = await _habitacionService.UpdateHabitacionAsync(habitacion.Id, updatedHabitacion);
            string expectedMessage = "Habitación actualizada exitosamente.";
            //Assert
            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
