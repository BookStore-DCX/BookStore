using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class PublisherService : IPublisherService
{
    private readonly IApiClient _apiClient;

    public PublisherService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<PublisherViewModel>>> GetAllAsync() => _apiClient.GetAsync<List<PublisherViewModel>>("Publisher");

    public Task<ApiResult<PublisherViewModel>> GetByIdAsync(int id) => _apiClient.GetAsync<PublisherViewModel>($"Publisher/{id}");

    public Task<ApiResult<PublisherViewModel>> CreateAsync(PublisherFormViewModel model) => _apiClient.PostAsync<PublisherFormViewModel, PublisherViewModel>("Publisher", model);

    public Task<ApiResult<PublisherViewModel>> UpdateAsync(int id, PublisherFormViewModel model) => _apiClient.PutAsync<PublisherFormViewModel, PublisherViewModel>($"Publisher/{id}", model);

    public Task<ApiResult<bool>> DeleteAsync(int id) => _apiClient.DeleteAsync($"Publisher/{id}");
}
