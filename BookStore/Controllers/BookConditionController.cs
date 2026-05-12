using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.BookCondition;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookConditionController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BookConditionController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var conditions = await _uow.BookConditions.GetAllAsync();

            var result = _mapper.Map<IEnumerable<BookConditionDto>>(conditions);

            return Ok(ApiResponse<IEnumerable<BookConditionDto>>.Ok(result));
        }
    }
}
