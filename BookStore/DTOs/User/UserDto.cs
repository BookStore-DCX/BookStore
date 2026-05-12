namespace BookStore.DTOs.User
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? RoleNumber { get; set; }
        public string? RoleName { get; set; }
    }
}
