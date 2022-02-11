using FluentValidation;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.User
{
    public class UserBecomeProducerValidator : AbstractValidator<UserBecomeProducerInput>
    {
        private readonly IUserAppService _userAppService;

        public UserBecomeProducerValidator(IUserAppService userAppService)
        {
            _userAppService = userAppService;

            ValidateProducerName();
            ValidateProducerDescription();
        }

        private void ValidateProducerName()
        {
            RuleFor(a => a.ProducerName).MaximumLength(50).WithMessage("Numele de producator trebuie sa aiba maxim 50 de caractere.");
            RuleFor(a => a.ProducerName).NotEmpty().WithMessage("Numele de producator trebuie completat.");

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

        private void ValidateProducerDescription()
        {
            RuleFor(a => a.ProducerDescription).MaximumLength(500).WithMessage("Descrierea producatorului trebuie sa aiba maxim 500 de caractere.");
            RuleFor(a => a.ProducerDescription).NotEmpty().WithMessage("Descrierea producatorului trebuie completata.");
        }
    }
}
