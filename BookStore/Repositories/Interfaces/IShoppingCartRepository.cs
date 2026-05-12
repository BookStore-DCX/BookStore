using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IShoppingCartRepository : IGenericRepository<Shoppingcart>
    {
        Task<IEnumerable<Shoppingcart>> GetCartByUserAsync(int userId);
        Task RemoveFromCartAsync(int userId, string isbn);
        Task ClearCartAsync(int userId);
    }
}
