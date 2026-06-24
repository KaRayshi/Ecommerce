using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dto.Cart
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        [Required]
        [Range(1, 100000)]
        public int Quantity { get; set; }
    }
}
