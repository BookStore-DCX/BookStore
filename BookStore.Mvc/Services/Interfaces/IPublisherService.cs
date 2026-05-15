using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;

namespace BookStore.Mvc.Services.Interfaces;

public interface IPublisherService
{
    Task<ApiResult<List<PublisherViewModel>>> GetAllAsync();
    Task<ApiResult<PublisherViewModel>> GetByIdAsync(int id);
    Task<ApiResult<PublisherViewModel>> CreateAsync(PublisherFormViewModel model);
    Task<ApiResult<PublisherViewModel>> UpdateAsync(int id, PublisherFormViewModel model);
    Task<ApiResult<bool>> DeleteAsync(int id);
}
