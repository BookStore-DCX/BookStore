namespace BookStore.DTOs.Book
{
	public class BookCreateDto
	{
		public string Isbn { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int? Category { get; set; }
		public string? Edition { get; set; }
		public int PublisherId { get; set; }
	}
}
