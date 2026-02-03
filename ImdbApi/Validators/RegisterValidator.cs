using FluentValidation;
using ImdbApi.Models;

namespace ImdbApi.Validators
{
    public class RegisterValidator : AbstractValidator<User>
    {
        public RegisterValidator()
        {
            RuleFor(u => u.Name).NotEmpty().WithMessage("Username cannot be empty.");
            RuleFor(u => u.Email).EmailAddress().WithMessage("Please provide a valid email.");
            RuleFor(u => u.Password).NotNull().WithMessage("Password cannot be null");
            RuleFor(u => u.Role).NotNull().WithMessage("Role cannot be null");
        }
    }
}
