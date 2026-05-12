using BookStore.Models;
using BookStore.Repositories.Implementations;

namespace BookStore.Repositories.Interfaces
{
	public interface IBookRepository : IGenericRepository<Book>
	{
		Task<Book?> GetBookWithDetailsAsync(string isbn);
		Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
		Task<IEnumerable<Book>> GetBooksByPublisherAsync(int publisherId);
		Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
	}
}
