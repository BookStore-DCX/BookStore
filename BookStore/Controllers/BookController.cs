using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Book;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookController : ControllerBase
{
	private readonly IUnitOfWork _uow;
	private readonly IMapper _mapper;
	public BookController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetAll()
	{
		var books = await _uow.Books.GetAllAsync();
		return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
	}

	[HttpGet("title/{title}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetByTitle(string title)
	{
		var book = await _uow.Books.GetBookWithDetailsByTitleAsync(title)
			?? throw new NotFoundException($"Book '{title}' not found");
		return Ok(ApiResponse<BookDto>.Ok(_mapper.Map<BookDto>(book)));
	}

	[HttpGet("category/{id}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetByCategory(int id)
	{
		var books = await _uow.Books.GetBooksByCategoryAsync(id);
		return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
	}

	[HttpGet("author/{authorName}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetByAuthor(string authorName)
	{
		var books = await _uow.Books.GetBooksByAuthorAsync(authorName);
		return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
	}

	[HttpGet("search")]
	[AllowAnonymous]
	public async Task<IActionResult> Search([FromQuery] string term)
	{
		var books = await _uow.Books.SearchBooksAsync(term);
		return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
	}

	[HttpPost]
	[Authorize(Roles = "Admin, StoreOwner")]
	public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
	{
		var book = _mapper.Map<Book>(dto);
		await _uow.Books.AddAsync(book);
		await _uow.SaveChangesAsync();
		return CreatedAtAction(nameof(GetByTitle), new { title = book.Title },
			ApiResponse<BookDto>.Created(_mapper.Map<BookDto>(book)));
	}

	[HttpPut("title/{title}")]
	[Authorize(Roles = "Admin, StoreOwner")]
	public async Task<IActionResult> Update(string title, [FromBody] BookUpdateDto dto)
	{
		var book = await _uow.Books.GetByTitleAsync(title)
			?? throw new NotFoundException($"Book '{title}' not found");
		_mapper.Map(dto, book);
		await _uow.Books.UpdateAsync(book);
		await _uow.SaveChangesAsync();
		return Ok(ApiResponse<BookDto>.Ok(_mapper.Map<BookDto>(book)));
	}

	[HttpDelete("title/{title}")]
	[Authorize(Roles = "StoreOwner, Admin")]
	public async Task<IActionResult> Delete(string title)
	{
		if (!await _uow.Books.ExistsByTitleAsync(title))
			throw new NotFoundException($"Book '{title}' not found");
		await _uow.Books.DeleteByTitleAsync(title);
		await _uow.SaveChangesAsync();
		return NoContent();
	}
}