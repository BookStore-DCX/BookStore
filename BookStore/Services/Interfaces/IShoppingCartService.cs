using BookStore.DTOs.PurchaseLog;

namespace BookStore.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<IEnumerable<ShoppingCartDto>> GetCartByUserAsync(int userId);
        Task<ShoppingCartDto> AddToCartAsync(ShoppingCartCreateDto dto);
        Task RemoveFromCartAsync(int userId, string isbn);
        Task ClearCartAsync(int userId);
    }
}
