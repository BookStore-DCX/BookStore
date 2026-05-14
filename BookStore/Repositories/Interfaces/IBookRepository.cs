using BookStore.Models;

using BookStore.Repositories.Implementations;

namespace BookStore.Repositories.Interfaces
{
	public interface IBookRepository : IGenericRepository<Book>
	{
		Task<Book?> GetBookWithDetailsAsync(string isbn);
		Task<IEnumerable<Book>> GetBooksByCategoryAsync(string categoryName);
		Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId);
		Task<IEnumerable<Book>> SearchBooksAsync(string? authorName, string? title, string? description);
	}
}