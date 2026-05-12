using BookStore.Data;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookContext _context;

        public UnitOfWork(
            BookContext context,
            IReviewRepository reviews,
            IBookConditionRepository bookConditions,
            IInventoryRepository inventories)
        {
            _context = context;

            Reviews = reviews;
            BookConditions = bookConditions;
            Inventories = inventories;
        }

        public IReviewRepository Reviews { get; }

        public IBookConditionRepository BookConditions { get; }

        public IInventoryRepository Inventories { get; }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}