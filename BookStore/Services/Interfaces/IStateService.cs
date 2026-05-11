using BookStore.DTOs.State;

namespace BookStore.Services.Interfaces
{
    public interface IStateService
    {
        Task<IEnumerable<StateDto>> GetAllStatesAsync();
        Task<StateDto> GetStateByCodeAsync(string stateCode);
    }
    }
