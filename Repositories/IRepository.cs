using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolucionDA.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object id);
        Task<int> InsertAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(object id);

        //Ejecutar SPS
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedure, object parameters);

        Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string storedProcedure, object parameters);
    }
}
