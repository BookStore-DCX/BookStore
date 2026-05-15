using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Auth;

namespace BookStore.Mvc.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResult<AuthResponseViewModel>> LoginAsync(LoginViewModel model);
    Task<ApiResult<UserViewModel>> RegisterAsync(RegisterViewModel model);
}
