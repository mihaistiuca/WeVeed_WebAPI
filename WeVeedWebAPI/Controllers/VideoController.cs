using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Resources.Base.AuthUtils;
using Resources.Base.Exception;
using Resources.Base.Responses;
using Resources.Base.SettingsModels;
using Resources.Base.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Application.Services.Video;
using WeVeed.Application.Services.View;
using WeVeedWebAPI.Extensions;

namespace WeVeedWebAPI.Controllers
{
    [Route("[controller]")]
    public class VideoController : Controller
    {
        private readonly IVideoAppService _videoAppService;
        private readonly IViewAppService _viewAppService;
        private readonly IOptions<AppGeneralSettings> _appGeneralSettings;

        private readonly IEmailSender _emailSender;

        public VideoController(IVideoAppService videoAppService, IEmailSender emailSender, IViewAppService viewAppService, IOptions<AppGeneralSettings> appGeneralSettings)
        {
            _videoAppService = videoAppService;
            _appGeneralSettings = appGeneralSettings;
            _viewAppService = viewAppService;

            _emailSender = emailSender;
        }

        #region Operations with Videos 

        [HttpPost("validatesns1")]
        public async Task<IEnumerable<string>> Test()
        {
            //ConfirmSubscriptionRequest request
            //if (!string.IsNullOrWhiteSpace(request.Token))
            //{
            //_emailSender.SendSNSConfirmationToken("content");

            //}
            string text = "has not been changed";
            try
            {
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    text = await reader.ReadToEndAsync();
                }
            }
            catch
            {
                text = "This has failed.";
            }

            _emailSender.SendSNSConfirmationToken(text);

            return new string[] { "111111111", "22222" };
        }

        [HttpGet("getPlayingNowVideoById/{id}")]
        public async Task<IActionResult> GetPlayingNowVideoById(string id)
        {
            var videoDto = await _videoAppService.GetPlayingNowVideoDto(id);
            var response = new BaseResponse<VideoPlayingNowDto>(videoDto);
            return Response.Ok(response);
        }

        [HttpGet("getWatchDtoById/{id}")]
        public async Task<IActionResult> GetWatchDtoById(string id)
        {
            var videoDto = await _videoAppService.GetWatchDto(id);

            // if the video has no encoded key (meaning it was not encoded), the endpoint returns null 
            if (videoDto.EncodedVideoKey == null)
            {
                videoDto = null;
            }

            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }

