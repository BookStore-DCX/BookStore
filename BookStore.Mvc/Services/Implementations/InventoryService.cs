using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class InventoryService : IInventoryService
{
    private readonly IApiClient _apiClient;

    public InventoryService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<InventoryViewModel>>> GetAllAsync() => _apiClient.GetAsync<List<InventoryViewModel>>("Inventory");

    public Task<ApiResult<InventoryViewModel>> CreateAsync(InventoryFormViewModel model) => _apiClient.PostAsync<InventoryFormViewModel, InventoryViewModel>("Inventory", model);

    public Task<ApiResult<InventoryViewModel>> UpdateAsync(int id, InventoryFormViewModel model) => _apiClient.PutAsync<InventoryFormViewModel, InventoryViewModel>($"Inventory/{id}", model);

    public Task<ApiResult<bool>> DeleteAsync(int id) => _apiClient.DeleteAsync($"Inventory/{id}");
}
