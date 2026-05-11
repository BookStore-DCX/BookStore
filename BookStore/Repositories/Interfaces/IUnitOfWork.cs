using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{

    IPublisherRepository Publishers { get; }
    ICategoryRepository Categories { get; }
    IStateRepository States { get; }
   
    Task<int> SaveChangesAsync();
}
