using Ecommerce.Dto.Category;
using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CreateCategoryDto createCatDto);
        Task<Category> UpdateCategoryAsync(int id, UpdateCategoryDto updateCatDto);
        Task<Category> DeleteCategoryAsync(int id);
    }
}
