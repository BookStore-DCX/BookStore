using BookStore.DTOs.Inventory;
using FluentValidation;

namespace BookStore.Validators
{
    public class InventoryCreateDtoValidator : AbstractValidator<InventoryCreateDto>
    {
        public InventoryCreateDtoValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty().Length(13).Matches(@"^[\d\-]+$");
            RuleFor(x => x.Ranks).InclusiveBetween(1, 6);
        }
    }
}
