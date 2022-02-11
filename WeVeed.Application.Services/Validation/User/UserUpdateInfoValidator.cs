using FluentValidation;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.User
{
    public class UserUpdateInfoValidator : AbstractValidator<UserUpdateInfoInput>
    {
        public UserUpdateInfoValidator()
        {
            ValidateNames();
            ValidateImageUrl();
        }

        private void ValidateNames()
        {
            RuleFor(a => a.FirstName).MaximumLength(50).WithMessage("Prenumele trebuie sa aiba maxim 50 de caractere.");
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("Prenumele trebuie completat.");

            RuleFor(a => a.LastName).MaximumLength(50).WithMessage("Numele trebuie sa aiba maxim 50 de caractere.");
            RuleFor(a => a.LastName).NotEmpty().WithMessage("Numele trebuie completat.");
        }

        private void ValidateImageUrl()
        {
            RuleFor(a => a.ProfileImageUrl).MaximumLength(500).WithMessage("URL-ul imaginii trebuie sa aiba maxim 500 de caractere.");
        }
    }
}
