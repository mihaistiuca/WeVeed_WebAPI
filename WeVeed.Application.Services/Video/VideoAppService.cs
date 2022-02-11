using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;
using WeVeed.Domain.Services;

namespace WeVeed.Application.Services.Video
{
    public class VideoAppService : IVideoAppService
    {
        private readonly IVideoService _videoService;
        private readonly ISeriesService _seriesService;
        private readonly IChannelService _channelService;
        private readonly IUserService _userService;
        private static Random rng = new Random();

        const int FirstChannelsCount = 4;
        const int FirstVideosInDiscoverChannelCount = 3;
        const int RestOfVideosInDiscoverChannelCount = 20;

        public VideoAppService(IVideoService videoService, ISeriesService seriesService, IChannelService channelService, IUserService userService)
        {
            _videoService = videoService;
            _seriesService = seriesService;
            _channelService = channelService;
            _userService = userService;
        }

        #region All

        public List<VideoDisplayCarouselDto> Shuffle(List<VideoDisplayCarouselDto> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                VideoDisplayCarouselDto value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public List<VideoDisplayUiDto> ShuffleForList(List<VideoDisplayUiDto> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                VideoDisplayUiDto value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetAllByIdsList(List<string> idsList)
        {
            var videos = await _videoService.GetAllByIdsList(idsList);
            var videosDtos = videos.Select(a => Mapper.Map<VideoDisplayCarouselDto>(a)).ToList();

            foreach (var a in videosDtos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
            }

            return videosDtos;
        }

        public async Task<VideoWatchDto> GetWatchDto(string id)
        {
            var video = await _videoService.GetByIdAsync(id);
            if (video == null)
            {
                return null;
            }

            var videoDto = Mapper.Map<VideoWatchDto>(video);

            if (!string.IsNullOrWhiteSpace(video.SeriesId))
            {
                var series = await _seriesService.GetByIdAsync(video.SeriesId);

                if (series != null)
                {
                    videoDto.SeriesId = video.SeriesId;
                    videoDto.SeriesName = series.Name;
                    videoDto.SeriesThumbnail = series.ThumbnailUrl;
                }
            }

            if (!string.IsNullOrWhiteSpace(video.UserProducerId))
            {
                var producer = await _userService.GetByIdAsync(video.UserProducerId);
                if (producer != null)
                {
                    videoDto.ProducerId = video.UserProducerId;
                    videoDto.ProducerName = producer.ProducerName;
                    videoDto.ProducerThumbnail = producer.ProfileImageUrl;
                }
            }

            videoDto.VideoUrl = $"{WeVeedConstants.VideosInitialPath}/{videoDto.EncodedVideoKey}.mp4";

            return videoDto;
        }

        public async Task<VideoPlayingNowDto> GetPlayingNowVideoDto(string id)
        {
            var video = await _videoService.GetByIdAsync(id);
            if (video == null)
            {
                return null;
            }

            var videoDto = Mapper.Map<VideoPlayingNowDto>(video);

            if (!string.IsNullOrWhiteSpace(video.SeriesId))
            {
                var series = await _seriesService.GetByIdAsync(video.SeriesId);

                if (series != null)
                {
                    videoDto.SeriesId = video.SeriesId;
                    videoDto.SeriesName = series.Name;
                    videoDto.SeriesThumbnail = series.ThumbnailUrl;
                }
            }

            if (!string.IsNullOrWhiteSpace(video.UserProducerId))
            {
                var producer = await _userService.GetByIdAsync(video.UserProducerId);
                if (producer != null)
                {
                    videoDto.ProducerId = video.UserProducerId;
                    videoDto.ProducerName = producer.ProducerName;
                    videoDto.ProducerThumbnail = producer.ProfileImageUrl;
                }
            }

            return videoDto;
        }

        public async Task<VideoUpdateDto> GetUpdateDtoByIdAsync(string id)
        {
            var video = await _videoService.GetByIdAsync(id);
            if (video == null)
            {
                return null;
            }

            return Mapper.Map<VideoUpdateDto>(video);
        }

        public async Task<string> CreateAsync(VideoCreateInput input, string userId)
        {
            var producer = await _userService.GetByIdAsync(userId);
            var id = await _videoService.CreateAsync(input, userId, producer.IsProducerValidatedByAdmin);

            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(input.SeriesId) && input.Episode.HasValue && input.Season.HasValue)
            {
                await _seriesService.UpdateSeasonEpisodeAsync(input.SeriesId, input.Season.Value, input.Episode.Value);
            }

            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(input.SeriesId))
            {
                await _seriesService.IncrementSeriesVideosCount(input.SeriesId);
            }

            // THIS SECTION IS COMMENTED BECAUSE ADDING TO CHANNELS NOW HAPPENS AFTER THE ENCODING AND IT'S DONE BY THE INTEGRATOR API! 
            //if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(input.SeriesId))
            //{
            //    var series = await _seriesService.GetByIdAsync(input.SeriesId);
            //    if(series == null)
            //    {
            //        return id;
            //    }

            //    var category = series.Category;
            //    var removedVideoId = await _channelService.AddVideoInChannel(id, category);
            //    if(category != WeVeedConstants.KidsChannel && 
            //        category != WeVeedConstants.MusicChannel && 
            //        category != WeVeedConstants.VlogChannel &&
            //        category != WeVeedConstants.GamingChannel)
            //    {
            //        var removedVideoFromGeneralId = await _channelService.AddVideoInChannel(id, WeVeedConstants.GeneralChannel);
            //    }

            //    if (!string.IsNullOrWhiteSpace(removedVideoId))
            //    {
            //        // set removed from channel flag to true 
            //        await _videoService.UpdateRemovedFromChannelFlagAsync(removedVideoId);
            //    }
            //}

            return id;
        }

