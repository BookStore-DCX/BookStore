using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Auth;

namespace BookStore.Mvc.Services.Interfaces;

public interface IUserService
{
    Task<ApiResult<List<UserViewModel>>> GetAllAsync();
    Task<ApiResult<UserViewModel>> GetByUsernameAsync(string username); 
    Task<ApiResult<UserViewModel>> RegisterAsync(RegisterViewModel model);
    Task<ApiResult<UserViewModel>> UpdateAsync(string username, UserUpdateViewModel model);
    Task<ApiResult<bool>> DeleteAsync(string username);
}
