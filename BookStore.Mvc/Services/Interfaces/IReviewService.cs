using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;

namespace BookStore.Mvc.Services.Interfaces;

public interface IReviewService
{
    Task<ApiResult<List<ReviewViewModel>>> GetByBookAsync(string isbn);
    Task<ApiResult<ReviewViewModel>> CreateAsync(ReviewCreateViewModel model);
    Task<ApiResult<bool>> DeleteAsync(string isbn, int reviewerId);
}
