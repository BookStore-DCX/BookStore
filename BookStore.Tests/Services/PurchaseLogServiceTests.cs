using AutoMapper;
using BookStore.DTOs.PurchaseLog;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using Xunit;

namespace BookStore.Tests.Services
{
    public class PurchaseLogServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPurchaseLogRepository> _repoMock;
        private readonly PurchaseLogService _service;

        public PurchaseLogServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _repoMock = new Mock<IPurchaseLogRepository>();

            _uowMock.Setup(u => u.PurchaseLogs)
                    .Returns(_repoMock.Object);


            _uowMock.Setup(u => u.SaveChangesAsync())
                    .ReturnsAsync(1);

            _service = new PurchaseLogService(
                _uowMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetPurchasesByUserAsync_ValidUserId_ReturnsMappedDtos()
        {
            var logs = new List<Purchaselog>
            {
                new Purchaselog
                {
                    UserId = 1,
                    InventoryId = 10
                }
            };

            var dtos = new List<PurchaseLogDto>
            {
                new PurchaseLogDto
                {
                    UserId = 1,
                    InventoryId = 10
                }
            };

            _repoMock.Setup(r => r.GetPurchasesByUserAsync(1))
                     .ReturnsAsync(logs);

            _mapperMock.Setup(m => m.Map<IEnumerable<PurchaseLogDto>>(logs))
                       .Returns(dtos);

            var result = await _service.GetPurchasesByUserAsync(1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().UserId);
        }

        [Fact]
        public async Task GetPurchasesByUserAsync_UserWithMultipleLogs_ReturnsAllMappedDtos()
        {
            var logs = new List<Purchaselog>
            {
                new Purchaselog
                {
                    UserId = 2,
                    InventoryId = 1
                },
                new Purchaselog
                {
                    UserId = 2,
                    InventoryId = 2
                }
            };

            var dtos = new List<PurchaseLogDto>
            {
                new PurchaseLogDto
                {
                    UserId = 2,
                    InventoryId = 1
                },
                new PurchaseLogDto
                {
                    UserId = 2,
                    InventoryId = 2
                }
            };

            _repoMock.Setup(r => r.GetPurchasesByUserAsync(2))
                     .ReturnsAsync(logs);

            _mapperMock.Setup(m => m.Map<IEnumerable<PurchaseLogDto>>(logs))
                       .Returns(dtos);

            var result = await _service.GetPurchasesByUserAsync(2);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreatePurchaseLogAsync_ValidDto_ReturnsCreatedDto()
        {
            var createDto = new PurchaseLogCreateDto
            {
                UserId = 1,
                InventoryId = 5
            };

            var log = new Purchaselog
            {
                UserId = 1,
                InventoryId = 5
            };

            var resultDto = new PurchaseLogDto
            {
                UserId = 1,
                InventoryId = 5
            };

            _mapperMock.Setup(m => m.Map<Purchaselog>(createDto))
                       .Returns(log);

            _repoMock.Setup(r => r.AddAsync(log))
                     .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<PurchaseLogDto>(log))
                       .Returns(resultDto);

            var result = await _service.CreatePurchaseLogAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(5, result.InventoryId);
        }

        [Fact]
        public async Task CreatePurchaseLogAsync_ValidDto_CallsSaveChangesOnce()
        {
            var createDto = new PurchaseLogCreateDto
            {
                UserId = 3,
                InventoryId = 7
            };

            var log = new Purchaselog
            {
                UserId = 3,
                InventoryId = 7
            };

            _mapperMock.Setup(m => m.Map<Purchaselog>(createDto))
                       .Returns(log);

            _repoMock.Setup(r => r.AddAsync(log))
                     .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<PurchaseLogDto>(log))
                       .Returns(new PurchaseLogDto());

            await _service.CreatePurchaseLogAsync(createDto);

            _uowMock.Verify(
                u => u.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task GetPurchasesByUserAsync_UserWithNoLogs_ReturnsEmptyCollection()
        {
            _repoMock.Setup(r => r.GetPurchasesByUserAsync(99))
                     .ReturnsAsync(new List<Purchaselog>());

            _mapperMock.Setup(
                m => m.Map<IEnumerable<PurchaseLogDto>>(
                    It.IsAny<IEnumerable<Purchaselog>>()))
                .Returns(new List<PurchaseLogDto>());

            var result = await _service.GetPurchasesByUserAsync(99);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPurchasesByUserAsync_RepositoryThrows_PropagatesException()
        {
            _repoMock.Setup(r => r.GetPurchasesByUserAsync(It.IsAny<int>()))
                     .ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(
                () => _service.GetPurchasesByUserAsync(1));
        }

        [Fact]
        public async Task CreatePurchaseLogAsync_SaveChangesThrows_PropagatesException()
        {
            var createDto = new PurchaseLogCreateDto
            {
                UserId = 1,
                InventoryId = 5
            };

            var log = new Purchaselog
            {
                UserId = 1,
                InventoryId = 5
            };

            _mapperMock.Setup(m => m.Map<Purchaselog>(It.IsAny<PurchaseLogCreateDto>()))
                       .Returns(log);

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Purchaselog>()))
                     .Returns(Task.CompletedTask);

            _uowMock.Setup(u => u.SaveChangesAsync())
                    .ThrowsAsync(new Exception("Save failed"));

            await Assert.ThrowsAsync<Exception>(
                () => _service.CreatePurchaseLogAsync(createDto));
        }

        [Fact]
        public async Task CreatePurchaseLogAsync_AddAsyncThrows_DoesNotCallSaveChanges()
        {
            var createDto = new PurchaseLogCreateDto
            {
                UserId = 1,
                InventoryId = 5
            };

            var log = new Purchaselog
            {
                UserId = 1,
                InventoryId = 5
            };

            _mapperMock.Setup(m => m.Map<Purchaselog>(
                    It.IsAny<PurchaseLogCreateDto>()))
                .Returns(log);

            _repoMock.Setup(r => r.AddAsync(
                    It.IsAny<Purchaselog>()))
                .ThrowsAsync(new Exception("Add failed"));

            await Assert.ThrowsAsync<Exception>(
                () => _service.CreatePurchaseLogAsync(createDto));

            _uowMock.Verify(
                u => u.SaveChangesAsync(),
                Times.Never);
        }
    }
}