using Ecommerce.Dto.Category;
using Ecommerce.Models;

namespace Ecommerce.Mappers
{
    public static class CategoryMappers
    {
        public static CategoryDto ToCategoryDto(this Category categoryModel)
        {
            return new CategoryDto
            {
                Id = categoryModel.Id,
                Name = categoryModel.Name,
                ImageUrl = categoryModel.ImageUrl // Map the ImageUrl going OUT
            };
        }

        public static Category ToCategoryFromCreateDto(this CreateCategoryDto createDto)
        {
            return new Category
            {
                Name = createDto.Name,
                ImageUrl = createDto.ImageUrl // Map the ImageUrl going IN
            };
        }

        public static void ToCategoryFromUpdateDto(this Category categoryModel, UpdateCategoryDto updateDto)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                categoryModel.Name = updateDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.ImageUrl))
            {
                categoryModel.ImageUrl = updateDto.ImageUrl;
            }
        }
    }
}