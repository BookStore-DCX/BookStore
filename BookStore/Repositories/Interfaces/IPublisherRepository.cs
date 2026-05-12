using BookStore.Models;
using BookStore.Data;

namespace BookStore.Repositories.Interfaces
{
    public interface IPublisherRepository : IGenericRepository<Publisher>
    {
        Task<IEnumerable<Publisher>> GetPublishersByStateAsync(string stateCode);
    }
}
