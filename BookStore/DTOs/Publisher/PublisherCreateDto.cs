namespace BookStore.DTOs.Publisher
{
    public class PublisherCreateDto
    {
        public int PublisherId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? StateCode { get; set; }
    }
}
