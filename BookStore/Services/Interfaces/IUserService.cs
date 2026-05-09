using BookStore.DTOs.User;

namespace BookStore.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<UserResponseDto> GetUserByUsernameAsync(string username);
        Task<UserResponseDto> UpdateUserAsync(int userId, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(int roleNumber);
    }
}
