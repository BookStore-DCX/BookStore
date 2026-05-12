using BookStore.DTOs.User;

namespace BookStore.Services.Interfaces
{
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int userId);
    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleNumber);
}
}
