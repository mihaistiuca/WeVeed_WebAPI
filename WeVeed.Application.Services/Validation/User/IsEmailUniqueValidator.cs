using FluentValidation;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.User
{
    public class IsEmailUniqueValidator : AbstractValidator<IsEmailUniqueInput>
    {
        private readonly IUserAppService _userAppService;

        public IsEmailUniqueValidator(IUserAppService userAppService)
        {
            _userAppService = userAppService;

            ValidateEmail();
        }

        private void ValidateEmail()
        {
            RuleFor(a => a.Email).MaximumLength(256).WithMessage("Adresa de email trebuie sa aiba maxim 256 de caractere.");
            RuleFor(a => a.Email).NotEmpty().WithMessage("Adresa de email este un camp obligatoriu.");

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (!string.IsNullOrWhiteSpace(input.Email))
                {
                    var isEmailUnique = await _userAppService.IsEmailUnique(input.Email, null);
                    if (!isEmailUnique)
                    {
                        context.AddFailure("Email", "Adresa de email este deja folosita.");
                    }
                }
            });
        }
    }
}
