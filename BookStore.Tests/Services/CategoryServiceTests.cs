using AutoMapper;
using BookStore.DTOs.Category;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using Xunit;

namespace BookStore.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _uowMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _service = new CategoryService(_uowMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            var categories = new List<Category>
            {
                new Category { CatId = 1, CatDescription = "Fiction" }
            };
            var dtos = new List<CategoryDto>
            {
                new CategoryDto { CatId = 1, CatDescription = "Fiction" }
            };
            _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories)).Returns(dtos);

            var result = await _service.GetAllCategoriesAsync();

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task GetCategoryByNameAsync_ValidName_ReturnsCategoryDto()
        {
            var category = new Category { CatId = 1, CatDescription = "Fiction" };
            var dto = new CategoryDto { CatId = 1, CatDescription = "Fiction" };
            _categoryRepoMock.Setup(r => r.GetByCategoryNameAsync("Fiction")).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(dto);

            var result = await _service.GetCategoryByNameAsync("Fiction");

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_EmptyList_ReturnsEmptyCollection()
        {
            _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());
            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>()))
                       .Returns(new List<CategoryDto>());

            var result = await _service.GetAllCategoriesAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCategoryByNameAsync_ValidName_MapsCorrectly()
        {
            var category = new Category { CatId = 2, CatDescription = "Science" };
            var dto = new CategoryDto { CatId = 2, CatDescription = "Science" };
            _categoryRepoMock.Setup(r => r.GetByCategoryNameAsync("Science")).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(dto);

            var result = await _service.GetCategoryByNameAsync("Science");

            Assert.Equal("Science", result.CatDescription);
        }

        [Fact]
        public async Task GetCategoryByNameAsync_InvalidName_ThrowsNotFoundException()
        {
            _categoryRepoMock.Setup(r => r.GetByCategoryNameAsync("Unknown")).ReturnsAsync((Category)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetCategoryByNameAsync("Unknown"));
        }

        [Fact]
        public async Task GetCategoryByNameAsync_NullName_ThrowsNotFoundException()
        {
            _categoryRepoMock.Setup(r => r.GetByCategoryNameAsync(null!)).ReturnsAsync((Category)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetCategoryByNameAsync(null!));
        }

        [Fact]
        public async Task GetCategoryByNameAsync_EmptyString_ThrowsNotFoundException()
        {
            _categoryRepoMock.Setup(r => r.GetByCategoryNameAsync("")).ReturnsAsync((Category)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetCategoryByNameAsync(""));
        }

        [Fact]
        public async Task GetCategoryByNameAsync_CaseMismatch_ThrowsNotFoundException()
        {
            _categoryRepoMock.Setup(r => r.GetByCategoryNameAsync("fiction")).ReturnsAsync((Category)null!);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetCategoryByNameAsync("fiction"));
        }
    }
}