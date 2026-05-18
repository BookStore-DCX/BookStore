using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;

namespace BookStore.Mvc.Services.Interfaces;

public interface IBookService
{
	Task<ApiResult<List<BookListItemViewModel>>> GetAllAsync();
	Task<ApiResult<BookDetailViewModel>> GetByIsbnAsync(string isbn);
	Task<ApiResult<List<BookListItemViewModel>>> SearchAsync(string? title, string? authorName, string? description);
	Task<ApiResult<BookListItemViewModel>> CreateAsync(BookFormViewModel model);
	Task<ApiResult<BookListItemViewModel>> UpdateAsync(string isbn, BookFormViewModel model);
	Task<ApiResult<bool>> DeleteAsync(string isbn);
}
