using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Reference;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class ReferenceDataService : IReferenceDataService
{
    private readonly IApiClient _apiClient;

    public ReferenceDataService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<CategoryViewModel>>> GetCategoriesAsync() => _apiClient.GetAsync<List<CategoryViewModel>>("Category");

    public Task<ApiResult<List<BookConditionViewModel>>> GetBookConditionsAsync() => _apiClient.GetAsync<List<BookConditionViewModel>>("BookCondition");

    public Task<ApiResult<List<StateViewModel>>> GetStatesAsync() => _apiClient.GetAsync<List<StateViewModel>>("State");
}
