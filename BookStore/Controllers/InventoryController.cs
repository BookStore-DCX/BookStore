using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Inventory;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, StoreOwner")]
    public class InventoryController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public InventoryController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventory = await _uow.Inventories.GetAllAsync();
            var result = _mapper.Map<IEnumerable<InventoryDto>>(inventory);
            return Ok(ApiResponse<IEnumerable<InventoryDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryCreateDto dto)
        {
            var inventory = _mapper.Map<Inventory>(dto);
            inventory.Purchased = 0;

            await _uow.Inventories.AddAsync(inventory);
            await _uow.SaveChangesAsync();

            var result = _mapper.Map<InventoryDto>(inventory);

            return CreatedAtAction(
                nameof(GetByBook),
                new { isbn = inventory.Isbn },
                ApiResponse<InventoryDto>.Created(result)
            );
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var inventory = await _uow.Inventories.GetAvailableInventoryAsync();
            var result = _mapper.Map<IEnumerable<InventoryDto>>(inventory);
            return Ok(ApiResponse<IEnumerable<InventoryDto>>.Ok(result));
        }

        [HttpGet("book/{isbn}")]
        public async Task<IActionResult> GetByBook(string isbn)
        {
            var inventory = await _uow.Inventories.GetInventoryByBookAsync(isbn);
            var result = _mapper.Map<IEnumerable<InventoryDto>>(inventory);
            return Ok(ApiResponse<IEnumerable<InventoryDto>>.Ok(result));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryUpdateDto dto)
        {
            var inventory = await _uow.Inventories.GetByIdAsync(id)
                ?? throw new NotFoundException($"Inventory ID {id} not found");

            if (!string.IsNullOrWhiteSpace(dto.Isbn))
            {
                inventory.Isbn = dto.Isbn;
            }

            if (dto.Ranks.HasValue)
            {
                inventory.Ranks = dto.Ranks.Value;
            }

            if (dto.Purchased.HasValue)
            {
                inventory.Purchased = dto.Purchased.Value;
            }

            await _uow.Inventories.UpdateAsync(inventory);
            await _uow.SaveChangesAsync();

            return Ok(ApiResponse<InventoryDto>.Ok(_mapper.Map<InventoryDto>(inventory)));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _uow.Inventories.DeleteAsync(id);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
