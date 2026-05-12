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
public class BookController : ControllerBase
{
	private readonly IUnitOfWork _uow;
	private readonly IMapper _mapper;
	public BookController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var books = await _uow.Books.GetAllAsync();
		return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
	}

	[HttpGet("{isbn}")]
	public async Task<IActionResult> GetByIsbn(string isbn)
	{
		var book = await _uow.Books.GetBookWithDetailsAsync(isbn)
			?? throw new NotFoundException($"Book {isbn} not found");
		return Ok(ApiResponse<BookDto>.Ok(_mapper.Map<BookDto>(book)));
	}

	[HttpGet("category/{id}")]
	public async Task<IActionResult> GetByCategory(int id)
	{
		var books = await _uow.Books.GetBooksByCategoryAsync(id);
		return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
	}

	[HttpGet("search")]
	public async Task<IActionResult> Search([FromQuery] string term)
	{
		var books = await _uow.Books.SearchBooksAsync(term);
		return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
	{
		var book = _mapper.Map<Book>(dto);
		await _uow.Books.AddAsync(book);
		await _uow.SaveChangesAsync();
		return CreatedAtAction(nameof(GetByIsbn), new { isbn = book.Isbn },
			ApiResponse<BookDto>.Created(_mapper.Map<BookDto>(book)));
	}

	[HttpPut("{isbn}")]
	public async Task<IActionResult> Update(string isbn, [FromBody] BookUpdateDto dto)
	{
		var book = await _uow.Books.GetByIdAsync(isbn)
			?? throw new NotFoundException($"Book {isbn} not found");
		_mapper.Map(dto, book);
		await _uow.Books.UpdateAsync(book);
		await _uow.SaveChangesAsync();
		return Ok(ApiResponse<BookDto>.Ok(_mapper.Map<BookDto>(book)));
	}

	[HttpDelete("{isbn}")]
	public async Task<IActionResult> Delete(string isbn)
	{
		if (!await _uow.Books.ExistsAsync(isbn))
			throw new NotFoundException($"Book {isbn} not found");
		await _uow.Books.DeleteAsync(isbn);
		await _uow.SaveChangesAsync();
		return NoContent();
	}
}
