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
public class AuthorController : ControllerBase
{
	private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
	public AuthorController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		var authors = await _uow.Authors.GetAllAsync();
		return Ok(ApiResponse<IEnumerable<AuthorDto>>.Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors)));
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		var a = await _uow.Authors.GetByIdAsync(id) ?? throw new NotFoundException($"Author {id} not found");
		return Ok(ApiResponse<AuthorDto>.Ok(_mapper.Map<AuthorDto>(a)));
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] AuthorCreateDto dto)
	{
		var author = _mapper.Map<Author>(dto);
		await _uow.Authors.AddAsync(author);
		await _uow.SaveChangesAsync();
		return CreatedAtAction(nameof(GetById), new { id = author.AuthorId },
			ApiResponse<AuthorDto>.Created(_mapper.Map<AuthorDto>(author)));
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		if (!await _uow.Authors.ExistsAsync(id)) throw new NotFoundException($"Author {id} not found");
		await _uow.Authors.DeleteAsync(id);
		await _uow.SaveChangesAsync();
		return NoContent();
	}
}
