using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Bookreview>
    {
        Task<IEnumerable<Bookreview>> GetReviewsByBookNameAsync(string bookName);
        Task<IEnumerable<Bookreview>> GetReviewsByBookIsbnAsync(string isbn);
        Task<IEnumerable<Bookreview>> GetReviewsByReviewerAsync(int reviewerId);

        Task<Reviewer?> GetReviewerByIdAsync(int reviewerId);
        Task<Reviewer?> GetReviewerByNameAsync(string fullName);
        Task<bool> ReviewExistsAsync(string isbn, int reviewerId);
        Task<int> GetNextReviewerIdAsync();
        Task AddReviewerAsync(Reviewer reviewer);
        Task DeleteReviewAsync(string isbn, int reviewerId);
    }
}
