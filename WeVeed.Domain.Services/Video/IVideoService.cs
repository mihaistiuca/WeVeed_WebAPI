using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public interface IVideoService
    {
        Task<List<Video>> GetAllByIdsList(List<string> idsList);

        Task<List<Video>> SearchVideoAsync(string word);

        Task<List<Video>> GetMostPopularVideosAsync(string channelName, int? skip = 0, int? limit = 20);

        Task<List<Video>> GetMostRecentVideosAsync(string channelName, int? skip = 0, int? limit = 10);

        Task<Video> GetByIdAsync(string id);

        Task<string> CreateAsync(VideoCreateInput input, string userId, bool isProducerValidatedByAdmin);

        Task<bool> UpdateAsync(VideoUpdateInput input);

        Task<bool> UpdateRemovedFromChannelFlagAsync(string videoId);

        Task<bool> DeleteAsync(string videoId);

        Task<bool> DeleteBySeriesAsync(string seriesId);

        Task<List<Video>> GetAllUserPaginatedAsync(string userId, AllVideoPaginateInput input);

        Task<List<Video>> GetAllSeriesPaginatedAsync(string seriesId, AllVideoPaginateInput input);

        Task<List<Video>> GetMostRecentByProducerAsync(string producerId);

        Task<List<Video>> GetMostViewedByProducerAsync(string producerId);

        Task<List<Video>> GetMostViewedBySeries(string seriesId);

        Task<int> CountVideosPerSeriesAsync(string seriesId);

        Task<bool> DoesVideoBelongToUser(string videoId, string userId);

        Task<Tuple<int, int>> GetLastEpisodeAndSeasonBySeries(string seriesId);

        Task<int> CountProducerEpisodesAsync(string producerId);

        Task<bool> IncrementViewsAsync(string videoId);

        Task<List<Video>> GetLastVideosBySeriesListAsync(List<string> seriesIds);

        Task<List<string>> GetAllIdsBySeries(string seriesId);

        Task<bool> ValidateProducerVideosByAdmin(ValidateProducerByAdminInput input);
    }
}
