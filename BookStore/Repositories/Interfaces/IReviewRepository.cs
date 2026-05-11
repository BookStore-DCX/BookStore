using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Bookreview>
    {
        Task<IEnumerable<Bookreview>> GetReviewsByBookAsync(string isbn);
        Task<IEnumerable<Bookreview>> GetReviewsByReviewerAsync(int reviewerId);
    }
}
