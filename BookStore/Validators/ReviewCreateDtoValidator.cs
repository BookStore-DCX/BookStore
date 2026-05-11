using BookStore.DTOs.Review;
using FluentValidation;

namespace BookStore.Validators
{
    public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
    {
        public ReviewCreateDtoValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty().Length(13).Matches(@"^[\d\-]+$");
            RuleFor(x => x.ReviewerId).GreaterThan(0);
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).When(x => x.Rating.HasValue);
            RuleFor(x => x.Comments).MaximumLength(255);
        }
    }

}
