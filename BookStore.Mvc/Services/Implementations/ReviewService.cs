using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IApiClient _apiClient;

    public ReviewService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<ReviewViewModel>>> GetByBookAsync(string isbn) => _apiClient.GetAsync<List<ReviewViewModel>>($"Review/book/{Uri.EscapeDataString(isbn)}");

    public Task<ApiResult<ReviewViewModel>> CreateAsync(ReviewCreateViewModel model) => _apiClient.PostAsync<ReviewCreateViewModel, ReviewViewModel>("Review", model);

    public Task<ApiResult<bool>> DeleteAsync(string isbn, int reviewerId) => _apiClient.DeleteAsync($"Review/{Uri.EscapeDataString(isbn)}/{reviewerId}");
}
