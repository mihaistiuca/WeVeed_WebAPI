using FluentValidation;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Application.Services.Validation.User
{
    public class UserFBRegisterValidator : AbstractValidator<UserFBRegisterInput>
    {
        private readonly IUserAppService _userAppService;

        public UserFBRegisterValidator(IUserAppService userAppService)
        {
            _userAppService = userAppService;

            ValidateFirstLastNames();
            ValidateProducerName();
            ValidateEmail();
            ValidateUserType();
        }

        private void ValidateFirstLastNames()
        {
            RuleFor(a => a.FirstName).MaximumLength(200).WithMessage("Prenumele trebuie sa aiba maxim 200 de caractere.");
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("Prenumele trebuie completat.");

            RuleFor(a => a.LastName).MaximumLength(200).WithMessage("Numele trebuie sa aiba maxim 200 de caractere.");
            RuleFor(a => a.LastName).NotEmpty().WithMessage("Numele trebuie completat.");
        }

        private void ValidateProducerName()
        {
            RuleFor(a => a.ProducerName).MaximumLength(50).WithMessage("Numele de producator trebuie sa aiba maxim 50 de caractere.");

            RuleFor(a => a).Custom((input, context) =>
            {
                if (input.UserType == WeVeedConstants.UserTypeProducer)
                {
                    if (string.IsNullOrWhiteSpace(input.ProducerName))
                    {
                        context.AddFailure("ProducerName", "Numele de producator trebuie completat.");
                    }
                }
                else if (input.UserType != WeVeedConstants.UserTypeProducer)
                {
                    if (!string.IsNullOrWhiteSpace(input.ProducerName))
                    {
                        context.AddFailure("ProducerName", "Numele de producator nu trebuie completat deoarece tipul de utilizator ales nu este 'Producator'.");
                    }
                }
            });

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (input.UserType == WeVeedConstants.UserTypeProducer && !string.IsNullOrWhiteSpace(input.ProducerName))
                {
                    var isProducerNameUnique = await _userAppService.IsProducerNameUnique(input.ProducerName, null);
                    if (!isProducerNameUnique)
                    {
                        context.AddFailure("ProducerName", "Numele de producator este deja folosit.");
                    }
                }
            });
        }

        private void ValidateEmail()
        {
            RuleFor(a => a.Email).MaximumLength(256).WithMessage("Adresa de email trebuie sa aiba maxim 256 de caractere.");
        }

        private void ValidateUserType()
        {
            RuleFor(a => a).Custom((input, context) =>
            {
                if (input.UserType != WeVeedConstants.UserTypeUser && input.UserType != WeVeedConstants.UserTypeProducer)
                {
                    context.AddFailure("UserType", "Tipul de utilizator ales nu este unul valid.");
                }
            });
        }
    }
}
