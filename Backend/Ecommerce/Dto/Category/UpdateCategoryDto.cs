namespace Ecommerce.Dto.Category
{
    public class UpdateCategoryDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty; // Added this!
    }
}
