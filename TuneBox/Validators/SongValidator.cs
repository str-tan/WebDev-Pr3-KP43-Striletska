using FluentValidation;
using TuneBox.DTOs;

namespace TuneBox.Validators
{
    public class SongValidator : AbstractValidator<SongCreateUpdateDto>
    {
        public SongValidator()
        {
            // Title validation
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("The song title is required.")
                .MaximumLength(100).WithMessage("The song title must not exceed 100 characters.");

            // Duration validation
            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("The duration of the song (in seconds) must be a greater than 0.");

            RuleFor(x => x.Genre)
                .NotEmpty().WithMessage("The genre is required.")
                // Перевірка, що поле не є числом
                .Must(g => !int.TryParse(g, out _)).WithMessage("The genre must be text, not a number.")
                .MaximumLength(50).WithMessage("The genre must not exceed 50 characters.");

            RuleFor(x => x.ArtistId)
                .GreaterThan(0).WithMessage("The artist ID must be a greater than 0.");
        }
    }
}