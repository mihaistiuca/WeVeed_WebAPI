using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources.Base.AuthUtils;
using Resources.Base.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Application.Services;
using WeVeedWebAPI.Extensions;

namespace WeVeedWebAPI.Controllers
{
    [Route("[controller]")]
    public class ChannelController : Controller
    {
        private readonly IChannelAppService _channelAppService;
        
        public ChannelController(IChannelAppService channelAppService)
        {
            _channelAppService = channelAppService;
        }

        [HttpPost("getPlayingNowVideoList")]
        public async Task<IActionResult> GetPlayingNowVideoList([FromBody] GetChannelPlayingNowVideoListInput input)
        {
            var videoDtos = await _channelAppService.GetPlayingNowVideoListAsync(input);
            var response = new BaseResponse<List<VideoPlayingNowDto>>(videoDtos);
            return Response.Ok(response);
        }

        [HttpPost("getChannelVideo")]
        public async Task<IActionResult> GetChannelVideo([FromBody] GetChannelVideoInput input)
        {
            var videoDto = await _channelAppService.GetChannelCurrentVideo(input);
            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }

        [HttpPost("getChannelNextVideo")]
        public async Task<IActionResult> GetChannelNextVideo([FromBody] GetChannelNextVideoInput input)
        {
            var videoDto = await _channelAppService.GetChannelNextVideo(input);
            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }

        [HttpPost("getChannelPreviousVideo")]
        public async Task<IActionResult> GetChannelPreviousVideo([FromBody] GetChannelNextVideoInput input)
        {
            var videoDto = await _channelAppService.GetChannelPreviousVideo(input);
            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("getMyChannelVideo")]
        public async Task<IActionResult> GetMyChannelVideo([FromBody] GetMyChannelVideoInput input)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var videoDto = await _channelAppService.GetMyChannelVideo(userId, input);
            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("getMyChannelNextVideo")]
        public async Task<IActionResult> GetMyChannelNextVideo([FromBody] GetMyChannelNextVideoInput input)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var videoDto = await _channelAppService.GetMyChannelNextVideo(userId, input);
            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("getMyChannelPreviousVideo")]
        public async Task<IActionResult> GetMyChannelPreviousVideo([FromBody] GetMyChannelNextVideoInput input)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var videoDto = await _channelAppService.GetMyChannelPreviousVideo(userId, input);
            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("getMyChannelPlayingNowVideoList")]
        public async Task<IActionResult> GetMyChannelPlayingNowVideoList([FromBody] GetMyChannelPlayingNowVideoListInput input)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var videoDtos = await _channelAppService.GetMyChannelPlayingNowVideoListAsync(userId, input);
            var response = new BaseResponse<List<VideoPlayingNowDto>>(videoDtos);
            return Response.Ok(response);
        }

        [HttpGet("get3EpisodesForEachChannel")]
        public async Task<IActionResult> Get3EpisodesForEachChannel()
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;

            var videoDtos = await _channelAppService.Get2EpisodesForEachChannelAsync(userId);
            var response = new BaseResponse<List<Tuple<string, List<VideoDisplayCarouselDto>>>>(videoDtos);
            return Response.Ok(response);
        }
    }
}
