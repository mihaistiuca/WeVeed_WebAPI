using FluentValidation;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Application.Services.Validation.User
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterInput>
    {
        private readonly IUserAppService _userAppService;

        public UserRegisterValidator(IUserAppService userAppService)
        {
            _userAppService = userAppService;

            ValidateFirstLastNames();
            ValidateProducerName();
            ValidateEmail();
            ValidatePassword();
            ValidateUserType();
        }

        private void ValidateFirstLastNames()
        {
            RuleFor(a => a.FirstName).MaximumLength(50).WithMessage("Prenumele trebuie sa aiba maxim 50 de caractere.");
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("Prenumele trebuie completat.");

            RuleFor(a => a.LastName).MaximumLength(50).WithMessage("Numele trebuie sa aiba maxim 50 de caractere.");
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
            RuleFor(a => a.Email).NotEmpty().WithMessage("Adresa de email este un camp obligatoriu.");
            RuleFor(a => a.Email).EmailAddress().WithMessage("Adresa de email nu este valida.");

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                var isEmailUnique = await _userAppService.IsEmailUnique(input.Email, null);
                if (!isEmailUnique)
                {
                    context.AddFailure("Email", "Adresa de email este deja folosita.");
                }
            });
        }

        private void ValidatePassword()
        {
            RuleFor(a => a.Password).MaximumLength(32).WithMessage("Parola trebuie sa aiba maxim 32 de caractere.");
            RuleFor(a => a.Password).MinimumLength(8).WithMessage("Parola trebuie sa aiba minim 8 de caractere.");
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
