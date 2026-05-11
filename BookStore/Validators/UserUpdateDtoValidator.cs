using BookStore.DTOs.User;
using FluentValidation;

namespace BookStore.Validators
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(20);

            RuleFor(x => x.LastName)
                .MaximumLength(30);

            RuleFor(x => x.PhoneNumber)
                .Length(10) 
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("PhoneNumber must be exactly 10 digits.");
        }
    }
}