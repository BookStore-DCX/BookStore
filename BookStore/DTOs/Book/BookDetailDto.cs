namespace BookStore.DTOs.Book
{
    public class BookDetailDto
    {
        public string Isbn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Edition { get; set; }
        public int? CategoryId { get; set; }
        public int PublisherId { get; set; }
        public string? Category { get; set; }
        public string? Publisher { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<BookCopyDto> Copies { get; set; } = new();
    }
}
