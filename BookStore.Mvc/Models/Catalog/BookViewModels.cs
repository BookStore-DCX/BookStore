using System.ComponentModel.DataAnnotations;

namespace BookStore.Mvc.Models.Catalog;

public class BookListItemViewModel
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Category { get; set; }
    public string? CategoryName { get; set; }
    public string? Edition { get; set; }
    public int PublisherId { get; set; }
    public string? PublisherName { get; set; }
    public int InventoryCount { get; set; }
}

public class BookDetailViewModel
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Publisher { get; set; }
    public List<string> Authors { get; set; } = new();
    public List<BookCopyViewModel> Copies { get; set; } = new();
    public List<ReviewViewModel> Reviews { get; set; } = new();
}

public class BookCopyViewModel
{
    public int InventoryId { get; set; }
    public int Ranks { get; set; }
    public string? ConditionDescription { get; set; }
    public decimal? Price { get; set; }
    public byte? Purchased { get; set; }
}

public class BookFormViewModel
{
    [Required]
    [Display(Name = "ISBN")]
    public string Isbn { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Display(Name = "Category")]
    public int? Category { get; set; }

    public string? Edition { get; set; }

    [Display(Name = "Publisher")]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a publisher.")]
    public int PublisherId { get; set; }
}
