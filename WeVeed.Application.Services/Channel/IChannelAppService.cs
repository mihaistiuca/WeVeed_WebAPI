using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services
{
    public interface IChannelAppService
    {
        Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> Get2EpisodesForEachChannelAsync(string userId);

        Task<List<VideoPlayingNowDto>> GetPlayingNowVideoListAsync(GetChannelPlayingNowVideoListInput input);

        Task<VideoWatchDto> GetChannelCurrentVideo(GetChannelVideoInput input);

        Task<VideoWatchDto> GetChannelNextVideo(GetChannelNextVideoInput input);

        Task<VideoWatchDto> GetChannelPreviousVideo(GetChannelNextVideoInput input);

        Task<VideoWatchDto> GetMyChannelVideo(string userId, GetMyChannelVideoInput input);

        Task<VideoWatchDto> GetMyChannelNextVideo(string userId, GetMyChannelNextVideoInput input);

        Task<VideoWatchDto> GetMyChannelPreviousVideo(string userId, GetMyChannelNextVideoInput input);

        Task<List<VideoPlayingNowDto>> GetMyChannelPlayingNowVideoListAsync(string userId, GetMyChannelPlayingNowVideoListInput input);
    }
}
