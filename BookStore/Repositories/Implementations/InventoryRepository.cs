using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(BookContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inventory>> GetInventoryByBookAsync(string isbn)
            => await _dbSet.AsNoTracking()
                .Include(i => i.RanksNavigation)
                .Where(i => i.Isbn == isbn)
                .ToListAsync();

        public async Task<IEnumerable<Inventory>> GetAvailableInventoryAsync()
            => await _dbSet.AsNoTracking()
                .Include(i => i.RanksNavigation)
                .Where(i => i.Purchased == 0)
                .ToListAsync();
    }
}
