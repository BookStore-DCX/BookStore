using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
	public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
	{
		public AuthorRepository(BookContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Author>> SearchAuthorsAsync(string searchTerm)
			=> await _dbSet.AsNoTracking()
				.Where(a => a.FirstName.Contains(searchTerm) || a.LastName.Contains(searchTerm))
				.ToListAsync();
	}

}
