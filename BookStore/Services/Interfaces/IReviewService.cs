using BookStore.DTOs.Review;

namespace BookStore.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetReviewsByBookNameAsync(string bookName);
        Task<IEnumerable<ReviewDto>> GetReviewsByBookIsbnAsync(string isbn);
        Task<ReviewDto> CreateReviewAsync(int userId, ReviewCreateDto dto);
        Task DeleteReviewAsync(string isbn, int reviewerId);
    }
}
