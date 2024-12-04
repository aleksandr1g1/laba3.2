using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace laba3test.Domain.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<Guid> InsertAsync(T entity);
        Task UpdateAsync(Guid id, T entity);
        Task DeleteAsync(Guid id);

    }
}
