using BookStore.DTOs.Book;

namespace BookStore.Services.Interfaces
{
	public interface IBookService
	{
		Task<IEnumerable<BookDto>> GetAllBooksAsync();
		Task<BookDto> GetBookByIsbnAsync(string isbn);
		Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId);
		Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId);
		Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
		Task<BookDto> CreateBookAsync(BookCreateDto dto);
		Task<BookDto> UpdateBookAsync(string isbn, BookUpdateDto dto);
		Task DeleteBookAsync(string isbn);
	}

}
