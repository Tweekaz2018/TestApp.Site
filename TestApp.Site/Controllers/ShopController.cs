using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Site.Models;
using TestApp.Site.Models.Shop;

namespace TestApp.Site.Controllers
{
    [AllowAnonymous]
    public class ShopController : Controller
    {
        private readonly IItemService _itemService;
        public ShopController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            ShopMainModel model = new ShopMainModel();
            model.Categories = await _itemService.GetCategories();
            if (id.HasValue)
            {
                var category = model.Categories.Where(x => x.Id == id.Value).FirstOrDefault();
                if (category == null)
                    return RedirectToAction("Index");
                model.title = category.name;
                model.items = await _itemService.GetItems(id.Value);
            }
            else
            {
                model.title = "All candies";
                model.items = await _itemService.GetItems();
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ItemDetails(int id)
        {
            Item item = await _itemService.GetItem(id);
            if (item == null)
                return View("Error", new ErrorModel("Can't find item"));
            return View(item);
        }

    }
}
