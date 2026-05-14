using AutoMapper;
using BookStore.DTOs.PurchaseLog;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class PurchaseLogService : IPurchaseLogService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PurchaseLogService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PurchaseLogDto>> GetPurchasesByUserAsync(int userId)
            => _mapper.Map<IEnumerable<PurchaseLogDto>>(await _uow.PurchaseLogs.GetPurchasesByUserAsync(userId));

        public async Task<PurchaseLogDto> CreatePurchaseLogAsync(PurchaseLogCreateDto dto)
        {
            var log = _mapper.Map<Purchaselog>(dto);
            await _uow.PurchaseLogs.AddAsync(log);
            await _uow.SaveChangesAsync();
            return _mapper.Map<PurchaseLogDto>(log);
        }
    }
}
