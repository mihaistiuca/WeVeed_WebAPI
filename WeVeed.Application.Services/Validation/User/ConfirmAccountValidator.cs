using FluentValidation;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.User
{
    public class ConfirmAccountValidator : AbstractValidator<ConfirmAccountInput>
    {
        public ConfirmAccountValidator()
        {
            ValidateCode();
        }

        private void ValidateCode()
        {
            RuleFor(a => a.Code).NotEmpty().WithMessage("Codul de validare nu a fost trimis.");
            RuleFor(a => a.Code).MaximumLength(100).WithMessage("Codul de validare nu a fost trimis corect.");
        }
    }
}
