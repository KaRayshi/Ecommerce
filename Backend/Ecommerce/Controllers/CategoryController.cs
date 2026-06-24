using Ecommerce.Dto.Category;
using Ecommerce.Interfaces;
using Ecommerce.Mappers;
using Ecommerce.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost("Create")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategory)
        {
            var category = await _categoryRepository.CreateCategoryAsync(createCategory);

            if (category == null)
            {
                return BadRequest("Category Invalid");
            }

            return Ok(category.ToCategoryDto());
        }

        [HttpGet("View_Category")]
        public async Task<IActionResult> GetAllCategory()
        {
            var category = await _categoryRepository.GetCategoriesAsync();

            if (category.Count == 0)
            {
                return BadRequest("No Category Available");
            }

            var categoryDto = category.Select(c => c.ToCategoryDto());
            return Ok(categoryDto);
        }

        [HttpGet("View_Category/{id:int}")]
        public async Task<IActionResult> GetAllCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return BadRequest("No Category Available");
            }

            return Ok(category);
        }

        [HttpPut("Update_Category/{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto updateCategory, int id)
        {
            var category = await _categoryRepository.UpdateCategoryAsync(id, updateCategory);

            if(category == null)
            {
                return BadRequest("No Category Available");
            }

            return Ok(category.ToCategoryDto());
        }

        [HttpDelete("Delete_Category/{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.DeleteCategoryAsync(id);

            if (category == null)
            {
                return NotFound("Category Not Found");
            }
            return Ok("Cateogry Deleted");
        }

    }
}
