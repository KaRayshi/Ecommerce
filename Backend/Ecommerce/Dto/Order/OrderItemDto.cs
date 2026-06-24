namespace Ecommerce.Dto.Order
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal HistoricalPrice { get; set; } 
        public decimal TotalPrice { get; set; } 
    }
}
