using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class VideoService : IVideoService
    {
        private IMongoCollection<Video> _videoCollection;
        const int TopCountRecent = 15;
        const int TopCountTop = 25;
        const int SearchTopCount = 30;

        public VideoService(IMongoDatabase mongoDatabase)
        {
            _videoCollection = mongoDatabase.GetCollection<Video>("video");
        }

        public async Task<List<Video>> GetAllByIdsList(List<string> idsList)
        {
            var idsBsonArray = idsList.Select(a => new ObjectId(a));

            var filter = Builders<Video>.Filter.In(a => a.Id, idsBsonArray.ToArray());
            var videos = (await _videoCollection.FindAsync(filter)).ToList();
            return videos;
        }

        public async Task<List<Video>> SearchVideoAsync(string word)
        {
            var regWord = "/.*" + word + ".*/i";
            var filter = Builders<Video>.Filter.Regex(a => a.Title, new BsonRegularExpression(regWord)) & Builders<Video>.Filter.Where(a => a.EncodedVideoKey != null && a.IsProducerValidatedByAdmin);
            var sort = Builders<Video>.Sort.Descending(a => a.NumberOfViews);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = 0,
                Limit = SearchTopCount
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        // IMPORTANT
        public async Task<List<Video>> GetMostPopularVideosAsync(string channelName, int? skip = 0, int? limit = TopCountTop)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.SeriesCategory, channelName) 
                & Builders<Video>.Filter.Where(a => a.EncodedVideoKey != null)
                & Builders<Video>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddMonths(-2) && a.IsProducerValidatedByAdmin);
            var sort = Builders<Video>.Sort.Descending(a => a.NumberOfViews);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = skip,
                Limit = limit
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        // IMPORTANT
        public async Task<List<Video>> GetMostRecentVideosAsync(string channelName, int? skip = 0, int? limit = TopCountRecent)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.SeriesCategory, channelName) & Builders<Video>.Filter.Where(a => a.EncodedVideoKey != null && a.IsProducerValidatedByAdmin);
            var sort = Builders<Video>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = skip,
                Limit = limit
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        public async Task<Video> GetByIdAsync(string id)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.Id, new ObjectId(id));
            var video = (await _videoCollection.FindAsync(filter)).FirstOrDefault();
            return video;
        }

        public async Task<string> CreateAsync(VideoCreateInput input, string userId, bool isProducerValidatedByAdmin)
        {
            var dbVideo = new Video
            {
                Title = input.Title.Trim(),
                Description = input.Description,
                Season = string.IsNullOrWhiteSpace(input.SeriesId) ? null : input.Season,
                Episode = string.IsNullOrWhiteSpace(input.SeriesId) ? null : input.Episode,
                VideoUrl = input.VideoUrl,
                ThumbnailUrl = input.ThumbnailUrl,

                SeriesId = input.SeriesId,
                SeriesCategory = input.SeriesCategory,

                UserProducerId = userId,
                Length = input.Length,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                NumberOfLikes = 0,
                NumberOfViews = 0,

                VideoGotOutOfChannels = false,

                ControlbarThumbnailsUrl = input.ControlbarThumbnailsUrl,
                EncodedVideoKey = null,

                IsProducerValidatedByAdmin = isProducerValidatedByAdmin
            };

            await _videoCollection.InsertOneAsync(dbVideo);
            return dbVideo.Id.ToString();
        }

        public async Task<bool> UpdateAsync(VideoUpdateInput input)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.Id, new ObjectId(input.Id));
            var update = Builders<Video>.Update.Set(a => a.Title, input.Title.Trim())
                                                .Set(a => a.ThumbnailUrl, input.ThumbnailUrl)
                                                .Set(a => a.Description, input.Description);
            var updateResult = await _videoCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> UpdateRemovedFromChannelFlagAsync(string videoId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.Id, new ObjectId(videoId));
            var update = Builders<Video>.Update.Set(a => a.VideoGotOutOfChannels, true);
            var updateResult = await _videoCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string videoId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.Id, new ObjectId(videoId));
            var deleteResult = await _videoCollection.DeleteOneAsync(filter);
            return (deleteResult.DeletedCount > 0);
        }

        public async Task<bool> DeleteBySeriesAsync(string seriesId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.SeriesId, seriesId);
            var deleteResult = await _videoCollection.DeleteManyAsync(filter);
            return (deleteResult.DeletedCount > 0);
        }

        public async Task<List<Video>> GetAllUserPaginatedAsync(string userId, AllVideoPaginateInput input)
        {
            if(input.PageSize == 0)
            {
                input.PageSize = 10;
            }

            if (input.Page == 0)
            {
                input.Page = 1;
            }

            var filter = Builders<Video>.Filter.Eq(a => a.UserProducerId, userId);
            var sort = Builders<Video>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = (input.Page - 1) * input.PageSize,
                Limit = input.PageSize
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        public async Task<List<Video>> GetAllSeriesPaginatedAsync(string seriesId, AllVideoPaginateInput input)
        {
            if (input.PageSize == 0)
            {
                input.PageSize = 10;
            }

            if (input.Page == 0)
            {
                input.Page = 1;
            }

            var filter = Builders<Video>.Filter.Eq(a => a.SeriesId, seriesId);
            var sort = Builders<Video>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = (input.Page - 1) * input.PageSize,
                Limit = input.PageSize
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        public async Task<List<string>> GetAllIdsBySeries(string seriesId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.SeriesId, seriesId);
            var videos = await _videoCollection.FindAsync(filter);
            var videosList = videos.ToList();

            return videosList.Select(a => a.Id.ToString()).ToList();
        }

        public async Task<List<Video>> GetMostRecentByProducerAsync(string producerId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.UserProducerId, producerId);
            var sort = Builders<Video>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = 0,
                Limit = 10
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        public async Task<List<Video>> GetMostViewedByProducerAsync(string producerId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.UserProducerId, producerId);
            var sort = Builders<Video>.Sort.Descending(a => a.NumberOfViews);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = 0,
                Limit = 10
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        public async Task<List<Video>> GetLastVideosBySeriesListAsync(List<string> seriesIds)
        {
            // IMPORTANT - used in my channel so they should be filtered by EncodedKey != null 
            var filter = Builders<Video>.Filter.Where(a => seriesIds.Contains(a.SeriesId)) & Builders<Video>.Filter.Where(a => a.EncodedVideoKey != null);
            var sort = Builders<Video>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = 0,
                Limit = 30
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        public async Task<List<Video>> GetMostViewedBySeries(string seriesId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.SeriesId, seriesId);
            var sort = Builders<Video>.Sort.Descending(a => a.NumberOfViews);
            var options = new FindOptions<Video>
            {
                Sort = sort,
                Skip = 0,
                Limit = 10
            };

            var videos = (await _videoCollection.FindAsync(filter, options)).ToList();
            return videos;
        }

        public async Task<int> CountVideosPerSeriesAsync(string seriesId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.SeriesId, seriesId);
            var count = await _videoCollection.CountDocumentsAsync(filter);
            return (int)count;
        }

        public async Task<bool> DoesVideoBelongToUser(string videoId, string userId)
        {
            if (string.IsNullOrWhiteSpace(videoId) || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var filter = Builders<Video>.Filter.Eq(a => a.Id, new ObjectId(videoId));
            var video = await _videoCollection.Find(filter).FirstOrDefaultAsync();
            return video?.UserProducerId == userId;
        }

        public async Task<Tuple<int, int>> GetLastEpisodeAndSeasonBySeries(string seriesId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.SeriesId, seriesId);
            var videos = (await _videoCollection.FindAsync(filter)).ToList();
            var lastVideo = videos.OrderByDescending(a => a.Season).ThenByDescending(a => a.Episode).FirstOrDefault();

            if(lastVideo == null || !lastVideo.Season.HasValue || !lastVideo.Episode.HasValue)
            {
                return new Tuple<int, int>(0, 0);
            }

            return new Tuple<int, int>(lastVideo.Season.Value, lastVideo.Episode.Value);
        }

        public async Task<int> CountProducerEpisodesAsync(string producerId)
        {
            var filter = Builders<Video>.Filter.Where(a => a.UserProducerId == producerId);
            var count = await _videoCollection.Find(filter).CountDocumentsAsync();
            return (int)count;
        }

        public async Task<bool> IncrementViewsAsync(string videoId)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.Id, new ObjectId(videoId));
            var increment = Builders<Video>.Update.Inc(a => a.NumberOfViews, 1);
            var updateResult = await _videoCollection.UpdateOneAsync(filter, increment);

            return updateResult.IsAcknowledged;
        }

        #region ValidateByAdmin

        public async Task<bool> ValidateProducerVideosByAdmin(ValidateProducerByAdminInput input)
        {
            var filter = Builders<Video>.Filter.Eq(a => a.UserProducerId, input.ProducerId);
            var update = Builders<Video>.Update.Set(a => a.IsProducerValidatedByAdmin, true);

            var updateResult = await _videoCollection.UpdateManyAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        #endregion
    }
}