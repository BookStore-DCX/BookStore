using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Data;
namespace BookStore.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(BookContext context) : base(context)
        {
        }
    }
}