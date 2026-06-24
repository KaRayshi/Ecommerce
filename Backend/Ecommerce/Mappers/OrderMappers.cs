using Ecommerce.Models;
using Ecommerce.Dto.Order;
using System.Linq;

namespace Ecommerce.Mappers
{
    public static class OrderMappers
    {
        public static OrderItem ToOrderItem(this CartItem cartItem, int orderId)
        {
            return new OrderItem
            {
                OrderId = orderId, 
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                HistoricalPrice = cartItem.Product?.Price ?? 0m
            };
        }

        public static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductName = orderItem.Product?.ProductName ?? "Unknown Product",
                Quantity = orderItem.Quantity,
                HistoricalPrice = orderItem.HistoricalPrice,
                TotalPrice = orderItem.Quantity * orderItem.HistoricalPrice,
                ImageUrl = orderItem.Product.ImageUrl
            };
        }

        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto
            {
                //ShippingAddress
                CustomerName = order.AppUser.FirstName + " " + order.AppUser.LastName,
                Id = order.Id,
                TotalPrice = order.TotalPrice,
                DateOrdered = order.DateOrdered,
                OrderItems = order.OrderItems.Select(item => item.ToOrderItemDto()).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}