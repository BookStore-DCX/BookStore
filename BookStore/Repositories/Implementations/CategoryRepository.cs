using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(BookContext context) : base(context)
        {
        }

        public async Task<Category?> GetByCategoryNameAsync(string name)
        {
            return await _context.Set<Category>()
                .Where(c => c.CatDescription == name)
                .Select(c => new Category
                {
                    CatId = c.CatId,
                    CatDescription = c.CatDescription
                })
                .FirstOrDefaultAsync();
        }
    }
}