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

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    public string? City { get; set; }

    [Display(Name = "State")]
    [Required(ErrorMessage = "State is required.")]
    public string? StateCode { get; set; }
}
