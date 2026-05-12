using BookStore.DTOs.Review;

namespace BookStore.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(string isbn);
        Task<IEnumerable<ReviewDto>> GetReviewsByReviewerAsync(int reviewerId);
        Task<ReviewDto> CreateReviewAsync(ReviewDto dto);
        Task DeleteReviewAsync(string isbn, int reviewerId);
    }
}
