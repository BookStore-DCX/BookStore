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
		{
			var book = await _dbSet.AsNoTracking().FirstOrDefaultAsync(b => b.Isbn == isbn);
			if (book == null) return null;

			// load category (join-like single fetch)
			if (book.Category.HasValue)
			{
				book.CategoryNavigation = await _context.Categories
					.AsNoTracking()
					.FirstOrDefaultAsync(c => c.CatId == book.Category.Value);
			}

			// load publisher
			book.Publisher = await _context.Publishers
				.AsNoTracking()
				.FirstOrDefaultAsync(p => p.PublisherId == book.PublisherId)
				?? book.Publisher; // keep non-nullability if not found

			// load authors via join between Bookauthors and Authors
			var bas = await _context.Bookauthors
				.AsNoTracking()
				.Where(ba => ba.Isbn == isbn)
				.Join(_context.Authors.AsNoTracking(),
					ba => ba.AuthorId,
					a => a.AuthorId,
					(ba, a) => new { ba.Isbn, ba.AuthorId, ba.PrimaryAuthor, Author = a })
				.ToListAsync();

			book.Bookauthors = bas
				.Select(x => new Bookauthor
				{
					Isbn = x.Isbn,
					AuthorId = x.AuthorId,
					PrimaryAuthor = x.PrimaryAuthor,
					Author = x.Author
				})
				.ToList();

			// load inventories and their ranks (bookcondition) via join
			var invs = await _context.Inventories
				.AsNoTracking()
				.Where(i => i.Isbn == isbn)
				.Join(_context.Bookconditions.AsNoTracking(),
					i => i.Ranks,
					bc => bc.Ranks,
					(i, bc) => new { Inventory = i, RanksNavigation = bc })
				.ToListAsync();

			book.Inventories = invs.Select(x =>
			{
				var inv = new Inventory
				{
					InventoryId = x.Inventory.InventoryId,
					Isbn = x.Inventory.Isbn,
					Ranks = x.Inventory.Ranks,
					Purchased = x.Inventory.Purchased,
					IsbnNavigation = book,
					RanksNavigation = x.RanksNavigation
				};
				return inv;
			}).ToList();

			return book;
		}

		public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(string categoryName)
		{
			// find matching category ids using join-like filter
			var matchingCatIds = await _context.Categories
				.AsNoTracking()
				.Where(c => c.CatDescription != null && c.CatDescription.Contains(categoryName))
				.Select(c => c.CatId)
				.ToListAsync();

			if (!matchingCatIds.Any()) return Enumerable.Empty<Book>();

			// fetch books for those category ids
			var books = await _dbSet.AsNoTracking()
				.Where(b => b.Category.HasValue && matchingCatIds.Contains(b.Category.Value))
				.ToListAsync();

			// eager-load inventories for these books via a single query + grouping
			var isbns = books.Select(b => b.Isbn).ToList();

			var invs = await _context.Inventories
				.AsNoTracking()
				.Where(i => isbns.Contains(i.Isbn))
				.Join(_context.Bookconditions.AsNoTracking(),
					i => i.Ranks,
					bc => bc.Ranks,
					(i, bc) => new { i.Isbn, Inventory = i, RanksNavigation = bc })
				.ToListAsync();

			var invByIsbn = invs.GroupBy(x => x.Isbn)
				.ToDictionary(g => g.Key, g => g.Select(x => new Inventory
				{
					InventoryId = x.Inventory.InventoryId,
					Isbn = x.Inventory.Isbn,
					Ranks = x.Inventory.Ranks,
					Purchased = x.Inventory.Purchased,
					RanksNavigation = x.RanksNavigation
				}).ToList());

			// attach category navigation and inventories
			var categories = await _context.Categories
				.AsNoTracking()
				.Where(c => matchingCatIds.Contains(c.CatId))
				.ToDictionaryAsync(c => c.CatId);

			foreach (var book in books)
			{
				if (book.Category.HasValue && categories.TryGetValue(book.Category.Value, out var cat))
					book.CategoryNavigation = cat;

				if (invByIsbn.TryGetValue(book.Isbn, out var list))
				{
					foreach (var inv in list) inv.IsbnNavigation = book;
					book.Inventories = list;
				}
				else
				{
					book.Inventories = new List<Inventory>();
				}
			}

			return books;
		}

		public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
		{
			// find ISBNs for the author
			var isbns = await _context.Bookauthors
				.AsNoTracking()
				.Where(ba => ba.AuthorId == authorId)
				.Select(ba => ba.Isbn)
				.Distinct()
				.ToListAsync();

			if (!isbns.Any()) return Enumerable.Empty<Book>();

			// fetch books
			var books = await _dbSet.AsNoTracking()
				.Where(b => isbns.Contains(b.Isbn))
				.ToListAsync();

			// fetch authors for these books
			var bas = await _context.Bookauthors
				.AsNoTracking()
				.Where(ba => isbns.Contains(ba.Isbn))
				.Join(_context.Authors.AsNoTracking(),
					ba => ba.AuthorId,
					a => a.AuthorId,
					(ba, a) => new { ba.Isbn, ba.AuthorId, ba.PrimaryAuthor, Author = a })
				.ToListAsync();

			var basByIsbn = bas.GroupBy(x => x.Isbn)
				.ToDictionary(g => g.Key, g => g.Select(x => new Bookauthor
				{
					Isbn = x.Isbn,
					AuthorId = x.AuthorId,
					PrimaryAuthor = x.PrimaryAuthor,
					Author = x.Author
				}).ToList());

			// fetch inventories
			var invs = await _context.Inventories
				.AsNoTracking()
				.Where(i => isbns.Contains(i.Isbn))
				.Join(_context.Bookconditions.AsNoTracking(),
					i => i.Ranks,
					bc => bc.Ranks,
					(i, bc) => new { i.Isbn, Inventory = i, RanksNavigation = bc })
				.ToListAsync();

			var invByIsbn = invs.GroupBy(x => x.Isbn)
				.ToDictionary(g => g.Key, g => g.Select(x => new Inventory
				{
					InventoryId = x.Inventory.InventoryId,
					Isbn = x.Inventory.Isbn,
					Ranks = x.Inventory.Ranks,
					Purchased = x.Inventory.Purchased,
					RanksNavigation = x.RanksNavigation
				}).ToList());

			// attach navigations
			foreach (var book in books)
			{
				if (basByIsbn.TryGetValue(book.Isbn, out var bookAuthors))
				{
					foreach (var ba in bookAuthors) ba.Isbn = book.Isbn;
					book.Bookauthors = bookAuthors;
				}
				else
				{
					book.Bookauthors = new List<Bookauthor>();
				}

				if (invByIsbn.TryGetValue(book.Isbn, out var invList))
				{
					foreach (var inv in invList) inv.IsbnNavigation = book;
					book.Inventories = invList;
				}
				else
				{
					book.Inventories = new List<Inventory>();
				}
			}

			return books;
		}

		public async Task<IEnumerable<Book>> SearchBooksAsync(string? authorName, string? title, string? description)
		{
			if (string.IsNullOrWhiteSpace(authorName)
				&& string.IsNullOrWhiteSpace(title)
				&& string.IsNullOrWhiteSpace(description))
			{
				throw new BadRequestException("At least one search parameter is required.");
			}

			// start with base book query
			var booksQuery = _dbSet.AsNoTracking().AsQueryable();

			if (!string.IsNullOrWhiteSpace(title))
			{
				booksQuery = booksQuery.Where(b => b.Title.Contains(title));
			}

			if (!string.IsNullOrWhiteSpace(description))
			{
				booksQuery = booksQuery.Where(b => b.Description != null && b.Description.Contains(description));
			}

			if (!string.IsNullOrWhiteSpace(authorName))
			{
				// get matching ISBNs for authorName using join between Bookauthors and Authors
				var matchingIsbns = await _context.Bookauthors
					.AsNoTracking()
					.Join(_context.Authors.AsNoTracking(),
						ba => ba.AuthorId,
						a => a.AuthorId,
						(ba, a) => new { ba.Isbn, a.FirstName, a.LastName })
					.Where(x =>
						(x.FirstName + " " + x.LastName).Contains(authorName) ||
						(x.LastName + " " + x.FirstName).Contains(authorName) ||
						(x.FirstName != null && x.FirstName.Contains(authorName)) ||
						(x.LastName != null && x.LastName.Contains(authorName)))
					.Select(x => x.Isbn)
					.Distinct()
					.ToListAsync();

				if (!matchingIsbns.Any()) return Enumerable.Empty<Book>();

				booksQuery = booksQuery.Where(b => matchingIsbns.Contains(b.Isbn));
			}

			var books = await booksQuery.ToListAsync();
			if (!books.Any()) return books;

			// load authors for all matched books in one query
			var isbns = books.Select(b => b.Isbn).ToList();

			var bas = await _context.Bookauthors
				.AsNoTracking()
				.Where(ba => isbns.Contains(ba.Isbn))
				.Join(_context.Authors.AsNoTracking(),
					ba => ba.AuthorId,
					a => a.AuthorId,
					(ba, a) => new { ba.Isbn, ba.AuthorId, ba.PrimaryAuthor, Author = a })
				.ToListAsync();

			var basByIsbn = bas.GroupBy(x => x.Isbn)
				.ToDictionary(g => g.Key, g => g.Select(x => new Bookauthor
				{
					Isbn = x.Isbn,
					AuthorId = x.AuthorId,
					PrimaryAuthor = x.PrimaryAuthor,
					Author = x.Author
				}).ToList());

			// load inventories and conditions for all matched books
			var invs = await _context.Inventories
				.AsNoTracking()
				.Where(i => isbns.Contains(i.Isbn))
				.Join(_context.Bookconditions.AsNoTracking(),
					i => i.Ranks,
					bc => bc.Ranks,
					(i, bc) => new { i.Isbn, Inventory = i, RanksNavigation = bc })
				.ToListAsync();

			var invByIsbn = invs.GroupBy(x => x.Isbn)
				.ToDictionary(g => g.Key, g => g.Select(x => new Inventory
				{
					InventoryId = x.Inventory.InventoryId,
					Isbn = x.Inventory.Isbn,
					Ranks = x.Inventory.Ranks,
					Purchased = x.Inventory.Purchased,
					RanksNavigation = x.RanksNavigation
				}).ToList());

			// attach to books
			foreach (var book in books)
			{
				if (basByIsbn.TryGetValue(book.Isbn, out var bookAuthors))
				{
					foreach (var ba in bookAuthors) ba.Isbn = book.Isbn;
					book.Bookauthors = bookAuthors;
				}
				else
				{
					book.Bookauthors = new List<Bookauthor>();
				}

				if (invByIsbn.TryGetValue(book.Isbn, out var invList))
				{
					foreach (var inv in invList) inv.IsbnNavigation = book;
					book.Inventories = invList;
				}
				else
				{
					book.Inventories = new List<Inventory>();
				}
			}

			return books;
		}

		public override async Task<IEnumerable<Book>> GetAllAsync()
		{
			var books = await _dbSet.AsNoTracking().ToListAsync();
			if (!books.Any()) return books;

			var isbns = books.Select(b => b.Isbn).ToList();

			// load categories and publishers in bulk
			var categoryIds = books.Where(b => b.Category.HasValue).Select(b => b.Category!.Value).Distinct().ToList();
			var categories = await _context.Categories
				.AsNoTracking()
				.Where(c => categoryIds.Contains(c.CatId))
				.ToDictionaryAsync(c => c.CatId);

			var publisherIds = books.Select(b => b.PublisherId).Distinct().ToList();
			var publishers = await _context.Publishers
				.AsNoTracking()
				.Where(p => publisherIds.Contains(p.PublisherId))
				.ToDictionaryAsync(p => p.PublisherId);

			// load authors for all books
			var bas = await _context.Bookauthors
				.AsNoTracking()
				.Where(ba => isbns.Contains(ba.Isbn))
				.Join(_context.Authors.AsNoTracking(),
					ba => ba.AuthorId,
					a => a.AuthorId,
					(ba, a) => new { ba.Isbn, ba.AuthorId, ba.PrimaryAuthor, Author = a })
				.ToListAsync();

			var basByIsbn = bas.GroupBy(x => x.Isbn)
				.ToDictionary(g => g.Key, g => g.Select(x => new Bookauthor
				{
					Isbn = x.Isbn,
					AuthorId = x.AuthorId,
					PrimaryAuthor = x.PrimaryAuthor,
					Author = x.Author
				}).ToList());

			// load inventories + conditions
			var invs = await _context.Inventories
				.AsNoTracking()
				.Where(i => isbns.Contains(i.Isbn))
				.Join(_context.Bookconditions.AsNoTracking(),
					i => i.Ranks,
					bc => bc.Ranks,
					(i, bc) => new { i.Isbn, Inventory = i, RanksNavigation = bc })
				.ToListAsync();

			var invByIsbn = invs.GroupBy(x => x.Isbn)
				.ToDictionary(g => g.Key, g => g.Select(x => new Inventory
				{
					InventoryId = x.Inventory.InventoryId,
					Isbn = x.Inventory.Isbn,
					Ranks = x.Inventory.Ranks,
					Purchased = x.Inventory.Purchased,
					RanksNavigation = x.RanksNavigation
				}).ToList());

			// attach navs
			foreach (var book in books)
			{
				if (book.Category.HasValue && categories.TryGetValue(book.Category.Value, out var cat))
					book.CategoryNavigation = cat;

				if (publishers.TryGetValue(book.PublisherId, out var pub))
					book.Publisher = pub;

				if (basByIsbn.TryGetValue(book.Isbn, out var bookAuthors))
					book.Bookauthors = bookAuthors;
				else
					book.Bookauthors = new List<Bookauthor>();

				if (invByIsbn.TryGetValue(book.Isbn, out var invList))
				{
					foreach (var inv in invList) inv.IsbnNavigation = book;
					book.Inventories = invList;
				}
				else
				{
					book.Inventories = new List<Inventory>();
				}
			}

			return books;
		}
	}
}