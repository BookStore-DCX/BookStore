using BookStore.DTOs.Inventory;

namespace BookStore.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllInventoryAsync();
        Task<IEnumerable<InventoryDto>> GetInventoryByBookAsync(string isbn);
        Task<IEnumerable<InventoryDto>> GetAvailableInventoryAsync();
        Task<InventoryDto> CreateInventoryAsync(InventoryCreateDto dto);
        Task<InventoryDto> UpdateInventoryAsync(int id, InventoryUpdateDto dto);
        Task DeleteInventoryAsync(int id);
    }

}
