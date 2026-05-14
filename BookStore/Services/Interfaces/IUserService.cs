using BookStore.DTOs.User;

namespace BookStore.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName);
        Task<UserDto> UpdateUserAsync(string username, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(string username);
    }
}
