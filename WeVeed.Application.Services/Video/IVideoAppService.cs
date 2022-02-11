using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Video
{
    public interface IVideoAppService
    {
        Task<List<VideoDisplayCarouselDto>> GetAllByIdsList(List<string> idsList);

        Task<List<VideoDisplayUiDto>> SearchVideoAsync(string word);

        Task<List<VideoDisplayCarouselDto>> GetDiscoverVideoAsync(string channelName,
            int? skipRecent = null, int? limitRecent = null,
            int? skipPopular = null, int? limitPopular = null);

        Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> GetAllDiscoverVideoAsync();

        Task<VideoWatchDto> GetWatchDto(string id);

        Task<VideoPlayingNowDto> GetPlayingNowVideoDto(string id);

        Task<VideoUpdateDto> GetUpdateDtoByIdAsync(string id);

        Task<string> CreateAsync(VideoCreateInput input, string userId);

        Task<bool> UpdateAsync(VideoUpdateInput input);

        Task<bool> DeleteAsync(string id);

        Task<bool> DoesVideoBelongToUser(string videoId, string userId);

        Task<List<VideoDisplayUiDto>> GetAllUserPaginatedAsync(string userId, AllVideoPaginateInput input);

        Task<List<VideoDisplayCarouselDto>> GetMostRecentByProducerAsync(string producerId);

        Task<List<VideoDisplayCarouselDto>> GetMostViewedByProducerAsync(string producerId);

        Task<List<VideoDisplayUiDto>> GetAllSeriesPaginatedAsync(string seriesId, AllVideoPaginateInput input);

        Task<List<VideoDisplayCarouselDto>> GetMostViewedBySeriesAsync(string seriesId);

        Task<List<string>> GetLastVideosBySeriesListAsync(List<string> seriesIds);

        Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> GetAllDiscoverFirstCategoriesAsync();

        Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> GetAllDiscoverRestOfCategoriesAsync();

        Task<List<VideoDisplayCarouselDto>> GetDiscoverRestOfVideosFromCategoryAsync(string channelName);

        Task<List<VideoDisplayCarouselDto>> GetMostRecentVideosAsync(string channelName, int? skipRecent = null, int? limitRecent = null);

        Task<List<VideoDisplayCarouselDto>> GetMostPopularVideosAsync(string channelName, int? skipPopular = null, int? limitPopular = null);

        Task<List<VideoDisplayUiDto>> GetDiscoverVideoAsyncForList(string channelName,
            int? skipRecent = null, int? limitRecent = null,
            int? skipPopular = null, int? limitPopular = null);

        Task<List<VideoDisplayUiDto>> GetMostPopularVideosCompleteAsyncForList(string channelName, int? skipPopular = null, int? limitPopular = null);

        Task<List<VideoDisplayUiDto>> GetMostRecentVideosCompleteAsyncForList(string channelName, int? skipRecent = null, int? limitRecent = null);

        Task<List<VideoDisplayCarouselDto>> GetLastVideoDtosBySeriesListAsync(List<string> seriesIds);
    }
}
