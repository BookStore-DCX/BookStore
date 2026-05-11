using AutoMapper;
using BookStore.Common;
using BookStore.DTOs;
using BookStore.DTOs.State;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StateController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public StateController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var states = await _uow.States.GetAllAsync();

            var result = _mapper.Map<IEnumerable<StateDto>>(states);

            return Ok(ApiResponse<IEnumerable<StateDto>>.Ok(result));
        }

        [HttpGet("{stateCode}")]
        public async Task<IActionResult> GetById(string stateCode)
        {
            var state = await _uow.States.GetByIdAsync(stateCode);

            if (state == null)
            {
                return NotFound(
                    ApiResponse<string>.Fail(
                        $"No state exists with code '{stateCode}'."
                    )
                );
            }

            var result = _mapper.Map<StateDto>(state);

            return Ok(ApiResponse<StateDto>.Ok(result));
        }
    }
}