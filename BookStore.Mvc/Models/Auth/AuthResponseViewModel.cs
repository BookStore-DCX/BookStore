namespace BookStore.Mvc.Models.Auth;

public class AuthResponseViewModel
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}
