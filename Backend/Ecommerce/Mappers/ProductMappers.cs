using Ecommerce.Dto.Product;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public static class ProductMappers
    {
        public static ProductDto ToProductDto(this Product productModel)
        {
            return new ProductDto
            {
                Id = productModel.Id,
                ProductName = productModel.ProductName,
                CategoryId = productModel.CategoryId,
                CategoryName = productModel.Category?.Name ?? "No Category",
                Price = productModel.Price,
                Stock = productModel.Stock,
                ImageUrl = productModel.ImageUrl
            };
        }

        public static Product ToProductFromCreateDto(this CreateProductDto addDto)
        {
            return new Product
            {
                ProductName = addDto.ProductName,
                CategoryId = addDto.CategoryId,
                Price = addDto.Price,
                Stock = addDto.Stock,
                ImageUrl = addDto.ImageUrl
            };
        }

        public static void UpdateProductFromDto(this Product existingProduct, UpdateProductDto updateDto)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.ProductName))
            {
                existingProduct.ProductName = updateDto.ProductName;
            }

            if (updateDto.CategoryId.HasValue)
            {
                existingProduct.CategoryId = updateDto.CategoryId.Value;
            }

            if (updateDto.Price.HasValue)
            {
                existingProduct.Price = updateDto.Price.Value;
            }

            if (updateDto.Stock.HasValue)
            {
                existingProduct.Stock = updateDto.Stock.Value;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.ImageUrl))
            {
                existingProduct.ImageUrl = updateDto.ImageUrl;
            }
        }
    }
}
