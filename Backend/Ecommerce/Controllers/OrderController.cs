using Ecommerce.Data;
using Ecommerce.Dto.Order;
using Ecommerce.Helpers;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutDto checkoutDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var completeOrder = await _orderRepository.CheckoutAsync(userId, checkoutDto.SelectedCartItemIds);
                var receiptDto = completeOrder.ToOrderDto();
                return Ok(receiptDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }          
        }

        [HttpGet("view_user_orders")]
        [Authorize]
        public async Task<IActionResult> GetAllUserOrders([FromQuery] OrderQueryObject query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User identifier not found in token.");
            }

            var orders = await _orderRepository.GetAllUserOrdersAsync(userId, query);

            var allReceipts = orders.Select(o => o.ToOrderDto()).ToList();

            return Ok(allReceipts);
        }

        [HttpGet("view_all_orders")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderQueryObject query)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(query);

            var allReceipts = orders.Select(o => o.ToOrderDto()).ToList();

            return Ok(allReceipts);
        }

        [HttpGet("view_order/{id}")]
        [Authorize(Roles = "Admin")] // You can uncomment this if you strictly enforce roles!
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var orderDto = order.ToOrderDto();

            return Ok(orderDto);
        }
    }
}