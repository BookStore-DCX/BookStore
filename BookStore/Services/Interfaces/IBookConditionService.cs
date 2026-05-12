using BookStore.DTOs.BookCondition;

namespace BookStore.Services.Interfaces
{
    public interface IBookConditionService
    {
        Task<IEnumerable<BookConditionDto>> GetAllConditionsAsync();
        Task<BookConditionDto> GetConditionByRankAsync(int rank);
    }

}
