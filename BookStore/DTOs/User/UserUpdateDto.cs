namespace BookStore.DTOs.User
{
    public class UserUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? RoleNumber { get; set; }
    }

}
