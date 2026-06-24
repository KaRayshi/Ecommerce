using Ecommerce.Dto.Product;
using Ecommerce.Helpers;
using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(ProductQueryObject query);
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(CreateProductDto createDto);
        Task<Product?> UpdateProductAsync(int Id, UpdateProductDto updateProduct);
        Task<Product?> DeleteProductAsync(int Id);
        Task<bool> IsNameTakenAsync(string productName, int excludeId);
    }
}
