using AutoMapper;
using BookStore.Common;
using BookStore.DTOs;
using BookStore.DTOs.Category;
using BookStore.Exceptions;
using BookStore.Repositories.Interfaces;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _uow.Categories.GetAllAsync();

            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _uow.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(
         ApiResponse<string>.Fail($"No category exists with ID {id}."));
            }

            var result = _mapper.Map<CategoryDto>(category);

            return Ok(ApiResponse<CategoryDto>.Ok(result));
        }
    }
}