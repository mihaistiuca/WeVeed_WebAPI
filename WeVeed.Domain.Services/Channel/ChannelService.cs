using MongoDB.Driver;
using Resources.Base.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class ChannelService : IChannelService
    {
        private const int _maxNumberOfVideosInChannels = 30;
        private readonly IMongoCollection<Channel> _channelCollection;

        public ChannelService(IMongoDatabase mongoDatabase)
        {
            _channelCollection = mongoDatabase.GetCollection<Channel>("channel");
        }

        public async Task<List<Channel>> GetAllChannels()
        {
            var filter = Builders<Channel>.Filter.In(a => a.Name, WeVeedConstants.Categories);
            var channels = (await _channelCollection.FindAsync(filter)).ToList();
            return channels;
        }

        public async Task<List<string>> GetChannelNextVideosForPlayingNow(GetChannelPlayingNowVideoListInput input)
        {
            if (!WeVeedConstants.Categories.Contains(input.ChannelName))
            {
                throw new HttpStatusCodeException(422, new List<string> { "Canalul nu exista." });
            }

            var filter = Builders<Channel>.Filter.Eq(a => a.Name, input.ChannelName);
            var channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();

            if (channel == null || channel.Videos == null || !channel.Videos.Any())
            {
                return null;
            }

            if (input.PlayingNowVideoId == null)
            {
                return channel.Videos;
            }

            var videoList = new List<string>();
            var playingNowIndex = channel.Videos.IndexOf(input.PlayingNowVideoId);

            if(playingNowIndex == -1)
            {
                return channel.Videos;
            }
            else if(playingNowIndex == 0)
            {
                channel.Videos.RemoveAt(0);
                return channel.Videos;
            }
            else if(playingNowIndex == channel.Videos.Count - 1)
            {
                channel.Videos.RemoveAt(playingNowIndex);
                return channel.Videos;
            }
            else
            {
                videoList.AddRange(channel.Videos.GetRange(playingNowIndex + 1, channel.Videos.Count - playingNowIndex - 1));
                videoList.AddRange(channel.Videos.GetRange(0, playingNowIndex));
            }

            return videoList;
        }

        public async Task<string> GetChannelCurrentVideoId(GetChannelVideoInput input)
        {
            if (!WeVeedConstants.Categories.Contains(input.ChannelName))
            {
                throw new HttpStatusCodeException(422, new List<string> { "Canalul nu exista." });
            }

            var filter = Builders<Channel>.Filter.Eq(a => a.Name, input.ChannelName);
            var channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();

            if(channel == null || channel.Videos == null || !channel.Videos.Any())
            {
                return null;
            }

            if(input.LastVideoId == null)
            {
                return channel.Videos.First();
            }

            var lastVideoIndex = channel.Videos.IndexOf(input.LastVideoId);

            if(lastVideoIndex == channel.Videos.Count - 1)
            {
                return channel.Videos.First();
            }

            return channel.Videos.Skip(lastVideoIndex + 1).First();
        }

        public async Task<string> GetChannelNextVideoId(GetChannelNextVideoInput input)
        {
            if (!WeVeedConstants.Categories.Contains(input.ChannelName))
            {
                throw new HttpStatusCodeException(422, new List<string> { "Canalul nu exista." });
            }

            if (input.CurrentVideoId == null)
            {
                throw new HttpStatusCodeException(422, new List<string> { "Video-ul curent este obligatoriu." });
            }

            var filter = Builders<Channel>.Filter.Eq(a => a.Name, input.ChannelName);
            var channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();

            if (channel == null || channel.Videos == null || !channel.Videos.Any())
            {
                return null;
            }

            var currentVideoIndex = channel.Videos.IndexOf(input.CurrentVideoId);

            if (currentVideoIndex == channel.Videos.Count - 1)
            {
                return channel.Videos.First();
            }

            return channel.Videos.Skip(currentVideoIndex + 1).First();
        }

        public async Task<string> GetChannelPreviousVideoId(GetChannelNextVideoInput input)
        {
            if (!WeVeedConstants.Categories.Contains(input.ChannelName))
            {
                throw new HttpStatusCodeException(422, new List<string> { "Canalul nu exista." });
            }

            if (input.CurrentVideoId == null)
            {
                throw new HttpStatusCodeException(422, new List<string> { "Video-ul curent este obligatoriu." });
            }

            var filter = Builders<Channel>.Filter.Eq(a => a.Name, input.ChannelName);
            var channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();

            if (channel == null || channel.Videos == null || !channel.Videos.Any())
            {
                return null;
            }

            var currentVideoIndex = channel.Videos.IndexOf(input.CurrentVideoId);

            if (currentVideoIndex == -1)
            {
                return channel.Videos.First();
            }

            if (currentVideoIndex == 0)
            {
                return channel.Videos.Last();
            }

            return channel.Videos.Skip(currentVideoIndex - 1).First();
        }

        public async Task<string> AddVideoInChannel(string videoId, string channelName)
        {
            var filter = Builders<Channel>.Filter.Eq(a => a.Name, channelName);
            var channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();

            if(channel == null)
            {
                await AddNewChannel(channelName);
                channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();
            }

            var currentVideoList = channel.Videos;
            string removedVideoId = null;
            if(currentVideoList.Count >= _maxNumberOfVideosInChannels)
            {
                removedVideoId = currentVideoList.FirstOrDefault();
                currentVideoList.RemoveAt(0);
            }
            currentVideoList.Add(videoId);

            var update = Builders<Channel>.Update.Set(a => a.Videos, currentVideoList);
            var updateResult = await _channelCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged ? removedVideoId : null;
        }

        public async Task AddNewChannel(string channelName)
        {
            var dbChannel = new Channel
            {
                Name = channelName,
                Videos = new List<string>(),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            await _channelCollection.InsertOneAsync(dbChannel);
        }

        public async Task DeleteVideoFromChannel(string videoId, string channelName)
        {
            try
            {
                //var filter = Builders<Channel>.Filter.Eq(a => a.Name, channelName);
                //var update = Builders<Channel>.Update.PullFilter(a => a.Videos, c => c == videoId);
                //var channel = await _channelCollection.UpdateOneAsync(filter, update);

                var filter = Builders<Channel>.Filter.Eq(a => a.Name, channelName);
                var channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();

                var removeResult = channel.Videos.Remove(videoId);
                if (!removeResult)
                {
                    return;
                }

                var update = Builders<Channel>.Update.Set(a => a.Videos, channel.Videos);

                var updateResult = await _channelCollection.UpdateOneAsync(filter, update);
            }
            catch (Exception e)
            {
            }
        }

        public async Task DeleteVideoListFromChannel(List<string> videoIds, string channelName)
        {
            try
            {
                //var filter = Builders<Channel>.Filter.Eq(a => a.Name, channelName);
                //var update = Builders<Channel>.Update.PullFilter(a => a.Videos, c => c == videoId);
                //var channel = await _channelCollection.UpdateOneAsync(filter, update);

                var filter = Builders<Channel>.Filter.Eq(a => a.Name, channelName);
                var channel = (await _channelCollection.FindAsync(filter)).FirstOrDefault();

                videoIds.ForEach(a =>
                {
                     channel.Videos.Remove(a);
                });

                var update = Builders<Channel>.Update.Set(a => a.Videos, channel.Videos);

                var updateResult = await _channelCollection.UpdateOneAsync(filter, update);
            }
            catch (Exception e)
            {
            }
        }
    }
}
