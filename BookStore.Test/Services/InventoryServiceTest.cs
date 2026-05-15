using Xunit;
using Moq;
using AutoMapper;
using BookStore.DTOs.Inventory;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using BookStore.Exceptions;

namespace BookStore.Test.Services
{
    public class InventoryServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly InventoryService _service;

        public InventoryServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new InventoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        #region Positive Test Cases

        [Fact]
        public async Task GetAllInventoryAsync_WithValidData_ReturnsAllInventory()
        {
            // Arrange
            var inventories = new List<Inventory>
            {
                new() { InventoryId = 1, Isbn = "978-0-123456-78-9", Ranks = 1, Purchased = 5 },
                new() { InventoryId = 2, Isbn = "978-0-123456-79-6", Ranks = 2, Purchased = 3 }
            };
            var expectedDtos = new List<InventoryDto>
            {
                new() { InventoryId = 1, Isbn = "978-0-123456-78-9", Ranks = 1, Purchased = (byte)5 },
                new() { InventoryId = 2, Isbn = "978-0-123456-79-6", Ranks = 2, Purchased = (byte)3 }
            };

            _mockUnitOfWork.Setup(u => u.Inventories.GetAllAsync()).ReturnsAsync(inventories);
            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryDto>>(inventories)).Returns(expectedDtos);

            // Act
            var result = await _service.GetAllInventoryAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(expectedDtos, result);
            _mockUnitOfWork.Verify(u => u.Inventories.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateInventoryAsync_WithValidDto_CreatesAndReturnsInventory()
        {
            // Arrange
            var createDto = new InventoryCreateDto { Isbn = "978-0-123456-78-9", Ranks = 1 };
            var inventory = new Inventory { InventoryId = 1, Isbn = "978-0-123456-78-9", Ranks = 1, Purchased = 0 };
            var expectedDto = new InventoryDto { InventoryId = 1, Isbn = "978-0-123456-78-9", Ranks = 1, Purchased = 0 };

            _mockMapper.Setup(m => m.Map<Inventory>(createDto)).Returns(inventory);
            _mockUnitOfWork.Setup(u => u.Inventories.AddAsync(It.IsAny<Inventory>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<InventoryDto>(inventory)).Returns(expectedDto);

            // Act
            var result = await _service.CreateInventoryAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.InventoryId, result.InventoryId);
            Assert.Equal(0, (double)result.Purchased);
            _mockUnitOfWork.Verify(u => u.Inventories.AddAsync(It.IsAny<Inventory>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetInventoryByBookAsync_WithValidIsbn_ReturnsInventoriesForBook()
        {
            // Arrange
            string isbn = "978-0-123456-78-9";
            var inventories = new List<Inventory>
            {
                new() { InventoryId = 1, Isbn = isbn, Ranks = 1, Purchased = 5 },
                new() { InventoryId = 2, Isbn = isbn, Ranks = 2, Purchased = 3 }
            };
            var expectedDtos = new List<InventoryDto>
            {
                new() { InventoryId = 1, Isbn = isbn, Ranks = 1, Purchased = 5 },
                new() { InventoryId = 2, Isbn = isbn, Ranks = 2, Purchased = 3 }
            };

            _mockUnitOfWork.Setup(u => u.Inventories.GetInventoryByBookAsync(isbn)).ReturnsAsync(inventories);
            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryDto>>(inventories)).Returns(expectedDtos);

            // Act
            var result = await _service.GetInventoryByBookAsync(isbn);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockUnitOfWork.Verify(u => u.Inventories.GetInventoryByBookAsync(isbn), Times.Once);
        }

        [Fact]
        public async Task DeleteInventoryAsync_WithValidId_DeletesInventorySuccessfully()
        {
            // Arrange
            int inventoryId = 1;

            _mockUnitOfWork.Setup(u => u.Inventories.DeleteAsync(inventoryId)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _service.DeleteInventoryAsync(inventoryId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Inventories.DeleteAsync(inventoryId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region Negative Test Cases

        [Fact]
        public async Task GetAllInventoryAsync_WithNoData_ReturnsEmptyCollection()
        {
            // Arrange
            var emptyInventories = new List<Inventory>();
            var emptyDtos = new List<InventoryDto>();

            _mockUnitOfWork.Setup(u => u.Inventories.GetAllAsync()).ReturnsAsync(emptyInventories);
            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryDto>>(emptyInventories)).Returns(emptyDtos);

            // Act
            var result = await _service.GetAllInventoryAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetInventoryByBookAsync_WithInvalidIsbn_ReturnsEmptyCollection()
        {
            // Arrange
            string invalidIsbn = "999-9-999999-99-9";
            var emptyInventories = new List<Inventory>();
            var emptyDtos = new List<InventoryDto>();

            _mockUnitOfWork.Setup(u => u.Inventories.GetInventoryByBookAsync(invalidIsbn)).ReturnsAsync(emptyInventories);
            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryDto>>(emptyInventories)).Returns(emptyDtos);

            // Act
            var result = await _service.GetInventoryByBookAsync(invalidIsbn);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateInventoryAsync_WithDatabaseFailure_ThrowsException()
        {
            // Arrange
            var createDto = new InventoryCreateDto { Isbn = "978-0-123456-78-9", Ranks = 1 };
            var inventory = new Inventory { InventoryId = 1, Isbn = "978-0-123456-78-9", Ranks = 1, Purchased = 0 };

            _mockMapper.Setup(m => m.Map<Inventory>(createDto)).Returns(inventory);
            _mockUnitOfWork.Setup(u => u.Inventories.AddAsync(It.IsAny<Inventory>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateInventoryAsync(createDto));
        }

        [Fact]
        public async Task DeleteInventoryAsync_WithRepositoryException_ThrowsException()
        {
            // Arrange
            int invalidInventoryId = 999;

            _mockUnitOfWork.Setup(u => u.Inventories.DeleteAsync(invalidInventoryId))
                .ThrowsAsync(new Exception("Inventory not found"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteInventoryAsync(invalidInventoryId));
        }

        #endregion
    }
}