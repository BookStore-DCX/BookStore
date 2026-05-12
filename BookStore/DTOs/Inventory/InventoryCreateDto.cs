namespace BookStore.DTOs.Inventory
{
    public class InventoryCreateDto
    {
        public string Isbn { get; set; } = string.Empty;
        public int Ranks { get; set; }
    }
}
