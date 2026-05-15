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

        public override async Task<IEnumerable<Inventory>> GetAllAsync()
            => await _dbSet.AsNoTracking()
                .Include(i => i.IsbnNavigation)
                .Include(i => i.RanksNavigation)
                .ToListAsync();

        public async Task<IEnumerable<Inventory>> GetInventoryByBookAsync(string isbn)
            => await _dbSet.AsNoTracking()
                .Join(
                    _context.Bookconditions,
                    inventory => inventory.Ranks,
                    bookcondition => bookcondition.Ranks,
                    (inventory, bookcondition) => new { inventory, bookcondition }
                )
                .Where(x => x.inventory.Isbn == isbn)
                .Select(x => new Inventory
                {
                    InventoryId = x.inventory.InventoryId,
                    Isbn = x.inventory.Isbn,
                    Ranks = x.inventory.Ranks,
                    Purchased = x.inventory.Purchased,
                    IsbnNavigation = x.inventory.IsbnNavigation,
                    RanksNavigation = x.bookcondition
                })
                .ToListAsync();

        public async Task<IEnumerable<Inventory>> GetAvailableInventoryAsync()
            => await _dbSet.AsNoTracking()
                .Join(
                    _context.Bookconditions,
                    inventory => inventory.Ranks,
                    bookcondition => bookcondition.Ranks,
                    (inventory, bookcondition) => new { inventory, bookcondition }
                )
                .Where(x => x.inventory.Purchased == 0)
                .Select(x => new Inventory
                {
                    InventoryId = x.inventory.InventoryId,
                    Isbn = x.inventory.Isbn,
                    Ranks = x.inventory.Ranks,
                    Purchased = x.inventory.Purchased,
                    IsbnNavigation = x.inventory.IsbnNavigation,
                    RanksNavigation = x.bookcondition
                })
                .ToListAsync();
    }
}
