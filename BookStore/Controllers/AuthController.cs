using BookStore.Common;
using BookStore.DTOs.Auth;
using BookStore.DTOs.User;
using BookStore.Exceptions;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if(result == null)
            {
                return Unauthorized(ApiResponse<string>.Fail("Invalid username or password"));
            }
            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful"));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
            {
                return Conflict(ApiResponse<string>.Fail($"Username '{dto.UserName}' already exists"));
            }

            return CreatedAtAction(nameof(Register), ApiResponse<UserDto>.Created(result));
        }
    }
}
