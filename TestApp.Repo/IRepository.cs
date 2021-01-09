using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Repo
{
    public interface IRepository<T>
    {
        IQueryable<T> GetRepo();
        Task<int> AddAsync(T item);
        Task UpdateAsync(T item);
        Task RemoveAsync(T item);
        Task AddRangeAsync(IEnumerable<T> items);
        Task RemoveRangeAsync(IEnumerable<T> items);
    }
}
