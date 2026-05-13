using AutoMapper;
using BookStore.Common;
using BookStore.DTOs;
using BookStore.DTOs.Category;
using BookStore.Exceptions;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _uow.Categories.GetAllAsync();

            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(result));
        }



        [HttpGet("{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByName(string name)
        {
            var category = await _uow.Categories.GetByCategoryNameAsync(name);

            if (category == null)
            {
                return NotFound(
         ApiResponse<string>.Fail($"No category exists with name '{name}'."));
            }

            var result = _mapper.Map<CategoryDto>(category);

            return Ok(ApiResponse<CategoryDto>.Ok(result));
        }
    }
}