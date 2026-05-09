using BookStore.Data;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookContext _context; 

        public UnitOfWork(BookContext context, IUserRepository users)
        {
            _context = context;
            Users = users;
        }
        public IUserRepository Users { get; }
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
    