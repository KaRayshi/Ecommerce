using Ecommerce.Helpers;
using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllUserOrdersAsync(string userId, OrderQueryObject query);
        Task<List<Order>> GetAllOrdersAsync(OrderQueryObject query);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CheckoutAsync(string userId, List<int> cartItemIds);
    }
}
