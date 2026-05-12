namespace BookStore.DTOs.Review
{
    public class ReviewDto
    {
        public string Isbn { get; set; } = string.Empty;
        public int ReviewerId { get; set; }
        public int? Rating { get; set; }
        public string? Comments { get; set; }
    }
}
