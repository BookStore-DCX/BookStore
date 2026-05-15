using FluentValidation;
using BookStore.DTOs.Publisher;

namespace BookStore.Validators
{
    public class PublisherCreateDtoValidator : AbstractValidator<PublisherCreateDto>
    {
        public PublisherCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(30);

            RuleFor(x => x.StateCode)
                .NotEmpty().WithMessage("State is required")
                .Length(2).WithMessage("State code must be exactly 2 characters");
        }
    }
}
