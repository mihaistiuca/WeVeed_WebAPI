using FluentValidation;
using Microsoft.AspNetCore.Http;
using Resources.Base.AuthUtils;
using Resources.Base.Exception;
using System.Collections.Generic;
using System.Linq;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.Series
{
    public class SeriesUpdateValidator : AbstractValidator<SeriesUpdateInput>
    {
        private readonly ISeriesAppService _seriesAppService;
        private IHttpContextAccessor _contextAccessor;

        public SeriesUpdateValidator(ISeriesAppService seriesAppService, IHttpContextAccessor contextAccessor)
        {
            _seriesAppService = seriesAppService;
            _contextAccessor = contextAccessor;

            ValidateSeriesBelongsToUser();
            ValidateId();
            ValidateName();
            ValidateDescription();
            ValidateThumbnailUrl();
        }

        private void ValidateSeriesBelongsToUser()
        {
            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (!string.IsNullOrWhiteSpace(input.Id))
                {
                    var userId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
                    if (userId == null)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "Ceva nu a mers bine. Te rog incearca din nou." });
                    }

                    var doesSeriesBelongToUser = await _seriesAppService.DoesSeriesBelongToUser(input.Id, userId);
                    if (!doesSeriesBelongToUser)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "User-ul curent nu poate edita aceasta emisiune deoarece nu ii apartine." });
                    }
                }
            });
        }

        private void ValidateId()
        {
            RuleFor(a => a.Name).NotEmpty().WithMessage("Id-ul emisiunii trebuie completat.");
        }

        private void ValidateName()
        {
            RuleFor(a => a.Name).MaximumLength(70).WithMessage("Numele emisiunii trebuie sa aiba maxim 70 de caractere.");
            RuleFor(a => a.Name).NotEmpty().WithMessage("Numele emisiunii trebuie completat.");

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (!string.IsNullOrWhiteSpace(input.Name) && !string.IsNullOrWhiteSpace(input.Id))
                {
                    var id = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
                    if (id == null)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "Ceva nu a mers bine. Te rog incearca din nou." });
                    }

                    var isSeriesNameUnique = await _seriesAppService.IsSeriesNameUnique(input.Name, input.Id, id);
                    if (!isSeriesNameUnique)
                    {
                        context.AddFailure("Name", "O emisiune avand acest nume exista deja.");
                    }
                }
            });
        }

        private void ValidateDescription()
        {
            RuleFor(a => a.Description).MaximumLength(1000).WithMessage("Descrierea emisiunii trebuie sa aiba maxim 1000 de caractere.");
            RuleFor(a => a.Description).NotEmpty().WithMessage("Descrierea emisiunii trebuie completata.");
        }

        private void ValidateThumbnailUrl()
        {
            RuleFor(a => a.ThumbnailUrl).MaximumLength(500).WithMessage("Thumbnail URL trebuie sa contina maxim 500 de caractere");
        }
    }
}
