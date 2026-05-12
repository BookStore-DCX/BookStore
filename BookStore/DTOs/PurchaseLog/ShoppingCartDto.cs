namespace BookStore.DTOs.PurchaseLog
{
    public class ShoppingCartDto
    {
        public int UserId { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public string? BookTitle { get; set; }
    }
}
