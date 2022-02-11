using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeVeed.Domain.Services
{
    public interface IFollowService
    {
        Task<bool> AddFollowToSeriesThisWeek(string seriesId, string producerId, string currentUserId, string seriesCategory);

        Task<bool> AddFollowToSeriesThisMonth(string seriesId, string producerId, string currentUserId, string seriesCategory);

        Task<List<string>> GetTopSeriesIdsWeekly();

        Task<List<string>> GetTopSeriesIdsMonthly();

        Task<List<string>> GetTopProducersIdsWeekly();

        Task<List<string>> GetTopProducersIdsMonthly();
    }
}
