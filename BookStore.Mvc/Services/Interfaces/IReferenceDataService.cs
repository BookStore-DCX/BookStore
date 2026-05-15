using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Reference;

namespace BookStore.Mvc.Services.Interfaces;

public interface IReferenceDataService
{
    Task<ApiResult<List<CategoryViewModel>>> GetCategoriesAsync();
    Task<ApiResult<List<BookConditionViewModel>>> GetBookConditionsAsync();
    Task<ApiResult<List<StateViewModel>>> GetStatesAsync();
}
