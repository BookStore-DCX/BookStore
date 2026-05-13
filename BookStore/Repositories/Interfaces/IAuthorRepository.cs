using BookStore.Models;
using BookStore.Repositories.Implementations;
namespace BookStore.Repositories.Interfaces
{
	public interface IAuthorRepository : IGenericRepository<Author>
	{
		Task<IEnumerable<Author>> SearchAuthorsAsync(string searchTerm);
		Task<Author?> GetByNameAsync(string authorName);
		Task<bool> ExistsByNameAsync(string authorName);
		Task DeleteByNameAsync(string authorName);
	}
}