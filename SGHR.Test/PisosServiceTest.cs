using Microsoft.EntityFrameworkCore;
using SGHR.Data;
using SGHR.Data.Services;
using Xunit;
using SGHR.Data.Models;
using SGHR.Data.Context;



namespace SGHR.Tests.Services
{
    public class PisosServicesTests
    {
        private SGHRContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SGHRContext(options);
        }

        [Fact]
        public async Task Save_Should_Create_Piso()
        {
            var context = GetDbContext();
            var service = new PisosServices(context);

            var piso = new Pisos
            {
                NumeroPiso = "Piso Test"
            };

            var result = await service.Save(piso);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetAll_Should_Return_Pisos()
        {
            var context = GetDbContext();

            context.Pisos.Add(new Pisos { NumeroPiso = "Piso 1" });
            context.Pisos.Add(new Pisos { NumeroPiso = "Piso 2" });

            await context.SaveChangesAsync();

            var service = new PisosServices(context);

            var result = await service.GetAll();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetById_Should_Return_Piso()
        {
            var context = GetDbContext();

            var piso = new Pisos
            {
                NumeroPiso = "Piso Test"
            };

            context.Pisos.Add(piso);
            await context.SaveChangesAsync();

            var service = new PisosServices(context);

            var result = await service.GetById(piso.PisosId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Update_Should_Update_Piso()
        {
            var context = GetDbContext();

            var piso = new Pisos
            {
                NumeroPiso = "Piso Viejo"
            };

            context.Pisos.Add(piso);
            await context.SaveChangesAsync();

            var service = new PisosServices(context);

            piso.NumeroPiso = "Piso Nuevo";

            var result = await service.Update(piso);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Delete_Should_Remove_Piso()
        {
            var context = GetDbContext();

            var piso = new Pisos
            {
                NumeroPiso = "Piso Test"
            };

            context.Pisos.Add(piso);
            await context.SaveChangesAsync();

            var service = new PisosServices(context);

            var result = await service.Delete(piso);

            Assert.True(result.IsSuccess);
        }
    }
}