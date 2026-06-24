namespace Ecommerce.Helpers
{
    public class ProductQueryObject
    {
        public string? Name { get; set; } = string.Empty;
        public string? Category { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
