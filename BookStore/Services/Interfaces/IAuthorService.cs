using BookStore.DTOs.Author;

namespace BookStore.Services.Interfaces
{
	public interface IAuthorService
	{
		Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
		Task<IEnumerable<AuthorDto>> SearchAuthorsAsync(string searchTerm);
		Task<AuthorDto> CreateAuthorAsync(AuthorCreateDto dto);
		Task<AuthorDto> UpdateAuthorAsync(int authorId, AuthorCreateDto dto);
		Task DeleteAuthorAsync(int authorId);
	}
}