namespace BookStore.Mvc.Models.Catalog;

public class ShoppingCartItemViewModel
{
    public int UserId { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string? BookTitle { get; set; }
}

public class ShoppingCartCreateViewModel
{
    public int UserId { get; set; }
    public string Isbn { get; set; } = string.Empty;
}

public class PurchaseLogViewModel
{
    public int UserId { get; set; }
    public int InventoryId { get; set; }
}
