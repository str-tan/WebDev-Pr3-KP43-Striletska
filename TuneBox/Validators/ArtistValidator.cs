using FluentValidation;
using TuneBox.DTOs;

namespace TuneBox.Validators
{
    public class ArtistValidator : AbstractValidator<ArtistCreateUpdateDto>
    {
        public ArtistValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Artist name is required.")
                .MaximumLength(100).WithMessage("Artist name cannot exceed 100 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                // Перевірка, що поле не є числом
                .Must(c => !int.TryParse(c, out _)).WithMessage("A country must be a text, not a number.")
                .MaximumLength(100).WithMessage("Country name cannot exceed 100 characters.");
        }
    }
}