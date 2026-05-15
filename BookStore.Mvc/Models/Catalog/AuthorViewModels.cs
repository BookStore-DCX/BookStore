using System.ComponentModel.DataAnnotations;

namespace BookStore.Mvc.Models.Catalog;

public class AuthorViewModel
{
    public int AuthorId { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
}

public class AuthorFormViewModel
{
    public int AuthorId { get; set; }

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;
}
