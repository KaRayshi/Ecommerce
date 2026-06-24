using Ecommerce.Dto.Cart;
using Ecommerce.Mappers;
using Ecommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Interfaces;
using Ecommerce.Helpers;
// 1. ADD THESE TWO NEW USINGS FOR SECURITY!
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;
        public CartController(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        [HttpPost("add_item")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ;

            var product = await _cartItemRepository.IsProductExistingAsync(addDto.ProductId);

            if (product == null)
            {
                return NotFound("Product Not Found in the Category");
            }

            if (product.Stock < addDto.Quantity)
            {
                return BadRequest("Stock Insufficient");
            }

            var CartItem = await _cartItemRepository.AddToCartAsync(userId, addDto);

            return Ok("Item successfully added to your cart!");
        }

        [HttpGet("view_cart")]
        [Authorize]
        public async Task<IActionResult> ViewCart([FromQuery]CartItemQueryObject query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            query.AppUserId = userId;

            var cartItems = await _cartItemRepository.GetAllItemAsync(query);

            var cartDtos = cartItems.Select(c => c.ToCartItemDto()).ToList();

            return Ok(cartDtos);
        }

        [HttpPut("update_quantity/{Id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItemQuantity(int Id, [FromBody] UpdateQuantityDto updatedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _cartItemRepository.UpdateCartItemQuantityAsync(userId, Id, updatedDto);

            if(cartItem == null)
            {
                return NotFound("Item Not Found");
            }

            return Ok("Item Updated");
        }

        [HttpDelete("delete_item/{Id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteCartItem(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _cartItemRepository.DeleteCartItemAsync(userId, Id);
            if(cartItem == null)
            {
                return NotFound("Item not found in your cart.");
            }
            return Ok("Item Removed From cart");
        }
    }
}
