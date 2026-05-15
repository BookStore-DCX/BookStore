namespace BookStore.DTOs.Publisher
{
    public class PublisherCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? StateCode { get; set; }
    }
}