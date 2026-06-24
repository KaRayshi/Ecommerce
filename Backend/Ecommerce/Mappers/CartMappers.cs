using Ecommerce.Dto.Cart;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public static class CartMappers
    {
        public static CartItem ToCartItemFromAddDto(this AddToCartDto addDto)
        {
            return new CartItem
            {
                ProductId = addDto.ProductId,
                Quantity = addDto.Quantity
            };
        }

        public static CartItemDto ToCartItemDto(this CartItem cartItemModel)
        {
            return new CartItemDto
            {
                Id = cartItemModel.Id,
                ProductId = cartItemModel.ProductId,
                Quantity = cartItemModel.Quantity,
                ProductName = cartItemModel.Product?.ProductName?? "Unknown Product",
                ProductPrice = cartItemModel.Product?.Price?? 0m,
                TotalPrice = (cartItemModel.Product?.Price ?? 0m) * cartItemModel.Quantity,
                ImageUrl = cartItemModel.Product?.ImageUrl ?? string.Empty
            };
        }

        public static void UpdateCartItemFromDto(this CartItem existingItem, UpdateQuantityDto updateDto)
        {
            existingItem.Quantity = updateDto.Quantity;
        }
    }
}
