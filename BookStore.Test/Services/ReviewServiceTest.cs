using Xunit;
using Moq;
using AutoMapper;
using BookStore.DTOs.Review;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;

namespace BookStore.Test.Services
{
    public class ReviewServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ReviewService _service;

        public ReviewServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new ReviewService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        #region Positive Test Cases

        [Fact]
        public async Task GetReviewsByBookAsync_WithValidIsbn_ReturnsReviewsForBook()
        {
            // Arrange
            string isbn = "978-0-123456-78-9";
            var reviews = new List<Bookreview>
            {
                new() { Isbn = isbn, ReviewerId = 1, Rating = 5, Comments = "Excellent book" },
                new() { Isbn = isbn, ReviewerId = 2, Rating = 4, Comments = "Very good" }
            };
            var expectedDtos = new List<ReviewDto>
            {
                new() { Isbn = isbn, ReviewerId = 1, Rating = 5, Comments = "Excellent book" },
                new() { Isbn = isbn, ReviewerId = 2, Rating = 4, Comments = "Very good" }
            };

            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewsByBookAsync(isbn)).ReturnsAsync(reviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDto>>(reviews)).Returns(expectedDtos);

            // Act
            var result = await _service.GetReviewsByBookAsync(isbn);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(expectedDtos, result);
            _mockUnitOfWork.Verify(u => u.Reviews.GetReviewsByBookAsync(isbn), Times.Once);
        }

        [Fact]
        public async Task GetReviewsByReviewerAsync_WithValidReviewerId_ReturnsReviewsByReviewer()
        {
            // Arrange
            int reviewerId = 1;
            var reviews = new List<Bookreview>
            {
                new() { Isbn = "978-0-123456-78-9", ReviewerId = reviewerId, Rating = 5, Comments = "Great" },
                new() { Isbn = "978-0-987654-32-1", ReviewerId = reviewerId, Rating = 4, Comments = "Good" }
            };
            var expectedDtos = new List<ReviewDto>
            {
                new() { Isbn = "978-0-123456-78-9", ReviewerId = reviewerId, Rating = 5, Comments = "Great" },
                new() { Isbn = "978-0-987654-32-1", ReviewerId = reviewerId, Rating = 4, Comments = "Good" }
            };

            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewsByReviewerAsync(reviewerId)).ReturnsAsync(reviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDto>>(reviews)).Returns(expectedDtos);

            // Act
            var result = await _service.GetReviewsByReviewerAsync(reviewerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockUnitOfWork.Verify(u => u.Reviews.GetReviewsByReviewerAsync(reviewerId), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_WithValidDto_CreatesAndReturnsReview()
        {
            // Arrange
            var reviewDto = new ReviewDto { Isbn = "978-0-123456-78-9", ReviewerId = 1, Rating = 5, Comments = "Outstanding" };
            var review = new Bookreview { Isbn = "978-0-123456-78-9", ReviewerId = 1, Rating = 5, Comments = "Outstanding" };
            var expectedDto = new ReviewDto { Isbn = "978-0-123456-78-9", ReviewerId = 1, Rating = 5, Comments = "Outstanding" };

            _mockMapper.Setup(m => m.Map<Bookreview>(reviewDto)).Returns(review);
            _mockUnitOfWork.Setup(u => u.Reviews.AddAsync(It.IsAny<Bookreview>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ReviewDto>(review)).Returns(expectedDto);

            // Act
            var result = await _service.CreateReviewAsync(reviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Isbn, result.Isbn);
            Assert.Equal(expectedDto.Rating, result.Rating);
            _mockUnitOfWork.Verify(u => u.Reviews.AddAsync(It.IsAny<Bookreview>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_WithValidIsbnAndReviewerId_DeletesReviewSuccessfully()
        {
            // Arrange
            string isbn = "978-0-123456-78-9";
            int reviewerId = 1;

            _mockUnitOfWork.Setup(u => u.Reviews.DeleteAsync(isbn, reviewerId)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _service.DeleteReviewAsync(isbn, reviewerId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Reviews.DeleteAsync(isbn, reviewerId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region Negative Test Cases

        [Fact]
        public async Task GetReviewsByBookAsync_WithInvalidIsbn_ReturnsEmptyCollection()
        {
            // Arrange
            string invalidIsbn = "999-9-999999-99-9";
            var emptyReviews = new List<Bookreview>();
            var emptyDtos = new List<ReviewDto>();

            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewsByBookAsync(invalidIsbn)).ReturnsAsync(emptyReviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDto>>(emptyReviews)).Returns(emptyDtos);

            // Act
            var result = await _service.GetReviewsByBookAsync(invalidIsbn);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetReviewsByReviewerAsync_WithInvalidReviewerId_ReturnsEmptyCollection()
        {
            // Arrange
            int invalidReviewerId = 999;
            var emptyReviews = new List<Bookreview>();
            var emptyDtos = new List<ReviewDto>();

            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewsByReviewerAsync(invalidReviewerId)).ReturnsAsync(emptyReviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDto>>(emptyReviews)).Returns(emptyDtos);

            // Act
            var result = await _service.GetReviewsByReviewerAsync(invalidReviewerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateReviewAsync_WithDatabaseFailure_ThrowsException()
        {
            // Arrange
            var reviewDto = new ReviewDto { Isbn = "978-0-123456-78-9", ReviewerId = 1, Rating = 5, Comments = "Outstanding" };
            var review = new Bookreview { Isbn = "978-0-123456-78-9", ReviewerId = 1, Rating = 5, Comments = "Outstanding" };

            _mockMapper.Setup(m => m.Map<Bookreview>(reviewDto)).Returns(review);
            _mockUnitOfWork.Setup(u => u.Reviews.AddAsync(It.IsAny<Bookreview>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateReviewAsync(reviewDto));
        }

        [Fact]
        public async Task DeleteReviewAsync_WithNonExistentReview_ThrowsException()
        {
            // Arrange
            string isbn = "978-0-123456-78-9";
            int reviewerId = 999;

            _mockUnitOfWork.Setup(u => u.Reviews.DeleteAsync(isbn, reviewerId))
                .ThrowsAsync(new Exception("Review not found"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteReviewAsync(isbn, reviewerId));
        }

        #endregion
    }
}