using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Bookreview>
    {
        Task<IEnumerable<Bookreview>> GetReviewsByBookNameAsync(string bookName);
        Task<IEnumerable<Bookreview>> GetReviewsByReviewerAsync(int reviewerId);
    }
}