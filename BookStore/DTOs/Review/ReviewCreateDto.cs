namespace BookStore.DTOs.Review
{
    public class ReviewCreateDto
    {
        public string Isbn { get; set; } = string.Empty;
        public int ReviewerId { get; set; }
        public int? Rating { get; set; }
        public string? Comments { get; set; }
    }

}
