using Ecommerce.Data;
using Ecommerce.Dto.Product;
using Ecommerce.Helpers;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<Product> CreateProductAsync(CreateProductDto createDto)
        {
            var existingProduct = await _context.Products.AnyAsync(p => p.ProductName.ToLower() == createDto.ProductName.ToLower());

            if (existingProduct)
            {
                return null;
            }

            var product = createDto.ToProductFromCreateDto();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            product.Category = await _context.Categories.FindAsync(product.CategoryId);

            return product;

        }

        public async Task<Product?> DeleteProductAsync(int Id)
        {
            var existingProduct = await _context.Products.FindAsync(Id);

            if(existingProduct == null)
            {
                return null;
            }

            //_context.Products.Remove(existingProduct);
            existingProduct.IsArchived = true;
            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<List<Product>> GetAllAsync(ProductQueryObject query)
        {
            var products = _context.Products.Where(p => p.IsArchived == false).Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                products = products.Where(p => p.ProductName.Contains(query.Name));
            }

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                products = products.Where(p => p.Category.Name.Contains(query.Category));
            }

            if (query.MinPrice.HasValue)
            {
                products = products.Where(p => p.Price >= query.MinPrice);
            }

            if (query.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= query.MaxPrice);
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if(query.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = query.IsDescending ? products.OrderByDescending(p => p.ProductName) : products.OrderBy(p => p.ProductName);
                }

                if (query.SortBy.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    products = query.IsDescending ? products.OrderByDescending(p => p.Category.Name) : products.OrderBy(p => p.Category.Name);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await products.Skip(skipNumber).Take(query.PageSize).ToListAsync();

        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            return product;
        }

        public async Task<bool> IsNameTakenAsync(string productName, int excludeId)
        {
            return await _context.Products.AnyAsync(p => p.ProductName.ToLower() == productName.ToLower() && p.Id != excludeId);
        }

        public async Task<Product?> UpdateProductAsync(int Id, UpdateProductDto updateDto)
        {

            var existingProduct = await _context.Products.FindAsync(Id);


            existingProduct.UpdateProductFromDto(updateDto);

            await _context.SaveChangesAsync();

            return existingProduct;
        }
    }
}
