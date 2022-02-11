using Resources.Base.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Application.Services.Video;
using WeVeed.Domain.Services;

namespace WeVeed.Application.Services
{
    public class ChannelAppService : IChannelAppService
    {
        private readonly IChannelService _channelService;
        private readonly IVideoAppService _videoAppService;
        private readonly ISeriesAppService _seriesAppService;

        public ChannelAppService(IChannelService channelService, IVideoAppService videoAppService, ISeriesAppService seriesAppService)
        {
            _channelService = channelService;
            _videoAppService = videoAppService;
            _seriesAppService = seriesAppService;
        }

        public async Task<List<Tuple<string, List<VideoDisplayCarouselDto>>>> Get2EpisodesForEachChannelAsync(string userId)
        {
            var channels = await _channelService.GetAllChannels();
            var videosIds = channels.Select(a => a.Videos).SelectMany(list => list).Distinct().ToList();

            var videos = await _videoAppService.GetAllByIdsList(videosIds);

            var bigList = new List<Tuple<string, List<VideoDisplayCarouselDto>>>();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var mychannel2RandomVideos = await GetMyChannelRandom2VideosListAsync(userId);
                bigList.Add(new Tuple<string, List<VideoDisplayCarouselDto>>("mychannel", mychannel2RandomVideos));
            }

            foreach (var channel in channels)
            {
                // get randomly 2 videos from the list 
                var random3Ids = new List<string>();
                if (channel.Videos.Count <= 3)
                {
                    random3Ids = channel.Videos.Take(3).ToList();
                }
                else
                {
                    var firstElementIndex = new Random().Next(channel.Videos.Count - 1);
                    var secondElementIndex = firstElementIndex == channel.Videos.Count - 1 ? 0 : firstElementIndex + 1;
                    var thirdElementIndex = secondElementIndex == channel.Videos.Count - 1 ? 0 : secondElementIndex + 1;
                    random3Ids.Add(channel.Videos.ElementAt(firstElementIndex));
                    random3Ids.Add(channel.Videos.ElementAt(secondElementIndex));
                    random3Ids.Add(channel.Videos.ElementAt(thirdElementIndex));
                }
                // ------------------------------------

                var random3Videos = videos.Where(a => random3Ids.Contains(a.Id)).ToList();
                bigList.Add(new Tuple<string, List<VideoDisplayCarouselDto>>(channel.Name, random3Videos));
            }

            return bigList;
        }

        public async Task<List<VideoPlayingNowDto>> GetPlayingNowVideoListAsync(GetChannelPlayingNowVideoListInput input)
        {
            var videoIdsList = await _channelService.GetChannelNextVideosForPlayingNow(input);
            if(videoIdsList == null || !videoIdsList.Any())
            {
                return null;
            }

            var dtoList = new List<VideoPlayingNowDto>();
            foreach(var videoId in videoIdsList)
            {
                var videoDto = await _videoAppService.GetPlayingNowVideoDto(videoId);
                if(videoDto != null)
                {
                    dtoList.Add(videoDto);
                }
            }

            return dtoList;
        }

        public async Task<VideoWatchDto> GetChannelCurrentVideo(GetChannelVideoInput input)
        {
            var currentVideoId = await _channelService.GetChannelCurrentVideoId(input);

            if(currentVideoId == null)
            {
                return null;
            }

            var videoWatchDto = await _videoAppService.GetWatchDto(currentVideoId);
            return videoWatchDto.EncodedVideoKey == null ? null : videoWatchDto;
        }

        public async Task<VideoWatchDto> GetChannelNextVideo(GetChannelNextVideoInput input)
        {
            var nextVideoId = await _channelService.GetChannelNextVideoId(input);

            if (nextVideoId == null)
            {
                return null;
            }

            var videoWatchDto = await _videoAppService.GetWatchDto(nextVideoId);
            return videoWatchDto.EncodedVideoKey == null ? null : videoWatchDto;
        }

        public async Task<VideoWatchDto> GetChannelPreviousVideo(GetChannelNextVideoInput input)
        {
            var nextVideoId = await _channelService.GetChannelPreviousVideoId(input);

            if (nextVideoId == null)
            {
                return null;
            }

            var videoWatchDto = await _videoAppService.GetWatchDto(nextVideoId);
            return videoWatchDto.EncodedVideoKey == null ? null : videoWatchDto;
        }

