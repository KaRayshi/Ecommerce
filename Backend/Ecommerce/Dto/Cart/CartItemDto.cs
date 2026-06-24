using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dto.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "Product Name cannot be over 20 characters")]
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}