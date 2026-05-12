using BookStore.DTOs.Book;
using FluentValidation;

namespace BookStore.Validators
{
	public class BookCreateDtoValidator : AbstractValidator<BookCreateDto>
	{
		public BookCreateDtoValidator()
		{
			RuleFor(x => x.Isbn)
				.NotEmpty()
				.Length(13).WithMessage("ISBN must be exactly 13 characters")
				.Matches(@"^\d{1}-\d{3}-\d{5}-\d{1}$")
				.WithMessage("ISBN format: X-XXX-XXXXX-X");

			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required")
				.MaximumLength(70).WithMessage("Title max 70 chars");

			RuleFor(x => x.Description)
				.MaximumLength(100).WithMessage("Description max 100 chars")
				.When(x => !string.IsNullOrEmpty(x.Description));

			RuleFor(x => x.Category)
				.InclusiveBetween(1, 9).WithMessage("Category must be 1-9")
				.When(x => x.Category.HasValue);

			RuleFor(x => x.PublisherId)
				.GreaterThan(0).WithMessage("Valid PublisherId required");
		}
	}

}
