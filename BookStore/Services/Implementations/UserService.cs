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
                throw new KeyNotFoundException($"User with ID {userId} not found");

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

        public async Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            _mapper.Map(dto, user);

            if (dto.RoleNumber.HasValue)
            {
                var roleExists = await _uow.Users.RoleExistsAsync(dto.RoleNumber.Value);
                if (!roleExists)
                    throw new KeyNotFoundException($"Role number {dto.RoleNumber.Value} not found");
            }

            await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();

            var updated = await _uow.Users.GetByIdWithRoleNameAsync(userId);
            if (updated == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            return updated;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            bool isExist = await _uow.Users.ExistsAsync(userId);
            if (isExist == false)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            await _uow.Users.DeleteAsync(userId);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(int roleNumber)
        {
            var roleExists = await _uow.Users.RoleExistsAsync(roleNumber);
            if (!roleExists)
                throw new KeyNotFoundException($"Role number {roleNumber} not found");

            return await _uow.Users.GetUsersByRoleWithRoleNameAsync(roleNumber);

        }
    }
}
