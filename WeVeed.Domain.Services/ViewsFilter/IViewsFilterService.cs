using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeVeed.Domain.Services
{
    public interface IViewsFilterService
    {
        Task AddViewsFilterAsync(string videoId, string seriesId, string producerId, string seriesCategory);

        Task<List<string>> GetMostViewedSeriesIdsWeekly();

        Task<List<string>> GetMostViewedSeriesIdsMonthly();

        Task<List<string>> GetMostViewedProducersIdsWeekly();

        Task<List<string>> GetMostViewedProducersIdsMonthly();
    }
}
