using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Review;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
        public ReviewController(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        [HttpGet("book/{isbn}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIsbn(string isbn)
        {
            var reviews = await _uow.Reviews.GetReviewsByBookAsync(isbn);

            return Ok(
                ApiResponse<IEnumerable<ReviewDto>>.Ok(
                    _mapper.Map<IEnumerable<ReviewDto>>(reviews)
                )
            );
        }

        [HttpGet("reviewer/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByReviewer(int id)
        {
            var reviews = await _uow.Reviews.GetReviewsByReviewerAsync(id);
            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews)));
        }

        [HttpPost]
                [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> Create([FromBody] ReviewDto dto)
        {
            var review = _mapper.Map<Bookreview>(dto);
            await _uow.Reviews.AddAsync(review);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByIsbn), new { isbn = review.Isbn },
                ApiResponse<ReviewDto>.Created(_mapper.Map<ReviewDto>(review)));
        }

        [HttpDelete("{isbn}/{reviewerId}")]
        [Authorize(Roles = "Admin, StoreOwner, RegisteredUser")]
        public async Task<IActionResult> Delete(string isbn, int reviewerId)
        {
            var reviews = await _uow.Reviews.GetReviewsByBookAsync(isbn);

            var review = reviews.FirstOrDefault(r => r.ReviewerId == reviewerId)
                ?? throw new NotFoundException("Review not found");

            await _uow.Reviews.DeleteAsync(review);

            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }

}
