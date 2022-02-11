using FluentValidation;
using Microsoft.AspNetCore.Http;
using Resources.Base.AuthUtils;
using Resources.Base.Exception;
using System.Collections.Generic;
using System.Linq;
using WeVeed.Application.Dtos;
using WeVeed.Application.Services.Video;

namespace WeVeed.Application.Services.Validation.Video
{
    public class VideoUpdateValidator : AbstractValidator<VideoUpdateInput>
    {
        private readonly IVideoAppService _videoAppService;
        private readonly IHttpContextAccessor _contextAccessor;

        public VideoUpdateValidator(IVideoAppService videoAppService, IHttpContextAccessor contextAccessor, ISeriesAppService seriesAppService)
        {
            _videoAppService = videoAppService;
            _contextAccessor = contextAccessor;

            ValidateTitle();
            ValidateDescription();
            ValidateThumbnailUrl();
            ValidateVideoBelongsToUser();
        }

        private void ValidateTitle()
        {
            RuleFor(a => a.Title).MaximumLength(70).WithMessage("Titlul video-ului trebuie sa contina maxim 70 de caractere");
            RuleFor(a => a.Title).NotEmpty().WithMessage("Titlul video-ului trebuie completat.");
        }

        private void ValidateDescription()
        {
            RuleFor(a => a.Title).MaximumLength(1000).WithMessage("Descrierea video-ului trebuie sa contina maxim 1000 de caractere");
        }

        private void ValidateThumbnailUrl()
        {
            RuleFor(a => a.ThumbnailUrl).MaximumLength(500).WithMessage("Thumbnail URL trebuie sa contina maxim 500 de caractere");
            RuleFor(a => a.ThumbnailUrl).NotEmpty().WithMessage("URL thumbnail-ului trebuie completat.");
        }

        private void ValidateVideoBelongsToUser()
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

                    var doesSeriesBelongToUser = await _videoAppService.DoesVideoBelongToUser(input.Id, userId);
                    if (!doesSeriesBelongToUser)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "Nu puteti modifica acest video deoarece nu va apartine." });
                    }
                }
            });
        }
    }
}
