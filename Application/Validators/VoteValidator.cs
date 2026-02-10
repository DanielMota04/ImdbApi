using Application.DTOs.Request.Movie;
using FluentValidation;

namespace Application.Validators
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
