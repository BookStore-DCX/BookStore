using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.PurchaseLog;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "RegisteredUser,Admin,StoreOwner")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public ShoppingCartController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            userId = GetCurrentUserId();
            var cart = await _uow.ShoppingCarts.GetCartByUserAsync(userId);
            return Ok(ApiResponse<IEnumerable<ShoppingCartDto>>.Ok(_mapper.Map<IEnumerable<ShoppingCartDto>>(cart)));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] ShoppingCartCreateDto dto)
        {
            dto.UserId = GetCurrentUserId();

            var existingCart = await _uow.ShoppingCarts.GetCartByUserAsync(dto.UserId);
            var existingItem = existingCart.FirstOrDefault(item => item.Isbn == dto.Isbn);
            if (existingItem != null)
            {
                return Ok(ApiResponse<ShoppingCartDto>.Ok(
                    _mapper.Map<ShoppingCartDto>(existingItem),
                    "Book is already in your cart"));
            }

            var item = _mapper.Map<Shoppingcart>(dto);
            await _uow.ShoppingCarts.AddAsync(item);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCart), new { userId = item.UserId },
                ApiResponse<ShoppingCartDto>.Created(_mapper.Map<ShoppingCartDto>(item)));
        }

        [HttpDelete("{userId}/{isbn}")]
        public async Task<IActionResult> RemoveItem(int userId, string isbn)
        {
            userId = GetCurrentUserId();
            await _uow.ShoppingCarts.RemoveFromCartAsync(userId, isbn);
            await _uow.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(new { message = "Item removed from cart" }));
        }

        [HttpDelete("{userId}/clear")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            userId = GetCurrentUserId();
            await _uow.ShoppingCarts.ClearCartAsync(userId);
            await _uow.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(new { message = "Cart cleared" }));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}

