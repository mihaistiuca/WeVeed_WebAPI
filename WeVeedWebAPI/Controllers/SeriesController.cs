using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Services;
using Microsoft.AspNetCore.Mvc;
using WeVeed.Application.Dtos;
using Resources.Base.Responses;
using WeVeedWebAPI.Extensions;
using System.Linq;
using Resources.Base.AuthUtils;
using Microsoft.AspNetCore.Authorization;
using Resources.Base.Exception;

namespace WeVeedWebAPI.Controllers
{
    [Route("[controller]")]
    public class SeriesController : Controller
    {
        private readonly ISeriesAppService _seriesAppService;

        public SeriesController(ISeriesAppService seriesAppService)
        {
            _seriesAppService = seriesAppService;
        }

        #region Series Follow 

        [Authorize]
        [HttpPost("followSeries")]
        public async Task<IActionResult> FollowSeries([FromBody] SeriesFollowInput input)
        {
            var currentUserId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (currentUserId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var followSeriesSucceeded = await _seriesAppService.FollowSeriesAsync(currentUserId, input);
            if (followSeriesSucceeded)
            {
                return Response.Ok(new BaseResponse(true));
            }
            else
            {
                return Response.ServerError(new BaseResponse(false));
            }
        }

        [Authorize]
        [HttpPost("unfollowSeries")]
        public async Task<IActionResult> UnFollowSeries([FromBody] SeriesFollowInput input)
        {
            var currentUserId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (currentUserId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var unFollowSeriesSucceeded = await _seriesAppService.UnFollowSeriesAsync(currentUserId, input);
            if (unFollowSeriesSucceeded)
            {
                return Response.Ok(new BaseResponse(true));
            }
            else
            {
                return Response.ServerError(new BaseResponse(false));
            }
        }

        #endregion

        #region Series Search and Explore 

        [HttpPost("searchSeries")]
        public async Task<IActionResult> SearchSeries([FromBody] SearchProducerInput input)
        {
            var series = await _seriesAppService.SearchSeriesAsync(input.Word);
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // recomandate 
        [HttpGet("getDiscoverSeries")]
        public async Task<IActionResult> GetDiscoverSeries()
        {
            var series = await _seriesAppService.GetDiscoverSeriesAsync();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // top urmarite ultima saptamana 
        [HttpGet("getDiscoverSeriesFollowedWeekly")]
        public async Task<IActionResult> GetDiscoverSeriesFollowedWeekly()
        {
            var series = await _seriesAppService.GetDiscoverSeriesFollowedWeekly();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // top urmarite ultima luna 
        [HttpGet("getDiscoverSeriesFollowedMonthly")]
        public async Task<IActionResult> GetDiscoverSeriesFollowedMontly()
        {
            var series = await _seriesAppService.GetDiscoverSeriesFollowedMonthly();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }
        
        // top urmarite all time 
        [HttpGet("getDiscoverSeriesFollowedAllTime")]
        public async Task<IActionResult> GetDiscoverSeriesFollowedAllTime()
        {
            var series = await _seriesAppService.GetDiscoverSeriesFollowedAllTime();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // cele mai noi 
        [HttpGet("getDiscoverMostRecentSeries")]
        public async Task<IActionResult> GetDiscoverMostRecentSeries()
        {
            var series = await _seriesAppService.GetDiscoverMostRecentSeries();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // cele mai vizionate ultima saptamana 
        [HttpGet("getDiscoverSeriesMostViewedWeekly")]
        public async Task<IActionResult> GetDiscoverSeriesMostViewedWeekly()
        {
            var series = await _seriesAppService.GetDiscoverSeriesMostViewedWeekly();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // cele mai vizionate ultima luna 
        [HttpGet("getDiscoverSeriesMostViewedMonthly")]
        public async Task<IActionResult> GetDiscoverSeriesMostViewedMonthly()
        {
            var series = await _seriesAppService.GetDiscoverSeriesMostViewedMonthly();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // cele mai vizionate all time 
        [HttpGet("getDiscoverSeriesMostViewedAllTime")]
        public async Task<IActionResult> GetDiscoverSeriesMostViewedAllTime()
        {
            var series = await _seriesAppService.GetDiscoverSeriesMostViewedAllTime();
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        #region Series Search and Explore - FOR PER CATEGORIES 

        // recomandate per categorie 
        [HttpGet("getDiscoverVideosByCategory/{category}")]
        public async Task<IActionResult> GetDiscoverSeriesByCategory(string category)
        {
            var series = await _seriesAppService.GetDiscoverSeriesAsync(category);
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // cele mai noi per categorie 
        [HttpGet("getDiscoverMostRecentSeriesByCategory/{category}")]
        public async Task<IActionResult> GetDiscoverMostRecentSeriesByCategory(string category)
        {
            var series = await _seriesAppService.GetDiscoverMostRecentSeries(category);
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // top urmarite all time per categorie 
        [HttpGet("getDiscoverSeriesFollowedAllTimeByCategory/{category}")]
        public async Task<IActionResult> GetDiscoverSeriesFollowedAllTimeByCategory(string category)
        {
            var series = await _seriesAppService.GetDiscoverSeriesFollowedAllTime(category);
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        // cele mai vizionate all time per categorie 
        [HttpGet("getDiscoverSeriesMostViewedAllTimeByCategory/{category}")]
        public async Task<IActionResult> GetDiscoverSeriesMostViewedAllTimeByCategory(string category)
        {
            var series = await _seriesAppService.GetDiscoverSeriesMostViewedAllTime(category);
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        #endregion

        #endregion

        #region Series For Auth Producer's Page 

        [Authorize]
        [HttpGet("getAllProducer")]
        public async Task<IActionResult> GetAllProducer()
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

            var series = await _seriesAppService.GetAllByProducer(id);
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        #endregion 

        #region Series For Other Producer's Page 

        [HttpGet("getAllOtherProducer/{producerId}")]
        public async Task<IActionResult> GetAllOtherProducer(string producerId)
        {
            if (string.IsNullOrWhiteSpace(producerId))
            {
                throw new HttpStatusCodeException(404, new List<string> { "Id-ul producatorului este necesar." });
            }

            var series = await _seriesAppService.GetAllByProducer(producerId);
            var response = new BaseResponse<List<ProducerSeriesDto>>(series);
            return Response.Ok(response);
        }

        #endregion

        #region Series for Add Video Page 

        [Authorize]
        [HttpGet("getAllWithLastEpisodes")]
        public async Task<IActionResult> GetAllWithLastEpisodes()
        {
            var userType = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserType)?.Value;
            if (userType == null || userType != "producer")
            {
                throw new HttpStatusCodeException(401, new List<string> { "Pentru aceasta actiune trebuie sa fii producator." });
            }
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var series = await _seriesAppService.GetAllWithLastEpisode(id);
            var response = new BaseResponse<List<SeriesLastEpisodeDto>>(series);
            return Response.Ok(response);
        }


        [Authorize]
        [HttpGet("seriesExistWithLastEpisodes/{seriesId}")]
        public async Task<IActionResult> SeriesExistWithLastEpisodes(string seriesId)
        {
            var userType = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserType)?.Value;
            if (userType == null || userType != "producer")
            {
                throw new HttpStatusCodeException(401, new List<string> { "Pentru aceasta actiune trebuie sa fii producator." });
            }
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var series = await _seriesAppService.GetSeriesWithLastEpisode(seriesId, id);
            var response = new BaseResponse<SeriesLastEpisodeDto>(series);
            return Response.Ok(response);
        }

        #endregion

        #region Series CRUD 

        // this is not authorized. The series could be viewed by anyone.
        [HttpGet("getViewById/{id}")]
        public async Task<IActionResult> GetViewById(string id)
        {
            var seriesDto = await _seriesAppService.GetViewByIdAsync(id);
            var response = new BaseResponse<SeriesViewDto>(seriesDto);
            return Response.Ok(response);
        }

        [HttpGet("getUpdateDtoById/{id}")]
        public async Task<IActionResult> GetUpdateDtoById(string id)
        {
            var seriesDto = await _seriesAppService.GetUpdateDtoByIdAsync(id);
            var response = new BaseResponse<SeriesUpdateDto>(seriesDto);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpGet("getMyFollowedSeries")]
        public async Task<IActionResult> GetMyFollowedSeries()
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ServerError(new BaseResponse(false));
            }

            var series = await _seriesAppService.GetMyFollowedSeries(id);
            var response = new BaseResponse<List<SeriesViewListDto>>(series);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SeriesCreateInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var seriesCreated = await _seriesAppService.CreateAsync(id, input);
            var response = new BaseResponse(seriesCreated);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPatch("update")]
        public async Task<IActionResult> Update([FromBody] SeriesUpdateInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var seriesUpdated = await _seriesAppService.UpdateAsync(id, input);
            var response = new BaseResponse(seriesUpdated);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpDelete("delete/{seriesId}")]
        public async Task<IActionResult> Delete(string seriesId)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var doesSeriesBelongToUser = await _seriesAppService.DoesSeriesBelongToUser(seriesId, userId);
            if (!doesSeriesBelongToUser)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var seriesDeleted = await _seriesAppService.DeleteAsync(seriesId);
            var response = new BaseResponse(seriesDeleted);
            return Response.Ok(response);
        }

        #endregion 

        #region Series Unicity Validators 

        [Authorize]
        [HttpPost("isSeriesNameUniqueCreate")]
        public async Task<IActionResult> IsSeriesNameUniqueCreate([FromBody] IsSeriesNameUniqueCreateInput input)
        {
            return Response.Ok(new BaseResponse(true));
        }

        [Authorize]
        [HttpPost("isSeriesNameUniqueUpdate")]
        public async Task<IActionResult> IsSeriesNameUniqueUpdate([FromBody] IsSeriesNameUniqueUpdateInput input)
        {
            return Response.Ok(new BaseResponse(true));
        }

        #endregion
    }   
}
