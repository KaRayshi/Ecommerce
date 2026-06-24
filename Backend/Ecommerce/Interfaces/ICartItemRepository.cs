using Ecommerce.Dto.Cart;
using Ecommerce.Helpers;
using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetAllItemAsync(CartItemQueryObject query);
        Task<CartItem> AddToCartAsync(string userId, AddToCartDto addToCartDto);
        Task<CartItem> UpdateCartItemQuantityAsync(string userID, int Id, UpdateQuantityDto updateQuantityDto);
        Task<CartItem> DeleteCartItemAsync(string userID, int Id);
        Task<Product?> IsProductExistingAsync(int id);

    }
}
