using Ecommerce.Data;
using Ecommerce.Dto.Order;
using Ecommerce.Helpers;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ecommerce.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Order> CheckoutAsync(string userId, List<int> SelectedCartItemIds)
        {

            if (SelectedCartItemIds == null || !SelectedCartItemIds.Any())
            {
                throw new Exception("Checkout failed: No items were received from the cart.");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Checkout failed: We could not identify the logged-in user.");
            }

            var selectedCartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.AppUserId == userId && SelectedCartItemIds.Contains(c.Id))
                .ToListAsync();

            if (selectedCartItems.Count == 0)
            {

               throw new Exception("No valid items selected for checkout.");
            }

            foreach (var item in selectedCartItems)
            {

                if (item.Product == null)
                {
                    throw new Exception($"Checkout failed. A product in your cart (Cart ID: {item.Id}) no longer exists in our system.");
                }

                if (item.Product.Stock < item.Quantity)
                {
                    throw new Exception($"Checkout failed. '{item.Product.ProductName}' only has {item.Product.Stock} left in stock.");
                }
            }

            decimal orderTotal = selectedCartItems.Sum(item => item.Product.Price * item.Quantity);

            var newOrder = new Order
            {
                AppUserId = userId,
                TotalPrice = orderTotal,
                DateOrdered = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            foreach (var cartItem in selectedCartItems)
            {
                var orderItem = cartItem.ToOrderItem(newOrder.Id);
                _context.OrderItems.Add(orderItem);

                cartItem.Product.Stock -= cartItem.Quantity;
            }

            _context.CartItems.RemoveRange(selectedCartItems);

            await _context.SaveChangesAsync();

            return await _context.Orders
                .Include(o => o.AppUser)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == newOrder.Id);
        }

        public async Task<List<Order>> GetAllOrdersAsync(OrderQueryObject query)
        {
            var orders = _context.Orders
             .Include(o => o.AppUser)
             .Include(o => o.OrderItems)
             .ThenInclude(i => i.Product)
             .AsQueryable();

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await orders.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<List<Order>> GetAllUserOrdersAsync(string userId, OrderQueryObject query)
        {

           var orders = _context.Orders
            .Where(o => o.AppUserId == userId)
            .Include(o => o.AppUser)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .AsQueryable();

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await orders.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
            .Include(o => o.AppUser)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);


        }
    }
}
