using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Inventory;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        [Authorize(Policy = "StoreOwner")]
        public async Task<IActionResult> GetAll()
        {
            var inventory = await _uow.Inventories.GetAllAsync();

            var result = _mapper.Map<IEnumerable<InventoryDto>>(inventory);

            return Ok(ApiResponse<IEnumerable<InventoryDto>>.Ok(result));
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var inventory = await _uow.Inventories.GetAvailableInventoryAsync();

            var result = _mapper.Map<IEnumerable<InventoryDto>>(inventory);

            return Ok(ApiResponse<IEnumerable<InventoryDto>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Policy = "StoreOwner")]
        public async Task<IActionResult> Create([FromBody] InventoryCreateDto dto)
        {
            var inventory = _mapper.Map<Inventory>(dto);

            await _uow.Inventories.AddAsync(inventory);

            await _uow.SaveChangesAsync();

            var result = _mapper.Map<InventoryDto>(inventory);

            return CreatedAtAction(
                nameof(GetAll),
                ApiResponse<InventoryDto>.Created(result)
            );
        }
    }
}
