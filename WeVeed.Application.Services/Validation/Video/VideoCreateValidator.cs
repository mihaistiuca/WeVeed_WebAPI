using FluentValidation;
using Microsoft.AspNetCore.Http;
using Resources.Base.AuthUtils;
using Resources.Base.Exception;
using System.Collections.Generic;
using System.Linq;
using WeVeed.Application.Dtos;
using WeVeed.Application.Services.Video;
using WeVeed.Domain.Entities;

namespace WeVeed.Application.Services.Validation.Video
{
    public class VideoCreateValidator : AbstractValidator<VideoCreateInput>
    {
        private readonly IVideoAppService _videoAppService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISeriesAppService _seriesAppService;

        public VideoCreateValidator(IVideoAppService videoAppService, IHttpContextAccessor contextAccessor, ISeriesAppService seriesAppService)
        {
            _videoAppService = videoAppService;
            _contextAccessor = contextAccessor;
            _seriesAppService = seriesAppService;

            ValidateTitle();
            ValidateDescription();
            ValidateVideoUrl();
            ValidateThumbnailUrl();
            ValidateControlbarThumbsUrl();
            ValidateLength();
            ValidateSeason();
            ValidateEpisode();
            ValidateSeasonEpisode();
            ValidateUserIsProducer();
            ValidateSeriesId();
            ValidateSeriesBelongsToUser();
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
        
        private void ValidateVideoUrl()
        {
            RuleFor(a => a.VideoUrl).MaximumLength(200).WithMessage("Video URL trebuie sa contina maxim 200 de caractere");
            RuleFor(a => a.VideoUrl).NotEmpty().WithMessage("URL video-ului trebuie completat.");
        }

        private void ValidateThumbnailUrl()
        {
            RuleFor(a => a.ThumbnailUrl).MaximumLength(500).WithMessage("Thumbnail URL trebuie sa contina maxim 500 de caractere");
            RuleFor(a => a.ThumbnailUrl).NotEmpty().WithMessage("URL thumbnail-ului trebuie completat.");
        }

        private void ValidateControlbarThumbsUrl()
        {
            RuleFor(a => a.ControlbarThumbnailsUrl).MaximumLength(500).WithMessage("Progress thumbnails URL trebuie sa contina maxim 500 de caractere");
            //RuleFor(a => a.ControlbarThumbnailsUrl).NotEmpty().WithMessage("Progress thumbnails URL trebuie completat.");
        }

        private void ValidateLength()
        {
            RuleFor(a => a).Custom((input, context) =>
            {
                if (input.Length <= 0)
                {
                    context.AddFailure("Length", "Dimensiunea video-ului trebuie sa fie un numar");
                }
            });
        }

        private void ValidateSeason()
        {
            RuleFor(a => a).Custom((input, context) =>
            {
                if (!string.IsNullOrWhiteSpace(input.SeriesId) && (input.Season == null || input.Season.Value == 0))
                {
                    context.AddFailure("Season", "Sezonul video-ului trebuie completat.");
                }
            });
        }

        private void ValidateEpisode()
        {
            RuleFor(a => a).Custom((input, context) =>
            {
                if (!string.IsNullOrWhiteSpace(input.SeriesId) && (input.Episode == null || input.Episode == 0))
                {
                    context.AddFailure("Episode", "Episodul video-ului trebuie completat.");
                }
            });
        }

        private void ValidateSeasonEpisode()
        {
            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if(!string.IsNullOrWhiteSpace(input.SeriesId) && input.Season != null && input.Episode != null && input.Season.Value != 0 && input.Episode.Value != 0)
                {
                    var isCombinationValid = await _seriesAppService.IsSeasonEpisodeCombinationValid(input.SeriesId, input.Season.Value, input.Episode.Value);
                    if (!isCombinationValid)
                    {
                        context.AddFailure("Episode", "Combinatia de episod - sezon nu este valida.");
                    }
                }
            });
        }

        private void ValidateUserIsProducer()
        {
            RuleFor(a => a).Custom((input, context) =>
            {
                var usertype = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserType)?.Value;
                if (usertype == null || usertype != "producer")
                {
                    throw new HttpStatusCodeException(500, new List<string> { "Pentru a adauga un video trebuie sa fii producator." });
                }
            });
        }

        private void ValidateSeriesId()
        {
            RuleFor(a => a.SeriesId).MaximumLength(30).WithMessage("Id-ul emisiunii nu este valid.");
            RuleFor(a => a.SeriesId).NotEmpty().WithMessage("Episodul trebuie sa faca parte dintr-o emisiune.");
        }

        private void ValidateSeriesBelongsToUser()
        {
            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                if (!string.IsNullOrWhiteSpace(input.SeriesId))
                {
                    var userId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
                    if (userId == null)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "Ceva nu a mers bine. Te rog incearca din nou." });
                    }

                    var doesSeriesBelongToUser = await _seriesAppService.DoesSeriesBelongToUser(input.SeriesId, userId);
                    if (!doesSeriesBelongToUser)
                    {
                        throw new HttpStatusCodeException(500, new List<string> { "Nu puteti crea un video in aceasta emisiune deoarece nu va apartine." });
                    }
                }
            });
        }

        private void ValidateCategory()
        {
            RuleFor(a => a.SeriesCategory).NotEmpty().WithMessage("Categoria video-ului trebuie completata.");
            RuleFor(a => a).Custom((input, context) =>
            {
                if (!string.IsNullOrWhiteSpace(input.SeriesCategory))
                {
                    if (!WeVeedConstants.Categories.Contains(input.SeriesCategory.ToLower()))
                    {
                        context.AddFailure("SeriesCategory", "Categoria specificata nu este valida.");
                    }
                }
            });
        }
    }
}
