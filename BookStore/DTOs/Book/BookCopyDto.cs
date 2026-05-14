namespace BookStore.DTOs.Book
{
    public class BookCopyDto
    {
        public int InventoryId { get; set; }
        public string? Condition { get; set; }
        public decimal? Price { get; set; }
    }
}