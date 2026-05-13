using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BookStore.Repositories.Implementations
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(BookContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Author>> SearchAuthorsAsync(string authorName)
            => await _dbSet.AsNoTracking()
                .Where(a => a.FirstName.Contains(authorName)
                    || a.LastName.Contains(authorName)
                    || (a.FirstName + " " + a.LastName).Contains(authorName)
                    || (a.LastName + " " + a.FirstName).Contains(authorName))
                .ToListAsync();
    }
}