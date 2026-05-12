using BookStore.Models;
using BookStore.Data;
using BookStore.Repositories.Interfaces;

namespace BookStore.Repositories.Implementations
{
    public class StateRepository : GenericRepository<State>, IStateRepository
    {
        public StateRepository(BookContext context) : base(context)
        {
        }
    }
}