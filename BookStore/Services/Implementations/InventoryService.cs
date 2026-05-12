using AutoMapper;
using BookStore.DTOs.Inventory;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public InventoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryDto>> GetAllInventoryAsync()
            => _mapper.Map<IEnumerable<InventoryDto>>(await _uow.Inventories.GetAllAsync());

        public async Task<IEnumerable<InventoryDto>> GetInventoryByBookAsync(string isbn)
            => _mapper.Map<IEnumerable<InventoryDto>>(await _uow.Inventories.GetInventoryByBookAsync(isbn));

        public async Task<IEnumerable<InventoryDto>> GetAvailableInventoryAsync()
            => _mapper.Map<IEnumerable<InventoryDto>>(await _uow.Inventories.GetAvailableInventoryAsync());

        public async Task<InventoryDto> CreateInventoryAsync(InventoryCreateDto dto)
        {
            var inventory = _mapper.Map<Inventory>(dto);
            inventory.Purchased = 0;
            await _uow.Inventories.AddAsync(inventory);
            await _uow.SaveChangesAsync();
            return _mapper.Map<InventoryDto>(inventory);
        }

        public async Task DeleteInventoryAsync(int id)
        {
            await _uow.Inventories.DeleteAsync(id);
            await _uow.SaveChangesAsync();
        }
    }

}
