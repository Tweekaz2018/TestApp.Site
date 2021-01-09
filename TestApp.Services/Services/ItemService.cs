using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Repo;
using TestApp.Services.Interfaces;

namespace TestApp.Services.Services
{
    public class ItemService : IItemService
    {
        private readonly IRepository<Category> _categoriesRepo;
        private readonly IRepository<Item> _itemsRepo;

        public ItemService(IRepository<Category> categoriesRepo, IRepository<Item> itemsRepo)
        {
            _categoriesRepo = categoriesRepo;
            _itemsRepo = itemsRepo;
        }
        public async Task<int> AddCategory(Category category)
        {
            return await _categoriesRepo.AddAsync(category);
        }

        public async Task<int> AddItem(Item item)
        {
            return await _itemsRepo.AddAsync(item);
        }

        public async Task DeleteCategory(int id)
        {
            Category category = await _categoriesRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == id);
            if (category != null)
                await _categoriesRepo.RemoveAsync(category);
            List<Item> items = await _itemsRepo.GetRepo().Where(x => x.CategoryId == id).ToListAsync();
            if (items.Count > 0)
                await _itemsRepo.RemoveRangeAsync(items);
        }

        public async Task DeleteItem(int id)
        {
            Item item = await _itemsRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                await _itemsRepo.RemoveAsync(item);
            }
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _categoriesRepo.GetRepo().ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItems(int catId = 0)
        {
            if (catId != 0)
                return await _itemsRepo.GetRepo().Where(x => x.CategoryId == catId).ToListAsync();
            return await _itemsRepo.GetRepo().ToListAsync();
        }


        public async Task UpdateCategory(Category category)
        {
            await _categoriesRepo.UpdateAsync(category);
        }

        public async Task UpdateItem(Item item)
        {
            await _itemsRepo.UpdateAsync(item);
        }

        public async Task<Item> GetItem(int id)
        {
            return await _itemsRepo.GetRepo().Where(x => x.Id == id).Include(x => x.Category).FirstOrDefaultAsync();
        }

        public async Task<int> ItemsCount()
        {
            return await _itemsRepo.GetRepo().CountAsync();
        }

        public async Task<IEnumerable<Item>> GetItems(int skip, int take)
        {
            return await _itemsRepo.GetRepo().Include(x => x.Category).Skip(skip).Take(take).ToListAsync();
        }
    }
}
