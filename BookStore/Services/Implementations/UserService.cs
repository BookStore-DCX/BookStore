using AutoMapper;
using BookStore.DTOs.User;
using BookStore.Models;
using BookStore.Repositories.Implementations;
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

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _uow.Users.GetAllAsync();

            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            return _mapper.Map<UserResponseDto>(user);
        }
        public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
        {
            var user = await _uow.Users.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new KeyNotFoundException($"User '{username}' not found");
            }

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(int userId, UserUpdateDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            _mapper.Map(dto, user); 

            await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            bool isExist = await _uow.Users.ExistsAsync(userId);
            if(isExist == false)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            await _uow.Users.DeleteAsync(userId);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(int roleNumber)
        {
            var users = await _uow.Users.GetUsersByRoleAsync(roleNumber);
            //if (!users.Any())
            //    throw new KeyNotFoundException($"No users found for role {roleNumber}");

            return _mapper.Map<IEnumerable<UserResponseDto>>(users);

        }
    }
}
