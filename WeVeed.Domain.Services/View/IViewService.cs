using System.Threading.Tasks;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public interface IViewService
    {
        Task<string> CreateAsync(string sessionId, string videoId, string seriesCategory);

        Task<View> GetBySessionAndVideoIdAsync(string sessionId, string videoId);

        Task<bool> UpdateViewTimeAsync(string sessionId, string videoId);
    }
}
