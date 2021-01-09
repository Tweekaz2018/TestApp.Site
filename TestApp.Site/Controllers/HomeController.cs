using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Site.Models;
using TestApp.Site.Models.Home;

namespace TestApp.Site.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public HomeController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int id = int.Parse(User.Identity.Name);
            User user = await _userService.GetUserWithOrder(id);
            UserProfileModel model = new UserProfileModel()
            {
                user = user
            };
            return View(model);
        }
    }
}
