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
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _ordersRepo;
        private readonly IRepository<OrderDeliveryMethod> _orderDeliveryMethodsRepo;
        private readonly IRepository<OrderPayMethod> _orderPayMethodsRepo;
        private readonly IRepository<OrderItem> _orderItemsRepo;
        private readonly IRepository<Cart> _cartsRepo;
        private readonly IRepository<CartItem> _cartItemsRepo;

        public OrderService(
            IRepository<Order> ordersRepo,
            IRepository<OrderDeliveryMethod> orderDeliveryMethodsRepo,
            IRepository<OrderPayMethod> orderPayMethodsRepo,
            IRepository<OrderItem> orderItemsRepo,
            IRepository<Cart> cartsRepo,
            IRepository<CartItem> cartItemsRepo)
        {
            _ordersRepo = ordersRepo;
            _orderDeliveryMethodsRepo = orderDeliveryMethodsRepo;
            _orderPayMethodsRepo = orderPayMethodsRepo;
            _orderItemsRepo = orderItemsRepo;
            _cartsRepo = cartsRepo;
            _cartItemsRepo = cartItemsRepo;
        }


        public async Task<int> AddDeliveryMethod(OrderDeliveryMethod item)
        {
            return await _orderDeliveryMethodsRepo.AddAsync(item);
        }

        public async Task<int> AddPayMethod(OrderPayMethod item)
        {
            return await _orderPayMethodsRepo.AddAsync(item);
        }

        public async Task<int> CreateOrder(Order order, int cartId)
        {
            var orderId = await _ordersRepo.AddAsync(order);
            Cart cart = await _cartsRepo.GetRepo().Where(x => x.Id == cartId).Include(x => x.CartItems).FirstOrDefaultAsync();
            if (cart == null)
                throw new Exception("Cant find cart");
            List<OrderItem> items = new List<OrderItem>();
            foreach (var cartItem in cart.CartItems)
            {
                items.Add(new OrderItem()
                {
                    ItemId = cartItem.ItemId,
                    quantity = cartItem.quantity,
                    OrderId = orderId
                });
            }
            await _cartItemsRepo.RemoveRangeAsync(cart.CartItems);
            await _orderItemsRepo.AddRangeAsync(items);
            return orderId;
        }

        public async Task DeleteOrder(int id)
        {
            Order order = await _ordersRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == id);
            if (order != null)
                await _ordersRepo.RemoveAsync(order);
        }

        public async Task RemoveDeliveryMethod(int id)
        {
            OrderDeliveryMethod deliveryMethod = await _orderDeliveryMethodsRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == id);
            if (deliveryMethod != null)
                await _orderDeliveryMethodsRepo.RemoveAsync(deliveryMethod);
        }

        public async Task RemovePayMethod(int id)
        {
            OrderPayMethod payMethod = await _orderPayMethodsRepo.GetRepo().SingleOrDefaultAsync(x => x.Id == id);
            if (payMethod != null)
                await _orderPayMethodsRepo.RemoveAsync(payMethod);
        }

        public async Task UpdateDeliveryMethod(OrderDeliveryMethod item)
        {
            await _orderDeliveryMethodsRepo.UpdateAsync(item);
        }

        public async Task UpdateOrder(Order order)
        {
            await _ordersRepo.UpdateAsync(order);
        }

        public async Task UpdatePayMethod(OrderPayMethod item)
        {
            await _orderPayMethodsRepo.UpdateAsync(item);
        }

        public async Task<Order> GetOrderDetailed(int id)
        {
            return await _ordersRepo.GetRepo().Where(x => x.Id == id)
                .Include(x => x.OrderDeliveryMethod)
                .Include(x => x.OrderPayMethod)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.item).FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrder(int id)
        {
            return await _ordersRepo.GetRepo().Where(x => x.Id == id)
                .Include(x => x.OrderDeliveryMethod)
                .Include(x => x.OrderPayMethod)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderDeliveryMethod>> GetOrderDeliveryMethods()
        {
            return await _orderDeliveryMethodsRepo.GetRepo().ToListAsync();
        }

        public async Task<IEnumerable<OrderPayMethod>> GetOrderPayMethods()
        {
            return await _orderPayMethodsRepo.GetRepo().ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrders(int skip, int take)
        {
            return await _ordersRepo.GetRepo().Include(x => x.User).Include(x => x.OrderDeliveryMethod).Include(x => x.OrderPayMethod).Include(x => x.OrderItems).ThenInclude(x => x.item).ToListAsync();
        }

        public async Task<int> OrdersCount()
        {
            return await _ordersRepo.GetRepo().CountAsync();
        }
    }
}
