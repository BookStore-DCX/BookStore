using BookStore.Data;
using BookStore.Models;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Implementations
{
    public class BookConditionRepository : GenericRepository<Bookcondition>, IBookConditionRepository
    {
        public BookConditionRepository(BookContext context) : base(context)
        {
        }
    }
}
