using FluentValidation;
using ImdbApi.DTOs.Request.Movie;

namespace ImdbApi.Validators
{
    public class CreateMovieValidator : AbstractValidator<CreateMovieRequestDTO>
    {
        public CreateMovieValidator()
        {
            RuleFor(m => m.Title).NotEmpty().WithMessage("Movie title cannot be empty")
                .Length(2, 50).WithMessage("Title cannot be shorter than 2 letter neither longer than 50");
        }
    }
}
