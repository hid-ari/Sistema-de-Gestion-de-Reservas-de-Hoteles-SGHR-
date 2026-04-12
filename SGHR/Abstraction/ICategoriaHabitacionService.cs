using SGHR.Data.Base;
using SGHR.Data.Models;

namespace SGHR.Data.Abstraction
{
    /// <summary>
    /// Contrato del servicio de Gestión de Categorías de Habitaciones.
    /// SRS §3.3 – Requerimientos Funcionales.
    /// </summary>
    public interface ICategoriaHabitacionService
    {
        /// <summary>RF1 – Obtiene el listado de todas las categorías.</summary>
        Task<OperationResult> GetAllCategoriasAsync();

        /// <summary>RF1 – Obtiene una categoría por su identificador.</summary>
        Task<OperationResult> GetCategoriaByIdAsync(int id);

        /// <summary>RF2 – Crea una nueva categoría con nombre, descripción, tarifa y características.</summary>
        Task<OperationResult> CreateCategoriaAsync(CategoriaHabitacion categoria);

        /// <summary>RF3 – Actualiza los datos de una categoría existente.</summary>
        Task<OperationResult> UpdateCategoriaAsync(int id, CategoriaHabitacion categoria);

        /// <summary>RF4 – Elimina una categoría siempre que no tenga habitaciones asociadas.</summary>
        Task<OperationResult> DeleteCategoriaAsync(int id);
    }
}
