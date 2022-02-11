using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public interface ISeriesService
    {
        Task<List<Series>> SearchSeriesAsync(string word);

        Task<List<Series>> GetMostPopularSeriesAsync(string category = null);

        Task<List<Series>> GetMostRecentSeriesAsync(string category = null);

        Task<List<Series>> GetAllByProducer(string producerId);

        Task<List<Series>> GetAllByIdsList(List<string> idsList, bool ignoreSeriesWithProducerNotValidated = false);

        Task<bool> CreateAsync(string userId, SeriesCreateInput input, bool isProducerValidatedByAdmin);

        Task<Series> GetByIdAsync(string id);

        Task<bool> UpdateSeasonEpisodeAsync(string seriesId, int season, int episode);

        Task<bool> UpdateAsync(string userId, SeriesUpdateInput input);

        Task<bool> DeleteAsync(string id);

        Task<int> CountProducerSeriesAsync(string producerId);

        Task<bool> IsSeriesNameUnique(string seriesName, string seriesId, string userId);

        Task<bool> DoesSeriesBelongToUser(string seriesId, string userId);

        Task<Series> GetSeriesVerifyUser(string seriesId, string userId);

        Task<bool> IsSeasonEpisodeCombinationValid(string seriesId, int season, int episode);

        Task<bool> IncrementSeriesFollowersCount(SeriesFollowInput input);

        Task<bool> DecrementSeriesFollowersCount(SeriesFollowInput input);

        Task<List<Series>> GetMostFollowedSeriesAsync(string category = null);

        Task<List<Series>> GetDiscoverMostRecentSeriesAsync(string category = null);

        Task<bool> IncrementSeriesViewsCount(string seriesId);

        Task<List<Series>> GetMostViewedSeriesAsync(string category = null);

        Task<bool> ValidateProducerSeriesByAdmin(ValidateProducerByAdminInput input);

        Task<bool> IncrementSeriesVideosCount(string seriesId);

        Task<bool> DecrementSeriesVideosCount(string seriesId);
    }
}
