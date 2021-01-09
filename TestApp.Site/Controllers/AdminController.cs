using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Site.Models;
using TestApp.Site.Models.Admin;

namespace TestApp.Site.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ISaveFile _saveFileService;
        private readonly IWebHostEnvironment _env;

        private int pageSize;

        public AdminController(IItemService itemService, IOrderService orderService, IUserService userService, IWebHostEnvironment env, ISaveFile saveFileService, int pageSize = 5)
        {
            _itemService = itemService;
            _orderService = orderService;
            _userService = userService;
            _saveFileService = saveFileService;
            _env = env;
            this.pageSize = pageSize;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetItemsPage(int page = 1)
        {
            int countItems = await _itemService.ItemsCount();
            int skip = (page - 1) * pageSize;
            IEnumerable<Item> items = await _itemService.GetItems(skip, pageSize);
            PageViewModel<Item> model = new PageViewModel<Item>(items, countItems, page, pageSize);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemAddModal()
        {
            ItemModel model = new ItemModel()
            {
                categories = await _itemService.GetCategories()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ItemModel model)
        {
            if (ModelState.IsValid)
            {
                string directory = "Images";
                string fileName = await _saveFileService.SaveFile(model.image, Path.Combine(_env.WebRootPath, directory));
                Item item = new Item()
                {
                    description = model.description,
                    price = model.price,
                    title = model.title,
                    imagePath = Path.Combine(directory, fileName),
                    CategoryId = model.categoryId
                };
                await _itemService.AddItem(item);
            }
            return RedirectToAction("GetItemsPage");
        }

        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemService.DeleteItem(id);
            return RedirectToAction("GetItemsPage");
        }

        public async Task<IActionResult> GetUsersPage(int page = 1)
        {
            int countUsers = await _userService.UsersCount();
            int skip = (page - 1) * pageSize;
            IEnumerable<User> users = await _userService.GetUsers(skip, pageSize);
            PageViewModel<User> model = new PageViewModel<User>(users, countUsers, page, pageSize);
            return View(model);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            User user = await _userService.GetUser(id);
            await _userService.DeleteUser(user);
            return RedirectToAction("GetUsersPage");
        }

        public async Task<IActionResult> GetOrdersPage(int page = 1)
        {
            int countOrders = await _orderService.OrdersCount();
            int skip = (page - 1) * pageSize;
            IEnumerable<Order> orders = await _orderService.GetOrders(skip, pageSize);
            PageViewModel<Order> model = new PageViewModel<Order>(orders, countOrders, page, pageSize);
            return View(model);
        }

        #region Delivery method
        public async Task<IActionResult> GetOrdersDeliveryMethodsPage(int page = 1)
        {
            IEnumerable<OrderDeliveryMethod> deliveryMethods = await _orderService.GetOrderDeliveryMethods();
            PageViewModel<OrderDeliveryMethod> model = new PageViewModel<OrderDeliveryMethod>(deliveryMethods, page, pageSize);
            return View(model);
        }
        public IActionResult AddDeliveryMethodModal()
        {
            return View();
        }

        public async Task<IActionResult> AddDeliveryMethod([FromForm] string deliveryMethodName)
        {
            if (!string.IsNullOrEmpty(deliveryMethodName))
            {
                await _orderService.AddDeliveryMethod(new OrderDeliveryMethod()
                {
                    name = deliveryMethodName
                });
            }
            return RedirectToAction("GetOrdersDeliveryMethodsPage");
        }

        public async Task UpdateDeliveryMethod(string name, int id)
        {
            await _orderService.UpdateDeliveryMethod(new OrderDeliveryMethod()
            {
                Id = id,
                name = name
            });
        }

        public async Task DeleteDeliveryMethod(int id)
        {
            await _orderService.RemoveDeliveryMethod(id);
        }
        #endregion 

        #region PayMethods
        public async Task<IActionResult> GetOrdersPayMethodsPage(int page = 1)
        {
            IEnumerable<OrderPayMethod> payMethods = await _orderService.GetOrderPayMethods();
            PageViewModel<OrderPayMethod> model = new PageViewModel<OrderPayMethod>(payMethods, page, pageSize);
            return View(model);
        }
        public IActionResult AddPayMethodModal()
        {
            return View();
        }

        public async Task<IActionResult> AddPayMethod([FromForm] string payMethodName)
        {
            await _orderService.AddPayMethod(new OrderPayMethod()
            {
                name = payMethodName
            });
            return RedirectToAction("GetOrdersPayMethodsPage");
        }

        public async Task UpdatePayMethod(string name, int id)
        {
            await _orderService.UpdatePayMethod(new OrderPayMethod()
            {
                Id = id,
                name = name
            });
        }

        public async Task DeletePayMethod(int id)
        {
            await _orderService.RemovePayMethod(id);
        }

        #endregion

        #region Categories

        public async Task<IActionResult> GetItemsCategories(int page = 1)
        {
            IEnumerable<Category> categories = await _itemService.GetCategories();
            PageViewModel<Category> model = new PageViewModel<Category>(categories, page, pageSize);
            return View(model);
        }

        public IActionResult AddCategoryModal()
        {
            return View();
        }

        public async Task<IActionResult> AddCategory([FromForm] string categoryName)
        {
            await _itemService.AddCategory(new Category()
            {
                name = categoryName
            });
            return RedirectToAction("GetItemsCategories");
        }

        public async Task UpdateCategory(string name, int id)
        {
            await _itemService.UpdateCategory(new Category()
            {
                Id = id,
                name = name
            });
        }

        public async Task DeleteCategory(int id)
        {
            await _itemService.DeleteCategory(id);
        }

        #endregion
    }
}
