using System.Threading.Tasks;

namespace WeVeed.Application.Services.View
{
    public interface IViewAppService
    {
        Task<bool> IncrementVideoViewsIfNecessaryAsync(string sessionId, string videoId);
    }
}
