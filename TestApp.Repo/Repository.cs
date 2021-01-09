using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Repo
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly DbSet<T> targetDbSet;
        private readonly Context dbContext;

        public Repository(Context context)
        {
            dbContext = context;
            targetDbSet = dbContext.Set<T>();
        }
        public IQueryable<T> GetRepo() => targetDbSet;

        public async Task<int> AddAsync(T item)
        {
            await targetDbSet.AddAsync(item);
            await dbContext.SaveChangesAsync();
            return item.Id;
        }

        public async Task UpdateAsync(T item)
        {
            targetDbSet.Update(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(T item)
        {
            targetDbSet.Remove(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            await targetDbSet.AddRangeAsync(items);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> items)
        {
            targetDbSet.RemoveRange(items);
            await dbContext.SaveChangesAsync();
        }
    }
}
