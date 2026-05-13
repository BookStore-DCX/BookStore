using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class ReviewRepository : GenericRepository<Bookreview>, IReviewRepository
    {
        private readonly BookContext _context;

        public ReviewRepository(BookContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bookreview>> GetReviewsByBookNameAsync(string bookName)
            => await _dbSet.AsNoTracking()
                .Join(_context.Books,
                    r => r.Isbn,
                    b => b.Isbn,
                    (r, b) => new { Review = r, Book = b })
                .Where(x => x.Book.Title == bookName)
                .Select(x => x.Review)
                .ToListAsync();

        public async Task<IEnumerable<Bookreview>> GetReviewsByReviewerAsync(int reviewerId)
            => await _dbSet.AsNoTracking().Where(r => r.ReviewerId == reviewerId).ToListAsync();
    }
}