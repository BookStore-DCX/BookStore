using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Book;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public BookController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var books = await _uow.Books.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin, StoreOwner")]
    public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
    {
        var book = _mapper.Map<Book>(dto);
        await _uow.Books.AddAsync(book);
        try
        {
            await _uow.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            if (msg.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException($"A book with ISBN '{dto.Isbn}' already exists.");
            }

            if (msg.Contains("truncated", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("too long", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("max", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Book data is invalid for database limits. Verify ISBN format X-XXX-XXXXX-X and text lengths.");
            }

            throw;
        }
        return CreatedAtAction(nameof(GetByIsbn), new { isbn = book.Isbn },
            ApiResponse<BookDto>.Created(_mapper.Map<BookDto>(book)));
    }

    [HttpGet("{isbn}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIsbn(string isbn)
    {
        var book = await _uow.Books.GetBookWithDetailsAsync(isbn)
            ?? throw new NotFoundException($"Book with ISBN '{isbn}' not found");

        var result = _mapper.Map<BookDetailDto>(book);
        return Ok(ApiResponse<BookDetailDto>.Ok(result));
    }

    [HttpPut("{isbn}")]
    [Authorize(Roles = "Admin, StoreOwner")]
    public async Task<IActionResult> Update(string isbn, [FromBody] BookUpdateDto dto)
    {
        var book = await _uow.Books.GetByIdAsync(isbn)
            ?? throw new NotFoundException($"Book with ISBN '{isbn}' not found");
        _mapper.Map(dto, book);
        await _uow.Books.UpdateAsync(book);
        await _uow.SaveChangesAsync();
        return Ok(ApiResponse<BookDto>.Ok(_mapper.Map<BookDto>(book)));
    }

    [HttpDelete("{isbn}")]
    [Authorize(Roles = "StoreOwner, Admin")]
    public async Task<IActionResult> Delete(string isbn)
    {
        if (!await _uow.Books.ExistsAsync(isbn))
            throw new NotFoundException($"Book with ISBN '{isbn}' not found");
        await _uow.Books.DeleteAsync(isbn);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("category/{name}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategory(string name)
    {
        var books = await _uow.Books.GetBooksByCategoryAsync(name);
        return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
    }

    [HttpGet("author/{authorId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByAuthor(int authorId)
    {
        var books = await _uow.Books.GetBooksByAuthorAsync(authorId);
        return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string? authorName, [FromQuery] string? title, [FromQuery] string? description)
    {
        var books = await _uow.Books.SearchBooksAsync(authorName, title, description);
        return Ok(ApiResponse<IEnumerable<BookDto>>.Ok(_mapper.Map<IEnumerable<BookDto>>(books)));
    }
}
