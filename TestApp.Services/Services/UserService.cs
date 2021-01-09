using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Repo;
using TestApp.Services.Interfaces;

namespace TestApp.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ISecurityService passwordService;
        private readonly IRepository<User> _usersRepo;
        private readonly IRepository<UserRole> _userRolesRepo;
        private readonly IRepository<Cart> _cartsRepo;
        private readonly IRepository<CartItem> _cartItemsRepo;
        private readonly IRepository<Order> _ordersRepo;
        private readonly IRepository<OrderItem> _orderItemsRepo;

        public UserService(
            IRepository<User> users,
            IRepository<UserRole> userRoles,
            IRepository<Cart> carts,
            IRepository<CartItem> cartItems,
            IRepository<Order> ordersRepo,
            IRepository<OrderItem> orderItemsRepo,
            ISecurityService securityService)
        {
            _usersRepo = users;
            _userRolesRepo = userRoles;
            _cartsRepo = carts;
            _cartItemsRepo = cartItems;
            _ordersRepo = ordersRepo;
            _orderItemsRepo = orderItemsRepo;
            passwordService = securityService;
        }

        public async Task<int> AddRole(UserRole role)
        {
            return await _userRolesRepo.AddAsync(role);
        }

        public async Task ChangeUserRole(int userId, int roleId)
        {
            User user = await _usersRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new Exception("Cant find user");
            UserRole role = await _userRolesRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == roleId);
            if (role == null)
                throw new Exception("Cant find role");
            user.UserRoleId = role.Id;
            await _usersRepo.UpdateAsync(user);
        }

        public async Task<bool> CheckUserPassword(string login, string password)
        {
            User user = await _usersRepo.GetRepo().SingleOrDefaultAsync(x => x.login == login);
            if (user == null)
                return false;
            return passwordService.CheckPassword(password, user.password);
        }

        public async Task DeleteRole(UserRole role)
        {
            await _userRolesRepo.RemoveAsync(role);
        }

        public async Task DeleteUser(User user)
        {
            Cart cart = await _cartsRepo.GetRepo().Where(x => x.UserId == user.Id).Include(x => x.CartItems).FirstOrDefaultAsync();
            if (cart != null)
            {
                if (cart.CartItems != null && cart.CartItems.Count > 0)
                    await _cartItemsRepo.RemoveRangeAsync(cart.CartItems);
                await _cartsRepo.RemoveAsync(cart);
            }
            List<Order> orders = await _ordersRepo.GetRepo().Where(x => x.UserId == user.Id).Include(x => x.OrderItems).ToListAsync();
            foreach (var order in orders)
            {
                if (order.OrderItems != null && order.OrderItems.Count > 0)
                    await _orderItemsRepo.RemoveRangeAsync(order.OrderItems);
                await _ordersRepo.RemoveAsync(order);
            }
            await _usersRepo.RemoveAsync(user);
        }

        public async Task<User> GetUser(int id)
        {
            return await _usersRepo.GetRepo().Where(x => x.Id == id).Include(x => x.UserRole).FirstAsync();
        }

        public async Task<User> GetUserWithOrder(int id)
        {
            return await _usersRepo.GetRepo().Where(x => x.Id == id)
                .Include(x => x.UserRole)
                .Include(x => x.Orders)
                    .ThenInclude(x => x.OrderItems)
                        .ThenInclude(x=>x.item)
                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _usersRepo.GetRepo().Include(x => x.UserRole).ToListAsync();
        }

        public async Task<int> RegisterUser(User user)
        {
            var validation = await _usersRepo.GetRepo().SingleOrDefaultAsync(x => x.login == user.login);
            if (validation != null)
                throw new InvalidOperationException("Login should not be repeated");
            string hashedPassword = passwordService.HashPassword(user.password);
            user.password = hashedPassword;
            user.UserRoleId = 2;
            await _usersRepo.AddAsync(user);
            await _cartsRepo.AddAsync(new Cart() { Id = user.Id, UserId = user.Id });
            return user.Id;
        }

        public async Task UpdateRole(UserRole role)
        {
            await _userRolesRepo.UpdateAsync(role);
        }

        public async Task<bool> CheckLogin(string login)
        {
            User user = await _usersRepo.GetRepo().SingleOrDefaultAsync(x => x.login == login);
            if (user == null)
                return true;
            return false;
        }

        public async Task<User> GetUserByLogin(string login)
        {
            return await _usersRepo.GetRepo().Where(x => x.login == login).Include(x => x.UserRole).FirstAsync();
        }

        public async Task<int> UsersCount()
        {
            return await _usersRepo.GetRepo().CountAsync();
        }

        public async Task<IEnumerable<User>> GetUsers(int skip, int take)
        {
            return await _usersRepo.GetRepo().Include(x => x.Orders).Include(x => x.UserRole).Skip(skip).Take(take).ToListAsync();
        }
    }
}