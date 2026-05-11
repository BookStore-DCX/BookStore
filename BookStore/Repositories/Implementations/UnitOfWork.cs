using BookStore.Data;
using BookStore.Repositories.Interfaces;
using BookStore.Data;
using BookStore.Repositories.Interfaces;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly BookContext _context;

    public UnitOfWork(
        BookContext context,
      
        IPublisherRepository publishers,
        ICategoryRepository categories,
        IStateRepository states)
    {
        _context = context;
   
        Publishers = publishers;
        Categories = categories;
        States = states;
      
    }


    public IPublisherRepository Publishers { get; }
    public ICategoryRepository Categories { get; }
    public IStateRepository States { get; }


    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}
