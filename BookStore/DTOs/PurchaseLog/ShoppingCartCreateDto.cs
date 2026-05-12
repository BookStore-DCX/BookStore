namespace BookStore.DTOs.PurchaseLog
{
    public class ShoppingCartCreateDto
    {
        public int UserId { get; set; }
        public string Isbn { get; set; } = string.Empty;
    }
}
