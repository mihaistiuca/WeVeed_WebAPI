using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Services;

namespace WeVeed.Application.Services
{
    public class SeriesAppService : ISeriesAppService
    {
        private readonly ISeriesService _seriesService;
        private readonly IUserService _userService;
        private readonly IVideoService _videoService;
        private readonly IChannelService _channelService;
        private readonly IFollowService _followService;
        private readonly IViewsFilterService _viewsFilterService;
        private static Random rng = new Random();

        public SeriesAppService(ISeriesService seriesService, IUserService userService, IFollowService followService,
            IVideoService videoService, IChannelService channelService, IViewsFilterService viewsFilterService)
        {
            _seriesService = seriesService;
            _userService = userService;
            _videoService = videoService;
            _channelService = channelService;
            _followService = followService;
            _viewsFilterService = viewsFilterService;
        }

        #region Series Follow 

        public async Task<bool> FollowSeriesAsync(string userId, SeriesFollowInput input)
        {
            // 0. verify that the series exist 
            var series = await _seriesService.GetByIdAsync(input.SeriesId);
            if(series == null)
            {
                return false;
            }

            // 1. add series in user's following list
            var addSeriesInUserFollowListResult = await _userService.AddSeriesInUserFollowedSeries(userId, input);
            if (!addSeriesInUserFollowListResult)
            {
                return false;
            }

            // 2. increment series followers count 
            var seriesFollowsIncrementResult = await _seriesService.IncrementSeriesFollowersCount(input);
            if (!seriesFollowsIncrementResult)
            {
                // should remove seriesId from user's following list
                await _userService.RemoveSeriesFromUserFollowedSeries(userId, input);
                return false;
            }

            // 3. increment producers series followers count 
            var producerFollowsIncrementResult = _userService.IncrementProducerSeriesFollowersCount(series.UserId);

            // 4. add a record in the weeklyfollow and monthlyfollow tables 
            await _followService.AddFollowToSeriesThisWeek(series.Id.ToString(), series.UserId, userId, series.Category);
            await _followService.AddFollowToSeriesThisMonth(series.Id.ToString(), series.UserId, userId, series.Category);

            // it does not matter so much if producers followers number has been incremented, return true
            return true;
        }

        public async Task<bool> UnFollowSeriesAsync(string userId, SeriesFollowInput input)
        {
            // 0. verify that the series exist 
            var series = await _seriesService.GetByIdAsync(input.SeriesId);
            if (series == null)
            {
                return false;
            }

            // 1. remove series from user's following list
            var removeSeriesFromUserFollowListResult = await _userService.RemoveSeriesFromUserFollowedSeries(userId, input);
            if (!removeSeriesFromUserFollowListResult)
            {
                return false;
            }

            // 2. decrement series followers count 
            var seriesFollowsDecrementResult = await _seriesService.DecrementSeriesFollowersCount(input);
            if (!seriesFollowsDecrementResult)
            {
                return false;
            }

            // 3. decrement producers series followers count 
            var producerFollowsDecrementResult = _userService.DecrementProducerSeriesFollowersCount(series.UserId);

            return true;
        }

        #endregion 

        #region Series Search and Explore

