using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly BookContext _context;

        public AuthRepository(BookContext context)
        {
            _context = context;
        }
        public async Task<User?> ValidateUserAsync(string userName, string password)
        {
            var user = await _context.Users
                .Include(u => u.RoleNumberNavigation)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            if (user.Password.StartsWith("$2a$") || user.Password.StartsWith("$2b$") || user.Password.StartsWith("$2y$"))
            {
                return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
            }

            return user.Password == password ? user : null;
        }
        public async Task<bool> UserExistsAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName);
        }
    }
}
