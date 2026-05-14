using BookStore.DTOs.Publisher;

namespace BookStore.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherDto>> GetAllPublishersAsync();
        Task<IEnumerable<PublisherDto>> GetPublishersByNameAsync(string name);
        Task<PublisherDto> GetPublisherByIdAsync(int id);
        Task<IEnumerable<PublisherDto>> GetPublishersByStateAsync(string stateCode);
        Task<PublisherDto> CreatePublisherAsync(PublisherCreateDto dto);
        Task<PublisherDto> UpdatePublisherAsync(int id, PublisherCreateDto dto);
        Task DeletePublisherAsync(int id);
    }
}