        [HttpGet("getUpdateDtoById/{id}")]
        public async Task<IActionResult> GetUpdateDtoById(string id)
        {
            var videoDto = await _videoAppService.GetUpdateDtoByIdAsync(id);
            var response = new BaseResponse<VideoUpdateDto>(videoDto);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] VideoCreateInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var videoId = await _videoAppService.CreateAsync(input, id);
            var response = new BaseResponse<IdResponse>(new IdResponse() { Id = videoId });
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPatch("update")]
        public async Task<IActionResult> Update([FromBody] VideoUpdateInput input)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var videoUpdated = await _videoAppService.UpdateAsync(input);
            var response = new BaseResponse(videoUpdated);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpDelete("delete/{videoId}")]
        public async Task<IActionResult> Delete(string videoId)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var doesSeriesBelongToUser = await _videoAppService.DoesVideoBelongToUser(videoId, userId);
            if (!doesSeriesBelongToUser)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var videoDeleted = await _videoAppService.DeleteAsync(videoId);
            var response = new BaseResponse(videoDeleted);
            return Response.Ok(response);
        }

        #endregion

        #region Videos for Authenticated Producer 

        [Authorize]
        [HttpGet("getMostRecentByProducer")]
        public async Task<IActionResult> GetMostRecentByProducer()
        {
            var userType = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserType)?.Value;
            if (userType == null || userType != "producer")
            {
                throw new HttpStatusCodeException(500, new List<string> { "Pentru aceasta actiune trebuie sa fii producator." });
            }
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var videos = await _videoAppService.GetMostRecentByProducerAsync(id);
            var response = new BaseResponse<List<VideoDisplayCarouselDto>>(videos);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpGet("getMostViewedByProducer")]
        public async Task<IActionResult> GetMostViewedByProducer()
        {
            var userType = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserType)?.Value;
            if (userType == null || userType != "producer")
            {
                throw new HttpStatusCodeException(500, new List<string> { "Pentru aceasta actiune trebuie sa fii producator." });
            }
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var videos = await _videoAppService.GetMostViewedByProducerAsync(id);
            var response = new BaseResponse<List<VideoDisplayCarouselDto>>(videos);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("getAllProducerPaginated")]
        public async Task<IActionResult> GetAllProducerPaginated([FromBody] AllVideoPaginateInput input)
        {
            var userType = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserType)?.Value;
            if (userType == null || userType != "producer")
            {
                throw new HttpStatusCodeException(500, new List<string> { "Pentru aceasta actiune trebuie sa fii producator." });
            }
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var videos = await _videoAppService.GetAllUserPaginatedAsync(id, input);
            var response = new BaseResponse<List<VideoDisplayUiDto>>(videos);
            return Response.Ok(response);
        }

        #endregion 

        #region Videos for Explore and Search 

        [HttpPost("searchVideos")]
        public async Task<IActionResult> SearchVideos([FromBody] SearchVideoInput input)
        {
            var videos = await _videoAppService.SearchVideoAsync(input.Word);
            var response = new BaseResponse<List<VideoDisplayUiDto>>(videos);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverVideos/{channelName}")]
        public async Task<IActionResult> GetDiscoverVideos(string channelName)
        {
            var videos = await _videoAppService.GetDiscoverVideoAsync(channelName, 0, 26, 0 , 26);
            var response = new BaseResponse<List<VideoDisplayCarouselDto>>(videos);
            return Response.Ok(response);
        }

        [HttpGet("getAllDiscoverVideos")]
        public async Task<IActionResult> GetAllDiscoverVideos()
        {
            var list = await _videoAppService.GetAllDiscoverVideoAsync();
            var response = new BaseResponse<List<Tuple<string, List<VideoDisplayCarouselDto>>>>(list);
            return Response.Ok(response);
        }

        [HttpGet("getAllDiscoverFirstCategories")]
        public async Task<IActionResult> GetAllDiscoverFirstCategories()
        {
            var list = await _videoAppService.GetAllDiscoverFirstCategoriesAsync();
            var response = new BaseResponse<List<Tuple<string, List<VideoDisplayCarouselDto>>>>(list);
            return Response.Ok(response);
        }

        [HttpGet("GetAllDiscoverRestOfCategories")]
        public async Task<IActionResult> GetAllDiscoverRestOfCategories()
        {
            var list = await _videoAppService.GetAllDiscoverRestOfCategoriesAsync();
            var response = new BaseResponse<List<Tuple<string, List<VideoDisplayCarouselDto>>>>(list);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverRestOfVideosFromCategory/{channelName}")]
        public async Task<IActionResult> GetDiscoverRestOfVideosFromCategory(string channelName)
        {
            var videos = await _videoAppService.GetDiscoverRestOfVideosFromCategoryAsync(channelName);
            var response = new BaseResponse<List<VideoDisplayCarouselDto>>(videos);
            return Response.Ok(response);
        }

        #endregion

        #region Videos for Explore and Search - FOR PER CATEGORIES 

        [HttpGet("getDiscoverExploreVideosByCategory/{category}")]
        public async Task<IActionResult> GetDiscoverExploreVideosByCategory(string category)
        {
            var videos = await _videoAppService.GetDiscoverVideoAsyncForList(category, 0, 15, 0, 15);
            var response = new BaseResponse<List<VideoDisplayUiDto>>(videos);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverExploreMostRecentVideosByCategory/{category}")]
        public async Task<IActionResult> GetDiscoverExploreMostRecentVideosByCategory(string category)
        {
            var videos = await _videoAppService.GetMostRecentVideosCompleteAsyncForList(category, 0, 25);
            var response = new BaseResponse<List<VideoDisplayUiDto>>(videos);
            return Response.Ok(response);
        }

        [HttpGet("getDiscoverExploreMostViewedVideosByCategory/{category}")]
        public async Task<IActionResult> GetDiscoverExploreMostViewedVideosByCategory(string category)
        {
            var videos = await _videoAppService.GetMostPopularVideosCompleteAsyncForList(category, 0, 25);
            var response = new BaseResponse<List<VideoDisplayUiDto>>(videos);
            return Response.Ok(response);
        }

        #endregion

        #region Videos for series page

        [HttpGet("getMostViewedBySeries/{seriesId}")]
        public async Task<IActionResult> GetMostViewedBySeries(string seriesId)
        {
            var videos = await _videoAppService.GetMostViewedBySeriesAsync(seriesId);
            var response = new BaseResponse<List<VideoDisplayCarouselDto>>(videos);
            return Response.Ok(response);
        }

        [HttpPost("getAllSeriesPaginated/{seriesId}")]
        public async Task<IActionResult> GetAllSeriesPaginated(string seriesId, [FromBody] AllVideoPaginateInput input)
        {
            var videos = await _videoAppService.GetAllSeriesPaginatedAsync(seriesId, input);
            var response = new BaseResponse<List<VideoDisplayUiDto>>(videos);
            return Response.Ok(response);
        }

        #endregion

        #region Videos for producer's page

        [HttpGet("getMostViewedByOtherProducer/{producerId}")]
        public async Task<IActionResult> GetMostViewedByOtherProducer(string producerId)
        {
            if (producerId == null)
            {
                throw new HttpStatusCodeException(404, new List<string> { "Id-ul producatorului este necesar." });
            }

            var videos = await _videoAppService.GetMostViewedByProducerAsync(producerId);
            var response = new BaseResponse<List<VideoDisplayCarouselDto>>(videos);
            return Response.Ok(response);
        }

        [HttpPost("getAllOtherProducerPaginated/{producerId}")]
        public async Task<IActionResult> GetAllOtherProducerPaginated(string producerId, [FromBody] AllVideoPaginateInput input)
        {
            if (producerId == null)
            {
                throw new HttpStatusCodeException(404, new List<string> { "Id-ul producatorului este necesar." });
            }

            var videos = await _videoAppService.GetAllUserPaginatedAsync(producerId, input);
            var response = new BaseResponse<List<VideoDisplayUiDto>>(videos);
            return Response.Ok(response);
        }

        #endregion

        #region Count Views

        [HttpPost("incrementViewsMobile")]
        public async Task<IActionResult> IncrementViewsMobile([FromBody] CountViewsMobileInput input)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var incrementResponse = await _viewAppService.IncrementVideoViewsIfNecessaryAsync(userId, input.VideoId);

            return Response.Ok(new BaseResponse(incrementResponse));
        }

        [HttpPost("ping")]
        public async Task<IActionResult> IncrementViews([FromBody] PingInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Ex) || string.IsNullOrWhiteSpace(input.Y))
            {
                return Response.ServerError(new BaseResponse(false));
            }

            var key = Encoding.ASCII.GetBytes(_appGeneralSettings.Value.Secret);

            var validationParameters = new TokenValidationParameters
            {
                // Clock skew compensates for server time drift.
                // We recommend 5 minutes or less:
                ClockSkew = TimeSpan.FromMinutes(5),
                // Specify the key used to sign the token:
                IssuerSigningKeys = new List<SecurityKey> { new SymmetricSecurityKey(key) },
                RequireSignedTokens = true,
                // Ensure the token hasn't expired:
                RequireExpirationTime = false,
                ValidateLifetime = false,
                // Ensure the token audience matches our audience value (default true):
                ValidateAudience = true,
                ValidAudience = _appGeneralSettings.Value.Audience,
                // Ensure the token was issued by a trusted authorization server (default true):
                ValidateIssuer = true,
                ValidIssuer = _appGeneralSettings.Value.Issuer
            };

            var claimsPrincipal = new ClaimsPrincipal();
            SecurityToken rawValidatedToken;
            try
            {
                claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(input.Y, validationParameters, out rawValidatedToken);
                var sessionId = claimsPrincipal.Claims.ToList().FirstOrDefault(a => a.Type == AppClaims.UserUI)?.Value;

                if(sessionId == null)
                {
                    return Response.ServerError(new BaseResponse(false));
                }

                var incrementResponse = await _viewAppService.IncrementVideoViewsIfNecessaryAsync(sessionId, input.Ex);
            }
            catch (Exception ex)
            {
                return Response.ServerError(new BaseResponse(false));
            }

            return Response.Ok(new BaseResponse(true));
        }

        #endregion

        #region Report Video

        [HttpPost("reportVideo")]
        public async Task<IActionResult> ReportVideo([FromBody] ReportVideoInput input)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (input.VideoId.Length > 50)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var emailSent = _emailSender.SendAdminEmailAboutAVideoReport(input.VideoId, userId, input.Reason);

            return Response.Ok(new BaseResponse(emailSent));
        }

        #endregion
    }
}
