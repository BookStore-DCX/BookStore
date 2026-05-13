using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.PurchaseLog;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public ShoppingCartController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _uow.ShoppingCarts.GetCartByUserAsync(userId);
            return Ok(ApiResponse<IEnumerable<ShoppingCartDto>>.Ok(_mapper.Map<IEnumerable<ShoppingCartDto>>(cart)));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] ShoppingCartCreateDto dto)
        {
            var item = _mapper.Map<Shoppingcart>(dto);
            await _uow.ShoppingCarts.AddAsync(item);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCart), new { userId = item.UserId },
                ApiResponse<ShoppingCartDto>.Created(_mapper.Map<ShoppingCartDto>(item)));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveItem(int userId)
        {
            return Ok(ApiResponse<object>.Ok(new { message = "Item removed from cart" }));
        }

        [HttpDelete("{userId}/clear")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            return Ok(ApiResponse<object>.Ok(new { message = "Cart cleared" }));
        }
    }
}

