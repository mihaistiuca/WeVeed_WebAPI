using FluentValidation;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.User
{
    public class IsLoggedProducerNameUniqueValidator : AbstractValidator<IsLoggedProducerNameUniqueInput>
    {
        private readonly IUserAppService _userAppService;

        public IsLoggedProducerNameUniqueValidator(IUserAppService userAppService)
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
        }
    }
}
