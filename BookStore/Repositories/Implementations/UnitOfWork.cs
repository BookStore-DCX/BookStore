using BookStore.Data;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Implementations
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly BookContext _context;
		public IBookRepository Books { get; }
		public IAuthorRepository Authors { get; }


		public UnitOfWork(BookContext context,
			IBookRepository books, IAuthorRepository authors
			)
		{
			_context = context;
			Books = books; Authors = authors;
		}

		public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
		public void Dispose() => _context.Dispose();
	}

}