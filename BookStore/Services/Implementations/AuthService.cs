using AutoMapper;
using BookStore.DTOs.Auth;
using BookStore.DTOs.User;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwt;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepo, IUserRepository userRepo, IUnitOfWork uow, IJwtService jwt, IMapper mapper)
        {
            _authRepo = authRepo;
            _userRepo = userRepo;
            _uow = uow;
            _jwt = jwt;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _authRepo.ValidateUserAsync(dto.UserName, dto.Password);
            if (user == null)
            {
                return null;
            }

                return new AuthResponseDto
            {
                Token = _jwt.GenerateToken(user),
                UserName = user.UserName,
                Role = user.RoleNumberNavigation?.PermRole1 ?? "Guest",
                Expiry = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            if (await _authRepo.UserExistsAsync(dto.UserName))
            {
                return null;     
            }

            var user = _mapper.Map<User>(dto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.RoleNumber = 1;

            await _userRepo.AddAsync(user);
            await _uow.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }
    }
}
