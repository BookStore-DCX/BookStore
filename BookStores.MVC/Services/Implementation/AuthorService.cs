using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class AuthorService : IAuthorService
{
	private readonly IApiClient _apiClient;

	public AuthorService(IApiClient apiClient)
	{
		_apiClient = apiClient;
	}

	public Task<ApiResult<List<AuthorViewModel>>> GetAllAsync() => _apiClient.GetAsync<List<AuthorViewModel>>("Author");

	public Task<ApiResult<AuthorViewModel>> GetByIdAsync(int id) => _apiClient.GetAsync<AuthorViewModel>($"Author/{id}");

	public Task<ApiResult<AuthorViewModel>> CreateAsync(AuthorFormViewModel model) => _apiClient.PostAsync<AuthorFormViewModel, AuthorViewModel>("Author", model);

	public Task<ApiResult<AuthorViewModel>> UpdateAsync(int id, AuthorFormViewModel model) => _apiClient.PutAsync<AuthorFormViewModel, AuthorViewModel>($"Author/{id}", model);

	public Task<ApiResult<bool>> DeleteAsync(int id) => _apiClient.DeleteAsync($"Author/{id}");
}
