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
            => await _dbSet
                .AsNoTracking()
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.IsbnNavigation)
                        .ThenInclude(b => b.Bookauthors)
                            .ThenInclude(ba => ba.Author)
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.RanksNavigation)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.InventoryId)
                .ToListAsync();
    }
}
