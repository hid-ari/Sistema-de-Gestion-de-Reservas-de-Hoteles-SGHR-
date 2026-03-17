using SGHR.Data.Base;
using SGHR.Data.Models;

namespace SGHR.Data.Abstraction
{
    public interface IHabitacionesService
    {
        public Task<OperationResult> GetAllHabitacionesAsync();
        public Task<OperationResult> GetHabitacionByIdAsync(Guid id);
        public Task<OperationResult> CreateHabitacionAsync(Habitacion habitacion);
        public Task<OperationResult> UpdateHabitacionAsync(Guid id, Habitacion habitacion);
        public Task<OperationResult> DeleteHabitacionAsync(Guid id);
    }
}
