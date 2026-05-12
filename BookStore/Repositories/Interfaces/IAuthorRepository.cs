using BookStore.Models;
using BookStore.Repositories.Implementations;

namespace BookStore.Repositories.Interfaces
{
	public interface IAuthorRepository : IGenericRepository<Author>
	{
		Task<IEnumerable<Author>> SearchAuthorsAsync(string searchTerm);
	}
}
