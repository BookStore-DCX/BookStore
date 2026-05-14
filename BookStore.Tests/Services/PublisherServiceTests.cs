using AutoMapper;
using BookStore.DTOs.Publisher;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using Xunit;

namespace BookStore.Tests.Services
{
    public class PublisherServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPublisherRepository> _publisherRepoMock;
        private readonly PublisherService _service;

        public PublisherServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _publisherRepoMock = new Mock<IPublisherRepository>();
            _uowMock.Setup(u => u.Publishers).Returns(_publisherRepoMock.Object);
            _service = new PublisherService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllPublishersAsync_ReturnsAllPublishers()
        {
            var publishers = new List<Publisher>
            {
                new Publisher { PublisherId = 1, Name = "Penguin", City = "New York", StateCode = "NY" }
            };
            var dtos = new List<PublisherDto>
            {
                new PublisherDto { PublisherId = 1, Name = "Penguin", City = "New York", StateCode = "NY" }
            };
            _publisherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(publishers);
            _mapperMock.Setup(m => m.Map<IEnumerable<PublisherDto>>(publishers)).Returns(dtos);

            var result = await _service.GetAllPublishersAsync();

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task GetPublisherByIdAsync_ValidId_ReturnsPublisherDto()
        {
            var publisher = new Publisher { PublisherId = 1, Name = "Penguin", City = "New York", StateCode = "NY" };
            var dto = new PublisherDto { PublisherId = 1, Name = "Penguin", City = "New York", StateCode = "NY" };
            _publisherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(publisher);
            _mapperMock.Setup(m => m.Map<PublisherDto>(publisher)).Returns(dto);

            var result = await _service.GetPublisherByIdAsync(1);

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task CreatePublisherAsync_ValidDto_ReturnsCreatedPublisherDto()
        {
            var createDto = new PublisherCreateDto { Name = "Penguin", City = "New York", StateCode = "NY" };
            var publisher = new Publisher { PublisherId = 1, Name = "Penguin", City = "New York", StateCode = "NY" };
            var dto = new PublisherDto { PublisherId = 1, Name = "Penguin", City = "New York", StateCode = "NY" };
            _mapperMock.Setup(m => m.Map<Publisher>(createDto)).Returns(publisher);
            _mapperMock.Setup(m => m.Map<PublisherDto>(publisher)).Returns(dto);
            _publisherRepoMock.Setup(r => r.AddAsync(publisher)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.CreatePublisherAsync(createDto);

            Assert.Equal(dto, result);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePublisherAsync_ValidId_ReturnsUpdatedPublisherDto()
        {
            var updateDto = new PublisherCreateDto { Name = "Updated Penguin", City = "Boston", StateCode = "MA" };
            var publisher = new Publisher { PublisherId = 1, Name = "Penguin", City = "New York", StateCode = "NY" };
            var dto = new PublisherDto { PublisherId = 1, Name = "Updated Penguin", City = "Boston", StateCode = "MA" };
            _publisherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(publisher);
            _mapperMock.Setup(m => m.Map(updateDto, publisher));
            _mapperMock.Setup(m => m.Map<PublisherDto>(publisher)).Returns(dto);
            _publisherRepoMock.Setup(r => r.UpdateAsync(publisher)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.UpdatePublisherAsync(1, updateDto);

            Assert.Equal(dto, result);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPublisherByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            _publisherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Publisher)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPublisherByIdAsync(99));
        }

        [Fact]
        public async Task UpdatePublisherAsync_InvalidId_ThrowsNotFoundException()
        {
            _publisherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Publisher)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.UpdatePublisherAsync(99, new PublisherCreateDto()));
        }

        [Fact]
        public async Task DeletePublisherAsync_InvalidId_ThrowsNotFoundException()
        {
            _publisherRepoMock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeletePublisherAsync(99));
        }

        [Fact]
        public async Task DeletePublisherAsync_ValidId_DeletesAndSaves()
        {
            _publisherRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _publisherRepoMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _service.DeletePublisherAsync(1);

            _publisherRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}