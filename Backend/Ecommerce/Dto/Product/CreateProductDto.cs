using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dto.Product
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Product Name cannot be over 20 characters")]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        [Range(1, 100000)]
        public int CategoryId { get; set; }
        [Required]
        [Range(1, 100000)]
        public decimal Price { get; set; }
        [Required]
        [Range(1, 100000)]
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
