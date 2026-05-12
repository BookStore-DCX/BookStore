using BookStore.Models;

namespace BookStore.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleNumber);
}
