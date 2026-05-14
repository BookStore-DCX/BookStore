using BookStore.Data;
using BookStore.DTOs.User;
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

        private IQueryable<UserDto> UserWithRoleNameQuery()
        {
            return from u in _context.Users.AsNoTracking()
                   join r in _context.Permroles.AsNoTracking()
                       on u.RoleNumber equals r.RoleNumber into roles
                   from r in roles.DefaultIfEmpty()
                   select new UserDto
                   {
                       UserId = u.UserId,
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       UserName = u.UserName,
                       PhoneNumber = u.PhoneNumber,
                       RoleNumber = u.RoleNumber,
                       RoleName = r != null ? r.PermRole1 : null
                   };
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(params object[] id)
        {
            if (id.Length == 0 || id[0] is not int userId)
            {
                return null;
            }

            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleNumber)
        {
            return await _dbSet.AsNoTracking()
                .Where(u => u.RoleNumber == roleNumber)
                .ToListAsync();
        }

        public async Task<bool> RoleExistsAsync(int roleNumber)
        {
            return await _context.Permroles.AnyAsync(r => r.RoleNumber == roleNumber);
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            return await _context.Permroles.AnyAsync(r => r.PermRole1 == roleName);
        }

        public async Task<IEnumerable<UserDto>> GetAllWithRoleNameAsync()
        {
            return await UserWithRoleNameQuery().ToListAsync();
        }

        public async Task<UserDto?> GetByIdWithRoleNameAsync(int userId)
        {
            return await UserWithRoleNameQuery().FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<UserDto?> GetUserByUsernameWithRoleNameAsync(string username)
        {
            return await UserWithRoleNameQuery().FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleWithRoleNameAsync(int roleNumber)
        {
            return await UserWithRoleNameQuery()
                .Where(u => u.RoleNumber == roleNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleNameAsync(string roleName)
        {
            return await UserWithRoleNameQuery()
                .Where(u => u.RoleName == roleName)
                .ToListAsync();
        }
    }
}