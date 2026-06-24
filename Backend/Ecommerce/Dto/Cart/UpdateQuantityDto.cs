using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Dto.Cart
{
    public class UpdateQuantityDto
    {
        [Range(1, 100000)]
        public int Quantity { get; set; }
    }
}
 