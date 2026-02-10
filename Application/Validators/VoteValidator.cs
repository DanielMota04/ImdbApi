using FluentValidation;
using ImdbApi.DTOs.Request.Movie;

namespace ImdbApi.Validators
{
    public class VoteValidator : AbstractValidator<VoteMovieRequestDTO>
    {
        public VoteValidator()
        {
            RuleFor(v => v.Vote).GreaterThanOrEqualTo(0).WithMessage("Value cannot be negative.")
                .LessThanOrEqualTo(4).WithMessage("Value must be less or equal 4.");
        }
    }
}
