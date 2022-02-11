using System;
using System.Threading.Tasks;
using WeVeed.Domain.Services;

namespace WeVeed.Application.Services.View
{
    public class ViewAppService : IViewAppService
    {
        private readonly IViewService _viewService;
        private readonly IVideoService _videoService;
        private readonly IViewsFilterService _viewsFilterService;
        private readonly IUserService _userService;
        private readonly ISeriesService _seriesService;
        const int MinutesToIncrementViews = 10;

        public ViewAppService(IViewService viewService, IVideoService videoService, IViewsFilterService viewsFilterService,
            IUserService userService, ISeriesService seriesService)
        {
            _viewService = viewService;
            _videoService = videoService;
            _viewsFilterService = viewsFilterService;
            _userService = userService;
            _seriesService = seriesService;
        }

        public async Task<bool> IncrementVideoViewsIfNecessaryAsync(string sessionId, string videoId)
        {
            var view = await _viewService.GetBySessionAndVideoIdAsync(sessionId, videoId);
            var video = await _videoService.GetByIdAsync(videoId);

            if(video == null)
            {
                return false;
            }

            if(view == null)
            {
                // video ul nu a fost urmarit de acest user, deci trebuie creat un view
                // de asemenea, nr de viz al vid trebuie incrementat 
                var incrementRespone = await _videoService.IncrementViewsAsync(videoId);

                // IMPORTANT FOR VIEWS FILTERING FOR VIDEO/SERIES/PRODUCER
                // if the views are incremented for the video, you also have to add in the "viewfilter" table 
                await _viewsFilterService.AddViewsFilterAsync(videoId, video.SeriesId, video.UserProducerId, video.SeriesCategory);
                // also, increment the views on user and on series
                await _userService.IncrementProducerViewsCount(video.UserProducerId);
                await _seriesService.IncrementSeriesViewsCount(video.SeriesId);

                if (incrementRespone)
                {
                    var createResponse = await _viewService.CreateAsync(sessionId, videoId, video.SeriesCategory);
                }
                else
                {
                    return false;
                }

                return true;
            }
            else
            {
                // video ul a mai fost vazut de acest user 
                // - daca au trecut mai mult de X minute de atunci, incrementeaza video viz si updateaza ultima data cand userul a viz vid 
                // - daca nu, nu face nimic 
                if((DateTime.Now.ToUniversalTime() - view.ViewTime).TotalMinutes > MinutesToIncrementViews)
                {
                    var incrementRespone = await _videoService.IncrementViewsAsync(videoId);

                    // IMPORTANT FOR VIEWS FILTERING FOR VIDEO/SERIES/PRODUCER
                    // if the views are incremented for the video, you also have to add in the "viewfilter" table 
                    await _viewsFilterService.AddViewsFilterAsync(videoId, video.SeriesId, video.UserProducerId, video.SeriesCategory);
                    // also, increment the views on user and on series
                    await _userService.IncrementProducerViewsCount(video.UserProducerId);
                    await _seriesService.IncrementSeriesViewsCount(video.SeriesId);

                    if (incrementRespone)
                    {
                        var createResponse = await _viewService.UpdateViewTimeAsync(sessionId, videoId);
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
