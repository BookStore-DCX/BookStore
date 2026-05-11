using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(BookContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .Include(u => u.RoleNumberNavigation)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(params object[] id)
        {
            if (id.Length == 0 || id[0] is not int userId)
                return null;

            return await _dbSet
                .Include(u => u.RoleNumberNavigation)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dbSet
                .Include(u => u.RoleNumberNavigation)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleNumber)
        {
            return await _dbSet
                .Include(u => u.RoleNumberNavigation)
                .Where(u => u.RoleNumber == roleNumber)
                .ToListAsync();
        }
        public async Task<bool> RoleExistsAsync(int roleNumber)
        {
            return await _context.Permroles.AnyAsync(r => r.RoleNumber == roleNumber);
        }
    }

}