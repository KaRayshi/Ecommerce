using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Column(TypeName = "Decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime DateOrdered { get; set; } = DateTime.Now;
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
