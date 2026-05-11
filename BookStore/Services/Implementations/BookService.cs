using AutoMapper;
using BookStore.DTOs.Book;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
	public class BookService : IBookService
	{
		private readonly IUnitOfWork _uow;
		private readonly IMapper _mapper;

		public BookService(IUnitOfWork uow, IMapper mapper)
		{
			_uow = uow;
			_mapper = mapper;
		}

		public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
			=> _mapper.Map<IEnumerable<BookDto>>(await _uow.Books.GetAllAsync());

		public async Task<BookDto> GetBookByIsbnAsync(string isbn)
		{
			var book = await _uow.Books.GetBookWithDetailsAsync(isbn)
				?? throw new NotFoundException($"Book with ISBN '{isbn}' not found");
			return _mapper.Map<BookDto>(book);
		}

		public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId)
			=> _mapper.Map<IEnumerable<BookDto>>(await _uow.Books.GetBooksByCategoryAsync(categoryId));

		public async Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId)
			=> _mapper.Map<IEnumerable<BookDto>>(await _uow.Books.GetBooksByPublisherAsync(publisherId));

		public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
			=> _mapper.Map<IEnumerable<BookDto>>(await _uow.Books.SearchBooksAsync(searchTerm));

		public async Task<BookDto> CreateBookAsync(BookCreateDto dto)
		{
			var book = _mapper.Map<Book>(dto);
			await _uow.Books.AddAsync(book);
			await _uow.SaveChangesAsync();
			return _mapper.Map<BookDto>(book);
		}

		public async Task<BookDto> UpdateBookAsync(string isbn, BookUpdateDto dto)
		{
			var book = await _uow.Books.GetByIdAsync(isbn)
				?? throw new NotFoundException($"Book with ISBN '{isbn}' not found");
			_mapper.Map(dto, book);
			await _uow.Books.UpdateAsync(book);
			await _uow.SaveChangesAsync();
			return _mapper.Map<BookDto>(book);
		}

		public async Task DeleteBookAsync(string isbn)
		{
			if (!await _uow.Books.ExistsAsync(isbn))
			{
				throw new NotFoundException($"Book with ISBN '{isbn}' not found");
			}

			await _uow.Books.DeleteAsync(isbn);
			await _uow.SaveChangesAsync();
		}
	}

}
