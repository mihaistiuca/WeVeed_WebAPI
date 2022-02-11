using FluentValidation;
using Microsoft.AspNetCore.Http;
using Resources.Base.AuthUtils;
using System.Linq;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.User
{
    public class ProducerUpdateInfoValidator : AbstractValidator<ProducerUpdateInfoInput>
    {
        private readonly IUserAppService _userAppService;
        private IHttpContextAccessor _contextAccessor;

        public ProducerUpdateInfoValidator(IUserAppService userAppService, IHttpContextAccessor contextAccessor)
        {
            _userAppService = userAppService;
            _contextAccessor = contextAccessor;

            ValidateNames();
            ValidateImageUrl();

            ValidateProducerName();
            ValidateProducerDescription();
            ValidateContacts();
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

        private void ValidateProducerName()
        {
            RuleFor(a => a.ProducerName).MaximumLength(50).WithMessage("Numele de producator trebuie sa aiba maxim 50 de caractere.");
            RuleFor(a => a.ProducerName).NotEmpty().WithMessage("Numele de producator trebuie completat.");

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (!string.IsNullOrWhiteSpace(input.ProducerName))
                {
                    var id = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;

                    var isProducerNameUnique = await _userAppService.IsProducerNameUnique(input.ProducerName, id);
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

        private void ValidateContacts()
        {
            RuleFor(a => a.EmailContact).MaximumLength(500).WithMessage("Email-ul trebuie sa aiba maxim 500 de caractere.");
            RuleFor(a => a.FacebookContactUrl).MaximumLength(500).WithMessage("URL-ul de Facebook trebuie sa aiba maxim 500 de caractere.");
            RuleFor(a => a.InstaContactUrl).MaximumLength(500).WithMessage("URL-ul de Instagram trebuie sa aiba maxim 500 de caractere.");
        }
    }
}
