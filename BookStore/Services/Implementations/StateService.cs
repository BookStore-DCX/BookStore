using AutoMapper;
using BookStore.DTOs.State;
using BookStore.Exceptions;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class StateService : IStateService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public StateService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StateDto>> GetAllStatesAsync()
        {
            var states = await _uow.States.GetAllAsync();

            return _mapper.Map<IEnumerable<StateDto>>(states);
        }

        public async Task<StateDto> GetStateByCodeAsync(string stateCode)
        {
            var state = await _uow.States.GetByIdAsync(stateCode)
                ?? throw new NotFoundException(
                    $"State with code '{stateCode}' not found"
                );

            return _mapper.Map<StateDto>(state);
        }
    }
}