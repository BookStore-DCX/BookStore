using BookStore.DTOs.Book;

namespace BookStore.Services.Interfaces
{
	public interface IBookService
	{
		Task<IEnumerable<BookDto>> GetAllBooksAsync();
		Task<BookDto> GetBookByIsbnAsync(string isbn);
		Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string categoryName);
		Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId);
		Task<IEnumerable<BookDto>> SearchBooksAsync(string? authorName, string? title, string? description);
		Task<BookDto> CreateBookAsync(BookCreateDto dto);
		Task<BookDto> UpdateBookAsync(string isbn, BookUpdateDto dto);
		Task DeleteBookAsync(string isbn);
	}
}
