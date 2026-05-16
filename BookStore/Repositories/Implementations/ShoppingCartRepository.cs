using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Implementations;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class ShoppingCartRepository : GenericRepository<Shoppingcart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(BookContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Shoppingcart>> GetCartByUserAsync(int userId)
            => await _dbSet
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .Include(s => s.IsbnNavigation)
                    .ThenInclude(b => b.Inventories)
                        .ThenInclude(i => i.RanksNavigation)
                .ToListAsync();

        public async Task RemoveFromCartAsync(int userId, string isbn)
        {
            var item = await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId && s.Isbn == isbn);
            if (item != null)
            {
                _dbSet.Remove(item);
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = await _dbSet.Where(s => s.UserId == userId).ToListAsync();
            _dbSet.RemoveRange(items);
        }
    }
}
