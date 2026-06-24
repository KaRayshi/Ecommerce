namespace Ecommerce.Helpers
{
    public class CartItemQueryObject
    {
        public string? AppUserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
