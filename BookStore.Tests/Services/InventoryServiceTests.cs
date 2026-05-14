using Xunit;
using Moq;
using AutoMapper;
using BookStore.DTOs.Inventory;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using BookStore.Exceptions;

namespace BookStore.Tests.Services
{
    public class InventoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new InventoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllInventoryAsync_WithValidData_ReturnsAllInventory()
        {
            var inventories = new List<Inventory>
            {
                new() { InventoryId = 1, Isbn = "978-0-123456-78-9", Ranks = 1, Purchased = 5 },
                new() { InventoryId = 2, Isbn = "978-0-123456-79-6", Ranks = 2, Purchased = 3 },
                new() { InventoryId = 3, Isbn = "978-0-123456-80-2", Ranks = 3, Purchased = 0 }
            };
            var expectedDtos = new List<InventoryDto>
            {
                new() { InventoryId = 1, Isbn = "978-0-123456-78-9", Ranks = 1, Purchased = (byte)5 },
                new() { InventoryId = 2, Isbn = "978-0-123456-79-6", Ranks = 2, Purchased = (byte)3 },
                new() { InventoryId = 3, Isbn = "978-0-123456-80-2", Ranks = 3, Purchased = (byte)0 }
            };

            _mockUnitOfWork.Setup(u => u.Inventories.GetAllAsync())
                .ReturnsAsync(inventories);
            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryDto>>(inventories))
                .Returns(expectedDtos);

            var result = await _service.GetAllInventoryAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(expectedDtos, result);
            _mockUnitOfWork.Verify(u => u.Inventories.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateInventoryAsync_WithValidDto_CreatesAndReturnsInventory()
        {
            var createDto = new InventoryCreateDto
            {
                Isbn = "978-0-123456-78-9",
                Ranks = 1
            };
            var inventory = new Inventory
            {
                InventoryId = 1,
                Isbn = "978-0-123456-78-9",
                Ranks = 1,
                Purchased = 0
            };
            var expectedDto = new InventoryDto
            {
                InventoryId = 1,
                Isbn = "978-0-123456-78-9",
                Ranks = 1,
                Purchased = (byte)0
            };

            _mockMapper.Setup(m => m.Map<Inventory>(createDto))
                .Returns(inventory);
            _mockUnitOfWork.Setup(u => u.Inventories.AddAsync(It.IsAny<Inventory>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<InventoryDto>(inventory))
                .Returns(expectedDto);

            var result = await _service.CreateInventoryAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal(expectedDto.InventoryId, result.InventoryId);
            Assert.Equal(expectedDto.Isbn, result.Isbn);
            Assert.Equal((byte)0, result.Purchased);
            _mockUnitOfWork.Verify(u => u.Inventories.AddAsync(It.IsAny<Inventory>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateInventoryAsync_WithValidData_UpdatesAndReturnsInventory()
        {
            int inventoryId = 1;
            var updateDto = new InventoryUpdateDto
            {
                Isbn = "978-0-987654-32-1",
                Ranks = 2,
                Purchased = 5
            };
            var existingInventory = new Inventory
            {
                InventoryId = 1,
                Isbn = "978-0-123456-78-9",
                Ranks = 1,
                Purchased = 0
            };
            var expectedDto = new InventoryDto
            {
                InventoryId = 1,
                Isbn = "978-0-987654-32-1",
                Ranks = 2,
                Purchased = (byte)5
            };

            _mockUnitOfWork.Setup(u => u.Inventories.GetByIdAsync(inventoryId))
                .ReturnsAsync(existingInventory);
            _mockUnitOfWork.Setup(u => u.Inventories.UpdateAsync(It.IsAny<Inventory>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<InventoryDto>(It.IsAny<Inventory>()))
                .Returns(expectedDto);
            
            var result = await _service.UpdateInventoryAsync(inventoryId, updateDto);

            Assert.NotNull(result);
            Assert.Equal("978-0-987654-32-1", result.Isbn);
            Assert.Equal(2, result.Ranks);
            Assert.Equal((byte)5, result.Purchased);
            _mockUnitOfWork.Verify(u => u.Inventories.UpdateAsync(It.IsAny<Inventory>()), Times.Once);
        }

        [Fact]
        public async Task GetInventoryByBookAsync_WithValidIsbn_ReturnsInventoryForBook()
        {
            string isbn = "978-0-123456-78-9";
            var inventories = new List<Inventory>
            {
                new() { InventoryId = 1, Isbn = isbn, Ranks = 1, Purchased = 5 },
                new() { InventoryId = 2, Isbn = isbn, Ranks = 2, Purchased = 3 }
            };
            var expectedDtos = new List<InventoryDto>
            {
                new() { InventoryId = 1, Isbn = isbn, Ranks = 1, Purchased = (byte)5 },
                new() { InventoryId = 2, Isbn = isbn, Ranks = 2, Purchased = (byte)3 }
            };

            _mockUnitOfWork.Setup(u => u.Inventories.GetInventoryByBookAsync(isbn))
                .ReturnsAsync(inventories);
            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryDto>>(inventories))
                .Returns(expectedDtos);

            var result = await _service.GetInventoryByBookAsync(isbn);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(isbn, item.Isbn));
        }

        [Fact]
        public async Task UpdateInventoryAsync_WithInvalidId_ThrowsNotFoundException()
        {
            int invalidId = 999;
            var updateDto = new InventoryUpdateDto { Isbn = "978-0-123456-78-9" };

            _mockUnitOfWork.Setup(u => u.Inventories.GetByIdAsync(invalidId))
                .ReturnsAsync((Inventory)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _service.UpdateInventoryAsync(invalidId, updateDto)
            );
            Assert.Contains("not found", exception.Message);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateInventoryAsync_WithRepositoryException_PropagatesException()
        {
            var createDto = new InventoryCreateDto
            {
                Isbn = "978-0-123456-78-9",
                Ranks = 1
            };
            var inventory = new Inventory
            {
                InventoryId = 1,
                Isbn = "978-0-123456-78-9",
                Ranks = 1,
                Purchased = 0
            };

            _mockMapper.Setup(m => m.Map<Inventory>(createDto))
                .Returns(inventory);
            _mockUnitOfWork.Setup(u => u.Inventories.AddAsync(It.IsAny<Inventory>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateInventoryAsync(createDto)
            );
        }

        [Fact]
        public async Task GetInventoryByBookAsync_WithInvalidIsbn_ReturnsEmptyList()
        {
            string invalidIsbn = "999-9-999999-99-9";
            var emptyInventories = new List<Inventory>();
            var expectedDtos = new List<InventoryDto>();

            _mockUnitOfWork.Setup(u => u.Inventories.GetInventoryByBookAsync(invalidIsbn))
                .ReturnsAsync(emptyInventories);
            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryDto>>(emptyInventories))
                .Returns(expectedDtos);

            var result = await _service.GetInventoryByBookAsync(invalidIsbn);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteInventoryAsync_WithInvalidId_DeletesAnyway()
        {
            int invalidId = 999;

            _mockUnitOfWork.Setup(u => u.Inventories.DeleteAsync(invalidId))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            await _service.DeleteInventoryAsync(invalidId);

            _mockUnitOfWork.Verify(u => u.Inventories.DeleteAsync(invalidId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}