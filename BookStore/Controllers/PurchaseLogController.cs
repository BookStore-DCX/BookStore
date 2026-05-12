using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.ShoppingCart;
using BookStore.DTOs.User;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PurchaseLogController : ControllerBase
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public PurchaseLogController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var logs = await _uow.PurchaseLogs.GetPurchasesByUserAsync(userId);
            return Ok(ApiResponse<IEnumerable<PurchaseLogDto>>.Ok(_mapper.Map<IEnumerable<PurchaseLogDto>>(logs)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseLogCreateDto dto)
        {
            var log = _mapper.Map<Purchaselog>(dto);
            await _uow.PurchaseLogs.AddAsync(log);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByUser), new { userId = log.UserId },
                ApiResponse<PurchaseLogDto>.Created(_mapper.Map<PurchaseLogDto>(log)));
        }
    }
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Policy = "AdminOnly")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public UserController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _uow.Users.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(_mapper.Map<IEnumerable<UserDto>>(users)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var u = await _uow.Users.GetByIdAsync(id) ?? throw new NotFoundException($"User {id} not found");
            return Ok(ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(u)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(id) ?? throw new NotFoundException($"User {id} not found");
            _mapper.Map(dto, user);
            await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();
            return Ok(ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(user)));
        }
    }

}
