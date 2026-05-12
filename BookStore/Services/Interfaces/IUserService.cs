using BookStore.DTOs.User;

namespace BookStore.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> UpdateUserAsync(int id, UserUpdateDto dto);
    Task DeleteUserAsync(int id);
    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleNumber);
}
