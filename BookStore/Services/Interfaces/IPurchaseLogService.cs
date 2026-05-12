using BookStore.DTOs.ShoppingCart;

namespace BookStore.Services.Interfaces
{
    public interface IPurchaseLogService
    {
        Task<IEnumerable<PurchaseLogDto>> GetPurchasesByUserAsync(int userId);
        Task<PurchaseLogDto> CreatePurchaseLogAsync(PurchaseLogCreateDto dto);
    }
}
