using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Repositories.Interfaces;

namespace BookStoreProjectApi.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {   
        private readonly BookContext _context;

        public UnitOfWork(
            BookContext context,
            IUserRepository users,
            IShoppingCartRepository shoppingCarts,
            IPurchaseLogRepository purchaseLogs)
        { 
            _context = context;
            Users = users;
            ShoppingCarts = shoppingCarts;
            PurchaseLogs = purchaseLogs;
        }

        public IUserRepository Users { get; }
        public IShoppingCartRepository ShoppingCarts { get; }
        public IPurchaseLogRepository PurchaseLogs { get; }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }

}
