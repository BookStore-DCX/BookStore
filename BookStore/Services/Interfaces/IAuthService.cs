using BookStore.DTOs.Auth;
using BookStore.DTOs.User;
namespace BookStore.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<UserResponseDto?> RegisterAsync(RegisterDto dto);
    }
}
