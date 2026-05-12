using AutoMapper;
using BookStore.DTOs.PurchaseLog;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ShoppingCartService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShoppingCartDto>> GetCartByUserAsync(int userId)
            => _mapper.Map<IEnumerable<ShoppingCartDto>>(await _uow.ShoppingCarts.GetCartByUserAsync(userId));

        public async Task<ShoppingCartDto> AddToCartAsync(ShoppingCartCreateDto dto)
        {
            var item = _mapper.Map<Shoppingcart>(dto);
            await _uow.ShoppingCarts.AddAsync(item);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ShoppingCartDto>(item);
        }

        public async Task RemoveFromCartAsync(int userId, string isbn)
        {
            await _uow.ShoppingCarts.RemoveFromCartAsync(userId, isbn);
            await _uow.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            await _uow.ShoppingCarts.ClearCartAsync(userId);
            await _uow.SaveChangesAsync();
        }
    }
}
