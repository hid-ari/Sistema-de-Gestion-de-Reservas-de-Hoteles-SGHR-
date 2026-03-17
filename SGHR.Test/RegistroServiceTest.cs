using Microsoft.EntityFrameworkCore;
using SGHR.Data.Abstraction;
using SGHR.Data.Base;
using SGHR.Data.Context;
using SGHR.Data.Models;
using SGHR.Data.Services;

namespace SGHR.Test
{
    public class RegistroServiceTest
    {
        private readonly IRegistroService _registroService;
        private readonly SGHRContext _context;

        public RegistroServiceTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: "SGHR_Registro_Test")
                .Options;

            _context = new SGHRContext(options);
            _registroService = new RegistroService(_context);
        }

        [Fact]
        public async Task GetRegistroById_ShouldFail_WhenNotFound()
        {
            int id = 999;

            var result = await _registroService.GetRegistroByIdAsync(id);
            string expectedMessage = "Cliente no encontrado.";

            Assert.NotNull(result);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task GetRegistroById_ShouldFail_WhenIdIsZero()
        {
            int id = 0;

            var result = await _registroService.GetRegistroByIdAsync(id);

            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CreateRegistro_ShouldFail_WhenIsNull()
        {
            Cliente cliente = null;

            var result = await _registroService.CreateRegistroAsync(cliente);
            string expectedMessage = "El cliente no puede ser nulo.";

            Assert.NotNull(result);
            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateRegistro_ShouldSucceed_WhenIsValid()
        {
            Cliente cliente = new Cliente
            {
                Id = 1,
                UserId = "user-1",
                Nombre = "Ana",
                Apellido = "Lopez",
                Email = "ana@example.com",
                Telefono = "8095551234"
            };

            var result = await _registroService.CreateRegistroAsync(cliente);
            var expectedMessage = "Cliente creado exitosamente.";

            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task DeleteRegistro_ShouldFail_WhenIdIsZero()
        {
            int id = 0;

            var result = await _registroService.DeleteRegistroAsync(id);
            string expectedMessage = "El ID no puede ser cero.";

            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task DeleteRegistro_ShouldFail_WhenNotFound()
        {
            int id = 777;

            var result = await _registroService.DeleteRegistroAsync(id);
            string expectedMessage = "Cliente no encontrado.";

            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task DeleteRegistro_ShouldSucceed_WhenIsValid()
        {
            Cliente cliente = new Cliente
            {
                Id = 2,
                UserId = "user-2",
                Nombre = "Mario",
                Apellido = "Perez",
                Email = "mario@example.com",
                Telefono = "8095555678"
            };
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            var result = await _registroService.DeleteRegistroAsync(cliente.Id);
            string expectedMessage = "Cliente eliminado exitosamente.";

            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task GetAllRegistros_ShouldSucceed()
        {
            Cliente cliente1 = new Cliente
            {
                Id = 3,
                UserId = "user-3",
                Nombre = "Laura",
                Apellido = "Diaz",
                Email = "laura@example.com",
                Telefono = "8095551111"
            };
            Cliente cliente2 = new Cliente
            {
                Id = 4,
                UserId = "user-4",
                Nombre = "Carlos",
                Apellido = "Gomez",
                Email = "carlos@example.com",
                Telefono = "8095552222"
            };
            await _context.Clientes.AddRangeAsync(cliente1, cliente2);
            await _context.SaveChangesAsync();

            var result = await _registroService.GetAllRegistrosAsync();

            Assert.IsType<OperationResult>(result);
            Assert.NotNull(result.Data);
            Assert.IsAssignableFrom<List<Cliente>>(result.Data);
            var clientesList = result.Data as List<Cliente>;
            Assert.Contains(clientesList, c => c.Id == cliente1.Id);
            Assert.Contains(clientesList, c => c.Id == cliente2.Id);
        }

        [Fact]
        public async Task GetRegistroById_ShouldSucceed_WhenFound()
        {
            Cliente cliente = new Cliente
            {
                Id = 5,
                UserId = "user-5",
                Nombre = "Sofia",
                Apellido = "Martinez",
                Email = "sofia@example.com",
                Telefono = "8095553333"
            };
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            var result = await _registroService.GetRegistroByIdAsync(cliente.Id);

            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateRegistro_ShouldFail_WhenIdIsZero()
        {
            int id = 0;
            Cliente cliente = new Cliente
            {
                UserId = "user-6",
                Nombre = "Luis",
                Apellido = "Reyes",
                Email = "luis@example.com",
                Telefono = "8095554444"
            };

            var result = await _registroService.UpdateRegistroAsync(id, cliente);
            string expectedMessage = "El ID no puede ser cero.";

            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task UpdateRegistro_ShouldFail_WhenNotFound()
        {
            int id = 999;
            Cliente cliente = new Cliente
            {
                UserId = "user-7",
                Nombre = "Elena",
                Apellido = "Suarez",
                Email = "elena@example.com",
                Telefono = "8095555555"
            };

            var result = await _registroService.UpdateRegistroAsync(id, cliente);
            string expectedMessage = "Cliente no encontrado.";

            Assert.IsType<OperationResult>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public async Task UpdateRegistro_ShouldSucceed_WhenIsValid()
        {
            Cliente cliente = new Cliente
            {
                Id = 6,
                UserId = "user-8",
                Nombre = "Pedro",
                Apellido = "Vargas",
                Email = "pedro@example.com",
                Telefono = "8095556666"
            };
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();

            Cliente updatedCliente = new Cliente
            {
                UserId = "user-8",
                Nombre = "Pedro Antonio",
                Apellido = "Vargas",
                Email = "pedro.antonio@example.com",
                Telefono = "8095557777"
            };

            var result = await _registroService.UpdateRegistroAsync(cliente.Id, updatedCliente);
            string expectedMessage = "Cliente actualizado exitosamente.";

            Assert.IsType<OperationResult>(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
