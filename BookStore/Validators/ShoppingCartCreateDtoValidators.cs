using BookStore.DTOs.PurchaseLog;
using FluentValidation;

namespace BookStore.Validators
{
    public class ShoppingCartCreateDtoValidator : AbstractValidator<ShoppingCartCreateDto>
    {
        public ShoppingCartCreateDtoValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.Isbn).NotEmpty().Length(13).Matches(@"^[\d\-]+$");
        }
    }
}
