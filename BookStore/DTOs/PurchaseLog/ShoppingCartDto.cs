namespace BookStore.DTOs.PurchaseLog
{
    public class ShoppingCartDto
    {
        public int UserId { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public string? BookTitle { get; set; }
        public int? InventoryId { get; set; }
        public string? Condition { get; set; }
        public decimal? Price { get; set; }
        public List<ShoppingCartCopyDto> AvailableCopies { get; set; } = new();
    }

    public class ShoppingCartCopyDto
    {
        public int InventoryId { get; set; }
        public string? Condition { get; set; }
        public decimal? Price { get; set; }
    }
}
