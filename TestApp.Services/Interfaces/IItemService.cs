using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Services.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetItems(int catId = 0);
        Task<IEnumerable<Category>> GetCategories();
        Task<int> AddItem(Item item);
        Task UpdateItem(Item item);
        Task DeleteItem(int id);
        Task<int> AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int id);
        Task<Item> GetItem(int id);
        Task<int> ItemsCount();
        Task<IEnumerable<Item>> GetItems(int skip, int take);
    }
}