        public async Task<VideoWatchDto> GetMyChannelVideo(string userId, GetMyChannelVideoInput input)
        {
            var seriesIds = await _seriesAppService.GetMyFollowedSeriesIds(userId);
            if(seriesIds == null || !seriesIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Nu urmaresti nicio emisiune. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            var videosIds = await _videoAppService.GetLastVideosBySeriesListAsync(seriesIds);
            if(videosIds == null || !videosIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Emisiunile pe care le urmaresti nu contin episoade. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            string searchedVideoId;
            var lastVideoIndex = videosIds.IndexOf(input.LastVideoId);

            if (input.LastVideoId == null)
            {
                searchedVideoId = videosIds.First();
            }
            else if (lastVideoIndex == videosIds.Count - 1)
            {
                searchedVideoId = videosIds.First();
            }
            else
            {
                searchedVideoId = videosIds.Skip(lastVideoIndex + 1).First();
            }

            var videoDto = await _videoAppService.GetWatchDto(searchedVideoId);
            return videoDto.EncodedVideoKey == null ? null : videoDto;
        }

        public async Task<VideoWatchDto> GetMyChannelNextVideo(string userId, GetMyChannelNextVideoInput input)
        {
            var seriesIds = await _seriesAppService.GetMyFollowedSeriesIds(userId);
            if (seriesIds == null || !seriesIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Nu urmaresti nicio emisiune. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            var videosIds = await _videoAppService.GetLastVideosBySeriesListAsync(seriesIds);
            if (videosIds == null || !videosIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Emisiunile pe care le urmaresti nu contin episoade. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            string searchedVideoId;
            var lastVideoIndex = videosIds.IndexOf(input.CurrentVideoId);

            if (input.CurrentVideoId== null)
            {
                searchedVideoId = videosIds.First();
            }
            else if (lastVideoIndex == videosIds.Count - 1)
            {
                searchedVideoId = videosIds.First();
            }
            else
            {
                searchedVideoId = videosIds.Skip(lastVideoIndex + 1).First();
            }

            var videoDto = await _videoAppService.GetWatchDto(searchedVideoId);
            return videoDto.EncodedVideoKey == null ? null : videoDto;
        }

        public async Task<VideoWatchDto> GetMyChannelPreviousVideo(string userId, GetMyChannelNextVideoInput input)
        {
            var seriesIds = await _seriesAppService.GetMyFollowedSeriesIds(userId);
            if (seriesIds == null || !seriesIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Nu urmaresti nicio emisiune. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            var videosIds = await _videoAppService.GetLastVideosBySeriesListAsync(seriesIds);
            if (videosIds == null || !videosIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Emisiunile pe care le urmaresti nu contin episoade. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            string searchedVideoId;
            var lastVideoIndex = videosIds.IndexOf(input.CurrentVideoId);

            if (input.CurrentVideoId == null)
            {
                searchedVideoId = videosIds.First();
            }
            else if (lastVideoIndex == 0)
            {
                searchedVideoId = videosIds.Last();
            }
            else
            {
                searchedVideoId = videosIds.Skip(lastVideoIndex - 1).First();
            }

            var videoDto = await _videoAppService.GetWatchDto(searchedVideoId);
            return videoDto.EncodedVideoKey == null ? null : videoDto;
        }

        public async Task<List<VideoPlayingNowDto>> GetMyChannelPlayingNowVideoListAsync(string userId, GetMyChannelPlayingNowVideoListInput input)
        {
            var seriesIds = await _seriesAppService.GetMyFollowedSeriesIds(userId);
            if (seriesIds == null || !seriesIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Nu urmaresti nicio emisiune. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            var videosIds = await _videoAppService.GetLastVideosBySeriesListAsync(seriesIds);
            if (videosIds == null || !videosIds.Any())
            {
                throw new HttpStatusCodeException(422, new List<string> { "Emisiunile pe care le urmaresti nu contin episoade. Mergi la pagina Descopera pentru a-ti imbunatati experienta WeVeed." });
            }

            var videoList = new List<string>();
            var playingNowIndex = videosIds.IndexOf(input.PlayingNowVideoId);

            if (playingNowIndex == -1)
            {
                //videosIds.RemoveAt(0);
                videoList.AddRange(videosIds);
            }
            else if (playingNowIndex == 0)
            {
                videosIds.RemoveAt(0);
                videoList.AddRange(videosIds);
            }
            else if (playingNowIndex == videosIds.Count - 1)
            {
                videosIds.RemoveAt(playingNowIndex);
                videoList.AddRange(videosIds);
            }
            else
            {
                videoList.AddRange(videosIds.GetRange(playingNowIndex + 1, videosIds.Count - playingNowIndex - 1));
                videoList.AddRange(videosIds.GetRange(0, playingNowIndex));
            }

            var dtoList = new List<VideoPlayingNowDto>();
            foreach (var videoId in videoList)
            {
                var videoDto = await _videoAppService.GetPlayingNowVideoDto(videoId);
                if (videoDto != null)
                {
                    dtoList.Add(videoDto);
                }
            }

            return dtoList;
        }

        public async Task<List<VideoDisplayCarouselDto>> GetMyChannelRandom2VideosListAsync(string userId)
        {
            var seriesIds = await _seriesAppService.GetMyFollowedSeriesIds(userId);
            if (seriesIds == null || !seriesIds.Any())
            {
                return new List<VideoDisplayCarouselDto>() { };
            }

            var random3VideosDtos = await _videoAppService.GetLastVideoDtosBySeriesListAsync(seriesIds);
            return random3VideosDtos;
        }
    }
}
