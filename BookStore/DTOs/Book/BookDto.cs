namespace BookStore.DTOs.Book
{
	public class BookDto
	{
		public string Isbn { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int? Category { get; set; }
		public string? CategoryName { get; set; }
		public string? Edition { get; set; }
		public int PublisherId { get; set; }
		public string? PublisherName { get; set; }
		public int InventoryCount { get; set; }
	}
}

