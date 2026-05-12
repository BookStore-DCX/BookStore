namespace BookStore.Repositories.Interfaces
{
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IPublisherRepository Publishers { get; }
        ICategoryRepository Categories { get; }
        IStateRepository States { get; }

        Task<int> SaveChangesAsync();
    }
}