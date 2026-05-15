using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Auth;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class UserService : IUserService
{
    private readonly IApiClient _apiClient;

    public UserService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<UserViewModel>>> GetAllAsync() => _apiClient.GetAsync<List<UserViewModel>>("User");

    public Task<ApiResult<UserViewModel>> GetByUsernameAsync(string username) => _apiClient.GetAsync<UserViewModel>($"User/{Uri.EscapeDataString(username)}");

    public Task<ApiResult<UserViewModel>> UpdateAsync(string username, UserUpdateViewModel model) => _apiClient.PutAsync<UserUpdateViewModel, UserViewModel>($"User/{Uri.EscapeDataString(username)}", model);
    public Task<ApiResult<UserViewModel>> RegisterAsync(RegisterViewModel model)
    {
        return _apiClient.PostAsync<RegisterViewModel, UserViewModel>("Auth/register", model);
    }
    public Task<ApiResult<bool>> DeleteAsync(string username) => _apiClient.DeleteAsync($"User/{Uri.EscapeDataString(username)}");
}