        public async Task<bool> UpdateAsync(VideoUpdateInput input)
        {
            return await _videoService.UpdateAsync(input);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var video = await _videoService.GetByIdAsync(id);
            if (video == null)
            {
                return false;
            }

            // delete video from the channels 
            await _channelService.DeleteVideoFromChannel(id, "general");
            await _channelService.DeleteVideoFromChannel(id, video.SeriesCategory);

            var result = await _videoService.DeleteAsync(id);
            if (result)
            {
                // here change last season and episode for the series 
                var series = await _seriesService.GetByIdAsync(video.SeriesId);
                if (video.Season == series.LastSeason && video.Episode == series.LastEpisode)
                {
                    var touple = await _videoService.GetLastEpisodeAndSeasonBySeries(video.SeriesId);
                    var updateResult = await _seriesService.UpdateSeasonEpisodeAsync(video.SeriesId, touple.Item1, touple.Item2);


                }

                await _seriesService.DecrementSeriesVideosCount(video.SeriesId);
                return true;
            }

            return result;
        }

        public async Task<List<VideoDisplayUiDto>> GetAllUserPaginatedAsync(string userId, AllVideoPaginateInput input)
        {
            var videos = await _videoService.GetAllUserPaginatedAsync(userId, input);
            var videosDtos = videos.Select(a => Mapper.Map<VideoDisplayUiDto>(a)).ToList();

            foreach (var a in videosDtos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return videosDtos;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetMostRecentByProducerAsync(string producerId)
        {
            var videos = await _videoService.GetMostRecentByProducerAsync(producerId);
            var videosDtos = videos.Select(a => Mapper.Map<VideoDisplayCarouselDto>(a)).ToList();

            foreach (var a in videosDtos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return videosDtos;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetMostViewedByProducerAsync(string producerId)
        {
            var videos = await _videoService.GetMostViewedByProducerAsync(producerId);
            var videosDtos = videos.Select(a => Mapper.Map<VideoDisplayCarouselDto>(a)).ToList();

            foreach (var a in videosDtos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return videosDtos;
        }

        public async Task<List<VideoDisplayUiDto>> GetAllSeriesPaginatedAsync(string seriesId, AllVideoPaginateInput input)
        {
            if (string.IsNullOrWhiteSpace(seriesId))
            {
                return new List<VideoDisplayUiDto>();
            }

            var videos = await _videoService.GetAllSeriesPaginatedAsync(seriesId, input);
            var videosDtos = videos.Select(a => Mapper.Map<VideoDisplayUiDto>(a)).ToList();

            foreach (var a in videosDtos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return videosDtos;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetMostViewedBySeriesAsync(string seriesId)
        {
            if (string.IsNullOrWhiteSpace(seriesId))
            {
                return new List<VideoDisplayCarouselDto>();
            }

            var videos = await _videoService.GetMostViewedBySeries(seriesId);
            var videosDtos = videos.Select(a => Mapper.Map<VideoDisplayCarouselDto>(a)).ToList();

            foreach (var a in videosDtos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return videosDtos;
        }

        public async Task<bool> DoesVideoBelongToUser(string videoId, string userId)
        {
            return await _videoService.DoesVideoBelongToUser(videoId, userId);
        }

        public async Task<List<string>> GetLastVideosBySeriesListAsync(List<string> seriesIds)
        {
            var entities = await _videoService.GetLastVideosBySeriesListAsync(seriesIds);
            if (entities != null)
            {
                return entities.Select(a => a.Id.ToString()).ToList();
            }

            return null;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetLastVideoDtosBySeriesListAsync(List<string> seriesIds)
        {
            var entities = await _videoService.GetLastVideosBySeriesListAsync(seriesIds);
            if (entities == null || !entities.Any())
            {
                return null;
            }

            var finalList = new List<VideoDisplayCarouselDto>();
            if (entities.Count <= 3)
            {
                finalList = entities.Select(a => Mapper.Map<VideoDisplayCarouselDto>(a)).ToList();
            }
            else
            {
                var firstElementIndex = new Random().Next(entities.Count - 1);
                var secondElementIndex = firstElementIndex == entities.Count - 1 ? 0 : firstElementIndex + 1;
                var thirdElementIndex = secondElementIndex == entities.Count - 1 ? 0 : secondElementIndex + 1;
                finalList.Add(Mapper.Map<VideoDisplayCarouselDto>(entities.ElementAt(firstElementIndex)));
                finalList.Add(Mapper.Map<VideoDisplayCarouselDto>(entities.ElementAt(secondElementIndex)));
                finalList.Add(Mapper.Map<VideoDisplayCarouselDto>(entities.ElementAt(thirdElementIndex)));
            }

            foreach (var a in finalList)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
            }

            return finalList;
        }

        #endregion

        #region Videos Explore and Search 

        public async Task<List<VideoDisplayUiDto>> SearchVideoAsync(string word)
        {
            var videos = await _videoService.SearchVideoAsync(word);
            if (videos == null)
            {
                return new List<VideoDisplayUiDto>();
            }

            var dtos = videos.Select(a => Mapper.Map<VideoDisplayUiDto>(a)).ToList();

            foreach (var a in dtos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            };

            return dtos;
        }

        public async Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> GetAllDiscoverVideoAsync()
        {
            var bigList = new List<Tuple<string, List<VideoDisplayCarouselDto>>>();

            foreach (var channelName in WeVeedConstants.CategoriesWithoutGeneral)
            {
                var videos = await GetDiscoverVideoAsync(channelName);
                bigList.Add(new Tuple<string, List<VideoDisplayCarouselDto>>(channelName, videos));
            }

            return bigList;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetDiscoverVideoAsync(string channelName, 
            int? skipRecent = null, int? limitRecent = null,
            int? skipPopular = null, int? limitPopular = null)
        {
            var popularVideos = await GetMostPopularVideosAsync(channelName, skipPopular, limitPopular);
            var recentVideos = await GetMostRecentVideosAsync(channelName, skipRecent, limitRecent);

            popularVideos.ForEach(a =>
            {
                if (!recentVideos.Select(e => e.Id).Contains(a.Id))
                {
                    recentVideos.Add(a);
                }
            });

            foreach (var a in recentVideos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return Shuffle(recentVideos);
        }

        public async Task<List<VideoDisplayCarouselDto>> GetMostPopularVideosAsync(string channelName, int? skipPopular = null, int? limitPopular = null)
        {
            var videos = await _videoService.GetMostPopularVideosAsync(channelName, skipPopular, limitPopular);
            if (videos == null)
            {
                return new List<VideoDisplayCarouselDto>();
            }

            return videos.Select(a => Mapper.Map<VideoDisplayCarouselDto>(a)).ToList();
        }

        public async Task<List<VideoDisplayCarouselDto>> GetMostRecentVideosAsync(string channelName, int? skipRecent = null, int? limitRecent = null)
        {
            var videos = await _videoService.GetMostRecentVideosAsync(channelName, skipRecent, limitRecent);
            if (videos == null)
            {
                return new List<VideoDisplayCarouselDto>();
            }

            return videos.Select(a => Mapper.Map<VideoDisplayCarouselDto>(a)).ToList();
        }

        #region FOR EXPLORE -> BY CATEGORIES 

        public async Task<List<VideoDisplayUiDto>> GetDiscoverVideoAsyncForList(string channelName,
            int? skipRecent = null, int? limitRecent = null,
            int? skipPopular = null, int? limitPopular = null)
        {
            var popularVideos = await GetMostPopularVideosAsyncForList(channelName, skipPopular, limitPopular);
            var recentVideos = await GetMostRecentVideosAsyncForList(channelName, skipRecent, limitRecent);

            popularVideos.ForEach(a =>
            {
                if (!recentVideos.Select(e => e.Id).Contains(a.Id))
                {
                    recentVideos.Add(a);
                }
            });

            foreach (var a in recentVideos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return ShuffleForList(recentVideos);
        }

        public async Task<List<VideoDisplayUiDto>> GetMostPopularVideosAsyncForList(string channelName, int? skipPopular = null, int? limitPopular = null)
        {
            var videos = await _videoService.GetMostPopularVideosAsync(channelName, skipPopular, limitPopular);
            if (videos == null)
            {
                return new List<VideoDisplayUiDto>();
            }

            return videos.Select(a => Mapper.Map<VideoDisplayUiDto>(a)).ToList();
        }

        public async Task<List<VideoDisplayUiDto>> GetMostRecentVideosAsyncForList(string channelName, int? skipRecent = null, int? limitRecent = null)
        {
            var videos = await _videoService.GetMostRecentVideosAsync(channelName, skipRecent, limitRecent);
            if (videos == null)
            {
                return new List<VideoDisplayUiDto>();
            }

            return videos.Select(a => Mapper.Map<VideoDisplayUiDto>(a)).ToList();
        }

        public async Task<List<VideoDisplayUiDto>> GetMostPopularVideosCompleteAsyncForList(string channelName, int? skipPopular = null, int? limitPopular = null)
        {
            var videos = await GetMostPopularVideosAsyncForList(channelName, skipPopular, limitPopular);
            if (videos == null)
            {
                return new List<VideoDisplayUiDto>();
            }

            foreach (var a in videos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return videos;
        }

        public async Task<List<VideoDisplayUiDto>> GetMostRecentVideosCompleteAsyncForList(string channelName, int? skipRecent = null, int? limitRecent = null)
        {
            var videos = await GetMostRecentVideosAsyncForList(channelName, skipRecent, limitRecent);
            if (videos == null)
            {
                return new List<VideoDisplayUiDto>();
            }

            foreach (var a in videos)
            {
                if (string.IsNullOrWhiteSpace(a.SeriesId))
                {
                    continue;
                }

                var series = await _seriesService.GetByIdAsync(a.SeriesId);

                if (series == null)
                {
                    continue;
                }

                a.SeriesName = series.Name;
                a.SeriesThumbnail = series.ThumbnailUrl;
            }

            return videos;
        }

        #endregion

        // **************************************************************************************************************************************************************
        public async Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> GetAllDiscoverFirstCategoriesAsync()
        {
            var bigList = new List<Tuple<string, List<VideoDisplayCarouselDto>>>();

            var firstCategories = WeVeedConstants.CategoriesWithoutGeneral.Take(FirstChannelsCount).ToList();

            foreach (var channelName in firstCategories)
            {
                var videos = await GetDiscoverVideoAsync(channelName, 0, FirstVideosInDiscoverChannelCount, 0, FirstVideosInDiscoverChannelCount);
                bigList.Add(new Tuple<string, List<VideoDisplayCarouselDto>>(channelName, videos));
            }

            return bigList;
        }

        public async Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> GetAllDiscoverRestOfCategoriesAsync()
        {
            var bigList = new List<Tuple<string, List<VideoDisplayCarouselDto>>>();

            var firstCategories = WeVeedConstants.CategoriesWithoutGeneral.Skip(FirstChannelsCount).ToList();

            foreach (var channelName in firstCategories)
            {
                var videos = await GetDiscoverVideoAsync(channelName, 0, FirstVideosInDiscoverChannelCount, 0, FirstVideosInDiscoverChannelCount);
                bigList.Add(new Tuple<string, List<VideoDisplayCarouselDto>>(channelName, videos));
            }

            return bigList;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetDiscoverRestOfVideosFromCategoryAsync(string channelName)
        {
            var videos = await GetDiscoverVideoAsync(channelName, 
                FirstVideosInDiscoverChannelCount, RestOfVideosInDiscoverChannelCount, 
                FirstVideosInDiscoverChannelCount, RestOfVideosInDiscoverChannelCount);

            var videosThatWereAlreadyInSlider = await GetDiscoverVideoAsync(channelName, 0, FirstVideosInDiscoverChannelCount, 0, FirstVideosInDiscoverChannelCount);

            foreach (var video in videosThatWereAlreadyInSlider)
            {
                if (videos.Any(a => a.Id == video.Id))
                {
                    videos.RemoveAll(a => a.Id == video.Id);
                }
            }

            return videos;
        }
        // **************************************************************************************************************************************************************

        #endregion 
    }
}
