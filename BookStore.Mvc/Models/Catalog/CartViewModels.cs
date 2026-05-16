namespace BookStore.Mvc.Models.Catalog;

public class ShoppingCartItemViewModel
{
    public int UserId { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string? BookTitle { get; set; }
    public int? InventoryId { get; set; }
    public string? Condition { get; set; }
    public decimal? Price { get; set; }
    public List<ShoppingCartCopyViewModel> AvailableCopies { get; set; } = new();
    public List<int> SelectedInventoryIds { get; set; } = new();
}

public class ShoppingCartCopyViewModel
{
    public int InventoryId { get; set; }
    public string? Condition { get; set; }
    public decimal? Price { get; set; }
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
    public string? BookTitle { get; set; }
    public string? AuthorName { get; set; }
    public string? Condition { get; set; }
    public decimal? Price { get; set; }
}
