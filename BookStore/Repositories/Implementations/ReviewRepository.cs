using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class ReviewRepository : GenericRepository<Bookreview>, IReviewRepository
    {
        public ReviewRepository(BookContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bookreview>> GetReviewsByBookAsync(string isbn)
            => await _dbSet.AsNoTracking().Where(r => r.Isbn == isbn).ToListAsync();

        public async Task<IEnumerable<Bookreview>> GetReviewsByReviewerAsync(int reviewerId)
            => await _dbSet.AsNoTracking().Where(r => r.ReviewerId == reviewerId).ToListAsync();
    }

}
