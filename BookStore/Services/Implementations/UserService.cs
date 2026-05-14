using AutoMapper;
using BookStore.DTOs.User;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uow = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _uow.Users.GetAllWithRoleNameAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _uow.Users.GetByIdWithRoleNameAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            return user;
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _uow.Users.GetUserByUsernameWithRoleNameAsync(username);
            if (user == null)
            {
                throw new KeyNotFoundException($"User '{username}' not found");
            }

            return user;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            var roleExists = await _uow.Users.RoleNameExistsAsync(roleName);
            if (!roleExists)
            {
                throw new KeyNotFoundException($"Role '{roleName}' not found");
            }

            return await _uow.Users.GetUsersByRoleNameAsync(roleName);
        }

        public async Task<UserDto> UpdateUserAsync(string username, UserUpdateDto dto)
        {
            var user = await _uow.Users.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new KeyNotFoundException($"User '{username}' not found");
            }

            _mapper.Map(dto, user);

            if (dto.RoleNumber.HasValue)
            {
                var roleExists = await _uow.Users.RoleExistsAsync(dto.RoleNumber.Value);
                if (!roleExists)
                {
                    throw new KeyNotFoundException($"Role number {dto.RoleNumber.Value} not found");
                }
            }

            await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Users.GetUserByUsernameWithRoleNameAsync(username);
            if (updated == null)
            {
                throw new KeyNotFoundException($"User '{username}' not found");
            }

            return updated;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            var user = await _uow.Users.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new KeyNotFoundException($"User '{username}' not found");
            }

            await _uow.Users.DeleteAsync(user.UserId);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}