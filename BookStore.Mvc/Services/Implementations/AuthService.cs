using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Auth;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IApiClient _apiClient;

    public AuthService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<AuthResponseViewModel>> LoginAsync(LoginViewModel model)
    {
        return _apiClient.PostAsync<LoginViewModel, AuthResponseViewModel>("Auth/login", model);
    }

    public Task<ApiResult<UserViewModel>> RegisterAsync(RegisterViewModel model)
    {
        return _apiClient.PostAsync<RegisterViewModel, UserViewModel>("Auth/register", model);
    }
}
