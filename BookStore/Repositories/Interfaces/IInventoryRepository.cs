using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        Task<IEnumerable<Inventory>> GetInventoryByBookAsync(string isbn);
        Task<IEnumerable<Inventory>> GetAvailableInventoryAsync();
    }
}
