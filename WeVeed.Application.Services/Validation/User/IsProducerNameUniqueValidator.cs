using FluentValidation;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.User
{
    public class IsProducerNameUniqueValidator : AbstractValidator<IsProducerNameUniqueInput>
    {
        private readonly IUserAppService _userAppService;

        public IsProducerNameUniqueValidator(IUserAppService userAppService)
        {
            _userAppService = userAppService;

            ValidateProducerName();
        }

        private void ValidateProducerName()
        {
            RuleFor(a => a.ProducerName).MaximumLength(50).WithMessage("Numele de producator trebuie sa aiba maxim 50 de caractere.");

            RuleFor(a => a).Custom((input, context) =>
            {
                if (string.IsNullOrWhiteSpace(input.ProducerName))
                {
                    context.AddFailure("ProducerName", "Numele de producator trebuie completat.");
                }
            });

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (!string.IsNullOrWhiteSpace(input.ProducerName))
                {
                    var isProducerNameUnique = await _userAppService.IsProducerNameUnique(input.ProducerName, null);
                    if (!isProducerNameUnique)
                    {
                        context.AddFailure("ProducerName", "Numele de producator este deja folosit.");
                    }
                }
            });
        }
    }
}
