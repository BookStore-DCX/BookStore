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
                .Include(r => r.IsbnNavigation)
                .Where(r => r.IsbnNavigation.Title == bookName)
                .ToListAsync();

        public async Task<IEnumerable<Bookreview>> GetReviewsByBookIsbnAsync(string isbn)
            => await _dbSet.AsNoTracking()
                .Include(r => r.IsbnNavigation)
                .Where(r => r.Isbn == isbn)
                .ToListAsync();

        public async Task<IEnumerable<Bookreview>> GetReviewsByReviewerAsync(int reviewerId)
            => await _dbSet.AsNoTracking().Where(r => r.ReviewerId == reviewerId).ToListAsync();

        public async Task<Reviewer?> GetReviewerByIdAsync(int reviewerId)
            => await _context.Reviewers.AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReviewerId == reviewerId);

        public async Task<Reviewer?> GetReviewerByNameAsync(string fullName)
        {
            var normalized = fullName.Trim().ToLower();
            return await _context.Reviewers.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name != null && r.Name.ToLower() == normalized);
        }

        public async Task<bool> ReviewExistsAsync(string isbn, int reviewerId)
            => await _dbSet.AsNoTracking()
                .AnyAsync(r => r.Isbn == isbn && r.ReviewerId == reviewerId);

        public async Task<int> GetNextReviewerIdAsync()
        {
            var maxId = await _context.Reviewers.MaxAsync(r => (int?)r.ReviewerId) ?? 0;
            return maxId + 1;
        }

        public Task AddReviewerAsync(Reviewer reviewer)
        {
            _context.Reviewers.Add(reviewer);
            return Task.CompletedTask;
        }

        public async Task DeleteReviewAsync(string isbn, int reviewerId)
        {
            var review = await _dbSet.FirstOrDefaultAsync(r => r.Isbn == isbn && r.ReviewerId == reviewerId);
            if (review != null)
            {
                _dbSet.Remove(review);
            }
        }
    }
}
