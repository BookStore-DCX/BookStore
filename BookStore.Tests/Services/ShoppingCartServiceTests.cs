using AutoMapper;
using BookStore.DTOs.PurchaseLog;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using Xunit;

namespace BookStore.Tests.Services
{
    public class ShoppingCartServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IShoppingCartRepository> _repoMock;
        private readonly ShoppingCartService _service;

        public ShoppingCartServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _repoMock = new Mock<IShoppingCartRepository>();
            _uowMock.Setup(u => u.ShoppingCarts).Returns(_repoMock.Object);
            _service = new ShoppingCartService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetCartByUserAsync_ValidUserId_ReturnsMappedDtos()
        {
            var carts = new List<Shoppingcart> { new Shoppingcart { UserId = 1, Isbn = "123" } };
            var dtos = new List<ShoppingCartDto> { new ShoppingCartDto { UserId = 1, Isbn = "123" } };

            _repoMock.Setup(r => r.GetCartByUserAsync(1)).ReturnsAsync(carts);
            _mapperMock.Setup(m => m.Map<IEnumerable<ShoppingCartDto>>(carts)).Returns(dtos);

            var result = await _service.GetCartByUserAsync(1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("123", result.First().Isbn);
        }

        [Fact]
        public async Task AddToCartAsync_ValidDto_ReturnsCreatedDto()
        {
            var createDto = new ShoppingCartCreateDto { UserId = 1, Isbn = "456" };
            var cart = new Shoppingcart { UserId = 1, Isbn = "456" };
            var resultDto = new ShoppingCartDto { UserId = 1, Isbn = "456" };

            _mapperMock.Setup(m => m.Map<Shoppingcart>(createDto)).Returns(cart);
            _repoMock.Setup(r => r.AddAsync(cart)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync())
        .ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<ShoppingCartDto>(cart)).Returns(resultDto);

            var result = await _service.AddToCartAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal("456", result.Isbn);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ValidUserAndIsbn_CallsRepoAndSaves()
        {
            _repoMock.Setup(r => r.RemoveFromCartAsync(1, "789")).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync())
        .ReturnsAsync(1);

            await _service.RemoveFromCartAsync(1, "789");

            _repoMock.Verify(r => r.RemoveFromCartAsync(1, "789"), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ClearCartAsync_ValidUserId_CallsRepoAndSaves()
        {
            _repoMock.Setup(r => r.ClearCartAsync(1)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync())
        .ReturnsAsync(1);

            await _service.ClearCartAsync(1);

            _repoMock.Verify(r => r.ClearCartAsync(1), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCartByUserAsync_EmptyCart_ReturnsEmptyCollection()
        {
            _repoMock.Setup(r => r.GetCartByUserAsync(99)).ReturnsAsync(new List<Shoppingcart>());
            _mapperMock.Setup(m => m.Map<IEnumerable<ShoppingCartDto>>(It.IsAny<IEnumerable<Shoppingcart>>()))
                       .Returns(new List<ShoppingCartDto>());

            var result = await _service.GetCartByUserAsync(99);

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddToCartAsync_SaveChangesThrows_PropagatesException()
        {
            var createDto = new ShoppingCartCreateDto { UserId = 1, Isbn = "000" };
            var cart = new Shoppingcart { UserId = 1, Isbn = "000" };

            _mapperMock.Setup(m => m.Map<Shoppingcart>(createDto)).Returns(cart);
            _repoMock.Setup(r => r.AddAsync(cart)).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("Save failed"));

            await Assert.ThrowsAsync<Exception>(() => _service.AddToCartAsync(createDto));
        }

        [Fact]
        public async Task RemoveFromCartAsync_RepositoryThrows_PropagatesException()
        {
            _repoMock.Setup(r => r.RemoveFromCartAsync(It.IsAny<int>(), It.IsAny<string>()))
                     .ThrowsAsync(new Exception("Remove failed"));

            await Assert.ThrowsAsync<Exception>(() => _service.RemoveFromCartAsync(1, "123"));
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ClearCartAsync_RepositoryThrows_DoesNotCallSaveChanges()
        {
            _repoMock.Setup(r => r.ClearCartAsync(It.IsAny<int>()))
                     .ThrowsAsync(new Exception("Clear failed"));

            await Assert.ThrowsAsync<Exception>(() => _service.ClearCartAsync(1));
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}