using BookStore.DTOs.Auth;
using FluentValidation;

namespace BookStore.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(20);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(30);

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required")
                .MaximumLength(30).WithMessage("Username can be above 30 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.PhoneNumber)
                .Length(10)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("PhoneNumber must be exactly 10 digits.");
        }
    }
}