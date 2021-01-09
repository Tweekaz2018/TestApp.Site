using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Repo;
using TestApp.Services.Interfaces;

namespace TestApp.Services.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _cartsRepo;
        private readonly IRepository<CartItem> _cartItemsRepo;
        private readonly IRepository<Item> _itemsRepo;
        public CartService(IRepository<Cart> cartsRepo,IRepository<CartItem> cartItemsRepo, IRepository<Item> itemsRepo)
        {
            _cartItemsRepo = cartItemsRepo;
            _cartsRepo = cartsRepo;
            _itemsRepo = itemsRepo;
        }

        public async Task AddItemToCart(int itemId, int cartId, int quantity)
        {
            Item item = await _itemsRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == itemId);
            if (item == null)
                throw new NullReferenceException("Item is null");
            Cart cart = await _cartsRepo.GetRepo().Where(x=>x.Id == cartId).Include(x => x.CartItems).FirstOrDefaultAsync();
            if (cart == null)
                throw new NullReferenceException("Cart is null");
            CartItem cartItem = cart.CartItems.FirstOrDefault(x => x.ItemId == itemId);
            if(cartItem == null)
            {
                cartItem = new CartItem()
                {
                    CartId = cartId,
                    ItemId = itemId,
                    quantity = quantity
                };
                await _cartItemsRepo.AddAsync(cartItem);
            }
            else
            {
                cartItem.quantity = cartItem.quantity + quantity;
                await _cartItemsRepo.UpdateAsync(cartItem);
            }
        }

        public async Task ClearCart(int cartId)
        {
            List<CartItem> cartItems = await _cartItemsRepo.GetRepo().Where(x => x.CartId == cartId).ToListAsync();
            await _cartItemsRepo.RemoveRangeAsync(cartItems);
        }

        public async Task DeleteFromCart(int itemId, int cartId)
        {
            var itemToDelete = await _cartItemsRepo.GetRepo().Where(x => x.CartId == cartId && x.ItemId == itemId).FirstOrDefaultAsync();
            if (itemToDelete != null)
                await _cartItemsRepo.RemoveAsync(itemToDelete);
        }

        public async Task<Cart> GetCart(int id)
        {
            return await _cartsRepo.GetRepo().Where(x => x.Id == id).Include(x => x.CartItems).ThenInclude(x => x.Item).FirstOrDefaultAsync();
        }
        public async Task SupplementItemInCart(int itemId, int cartId)
        {
            CartItem cartItem = await _cartItemsRepo.GetRepo().Where(x => x.ItemId == itemId && x.CartId == cartId).FirstOrDefaultAsync();
            if(cartItem != null)
            {
                cartItem.quantity++;
                await _cartItemsRepo.UpdateAsync(cartItem);
            }
        }

        public async Task DevideItemInCart(int itemId, int cartId)
        {
            CartItem cartItem = await _cartItemsRepo.GetRepo().Where(x => x.ItemId == itemId && x.CartId == cartId).FirstOrDefaultAsync();
            if (cartItem != null)
            {
                cartItem.quantity--;
                if (cartItem.quantity == 0)
                    await _cartItemsRepo.RemoveAsync(cartItem);
                else
                    await _cartItemsRepo.UpdateAsync(cartItem);
            }
        }
    }
}
