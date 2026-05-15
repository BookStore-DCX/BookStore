using AutoMapper;
using BookStore.Common;
using BookStore.DTOs.Review;
using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ReviewController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet("book/name/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByBookName(string name)
        {
            var reviews = await _uow.Reviews.GetReviewsByBookNameAsync(name);
            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews)));
        }

        [HttpGet("book/{isbn}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByBookIsbn(string isbn)
        {
            var reviews = await _uow.Reviews.GetReviewsByBookIsbnAsync(isbn);
            return Ok(ApiResponse<IEnumerable<ReviewDto>>.Ok(_mapper.Map<IEnumerable<ReviewDto>>(reviews)));
        }

        [HttpPost]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId");
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedException("Invalid user id in token.");
            }

            var user = await _uow.Users.GetByIdAsync(userId)
                ?? throw new NotFoundException($"User with ID {userId} not found");

            var reviewer = await _uow.Reviews.GetReviewerByIdAsync(userId);
            if (reviewer == null)
            {
                var reviewerName = $"{user.FirstName} {user.LastName}".Trim();
                if (string.IsNullOrWhiteSpace(reviewerName))
                {
                    reviewerName = user.UserName;
                }

                if (reviewerName.Length > 20)
                {
                    reviewerName = reviewerName[..20];
                }

                reviewer = new Reviewer
                {
                    ReviewerId = userId,
                    Name = reviewerName,
                    EmployedBy = null
                };

                await _uow.Reviews.AddReviewerAsync(reviewer);
                await _uow.SaveChangesAsync();
            }

            if (await _uow.Reviews.ReviewExistsAsync(dto.Isbn, reviewer.ReviewerId))
            {
                throw new ConflictException("You have already reviewed this book.");
            }

            var review = new Bookreview
            {
                Isbn = dto.Isbn,
                ReviewerId = reviewer.ReviewerId,
                Rating = dto.Rating,
                Comments = dto.Comments
            };

            await _uow.Reviews.AddAsync(review);
            await _uow.SaveChangesAsync();

            var dtoResult = _mapper.Map<ReviewDto>(review);
            if (dtoResult.BookName == null)
            {
                var book = await _uow.Books.GetByIdAsync(dto.Isbn);
                dtoResult.BookName = book?.Title;
            }

            return Ok(ApiResponse<ReviewDto>.Ok(dtoResult));
        }

        [HttpDelete("{isbn}/{reviewerId:int}")]
        [Authorize(Roles = "Admin, StoreOwner, RegisteredUser")]
        public async Task<IActionResult> Delete(string isbn, int reviewerId)
        {
            await _uow.Reviews.DeleteReviewAsync(isbn, reviewerId);
            await _uow.SaveChangesAsync();
            return Ok(ApiResponse<string>.Ok("Review deleted successfully"));
        }
    }
}