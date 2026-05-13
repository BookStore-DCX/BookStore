using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetByCategoryNameAsync(string name);
    }
}