using System.ComponentModel.DataAnnotations;

namespace BookStore.Mvc.Models.Catalog;

public class ReviewViewModel
{
    public string Isbn { get; set; } = string.Empty;
    public string? BookName { get; set; }
    public int ReviewerId { get; set; }
    public int? Rating { get; set; }
    public string? Comments { get; set; }
}

public class ReviewCreateViewModel
{
    [Required]
    public string Isbn { get; set; } = string.Empty;

    [Range(1, 10)]
    public int? Rating { get; set; }

    public string? Comments { get; set; }
}
