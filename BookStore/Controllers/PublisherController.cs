using AutoMapper;
using BookStore.Models;
using BookStore.Common;
using BookStore.DTOs.Publisher;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.Repositories.Interfaces;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [AllowAnonymous]
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
            var p = await _uow.Publishers.GetByIdAsync(id)
                    ?? throw new NotFoundException($"Publisher {id} not found");

            return Ok(
                ApiResponse<PublisherDto>.Ok(
                    _mapper.Map<PublisherDto>(p)
                )
            );
        }

        [HttpGet("state/{stateCode}")]
        public async Task<IActionResult> GetByState(string stateCode)
        {
            var pubs = await _uow.Publishers.GetPublishersByStateAsync(stateCode);

            return Ok(
                ApiResponse<IEnumerable<PublisherDto>>.Ok(
                    _mapper.Map<IEnumerable<PublisherDto>>(pubs)
                )
            );
        }

        [HttpPost]
        [Authorize(Policy = "StoreOwner")]
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