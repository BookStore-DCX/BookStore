using BookStore.DTOs.User;
using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
public interface IUserRepository : IGenericRepository<User>
{
        Task<User?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleNumber);
        Task<bool> RoleExistsAsync(int roleNumber);

        Task<IEnumerable<UserDto>> GetAllWithRoleNameAsync();
        Task<UserDto?> GetByIdWithRoleNameAsync(int userId);
        Task<UserDto?> GetUserByUsernameWithRoleNameAsync(string username);
        Task<IEnumerable<UserDto>> GetUsersByRoleWithRoleNameAsync(int roleNumber);
    }
}
