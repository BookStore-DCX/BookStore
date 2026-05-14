using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Author;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorController : ControllerBase
{
	private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
	public AuthorController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }
	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetAll()
	{
		var authors = await _uow.Authors.GetAllAsync();
		return Ok(ApiResponse<IEnumerable<AuthorDto>>.Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors)));
	}

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var author = await _uow.Authors.GetByIdAsync(id);
        if (author == null) throw new NotFoundException($"Author ID {id} not found");
        return Ok(ApiResponse<AuthorDto>.Ok(_mapper.Map<AuthorDto>(author)));
    }

    [HttpGet("search/{authorName}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetByName(string authorName)
	{
		var authors = await _uow.Authors.SearchAuthorsAsync(authorName);
		if (!authors.Any()) throw new NotFoundException($"No authors found matching '{authorName}'");
		return Ok(ApiResponse<IEnumerable<AuthorDto>>.Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors)));
	}
	[HttpPost]
	[Authorize(Roles = "Admin, StoreOwner")]
	public async Task<IActionResult> Create([FromBody] AuthorCreateDto dto)
	{
		var author = _mapper.Map<Author>(dto);
		await _uow.Authors.AddAsync(author);
		await _uow.SaveChangesAsync();
		return CreatedAtAction(nameof(GetByName), new { authorName = author.LastName },
			ApiResponse<AuthorDto>.Created(_mapper.Map<AuthorDto>(author)));
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin, StoreOwner")]
	public async Task<IActionResult> Update(int id, [FromBody] AuthorCreateDto dto)
	{
		var author = await _uow.Authors.GetByIdAsync(id)
			?? throw new NotFoundException($"Author ID {id} not found");
		_mapper.Map(dto, author);
		await _uow.Authors.UpdateAsync(author);
		await _uow.SaveChangesAsync();
		return Ok(ApiResponse<AuthorDto>.Ok(_mapper.Map<AuthorDto>(author)));
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin, StoreOwner")]
	public async Task<IActionResult> Delete(int id)
	{
		if (!await _uow.Authors.ExistsAsync(id)) throw new NotFoundException($"Author ID {id} not found");
		await _uow.Authors.DeleteAsync(id);
		await _uow.SaveChangesAsync();
		return NoContent();
	}
}