using AutoMapper;
using BookStore.DTOs.BookCondition;
using BookStore.Exceptions;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class BookConditionService : IBookConditionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BookConditionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookConditionDto>> GetAllConditionsAsync()
            => _mapper.Map<IEnumerable<BookConditionDto>>(await _uow.BookConditions.GetAllAsync());

        public async Task<BookConditionDto> GetConditionByRankAsync(int rank)
        {
            var condition = await _uow.BookConditions.GetByIdAsync(rank)
                ?? throw new NotFoundException($"Book condition rank {rank} not found");
            return _mapper.Map<BookConditionDto>(condition);
        }
    }

}
