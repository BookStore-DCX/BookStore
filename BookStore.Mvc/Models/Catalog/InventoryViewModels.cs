using System.ComponentModel.DataAnnotations;

namespace BookStore.Mvc.Models.Catalog;

public class InventoryViewModel
{
    public int InventoryId { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public int Ranks { get; set; }
    public string? ConditionDescription { get; set; }
    public byte? Purchased { get; set; }
    public decimal? Price { get; set; }
}

public class InventoryFormViewModel
{
    public int InventoryId { get; set; }

    [Required]
    [Display(Name = "ISBN")]
    public string Isbn { get; set; } = string.Empty;

    [Display(Name = "Condition")]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a condition.")]
    public int Ranks { get; set; }

    public byte? Purchased { get; set; }
}
