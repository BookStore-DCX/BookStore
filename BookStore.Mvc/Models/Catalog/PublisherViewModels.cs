using System.ComponentModel.DataAnnotations;

namespace BookStore.Mvc.Models.Catalog;

public class PublisherViewModel
{
    public int PublisherId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? StateCode { get; set; }
    public string? StateName { get; set; }
}

public class PublisherFormViewModel
{
    public int PublisherId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? City { get; set; }

    [Display(Name = "State")]
    public string? StateCode { get; set; }
}