        public List<ProducerSeriesDto> Shuffle(List<ProducerSeriesDto> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                ProducerSeriesDto value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        // DONE
        public async Task<List<ProducerSeriesDto>> SearchSeriesAsync(string word)
        {
            var series = await _seriesService.SearchSeriesAsync(word);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var seriesDtos = new List<ProducerSeriesDto>();
            series.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        // DONE
        public async Task<List<ProducerSeriesDto>> GetDiscoverSeriesAsync(string category = null)
        {
            var popularSeries = await GetMostPopularSeriesAsync(category);
            var recentSeries = await GetMostRecentSeriesAsync(category);

            popularSeries.ForEach(a =>
            {
                if (!recentSeries.Select(e => e.Id).Contains(a.Id))
                {
                    a.IsNew = false;
                    recentSeries.Add(a);
                }
            });

            return Shuffle(recentSeries);
        }

        // DONE
        public async Task<List<ProducerSeriesDto>> GetMostPopularSeriesAsync(string category = null)
        {
            var series = await _seriesService.GetMostPopularSeriesAsync(category);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var seriesDtos = new List<ProducerSeriesDto>();
            series.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        // DONE
        public async Task<List<ProducerSeriesDto>> GetMostRecentSeriesAsync(string category = null)
        {
            var series = await _seriesService.GetMostRecentSeriesAsync(category);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var seriesDtos = new List<ProducerSeriesDto>();
            series.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        public async Task<List<ProducerSeriesDto>> GetDiscoverSeriesFollowedWeekly()
        {
            var seriesIds = await _followService.GetTopSeriesIdsWeekly();
            if (seriesIds == null || !seriesIds.Any())
            {
                return new List<ProducerSeriesDto>();
            }

            var series = await _seriesService.GetAllByIdsList(seriesIds, true);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var sorted = series.OrderBy(a => seriesIds.IndexOf(a.Id.ToString())).ToList();

            var seriesDtos = new List<ProducerSeriesDto>();
            sorted.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        public async Task<List<ProducerSeriesDto>> GetDiscoverSeriesFollowedMonthly()
        {
            var seriesIds = await _followService.GetTopSeriesIdsMonthly();
            if (seriesIds == null || !seriesIds.Any())
            {
                return new List<ProducerSeriesDto>();
            }

            var series = await _seriesService.GetAllByIdsList(seriesIds, true);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var sorted = series.OrderBy(a => seriesIds.IndexOf(a.Id.ToString())).ToList();

            var seriesDtos = new List<ProducerSeriesDto>();
            sorted.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        // DONE
        public async Task<List<ProducerSeriesDto>> GetDiscoverSeriesFollowedAllTime(string category = null)
        {
            var series = await _seriesService.GetMostFollowedSeriesAsync(category);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var seriesDtos = new List<ProducerSeriesDto>();
            series.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        // DONE
        public async Task<List<ProducerSeriesDto>> GetDiscoverMostRecentSeries(string category = null)
        {
            var series = await _seriesService.GetDiscoverMostRecentSeriesAsync(category);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var seriesDtos = new List<ProducerSeriesDto>();
            series.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        public async Task<List<ProducerSeriesDto>> GetDiscoverSeriesMostViewedWeekly()
        {
            var seriesIds = await _viewsFilterService.GetMostViewedSeriesIdsWeekly();
            if (seriesIds == null || !seriesIds.Any())
            {
                return new List<ProducerSeriesDto>();
            }

            var series = await _seriesService.GetAllByIdsList(seriesIds, true);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var sorted = series.OrderBy(a => seriesIds.IndexOf(a.Id.ToString())).ToList();

            var seriesDtos = new List<ProducerSeriesDto>();
            sorted.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        public async Task<List<ProducerSeriesDto>> GetDiscoverSeriesMostViewedMonthly()
        {
            var seriesIds = await _viewsFilterService.GetMostViewedSeriesIdsMonthly();
            if (seriesIds == null || !seriesIds.Any())
            {
                return new List<ProducerSeriesDto>();
            }

            var series = await _seriesService.GetAllByIdsList(seriesIds, true);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var sorted = series.OrderBy(a => seriesIds.IndexOf(a.Id.ToString())).ToList();

            var seriesDtos = new List<ProducerSeriesDto>();
            sorted.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        // DONE
        public async Task<List<ProducerSeriesDto>> GetDiscoverSeriesMostViewedAllTime(string category = null)
        {
            var series = await _seriesService.GetMostViewedSeriesAsync(category);
            if (series == null)
            {
                return new List<ProducerSeriesDto>();
            }

            var seriesDtos = new List<ProducerSeriesDto>();
            series.ForEach(a =>
            {
                var dto = Mapper.Map<ProducerSeriesDto>(a);

                var producer = _userService.GetById(a.UserId);

                if (producer != null)
                {
                    dto.ProducerId = a.UserId;
                    dto.ProducerName = producer.ProducerName;
                    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;
                }

                seriesDtos.Add(dto);
            });

            return seriesDtos;
        }

        #endregion

        #region Series for Producer's Page 

        public async Task<List<ProducerSeriesDto>> GetAllByProducer(string producerId)
        {
            var series = await _seriesService.GetAllByProducer(producerId);
            var seriesDtos = series.Select(a => Mapper.Map<ProducerSeriesDto>(a)).ToList();

            seriesDtos.ForEach(a =>
            {
                var producer = _userService.GetById(producerId);

                if(producer == null)
                {
                    return;
                }

                a.ProducerId = producerId;
                a.ProducerName = producer.ProducerName;
                a.ProducerProfileImageUrl = producer.ProfileImageUrl;
            });

            return seriesDtos;
        }

        #endregion

        #region Series for Add Video Page 

        public async Task<List<SeriesLastEpisodeDto>> GetAllWithLastEpisode(string producerId)
        {
            var series = await _seriesService.GetAllByProducer(producerId);
            return series.Select(a => Mapper.Map<SeriesLastEpisodeDto>(a)).ToList();
        }

        public async Task<SeriesLastEpisodeDto> GetSeriesWithLastEpisode(string seriesId, string userId)
        {
            var series = await _seriesService.GetSeriesVerifyUser(seriesId, userId);
            return Mapper.Map<SeriesLastEpisodeDto>(series);
        }

        #endregion

        #region Series Operations CRUD 

        public async Task<SeriesUpdateDto> GetUpdateDtoByIdAsync(string id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null || series.UserId == null)
            {
                return null;
            }

            return Mapper.Map<SeriesUpdateDto>(series);
        }

        public async Task<SeriesViewDto> GetViewByIdAsync(string id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null || series.UserId == null)
            {
                return null;
            }

            var dto = Mapper.Map<SeriesViewDto>(series);

            var producer = await _userService.GetByIdAsync(series.UserId);
            if (producer == null)
            {
                return null;
            }

            dto.ProducerId = series.UserId;
            dto.ProducerName = producer.ProducerName;
            dto.ProducerProfileImageUrl = producer.ProfileImageUrl;

            return dto;
        }

        public async Task<List<string>> GetMyFollowedSeriesIds(string userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            if (user.SeriesFollowed == null || !user.SeriesFollowed.Any())
            {
                return new List<string>();
            }

            return user.SeriesFollowed;
        }

        public async Task<List<SeriesViewListDto>> GetMyFollowedSeries(string userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if(user == null)
            {
                return null;
            }

            if(user.SeriesFollowed == null || !user.SeriesFollowed.Any())
            {
                return new List<SeriesViewListDto>();
            }

            var series = await _seriesService.GetAllByIdsList(user.SeriesFollowed);
            if(series == null || !series.Any())
            {
                return null;
            }

            var dtos = new List<SeriesViewListDto>();
            //series.ForEach(async a =>
            //{
            //    var dto = Mapper.Map<SeriesViewListDto>(a);
            //    dto.SeasonCount = a.LastSeason;
            //    dto.EpisodeCount = await _videoService.CountVideosPerSeriesAsync(dto.Id);

            //    var producer = await _userService.GetByIdAsync(a.UserId);
            //    dto.ProducerId = a.UserId;
            //    dto.ProducerName = producer.ProducerName;
            //    dto.ProducerProfileImageUrl = producer.ProfileImageUrl;

            //    dtos.Add(dto);
            //});

            foreach (var item in series)
            {
                var dto = Mapper.Map<SeriesViewListDto>(item);

                var producer = await _userService.GetByIdAsync(item.UserId);
                dto.ProducerId = item.UserId;
                dto.ProducerName = producer.ProducerName;
                dto.ProducerProfileImageUrl = producer.ProfileImageUrl;

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<bool> CreateAsync(string currentUserId, SeriesCreateInput input)
        {
            var producer = await _userService.GetByIdAsync(currentUserId);
            var status = await _seriesService.CreateAsync(currentUserId, input, producer.IsProducerValidatedByAdmin);
            return status;
        }

        public async Task<bool> UpdateAsync(string id, SeriesUpdateInput input)
        {
            var status = await _seriesService.UpdateAsync(id, input);
            return status;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            var videosIds = await _videoService.GetAllIdsBySeries(id);

            // delete all episodes of the series from the general channel and from the specific channel 
            await _channelService.DeleteVideoListFromChannel(videosIds, "general");
            await _channelService.DeleteVideoListFromChannel(videosIds, series.Category);

            var result = await _seriesService.DeleteAsync(id);
            if (result)
            {
                var deleteVideosResult = await _videoService.DeleteBySeriesAsync(id);
            }

            return result;
        }

        #endregion

        #region Series Validations 

        public async Task<bool> IsSeriesNameUnique(string seriesName, string seriesId, string userId)
        {
            return await _seriesService.IsSeriesNameUnique(seriesName, seriesId, userId);
        }

        public async Task<bool> DoesSeriesBelongToUser(string seriesId, string userId)
        {
            return await _seriesService.DoesSeriesBelongToUser(seriesId, userId);
        }

        public async Task<bool> IsSeasonEpisodeCombinationValid(string seriesId, int season, int episode)
        {
            return await _seriesService.IsSeasonEpisodeCombinationValid(seriesId, season, episode);
        }

        #endregion 
    }
}
