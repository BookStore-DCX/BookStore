namespace BookStore.DTOs.PurchaseLog
{
    public class PurchaseLogDto
    {
        public int UserId { get; set; }
        public int InventoryId { get; set; }
        public string? BookTitle { get; set; }
        public string? AuthorName { get; set; }
        public string? Condition { get; set; }
        public decimal? Price { get; set; }
    }
}
