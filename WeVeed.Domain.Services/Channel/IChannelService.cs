using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public interface IChannelService
    {
        Task<List<Channel>> GetAllChannels();

        Task<List<string>> GetChannelNextVideosForPlayingNow(GetChannelPlayingNowVideoListInput input);

        Task<string> AddVideoInChannel(string videoId, string channelName);

        Task AddNewChannel(string channelName);

        Task<string> GetChannelCurrentVideoId(GetChannelVideoInput input);

        Task<string> GetChannelNextVideoId(GetChannelNextVideoInput input);

        Task<string> GetChannelPreviousVideoId(GetChannelNextVideoInput input);

        Task DeleteVideoFromChannel(string videoId, string channelName);

        Task DeleteVideoListFromChannel(List<string> videoIds, string channelName);
    }
}
