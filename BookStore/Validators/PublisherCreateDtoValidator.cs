using FluentValidation;
using BookStore.DTOs.Publisher;

namespace BookStore.Validators
{
    public class PublisherCreateDtoValidator : AbstractValidator<PublisherCreateDto>
    {
        public PublisherCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.City)
                .MaximumLength(30);

            RuleFor(x => x.StateCode)
                .Length(2)
                .When(x => !string.IsNullOrWhiteSpace(x.StateCode));
        }
    }
}