using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetUsersByRoleAsync(int roleNumber);
        Task<bool> RoleExistsAsync(int roleNumber);
    }
}
