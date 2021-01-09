using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp.Services.Interfaces;
using TestApp.Site.Models;
using TestApp.Site.Models.Cart;

namespace TestApp.Site.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCart(int.Parse(User.Identity.Name));
            IndexModel model = new IndexModel()
            {
                cart = cart
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult AddItemToCart(int id)
        {
            ViewBag.ItemId = id;
            return View();
        }

        [HttpPost]
        public async Task AddItemToCart(int itemId, int quantity)
        {
            int userCartId = int.Parse(User.Identity.Name);
            await _cartService.AddItemToCart(itemId, userCartId, quantity);
        }

        public async Task<IActionResult> RemoveItemFromCart(int cartId, int itemId)
        {
            await _cartService.DeleteFromCart(itemId, cartId);
            return Redirect("/cart/");
        }

        public async Task<IActionResult> SupplementItemInCart(int cartId, int itemId)
        {
            await _cartService.SupplementItemInCart(itemId, cartId);
            return Redirect("/cart/");
        }

        public async Task<IActionResult> DevideItemIncart(int cartId, int itemId)
        {
            await _cartService.DevideItemInCart(itemId, cartId);
            return Redirect("/cart/");
        }


        public async Task<IActionResult> ClearCart(int cartId)
        {
            await _cartService.ClearCart(cartId);
            return Redirect("/cart/");
        }
    }
}