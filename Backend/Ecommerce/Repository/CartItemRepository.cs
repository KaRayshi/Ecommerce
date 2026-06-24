using Ecommerce.Data;
using Ecommerce.Dto.Cart;
using Ecommerce.Helpers;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<CartItem> AddToCartAsync(string userID, AddToCartDto addToCartDto)
        {
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == addToCartDto.ProductId
                && c.AppUserId == userID);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += addToCartDto.Quantity;
                await _context.SaveChangesAsync();
                return existingCartItem;
            }
 
            var newCartItem = addToCartDto.ToCartItemFromAddDto();
            newCartItem.AppUserId = userID;

            _context.CartItems.Add(newCartItem);
            await _context.SaveChangesAsync();

            return newCartItem;

        }

        public async Task<CartItem> DeleteCartItemAsync(string userID, int Id)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == Id && c.AppUserId == userID);

            if (cartItem == null)
            {
                return null;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<List<CartItem>> GetAllItemAsync(CartItemQueryObject query)
        {
           var items = _context.CartItems.Include(c => c.Product).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.AppUserId))
            {
                items = items.Where(i => i.AppUserId == query.AppUserId);
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await items.Skip(skipNumber).Take(query.PageSize).ToListAsync();   
        }

        public async Task<Product?> IsProductExistingAsync(int id)
        {
           return await _context.Products.FindAsync(id);
        }

        public async Task<CartItem> UpdateCartItemQuantityAsync(string userID, int Id, UpdateQuantityDto updateQuantityDto)
        {
            var cartItem = await _context.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == Id && c.AppUserId == userID);

            if (cartItem == null)
            {
                return null;
            }

            if (updateQuantityDto.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return cartItem;
            }

            if (updateQuantityDto.Quantity > cartItem.Product.Stock)
            {
                updateQuantityDto.Quantity = cartItem.Product.Stock;
            }

            cartItem.UpdateCartItemFromDto(updateQuantityDto);
            await _context.SaveChangesAsync();

            return cartItem;
        }
    }
}
