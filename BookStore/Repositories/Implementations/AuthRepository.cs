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

        private IQueryable<User> UserWithRoleQuery()
        {
            return from u in _context.Users.AsNoTracking()
                   join r in _context.Permroles.AsNoTracking()
                       on u.RoleNumber equals r.RoleNumber into roles
                   from r in roles.DefaultIfEmpty()
                   select new User
                   {
                       UserId = u.UserId,
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       PhoneNumber = u.PhoneNumber,
                       UserName = u.UserName,
                       Password = u.Password,
                       RoleNumber = u.RoleNumber,
                       RoleNumberNavigation = r
                   };
        }
        public async Task<User?> ValidateUserAsync(string userName, string password)
        {
             var user = await UserWithRoleQuery()
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
