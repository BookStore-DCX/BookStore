using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IPurchaseLogRepository : IGenericRepository<Purchaselog>
    {
        Task<IEnumerable<Purchaselog>> GetPurchasesByUserAsync(int userId);
    }
}
