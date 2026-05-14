using BookStore.Data;
using BookStore.Exceptions;
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
				.Include(b => b.Inventories)
				.ThenInclude(i => i.RanksNavigation)
				.FirstOrDefaultAsync(b => b.Isbn == isbn);

		public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(string categoryName)
			=> await _dbSet.AsNoTracking()
				.Include(b => b.CategoryNavigation)
				.Include(b => b.Inventories)
				.Where(b => b.CategoryNavigation != null
					&& b.CategoryNavigation.CatDescription.Contains(categoryName))
				.ToListAsync();

		public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
			=> await _dbSet.AsNoTracking()
				.Include(b => b.Bookauthors)
				.ThenInclude(ba => ba.Author)
				.Include(b => b.Inventories)
				.Where(b => b.Bookauthors.Any(ba => ba.AuthorId == authorId))
				.ToListAsync();

		public async Task<IEnumerable<Book>> SearchBooksAsync(string? authorName, string? title, string? description)
		{
			if (string.IsNullOrWhiteSpace(authorName)
				&& string.IsNullOrWhiteSpace(title)
				&& string.IsNullOrWhiteSpace(description))
			{
				throw new BadRequestException("At least one search parameter is required.");
			}

			var query = _dbSet.AsNoTracking()
				.Include(b => b.Bookauthors)
				.ThenInclude(ba => ba.Author)
				.Include(b => b.Inventories)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(title))
			{
				query = query.Where(b => b.Title.Contains(title));
			}

			if (!string.IsNullOrWhiteSpace(description))
			{
				query = query.Where(b => b.Description != null && b.Description.Contains(description));
			}

			if (!string.IsNullOrWhiteSpace(authorName))
			{
				query = query.Where(b => b.Bookauthors.Any(ba =>
					(ba.Author.FirstName + " " + ba.Author.LastName).Contains(authorName) ||
					(ba.Author.LastName + " " + ba.Author.FirstName).Contains(authorName)));
			}

			return await query.ToListAsync();
		}

        public override async Task<IEnumerable<Book>> GetAllAsync()
            => await _dbSet.AsNoTracking()
                .Include(b => b.CategoryNavigation)
                .Include(b => b.Publisher)
                .Include(b => b.Inventories)
                .ToListAsync();
	}
}