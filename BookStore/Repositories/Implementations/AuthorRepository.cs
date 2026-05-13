using System.Xml.Linq;
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

		public async Task<IEnumerable<Author>> SearchAuthorsAsync(string authorName)
			=> await _dbSet.AsNoTracking()
				.Where(a => a.FirstName.Contains(authorName)
					|| a.LastName.Contains(authorName)
					|| (a.FirstName + " " + a.LastName).Contains(authorName)
					|| (a.LastName + " " + a.FirstName).Contains(authorName))
				.ToListAsync();

		public async Task<Author?> GetByNameAsync(string authorName)
		{
			if (string.IsNullOrWhiteSpace(authorName)) return null;
			var lowered = authorName.Trim().ToLower();
			return await _dbSet.FirstOrDefaultAsync(a =>
				(a.FirstName != null && a.FirstName.ToLower() == lowered) ||
				(a.LastName != null && a.LastName.ToLower() == lowered) ||
				((a.FirstName + " " + a.LastName).ToLower() == lowered) ||
				((a.LastName + " " + a.FirstName).ToLower() == lowered));
		}

		public async Task<bool> ExistsByNameAsync(string authorName)
		{
			if (string.IsNullOrWhiteSpace(authorName)) return false;
			var lowered = authorName.Trim().ToLower();
			return await _dbSet.AnyAsync(a =>
				(a.FirstName != null && a.FirstName.ToLower() == lowered) ||
				(a.LastName != null && a.LastName.ToLower() == lowered) ||
				((a.FirstName + " " + a.LastName).ToLower() == lowered) ||
				((a.LastName + " " + a.FirstName).ToLower() == lowered));
		}

		public async Task DeleteByNameAsync(string authorName)
		{
			var author = await GetByNameAsync(authorName);
			if (author != null) _dbSet.Remove(author);
		}
	}
}