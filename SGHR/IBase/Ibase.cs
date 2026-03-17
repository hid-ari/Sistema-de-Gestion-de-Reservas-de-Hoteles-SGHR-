using SGHR.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SGHR.Data.IBase
{
    public interface IbaseService<TEntity> where TEntity : class
    {
        Task<OperationResult> Save(TEntity entity);
        Task<OperationResult> Update(TEntity entity);
        Task<OperationResult> Delete(TEntity entity);
        Task<OperationResult> GetById(int id);
        Task<OperationResult> GetAll();
    }
}