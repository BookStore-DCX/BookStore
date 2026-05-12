using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
	public class BookRepository : GenericRepository<Book>, IBookRepository
	{
		public BookRepository(BookContext context) : base(context)
		{
		}

		public async Task<Book?> GetBookWithDetailsAsync(string isbn)
			=> await _dbSet.AsNoTracking()
				.Include(b => b.CategoryNavigation)
				.Include(b => b.Publisher)
				.Include(b => b.Bookauthors)
				.ThenInclude(ba => ba.Author)
				.FirstOrDefaultAsync(b => b.Isbn == isbn);

		public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
			=> await _dbSet.AsNoTracking().Where(b => b.Category == categoryId).ToListAsync();

		public async Task<IEnumerable<Book>> GetBooksByPublisherAsync(int publisherId)
			=> await _dbSet.AsNoTracking().Where(b => b.PublisherId == publisherId).ToListAsync();

		public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
			=> await _dbSet.AsNoTracking()
				.Where(b => b.Title.Contains(searchTerm) || (b.Description != null && b.Description.Contains(searchTerm)))
				.ToListAsync();
	}

}
