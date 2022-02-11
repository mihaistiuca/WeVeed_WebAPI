using FluentValidation;
using Microsoft.AspNetCore.Http;
using Resources.Base.AuthUtils;
using Resources.Base.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Validation.Series
{
    public class IsSeriesNameUniqueUpdateValidator : AbstractValidator<IsSeriesNameUniqueUpdateInput>
    {
        private readonly ISeriesAppService _seriesAppService;
        private IHttpContextAccessor _contextAccessor;

        public IsSeriesNameUniqueUpdateValidator(ISeriesAppService seriesAppService, IHttpContextAccessor contextAccessor)
        {
            _seriesAppService = seriesAppService;
            _contextAccessor = contextAccessor;

            ValidateId();
            ValidateName();
        }

        private void ValidateId()
        {
            RuleFor(a => a.SeriesId).NotEmpty().WithMessage("Id-ul emisiunii trebuie completat.");
        }

        private void ValidateName()
        {
            RuleFor(a => a.Name).MaximumLength(70).WithMessage("Numele emisiunii trebuie sa aiba maxim 70 de caractere.");
            RuleFor(a => a.Name).NotEmpty().WithMessage("Numele emisiunii trebuie completat.");

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (!string.IsNullOrWhiteSpace(input.Name))
                {
                    var id = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
                    if (id == null)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "Ceva nu a mers bine. Te rog incearca din nou." });
                    }

                    var isSeriesNameUnique = await _seriesAppService.IsSeriesNameUnique(input.Name, input.SeriesId, id);
                    if (!isSeriesNameUnique)
                    {
                        context.AddFailure("Name", "O emisiune cu acelasi titlu exista deja.");
                    }
                }
            });
        }
    }
}
