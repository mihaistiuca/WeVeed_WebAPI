using Microsoft.AspNetCore.Mvc;
using Resources.Base.Responses;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Application.Services;
using WeVeedWebAPI.Extensions;

namespace WeVeedWebAPI.Controllers
{
    [Route("[controller]")]
    public class ExploreController : Controller
    {
        private readonly IChannelAppService _channelAppService;
        private readonly IUserAppService _userAppService;

        public ExploreController(IChannelAppService channelAppService, IUserAppService userAppService)
        {
            _channelAppService = channelAppService;
            _userAppService = userAppService;
        }

        [HttpPost("getChannelVideo")]
        public async Task<IActionResult> GetChannelVideo([FromBody] GetChannelVideoInput input)
        {
            var videoDto = await _channelAppService.GetChannelCurrentVideo(input);
            var response = new BaseResponse<VideoWatchDto>(videoDto);
            return Response.Ok(response);
        }
    }
}
