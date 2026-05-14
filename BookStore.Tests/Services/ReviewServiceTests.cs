using Xunit;
using Moq;
using AutoMapper;
using BookStore.DTOs.Review;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using BookStore.Exceptions;

namespace BookStore.Services.Tests
{
    public class ReviewServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ReviewService _service;

        public ReviewServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new ReviewService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        // ===== POSITIVE TEST CASES =====

        [Fact]
        public async Task GetReviewsByBookNameAsync_WithValidBookName_ReturnsAllReviews()
        {
            // Arrange
            string bookName = "The Great Gatsby";
            var reviews = new List<Bookreview>
            {
                new() { Isbn = "978-0-123456-78-9", ReviewerId = 1, Rating = 5, Comments = "Excellent book" },
                new() { Isbn = "978-0-123456-78-9", ReviewerId = 2, Rating = 4, Comments = "Very good" },
                new() { Isbn = "978-0-123456-78-9", ReviewerId = 3, Rating = 5, Comments = "Loved it" }
            };
            var expectedDtos = new List<ReviewDto>
            {
                new() { Isbn = "978-0-123456-78-9", ReviewerId = 1, Rating = 5, Comments = "Excellent book" },
                new() { Isbn = "978-0-123456-78-9", ReviewerId = 2, Rating = 4, Comments = "Very good" },
                new() { Isbn = "978-0-123456-78-9", ReviewerId = 3, Rating = 5, Comments = "Loved it" }
            };

            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewsByBookNameAsync(bookName))
                .ReturnsAsync(reviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDto>>(reviews))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetReviewsByBookNameAsync(bookName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.All(result, item => Assert.Equal("978-0-123456-78-9", item.Isbn));
            _mockUnitOfWork.Verify(u => u.Reviews.GetReviewsByBookNameAsync(bookName), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_WithValidUserAndReview_CreatesReviewWithExistingReviewer()
        {
            // Arrange
            int userId = 1;
            var createDto = new ReviewCreateDto
            {
                Isbn = "978-0-123456-78-9",
                Rating = 5,
                Comments = "Outstanding book"
            };
            var user = new User
            {
                UserId = userId,
                FirstName = "John",
                LastName = "Doe"
            };
            var existingReviewer = new Reviewer
            {
                ReviewerId = 1,
                Name = "John Doe",
                EmployedBy = null
            };
            var review = new Bookreview
            {
                Isbn = "978-0-123456-78-9",
                ReviewerId = 1,
                Rating = 5,
                Comments = "Outstanding book"
            };
            var expectedDto = new ReviewDto
            {
                Isbn = "978-0-123456-78-9",
                ReviewerId = 1,
                Rating = 5,
                Comments = "Outstanding book"
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewerByNameAsync("John Doe"))
                .ReturnsAsync(existingReviewer);
            _mockUnitOfWork.Setup(u => u.Reviews.AddAsync(It.IsAny<Bookreview>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<ReviewDto>(It.IsAny<Bookreview>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateReviewAsync(userId, createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("978-0-123456-78-9", result.Isbn);
            Assert.Equal(5, result.Rating);
            Assert.Equal("Outstanding book", result.Comments);
            _mockUnitOfWork.Verify(u => u.Reviews.AddAsync(It.IsAny<Bookreview>()), Times.Once);
        }

        [Fact]
        public async Task CreateReviewAsync_WithValidUserAndNewReviewer_CreatesReviewerAndReview()
        {
            // Arrange
            int userId = 2;
            var createDto = new ReviewCreateDto
            {
                Isbn = "978-0-987654-32-1",
                Rating = 4,
                Comments = "Good read"
            };
            var user = new User
            {
                UserId = userId,
                FirstName = "Jane",
                LastName = "Smith"
            };
            var newReviewer = new Reviewer
            {
                ReviewerId = 10,
                Name = "Jane Smith",
                EmployedBy = null
            };
            var review = new Bookreview
            {
                Isbn = "978-0-987654-32-1",
                ReviewerId = 10,
                Rating = 4,
                Comments = "Good read"
            };
            var expectedDto = new ReviewDto
            {
                Isbn = "978-0-987654-32-1",
                ReviewerId = 10,
                Rating = 4,
                Comments = "Good read"
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewerByNameAsync("Jane Smith"))
                .ReturnsAsync((Reviewer)null);
            _mockUnitOfWork.Setup(u => u.Reviews.GetNextReviewerIdAsync())
                .ReturnsAsync(10);
            _mockUnitOfWork.Setup(u => u.Reviews.AddReviewerAsync(It.IsAny<Reviewer>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockUnitOfWork.Setup(u => u.Reviews.AddAsync(It.IsAny<Bookreview>()))
                .Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<ReviewDto>(It.IsAny<Bookreview>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateReviewAsync(userId, createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("978-0-987654-32-1", result.Isbn);
            Assert.Equal(10, result.ReviewerId);
            _mockUnitOfWork.Verify(u => u.Reviews.AddReviewerAsync(It.IsAny<Reviewer>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Reviews.AddAsync(It.IsAny<Bookreview>()), Times.Once);
        }

        [Fact]
        public async Task GetReviewsByBookIsbnAsync_WithValidIsbn_ReturnsReviewsForIsbn()
        {
            // Arrange
            string isbn = "978-0-123456-78-9";
            var reviews = new List<Bookreview>
            {
                new() { Isbn = isbn, ReviewerId = 1, Rating = 5, Comments = "Perfect" },
                new() { Isbn = isbn, ReviewerId = 2, Rating = 4, Comments = "Good" }
            };
            var expectedDtos = new List<ReviewDto>
            {
                new() { Isbn = isbn, ReviewerId = 1, Rating = 5, Comments = "Perfect" },
                new() { Isbn = isbn, ReviewerId = 2, Rating = 4, Comments = "Good" }
            };

            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewsByBookIsbnAsync(isbn))
                .ReturnsAsync(reviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDto>>(reviews))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetReviewsByBookIsbnAsync(isbn);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(isbn, item.Isbn));
        }

        // ===== NEGATIVE TEST CASES =====

        [Fact]
        public async Task CreateReviewAsync_WithInvalidUserId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidUserId = 999;
            var createDto = new ReviewCreateDto
            {
                Isbn = "978-0-123456-78-9",
                Rating = 5,
                Comments = "Great book"
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(invalidUserId))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _service.CreateReviewAsync(invalidUserId, createDto)
            );
            Assert.Contains("not found", exception.Message);
            _mockUnitOfWork.Verify(u => u.Reviews.AddAsync(It.IsAny<Bookreview>()), Times.Never);
        }

        [Fact]
        public async Task GetReviewsByBookNameAsync_WithInvalidBookName_ReturnsEmptyList()
        {
            // Arrange
            string invalidBookName = "Non-Existent Book";
            var emptyReviews = new List<Bookreview>();
            var expectedDtos = new List<ReviewDto>();

            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewsByBookNameAsync(invalidBookName))
                .ReturnsAsync(emptyReviews);
            _mockMapper.Setup(m => m.Map<IEnumerable<ReviewDto>>(emptyReviews))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetReviewsByBookNameAsync(invalidBookName);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateReviewAsync_WithRepositoryException_PropagatesException()
        {
            // Arrange
            int userId = 1;
            var createDto = new ReviewCreateDto
            {
                Isbn = "978-0-123456-78-9",
                Rating = 5,
                Comments = "Good"
            };
            var user = new User
            {
                UserId = userId,
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.Reviews.GetReviewerByNameAsync(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateReviewAsync(userId, createDto)
            );
        }

        [Fact]
        public async Task DeleteReviewAsync_WithValidIsbnAndReviewerId_DeletesReview()
        {
            // Arrange
            string isbn = "978-0-123456-78-9";
            int reviewerId = 1;

            _mockUnitOfWork.Setup(u => u.Reviews.DeleteReviewAsync(isbn, reviewerId))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _service.DeleteReviewAsync(isbn, reviewerId);

            // Assert
            _mockUnitOfWork.Verify(u => u.Reviews.DeleteReviewAsync(isbn, reviewerId), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}