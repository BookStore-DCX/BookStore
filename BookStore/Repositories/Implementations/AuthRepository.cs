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
            return await _context.Users
           .Include(u => u.RoleNumberNavigation)
           .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
        }
        public async Task<bool> UserExistsAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName);
        }
    }
}
