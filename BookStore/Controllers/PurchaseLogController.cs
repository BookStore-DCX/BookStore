using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.PurchaseLog;
using BookStore.Exceptions;
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
    [Authorize]
    public class PurchaseLogController : ControllerBase
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public PurchaseLogController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedException("Invalid user id in token.");
            }

            var logs = await _uow.PurchaseLogs.GetPurchasesByUserAsync(userId);
            return Ok(ApiResponse<IEnumerable<PurchaseLogDto>>.Ok(_mapper.Map<IEnumerable<PurchaseLogDto>>(logs)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseLogCreateDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedException("Invalid user id in token.");
            }

            dto.UserId = userId;

            var log = _mapper.Map<Purchaselog>(dto);
            await _uow.PurchaseLogs.AddAsync(log);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMy), ApiResponse<PurchaseLogDto>.Created(_mapper.Map<PurchaseLogDto>(log)));
        }
    }
}
