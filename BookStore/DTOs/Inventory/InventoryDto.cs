namespace BookStore.DTOs.Inventory
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public int Ranks { get; set; }
        public string? ConditionDescription { get; set; }
        public byte? Purchased { get; set; }
        public decimal? Price { get; set; }
    }
}
