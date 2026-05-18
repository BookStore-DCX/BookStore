using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Models.Catalog;

namespace BookStore.Mvc.Services.Interfaces;

public interface IInventoryService
{
    Task<ApiResult<List<InventoryViewModel>>> GetAllAsync();
    Task<ApiResult<InventoryViewModel>> CreateAsync(InventoryFormViewModel model);
    Task<ApiResult<InventoryViewModel>> UpdateAsync(int id, InventoryFormViewModel model);
    Task<ApiResult<bool>> DeleteAsync(int id);
}
