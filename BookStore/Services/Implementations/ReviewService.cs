using AutoMapper;
using BookStore.DTOs.Review;
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

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(string isbn)
            => _mapper.Map<IEnumerable<ReviewDto>>(await _uow.Reviews.GetReviewsByBookAsync(isbn));

        public async Task<IEnumerable<ReviewDto>> GetReviewsByReviewerAsync(int reviewerId)
            => _mapper.Map<IEnumerable<ReviewDto>>(await _uow.Reviews.GetReviewsByReviewerAsync(reviewerId));

        public async Task<ReviewDto> CreateReviewAsync(ReviewCreateDto dto)
        {
            var review = _mapper.Map<Bookreview>(dto);
            await _uow.Reviews.AddAsync(review);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task DeleteReviewAsync(string isbn, int reviewerId)
        {
            await _uow.Reviews.DeleteAsync(isbn, reviewerId);
            await _uow.SaveChangesAsync();
        }
    }

}
