using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Publisher;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PublisherController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pubs = await _uow.Publishers.GetAllAsync();

            return Ok(
                ApiResponse<IEnumerable<PublisherDto>>.Ok(
                    _mapper.Map<IEnumerable<PublisherDto>>(pubs)
                )
            );
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var publisher = await _uow.Publishers.GetByIdAsync(id);

            if (publisher == null)
            {
                return NotFound(
                    ApiResponse<string>.Fail(
                        $"No publisher exists with ID {id}."
                    )
                );
            }

            return Ok(
                ApiResponse<PublisherDto>.Ok(
                    _mapper.Map<PublisherDto>(publisher)
                )
            );
        }

        [HttpGet("state/{stateCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByState(string stateCode)
        {
            var pubs = await _uow.Publishers.GetPublishersByStateAsync(stateCode);

            if (!pubs.Any())
            {
                return NotFound(
                    ApiResponse<string>.Fail(
                        $"No publishers found for state code '{stateCode}'."
                    )
                );
            }

            return Ok(
                ApiResponse<IEnumerable<PublisherDto>>.Ok(
                    _mapper.Map<IEnumerable<PublisherDto>>(pubs)
                )
            );
        }


        [HttpPost]
        [Authorize(Roles = "StoreOwner,Admin")]
        public async Task<IActionResult> Create([FromBody] PublisherCreateDto dto)
        {
            var pub = _mapper.Map<Publisher>(dto);

            await _uow.Publishers.AddAsync(pub);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = pub.PublisherId },
                ApiResponse<PublisherDto>.Created(
                    _mapper.Map<PublisherDto>(pub)
                )
            );
        }
    }
}