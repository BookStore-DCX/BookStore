using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;

namespace BookStore.Mvc.Services.Interfaces;

public interface ICartService
{
    Task<ApiResult<List<ShoppingCartItemViewModel>>> GetCartAsync(int userId);
    Task<ApiResult<ShoppingCartItemViewModel>> AddAsync(ShoppingCartCreateViewModel model);
    Task<ApiResult<bool>> RemoveAsync(int userId, string isbn);
    Task<ApiResult<bool>> ClearAsync(int userId);
    Task<ApiResult<PurchaseLogViewModel>> PurchaseAsync(int inventoryId);
    Task<ApiResult<List<PurchaseLogViewModel>>> MyPurchasesAsync();
}
