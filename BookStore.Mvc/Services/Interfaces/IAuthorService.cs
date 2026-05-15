using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;

namespace BookStore.Mvc.Services.Interfaces;

public interface IAuthorService
{
    Task<ApiResult<List<AuthorViewModel>>> GetAllAsync();
    Task<ApiResult<AuthorViewModel>> GetByIdAsync(int id);
    Task<ApiResult<AuthorViewModel>> CreateAsync(AuthorFormViewModel model);
    Task<ApiResult<AuthorViewModel>> UpdateAsync(int id, AuthorFormViewModel model);
    Task<ApiResult<bool>> DeleteAsync(int id);
}
