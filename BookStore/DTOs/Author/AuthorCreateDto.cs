namespace BookStore.DTOs.Author
{
	public class AuthorCreateDto
	{
		public int AuthorId { get; set; }
		public string LastName { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
	}

}
