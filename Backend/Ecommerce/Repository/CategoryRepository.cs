using Ecommerce.Data;
using Ecommerce.Dto.Category;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDto createCatDto)
        {
            var existingCategory = await _context.Categories.AnyAsync(c => c.Name.ToLower() == createCatDto.Name.ToLower());

            if (existingCategory)
            {
                return null;
            }

            var category = createCatDto.ToCategoryFromCreateDto();
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
           var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

           if(existingCategory == null)
            {
                return null;
            }

            _context.Categories.Remove(existingCategory);
            await _context.SaveChangesAsync();

            return existingCategory;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
           var categories = await _context.Categories.ToListAsync();

            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int catId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == catId);

            if(category == null)
            {
                return null;
            }

            var catDto = category.ToCategoryDto();

            return category;
        }

        public async Task<Category> UpdateCategoryAsync(int id, UpdateCategoryDto updateCatDto)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if(existingCategory == null)
            {
                return null;
            }

            existingCategory.ToCategoryFromUpdateDto(updateCatDto);
            await _context.SaveChangesAsync();

            return existingCategory;

        }
    }
}
