using FluentValidation;
using Microsoft.AspNetCore.Http;
using Resources.Base.AuthUtils;
using Resources.Base.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Application.Services.Validation.Series
{
    public class SeriesCreateValidator : AbstractValidator<SeriesCreateInput>
    {
        private readonly ISeriesAppService _seriesAppService;
        private IHttpContextAccessor _contextAccessor;

        public SeriesCreateValidator(ISeriesAppService seriesAppService, IHttpContextAccessor contextAccessor)
        {
            _seriesAppService = seriesAppService;
            _contextAccessor = contextAccessor;

            ValidateUserIsProducer();
            ValidateName();
            ValidateDescription();
            ValidateThumbnailUrl();
            ValidateCategory();
        }

        private void ValidateUserIsProducer()
        {
            RuleFor(a => a).Custom((input, context) =>
            {
                var usertype = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserType)?.Value;
                if(usertype == null || usertype != "producer")
                {
                    throw new HttpStatusCodeException(500, new List<string> { "Pentru a crea o emisiune trebuie sa fii producator." });
                }
            });
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
                    if(id == null)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "Ceva nu a mers bine. Te rog incearca din nou." });
                    }

                    var isSeriesNameUnique = await _seriesAppService.IsSeriesNameUnique(input.Name, null, id);
                    if (!isSeriesNameUnique)
                    {
                        context.AddFailure("Name", "Aceasta emisiune este deja creata.");
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
            //RuleFor(a => a).Custom((input, context) =>
            //{
            //    Uri uriResult;
            //    bool result = Uri.TryCreate(input.ThumbnailUrl, UriKind.Absolute, out uriResult)
            //                  && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            //    if (!result)
            //    {
            //        context.AddFailure("ThumbnailUrl", "Thumbnail URL nu este un URL valid. Te rugam sa specifici un URL de tip HTTP sau HTTPS.");
            //    }
            //});
        }

        private void ValidateCategory()
        {
            RuleFor(a => a.Category).NotEmpty().WithMessage("Categoria emisiunii trebuie completata.");
            RuleFor(a => a).Custom((input, context) =>
            {
                if (!string.IsNullOrWhiteSpace(input.Category))
                {
                    if (!WeVeedConstants.Categories.Contains(input.Category.ToLower()))
                    {
                        context.AddFailure("Category", "Categoria specificata nu este valida.");
                    }
                }
            });
        }
    }
}
