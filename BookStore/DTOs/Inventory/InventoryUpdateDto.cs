namespace BookStore.DTOs.Inventory
{
    public class InventoryUpdateDto
    {
        public string? Isbn { get; set; }
        public int? Ranks { get; set; }
        public byte? Purchased { get; set; }
    }
}