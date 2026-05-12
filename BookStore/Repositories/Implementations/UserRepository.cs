using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(BookContext context) : base(context)
    {
    }

    public async Task<User?> GetByUserNameAsync(string userName)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName);

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleNumber)
        => await _dbSet.AsNoTracking().Where(u => u.RoleNumber == roleNumber).ToListAsync();
}
