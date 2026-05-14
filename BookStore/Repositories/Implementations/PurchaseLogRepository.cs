using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Implementations;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class PurchaseLogRepository : GenericRepository<Purchaselog>, IPurchaseLogRepository
    {
        public PurchaseLogRepository(BookContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Purchaselog>> GetPurchasesByUserAsync(int userId)
            => await _dbSet.AsNoTracking().Where(p => p.UserId == userId).ToListAsync();
    }
}
