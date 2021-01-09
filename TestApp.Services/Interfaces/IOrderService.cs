using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> AddDeliveryMethod(OrderDeliveryMethod item);
        Task UpdateDeliveryMethod(OrderDeliveryMethod item);
        Task RemoveDeliveryMethod(int id);
        Task<int> AddPayMethod(OrderPayMethod item);
        Task UpdatePayMethod(OrderPayMethod item);
        Task RemovePayMethod(int id);
        Task<int> CreateOrder(Order order, int cartId);
        Task UpdateOrder(Order order);
        Task DeleteOrder(int id);
        Task<Order> GetOrder(int id);
        Task<Order> GetOrderDetailed(int id);
        Task<IEnumerable<OrderDeliveryMethod>> GetOrderDeliveryMethods();
        Task<IEnumerable<OrderPayMethod>> GetOrderPayMethods();
        Task<IEnumerable<Order>> GetOrders(int skip, int take);
        Task<int> OrdersCount();
    }
}
