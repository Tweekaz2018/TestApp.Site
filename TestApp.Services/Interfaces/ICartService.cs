using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCart(int id);
        Task AddItemToCart(int itemId, int cartId, int quantity);
        Task DeleteFromCart(int itemId, int cartId);
        Task ClearCart(int cartId);
        Task SupplementItemInCart(int itemId, int cartId);
        Task DevideItemInCart(int itemId, int cartId);
    }
}
