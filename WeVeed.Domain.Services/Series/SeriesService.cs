using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class SeriesService : ISeriesService
    {
        private IMongoCollection<Series> _seriesCollection;
        const int TopCountRecent = 10;
        const int TopCountTop = 20;
        const int SearchTopCount = 10;

        public SeriesService(IMongoDatabase mongoDatabase)
        {
            _seriesCollection = mongoDatabase.GetCollection<Series>("series");
        }

        #region Series Search and Explore 

        public async Task<List<Series>> SearchSeriesAsync(string word)
        {
            var regWord = "/.*" + word + ".*/i";
            var filter = Builders<Series>.Filter.Regex(a => a.Name, new BsonRegularExpression(regWord)) & Builders<Series>.Filter.Where(a => a.IsProducerValidatedByAdmin);
            var sort = Builders<Series>.Sort.Descending(a => a.FollowersCount);
            var options = new FindOptions<Series>
            {
                Sort = sort,
                Skip = 0,
                Limit = SearchTopCount
            };

            var series = (await _seriesCollection.FindAsync(filter, options)).ToList();
            return series;
        }

        // IMPORTANT
        public async Task<List<Series>> GetMostPopularSeriesAsync(string category = null)
        {
            var filter = Builders<Series>.Filter.Where(a => a.IsProducerValidatedByAdmin);
            if(category != null)
            {
                filter = filter & Builders<Series>.Filter.Where(a => a.Category == category);
            }

            var sort = Builders<Series>.Sort.Descending(a => a.FollowersCount);
            var options = new FindOptions<Series>
            {
                Sort = sort,
                Skip = 0,
                Limit = TopCountTop
            };

            // MIGHT NOT WORK a => true
            var series = (await _seriesCollection.FindAsync(filter, options)).ToList();
            return series;
        }

        // IMPORTANT
        public async Task<List<Series>> GetMostRecentSeriesAsync(string category = null)
        {
            var filter = Builders<Series>.Filter.Where(a => a.IsProducerValidatedByAdmin);
            if (category != null)
            {
                filter = filter & Builders<Series>.Filter.Where(a => a.Category == category);
            }

            var sort = Builders<Series>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<Series>
            {
                Sort = sort,
                Skip = 0,
                Limit = TopCountRecent
            };

            var series = (await _seriesCollection.FindAsync(filter, options)).ToList();
            return series;
        }

        public async Task<List<Series>> GetDiscoverMostRecentSeriesAsync(string category = null)
        {
            var filter = Builders<Series>.Filter.Where(a => a.IsProducerValidatedByAdmin);
            if (category != null)
            {
                filter = filter & Builders<Series>.Filter.Where(a => a.Category == category);
            }

            var sort = Builders<Series>.Sort.Descending(a => a.CreatedDate);
            var options = new FindOptions<Series>
            {
                Sort = sort,
                Skip = 0,
                Limit = 20
            };

            var series = (await _seriesCollection.FindAsync(filter, options)).ToList();
            return series;
        }

        // IMPORTANT
        public async Task<List<Series>> GetMostFollowedSeriesAsync(string category = null)
        {
            var filter = Builders<Series>.Filter.Where(a => a.IsProducerValidatedByAdmin);
            if (category != null)
            {
                filter = filter & Builders<Series>.Filter.Where(a => a.Category == category);
            }

            var sort = Builders<Series>.Sort.Descending(a => a.FollowersCount);
            var options = new FindOptions<Series>
            {
                Sort = sort,
                Skip = 0,
                Limit = 30
            };

            // MIGHT NOT WORK a => true
            var series = (await _seriesCollection.FindAsync(filter, options)).ToList();
            return series;
        }

        // IMPORTANT
        public async Task<List<Series>> GetMostViewedSeriesAsync(string category = null)
        {
            var filter = Builders<Series>.Filter.Where(a => a.IsProducerValidatedByAdmin);
            if (category != null)
            {
                filter = filter & Builders<Series>.Filter.Where(a => a.Category == category);
            }

            var sort = Builders<Series>.Sort.Descending(a => a.NumberOfViewsOnVideos);
            var options = new FindOptions<Series>
            {
                Sort = sort,
                Skip = 0,
                Limit = 30
            };

            // MIGHT NOT WORK a => true
            var series = (await _seriesCollection.FindAsync(filter, options)).ToList();
            return series;
        }

        #endregion

        #region Series Operations CRUD 

        public async Task<List<Series>> GetAllByProducer(string producerId)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.UserId, producerId);
            var series = (await _seriesCollection.FindAsync(filter)).ToList();
            return series;
        }

        public async Task<List<Series>> GetAllByIdsList(List<string> idsList, bool ignoreSeriesWithProducerNotValidated = false)
        {
            var idsBsonArray = idsList.Select(a => new ObjectId(a));
            if (ignoreSeriesWithProducerNotValidated)
            {
                var filter = Builders<Series>.Filter.In(a => a.Id, idsBsonArray.ToArray()) & Builders<Series>.Filter.Where(a => a.IsProducerValidatedByAdmin);
                var series = (await _seriesCollection.FindAsync(filter)).ToList();
                return series;
            }
            else
            {
                var filter = Builders<Series>.Filter.In(a => a.Id, idsBsonArray.ToArray());
                var series = (await _seriesCollection.FindAsync(filter)).ToList();
                return series;
            }
        }

        public Series GetById(string id)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(id));
            var series = _seriesCollection.Find(filter).FirstOrDefault();
            return series;
        }

        public async Task<Series> GetByIdAsync(string id)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(id));
            var series = (await _seriesCollection.FindAsync(filter)).FirstOrDefault();
            return series;
        }

        public async Task<bool> CreateAsync(string userId, SeriesCreateInput input, bool isProducerValidatedByAdmin)
        {
            var dbSeries = new Series
            {
                Name = input.Name.Trim(),
                Description = input.Description,
                ThumbnailUrl = input.ThumbnailUrl,
                Category = input.Category,
                UserId = userId,
                LastEpisode = 0,
                LastSeason = 0,
                IsProducerValidatedByAdmin = isProducerValidatedByAdmin,
                EpisodesCount = 0
            };

            await _seriesCollection.InsertOneAsync(dbSeries);

            return true;
        }

        public async Task<bool> UpdateSeasonEpisodeAsync(string seriesId, int season, int episode)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(seriesId));
            var update = Builders<Series>.Update.Set(a => a.LastSeason, season)
                                                .Set(a => a.LastEpisode, episode);

            var updateResult = await _seriesCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> UpdateAsync(string userId, SeriesUpdateInput input)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(input.Id));
            var update = Builders<Series>.Update.Set(a => a.Name, input.Name.Trim())
                                                .Set(a => a.ThumbnailUrl, input.ThumbnailUrl)
                                                .Set(a => a.Description, input.Description);
            var updateResult = await _seriesCollection.UpdateOneAsync(filter, update);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(id));
            var deleteResult = await _seriesCollection.DeleteOneAsync(filter);
            return (deleteResult.DeletedCount > 0);
        }

        public async Task<int> CountProducerSeriesAsync(string producerId)
        {
            var filter = Builders<Series>.Filter.Where(a => a.UserId == producerId);
            var count = await _seriesCollection.Find(filter).CountDocumentsAsync();
            return (int)count;
        }

        #endregion

        #region Series Follow 

        public async Task<bool> IncrementSeriesFollowersCount(SeriesFollowInput input)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(input.SeriesId));
            var incremenetUpdate = Builders<Series>.Update.Inc(a => a.FollowersCount, 1);
            var updateResult = await _seriesCollection.UpdateOneAsync(filter, incremenetUpdate);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DecrementSeriesFollowersCount(SeriesFollowInput input)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(input.SeriesId));
            var incremenetUpdate = Builders<Series>.Update.Inc(a => a.FollowersCount, -1);
            var updateResult = await _seriesCollection.UpdateOneAsync(filter, incremenetUpdate);
            return updateResult.IsAcknowledged;
        }

        #endregion

        #region SeriesViews

        public async Task<bool> IncrementSeriesViewsCount(string seriesId)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(seriesId));
            var incremenetUpdate = Builders<Series>.Update.Inc(a => a.NumberOfViewsOnVideos, 1);
            var updateResult = await _seriesCollection.UpdateOneAsync(filter, incremenetUpdate);
            return updateResult.IsAcknowledged;
        }

        #endregion

        #region Unicity Validators

        public async Task<bool> IsSeasonEpisodeCombinationValid(string seriesId, int season, int episode)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(seriesId));
            var series = await _seriesCollection.Find(filter).FirstOrDefaultAsync();

            if (series.LastSeason == 0 || series.LastEpisode == 0)
            {
                return season == 1 && episode == 1;
            }

            // the season has increased by one and the episode is 1
            if (season == series.LastSeason + 1 && episode == 1)
            {
                return true;
            }

            // the episode has increased by one and the season remained the same 
            if (episode == series.LastEpisode + 1 && season == series.LastSeason)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsSeriesNameUnique(string seriesName, string seriesId, string userId)
        {
            seriesName = seriesName.Trim().ToLower();
            var regWord = "/^(\\s*)" + seriesName + "(\\s*)$/i";

            if (string.IsNullOrEmpty(seriesId))
            {
                var filter = Builders<Series>.Filter.Eq(a => a.UserId, userId) & Builders<Series>.Filter.Regex(a => a.Name, new BsonRegularExpression(regWord));
                var count = await _seriesCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
            else
            {
                var filter = Builders<Series>.Filter.Ne(a => a.Id, new ObjectId(seriesId)) & 
                    Builders<Series>.Filter.Eq(a => a.UserId, userId) & 
                    Builders<Series>.Filter.Regex(a => a.Name, new BsonRegularExpression(regWord));
                var count = await _seriesCollection.Find(filter).CountDocumentsAsync();
                return count == 0;
            }
        }

        public async Task<bool> DoesSeriesBelongToUser(string seriesId, string userId)
        {
            if(string.IsNullOrWhiteSpace(seriesId) || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(seriesId));
            var series = await _seriesCollection.Find(filter).FirstOrDefaultAsync();
            return series?.UserId == userId;
        }

        public async Task<Series> GetSeriesVerifyUser(string seriesId, string userId)
        {
            if (string.IsNullOrWhiteSpace(seriesId) || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(seriesId));
            var series = await _seriesCollection.Find(filter).FirstOrDefaultAsync();

            return series?.UserId == userId ? series : null;
        }

        #endregion

        #region ValidateByAdmin

        public async Task<bool> ValidateProducerSeriesByAdmin(ValidateProducerByAdminInput input)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.UserId, input.ProducerId);
            var update = Builders<Series>.Update.Set(a => a.IsProducerValidatedByAdmin, true);

            var updateResult = await _seriesCollection.UpdateManyAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        #endregion

        #region Series - Videos 

        public async Task<bool> IncrementSeriesVideosCount(string seriesId)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(seriesId));
            var incremenetUpdate = Builders<Series>.Update.Inc(a => a.EpisodesCount, 1);
            var updateResult = await _seriesCollection.UpdateOneAsync(filter, incremenetUpdate);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DecrementSeriesVideosCount(string seriesId)
        {
            var filter = Builders<Series>.Filter.Eq(a => a.Id, new ObjectId(seriesId));
            var incremenetUpdate = Builders<Series>.Update.Inc(a => a.EpisodesCount, -1);
            var updateResult = await _seriesCollection.UpdateOneAsync(filter, incremenetUpdate);

            var series = await GetByIdAsync(seriesId);
            if(series.EpisodesCount < 0)
            {
                var make0Update = Builders<Series>.Update.Set(a => a.EpisodesCount, 0);
                var updateResult2 = await _seriesCollection.UpdateOneAsync(filter, make0Update);
            }

            return updateResult.IsAcknowledged;
        }

        #endregion
    }
}
