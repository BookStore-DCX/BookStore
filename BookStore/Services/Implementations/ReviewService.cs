using AutoMapper;
using BookStore.DTOs.Review;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookNameAsync(string bookName)
            => _mapper.Map<IEnumerable<ReviewDto>>(await _uow.Reviews.GetReviewsByBookNameAsync(bookName));

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookIsbnAsync(string isbn)
            => _mapper.Map<IEnumerable<ReviewDto>>(await _uow.Reviews.GetReviewsByBookIsbnAsync(isbn));

        public async Task<ReviewDto> CreateReviewAsync(int userId, ReviewCreateDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(userId)
                ?? throw new NotFoundException($"User with ID {userId} not found");

            var reviewer = await _uow.Reviews.GetReviewerByIdAsync(userId);
            if (reviewer == null)
            {
                var reviewerName = $"{user.FirstName} {user.LastName}".Trim();
                if (string.IsNullOrWhiteSpace(reviewerName))
                {
                    reviewerName = user.UserName;
                }

                if (reviewerName.Length > 20)
                {
                    reviewerName = reviewerName[..20];
                }

                reviewer = new Reviewer
                {
                    ReviewerId = userId,
                    Name = reviewerName,
                    EmployedBy = null
                };

                await _uow.Reviews.AddReviewerAsync(reviewer);
                await _uow.SaveChangesAsync();
            }

            if (await _uow.Reviews.ReviewExistsAsync(dto.Isbn, reviewer.ReviewerId))
            {
                throw new ConflictException("You have already reviewed this book.");
            }

            var review = new Bookreview
            {
                Isbn = dto.Isbn,
                ReviewerId = reviewer.ReviewerId,
                Rating = dto.Rating,
                Comments = dto.Comments
            };

            await _uow.Reviews.AddAsync(review);
            await _uow.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task DeleteReviewAsync(string isbn, int reviewerId)
        {
            await _uow.Reviews.DeleteReviewAsync(isbn, reviewerId);
            await _uow.SaveChangesAsync();
        }
    }
}