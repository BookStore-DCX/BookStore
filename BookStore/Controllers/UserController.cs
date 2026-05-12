using BookStore.Common;
using BookStore.DTOs.User;
using BookStore.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(result));
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var result = await _userService.GetUserByIdAsync(id);
                return Ok(ApiResponse<UserDto>.Ok(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
        {
            try
            {
                var result = await _userService.GetUserByUsernameAsync(username);
                return Ok(ApiResponse<UserDto>.Ok(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpGet("role/{roleNumber:int}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(int roleNumber)
        {
            try
            {
                var result = await _userService.GetUsersByRoleAsync(roleNumber);
                return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, dto);
                return Ok(ApiResponse<UserDto>.Ok(result, "Updated"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(ApiResponse<bool>.Ok(result, "Deleted"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message));
            }
        }
    }
}
