using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookContext _context;

        public UnitOfWork(
            BookContext context,
            IUserRepository users,
            IPublisherRepository publishers,
            ICategoryRepository categories,
            IStateRepository states,
            IShoppingCartRepository shoppingCarts,
            IPurchaseLogRepository purchaseLogs,
            IReviewRepository reviews,
            IBookConditionRepository bookConditions,
            IInventoryRepository inventories,
            IBookRepository books,
            IAuthorRepository authors)
        {
            _context = context;
            Users = users;
            Publishers = publishers;
            Categories = categories;
            States = states;
            ShoppingCarts = shoppingCarts;
            PurchaseLogs = purchaseLogs;
            Reviews = reviews;
            BookConditions = bookConditions;
            Inventories = inventories;
            Books = books;
            Authors = authors;
        }
        public IBookRepository Books { get; }

        public IBookConditionRepository BookConditions { get; }

        public ICategoryRepository Categories { get; }

        public IInventoryRepository Inventories { get; }

        public IPurchaseLogRepository PurchaseLogs { get; }

        public IPublisherRepository Publishers { get; }

        public IShoppingCartRepository ShoppingCarts { get; }

        public IStateRepository States { get; }

        public IUserRepository Users { get; }
        public IReviewRepository Reviews { get; }
        public IAuthorRepository Authors { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}