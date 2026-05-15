using AutoMapper;
using BookStore.DTOs.BookCondition;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using Moq;
using System.Timers;
using Xunit;

namespace BookStore.Tests.Services
{
    public class BookconditionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BookConditionService _service;

        public BookconditionServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new BookConditionService(_mockUnitOfWork.Object, _mockMapper.Object);
        }


        [Fact]
        public async Task GetAllConditionsAsync_WithValidData_ReturnsAllConditions()
        {
            var bookConditions = new List<Bookcondition>
            {
                new() { Ranks = 1, Description = "New", Price = 100 },
                new() { Ranks = 2, Description = "Good", Price = 80 },
                new() { Ranks = 3, Description = "Fair", Price = 60 }
            };
            var expectedDtos = new List<BookConditionDto>
            {
                new() { Ranks = 1, Description = "New", Price = 100 },
                new() { Ranks = 2, Description = "Good", Price = 80 },
                new() { Ranks = 3, Description = "Fair", Price = 60 }
            };

            _mockUnitOfWork.Setup(u => u.BookConditions.GetAllAsync())
                .ReturnsAsync(bookConditions);
            _mockMapper.Setup(m => m.Map<IEnumerable<BookConditionDto>>(bookConditions))
                .Returns(expectedDtos);

            var result = await _service.GetAllConditionsAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(expectedDtos, result);
            _mockUnitOfWork.Verify(u => u.BookConditions.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetConditionByRankAsync_WithValidRank_ReturnsConditionDto()
        {
            int rank = 1;
            var bookCondition = new Bookcondition
            {
                Ranks = 1,
                Description = "New",
                FullDescription = "Completely new book",
                Price = 100
            };
            var expectedDto = new BookConditionDto
            {
                Ranks = 1,
                Description = "New",
                Price = 100
            };

            _mockUnitOfWork.Setup(u => u.BookConditions.GetByIdAsync(rank))
                .ReturnsAsync(bookCondition);
            _mockMapper.Setup(m => m.Map<BookConditionDto>(bookCondition))
                .Returns(expectedDto);

            var result = await _service.GetConditionByRankAsync(rank);

            Assert.NotNull(result);
            Assert.Equal(expectedDto.Ranks, result.Ranks);
            Assert.Equal(expectedDto.Description, result.Description);
            _mockUnitOfWork.Verify(u => u.BookConditions.GetByIdAsync(rank), Times.Once);
        }

        [Fact]
        public async Task GetAllConditionsAsync_WithEmptyList_ReturnsEmptyEnumerable()
        {
            var emptyConditions = new List<Bookcondition>();
            var expectedDtos = new List<BookConditionDto>();

            _mockUnitOfWork.Setup(u => u.BookConditions.GetAllAsync())
                .ReturnsAsync(emptyConditions);
            _mockMapper.Setup(m => m.Map<IEnumerable<BookConditionDto>>(emptyConditions))
                .Returns(expectedDtos);

            var result = await _service.GetAllConditionsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetConditionByRankAsync_WithDifferentValidRanks_ReturnsCorrectCondition()
        {
            int rank = 5;
            var bookCondition = new Bookcondition
            {
                Ranks = 5,
                Description = "Poor",
                Price = 20
            };
            var expectedDto = new BookConditionDto
            {
                Ranks = 5,
                Description = "Poor",
                Price = 20
            };

            _mockUnitOfWork.Setup(u => u.BookConditions.GetByIdAsync(rank))
                .ReturnsAsync(bookCondition);
            _mockMapper.Setup(m => m.Map<BookConditionDto>(bookCondition))
                .Returns(expectedDto);

            var result = await _service.GetConditionByRankAsync(rank);

            Assert.NotNull(result);
            Assert.Equal(5, result.Ranks);
            Assert.Equal("Poor", result.Description);
        }


        [Fact]
        public async Task GetConditionByRankAsync_WithInvalidRank_ThrowsNotFoundException()
        {
            int invalidRank = 999;
            _mockUnitOfWork.Setup(u => u.BookConditions.GetByIdAsync(invalidRank))
                .ReturnsAsync((Bookcondition)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetConditionByRankAsync(invalidRank)
            );
            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public async Task GetConditionByRankAsync_WithNegativeRank_ThrowsNotFoundException()
        {
            int negativeRank = -1;
            _mockUnitOfWork.Setup(u => u.BookConditions.GetByIdAsync(negativeRank))
                .ReturnsAsync((Bookcondition)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetConditionByRankAsync(negativeRank)
            );
        }

        [Fact]
        public async Task GetConditionByRankAsync_WithZeroRank_ThrowsNotFoundException()
        {
            int zeroRank = 0;
            _mockUnitOfWork.Setup(u => u.BookConditions.GetByIdAsync(zeroRank))
                .ReturnsAsync((Bookcondition)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetConditionByRankAsync(zeroRank)
            );
        }

        [Fact]
        public async Task GetAllConditionsAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            _mockUnitOfWork.Setup(u => u.BookConditions.GetAllAsync())
                .ThrowsAsync(new InvalidOperationException("Database error"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GetAllConditionsAsync()
            );
        }
    }
}