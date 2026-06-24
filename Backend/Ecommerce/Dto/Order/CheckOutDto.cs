using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dto.Order
{
    public class CheckoutDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "You must select at least one item to checkout.")]
        public List<int> SelectedCartItemIds { get; set; } = new List<int>();
    }
}