using AutoMapper;
using BookStore.DTOs.Publisher;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PublisherService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PublisherDto>> GetAllPublishersAsync()
        {
            var publishers = await _uow.Publishers.GetAllAsync();

            return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
        }

        public async Task<PublisherDto> GetPublisherByIdAsync(int id)
        {
            var publisher = await _uow.Publishers.GetByIdAsync(id)
                ?? throw new NotFoundException(
                    $"Publisher with ID {id} not found"
                );

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<IEnumerable<PublisherDto>> GetPublishersByStateAsync(
            string stateCode
        )
        {
            var publishers = await _uow.Publishers
                .GetPublishersByStateAsync(stateCode);

            return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
        }

        public async Task<PublisherDto> CreatePublisherAsync(
            PublisherCreateDto dto
        )
        {
            var publisher = _mapper.Map<Publisher>(dto);

            await _uow.Publishers.AddAsync(publisher);
            await _uow.SaveChangesAsync();

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<PublisherDto> UpdatePublisherAsync(
            int id,
            PublisherCreateDto dto
        )
        {
            var publisher = await _uow.Publishers.GetByIdAsync(id)
                ?? throw new NotFoundException(
                    $"Publisher with ID {id} not found"
                );

            _mapper.Map(dto, publisher);

            await _uow.Publishers.UpdateAsync(publisher);
            await _uow.SaveChangesAsync();

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task DeletePublisherAsync(int id)
        {
            if (!await _uow.Publishers.ExistsAsync(id))
            {
                throw new NotFoundException(
                    $"Publisher with ID {id} not found"
                );
            }

            await _uow.Publishers.DeleteAsync(id);
            await _uow.SaveChangesAsync();
        }
    }
}