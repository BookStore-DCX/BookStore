using AutoMapper;
using BookStore.DTOs.Author;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
	public class AuthorService : IAuthorService
	{
		private readonly IUnitOfWork _uow;
		private readonly IMapper _mapper;
		public AuthorService(IUnitOfWork uow, IMapper mapper)
		{
			_uow = uow;
			_mapper = mapper;
		}
		public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
			=> _mapper.Map<IEnumerable<AuthorDto>>(await _uow.Authors.GetAllAsync());
		public async Task<IEnumerable<AuthorDto>> SearchAuthorsAsync(string searchTerm)
			=> _mapper.Map<IEnumerable<AuthorDto>>(await _uow.Authors.SearchAuthorsAsync(searchTerm));
		public async Task<AuthorDto> CreateAuthorAsync(AuthorCreateDto dto)
		{
			var author = _mapper.Map<Author>(dto);
			await _uow.Authors.AddAsync(author);
			await _uow.SaveChangesAsync();
			return _mapper.Map<AuthorDto>(author);
		}
		public async Task<AuthorDto> UpdateAuthorAsync(string authorName, AuthorCreateDto dto)
		{
			var author = await _uow.Authors.GetByNameAsync(authorName)
				?? throw new NotFoundException($"Author '{authorName}' not found");
			_mapper.Map(dto, author);
			await _uow.Authors.UpdateAsync(author);
			await _uow.SaveChangesAsync();
			return _mapper.Map<AuthorDto>(author);
		}
		public async Task DeleteAuthorAsync(string authorName)
		{
			if (!await _uow.Authors.ExistsByNameAsync(authorName))
			{
				throw new NotFoundException($"Author '{authorName}' not found");
			}
			await _uow.Authors.DeleteByNameAsync(authorName);
			await _uow.SaveChangesAsync();
		}
	}
}