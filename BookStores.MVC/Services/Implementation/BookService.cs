
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class BookService : IBookService
{
	private readonly IApiClient _apiClient;

	public BookService(IApiClient apiClient)
	{
		_apiClient = apiClient;
	}

	public Task<ApiResult<List<BookListItemViewModel>>> GetAllAsync()
	{
		return _apiClient.GetAsync<List<BookListItemViewModel>>("Book");
	}

	public Task<ApiResult<BookDetailViewModel>> GetByIsbnAsync(string isbn)
	{
		return _apiClient.GetAsync<BookDetailViewModel>($"Book/{Uri.EscapeDataString(isbn)}");
	}

	public Task<ApiResult<List<BookListItemViewModel>>> SearchAsync(string? title, string? authorName, string? description)
	{
		var query = new Dictionary<string, string?>();
		if (!string.IsNullOrWhiteSpace(title)) query["title"] = title;
		if (!string.IsNullOrWhiteSpace(authorName)) query["authorName"] = authorName;
		if (!string.IsNullOrWhiteSpace(description)) query["description"] = description;

		var queryString = string.Join("&", query.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));
		return _apiClient.GetAsync<List<BookListItemViewModel>>($"Book/search?{queryString}");
	}

	public Task<ApiResult<BookListItemViewModel>> CreateAsync(BookFormViewModel model)
	{
		return _apiClient.PostAsync<BookFormViewModel, BookListItemViewModel>("Book", model);
	}

	public Task<ApiResult<BookListItemViewModel>> UpdateAsync(string isbn, BookFormViewModel model)
	{
		return _apiClient.PutAsync<BookFormViewModel, BookListItemViewModel>($"Book/{Uri.EscapeDataString(isbn)}", model);
	}

	public Task<ApiResult<bool>> DeleteAsync(string isbn)
	{
		return _apiClient.DeleteAsync($"Book/{Uri.EscapeDataString(isbn)}");
	}
}
