using BookStore.Models;
using BookStore.Data;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class PublisherRepository : GenericRepository<Publisher>, IPublisherRepository
    {
        public PublisherRepository(BookContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Publisher>> GetPublishersByStateAsync(string stateCode)
            => await _dbSet.AsNoTracking().Where(p => p.StateCode == stateCode).ToListAsync();
    }
}