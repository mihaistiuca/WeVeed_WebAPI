using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services
{
    public interface ISeriesAppService
    {
        Task<List<ProducerSeriesDto>> SearchSeriesAsync(string word);

        Task<List<ProducerSeriesDto>> GetDiscoverSeriesAsync(string category = null);

        Task<SeriesUpdateDto> GetUpdateDtoByIdAsync(string id);

        Task<SeriesViewDto> GetViewByIdAsync(string id);

        Task<List<SeriesViewListDto>> GetMyFollowedSeries(string userId);

        Task<List<ProducerSeriesDto>> GetAllByProducer(string producerId);

        Task<List<SeriesLastEpisodeDto>> GetAllWithLastEpisode(string producerId);

        Task<SeriesLastEpisodeDto> GetSeriesWithLastEpisode(string seriesId, string userId);

        Task<bool> CreateAsync(string currentUserId, SeriesCreateInput input);

        Task<bool> UpdateAsync(string id, SeriesUpdateInput input);

        Task<bool> DeleteAsync(string id);

        Task<bool> IsSeriesNameUnique(string seriesName, string seriesId, string userId);

        Task<bool> DoesSeriesBelongToUser(string seriesId, string userId);

        Task<bool> IsSeasonEpisodeCombinationValid(string seriesId, int season, int episode);

        Task<bool> FollowSeriesAsync(string userId, SeriesFollowInput input);

        Task<bool> UnFollowSeriesAsync(string userId, SeriesFollowInput input);

        Task<List<string>> GetMyFollowedSeriesIds(string userId);

        Task<List<ProducerSeriesDto>> GetDiscoverSeriesFollowedWeekly();

        Task<List<ProducerSeriesDto>> GetDiscoverSeriesFollowedMonthly();

        Task<List<ProducerSeriesDto>> GetDiscoverSeriesFollowedAllTime(string category = null);

        Task<List<ProducerSeriesDto>> GetDiscoverMostRecentSeries(string category = null);

        Task<List<ProducerSeriesDto>> GetDiscoverSeriesMostViewedWeekly();

        Task<List<ProducerSeriesDto>> GetDiscoverSeriesMostViewedMonthly();

        Task<List<ProducerSeriesDto>> GetDiscoverSeriesMostViewedAllTime(string category = null);
    }
}
