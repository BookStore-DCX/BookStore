using System.ComponentModel.DataAnnotations;

namespace BookStore.Mvc.Models.Auth;

public class UserViewModel
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int? RoleNumber { get; set; }
    public string? RoleName { get; set; }
}

public class UserUpdateViewModel
{
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Role")]
    public int? RoleNumber { get; set; }
}
