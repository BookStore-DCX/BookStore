using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class CartService : ICartService
{
    private readonly IApiClient _apiClient;

    public CartService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<ShoppingCartItemViewModel>>> GetCartAsync(int userId) => _apiClient.GetAsync<List<ShoppingCartItemViewModel>>($"ShoppingCart/{userId}");

    public Task<ApiResult<ShoppingCartItemViewModel>> AddAsync(ShoppingCartCreateViewModel model) => _apiClient.PostAsync<ShoppingCartCreateViewModel, ShoppingCartItemViewModel>("ShoppingCart", model);

    public Task<ApiResult<bool>> RemoveAsync(int userId) => _apiClient.DeleteAsync($"ShoppingCart/{userId}");

    public Task<ApiResult<bool>> ClearAsync(int userId) => _apiClient.DeleteAsync($"ShoppingCart/{userId}/clear");

    public Task<ApiResult<PurchaseLogViewModel>> PurchaseAsync(int inventoryId)
    {
        return _apiClient.PostAsync<object, PurchaseLogViewModel>("PurchaseLog", new { inventoryId });
    }

    public Task<ApiResult<List<PurchaseLogViewModel>>> MyPurchasesAsync() => _apiClient.GetAsync<List<PurchaseLogViewModel>>("PurchaseLog/my");
}
