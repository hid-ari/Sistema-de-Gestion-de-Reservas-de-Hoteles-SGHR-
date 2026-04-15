using SGHR.Data.Base;
using SGHR.Data.Models;

namespace SGHR.Data.Abstraction
{
    public interface IRegistroService
    {
        Task<OperationResult> GetAllRegistrosAsync();
        Task<OperationResult> GetRegistroByIdAsync(int id);
        Task<OperationResult> CreateRegistroAsync(Cliente cliente);
        Task<OperationResult> UpdateRegistroAsync(int id, Cliente cliente);
        Task<OperationResult> DeleteRegistroAsync(int id);
    }
}
