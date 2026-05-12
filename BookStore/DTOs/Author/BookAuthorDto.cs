namespace BookStore.DTOs.Author
{
	public class BookAuthorDto
	{
		public string Isbn { get; set; } = string.Empty;
		public int AuthorId { get; set; }
		public string? PrimaryAuthor { get; set; }
	}

}
