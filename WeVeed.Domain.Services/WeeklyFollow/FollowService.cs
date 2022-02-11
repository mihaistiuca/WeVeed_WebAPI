using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class FollowService : IFollowService
    {
        const int LimitNumberSeries = 30;
        const int LimitNumberProducer = 30;

        private IMongoCollection<WeeklyFollow> _weeklyFollowCollection;
        private IMongoCollection<MonthlyFollow> _monthlyFollowCollection;

        public FollowService(IMongoDatabase mongoDatabase)
        {
            _weeklyFollowCollection = mongoDatabase.GetCollection<WeeklyFollow>("weeklyfollow");
            _monthlyFollowCollection = mongoDatabase.GetCollection<MonthlyFollow>("monthlyfollow");
        }

        public async Task<bool> AddFollowToSeriesThisWeek(string seriesId, string producerId, string currentUserId, string seriesCategory)
        {
            var filter = Builders<WeeklyFollow>.Filter.Eq(a => a.SeriesId, seriesId) & Builders<WeeklyFollow>.Filter.Eq(a => a.UserId, currentUserId);
            var existingFollow = await _weeklyFollowCollection.Find(filter).ToListAsync();

            if (existingFollow != null && existingFollow.Any(a => a.CreatedDate > DateTime.Now.AddDays(-7)))
            {
                return false;
            }

            var dbFollow = new WeeklyFollow
            {
                SeriesId = seriesId,
                ProducerId = producerId,
                UserId = currentUserId,
                SeriesCategory = seriesCategory,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            await _weeklyFollowCollection.InsertOneAsync(dbFollow);

            return true;
        }

        public async Task<bool> AddFollowToSeriesThisMonth(string seriesId, string producerId, string currentUserId, string seriesCategory)
        {
            var filter = Builders<MonthlyFollow>.Filter.Eq(a => a.SeriesId, seriesId) & Builders<MonthlyFollow>.Filter.Eq(a => a.UserId, currentUserId);
            var existingFollow = await _monthlyFollowCollection.Find(filter).ToListAsync();

            if (existingFollow != null && existingFollow.Any(a => a.CreatedDate > DateTime.Now.AddDays(-30)))
            {
                return false;
            }

            var dbFollow = new MonthlyFollow
            {
                SeriesId = seriesId,
                ProducerId = producerId,
                UserId = currentUserId,
                SeriesCategory = seriesCategory,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            await _monthlyFollowCollection.InsertOneAsync(dbFollow);

            return true;
        }

        public async Task<List<string>> GetTopSeriesIdsWeekly()
        {
            var filter = Builders<WeeklyFollow>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-7));

            var aggregateGroupResult = await _weeklyFollowCollection.Aggregate()
                .Match(filter)
                .Group(a => a.SeriesId,
                    g => new
                    {
                        SeriesId = g.First().SeriesId,
                        Count = g.Count()
                    }).SortByDescending(a=>a.Count).Limit(LimitNumberSeries).ToListAsync();

            var sortedResult = aggregateGroupResult.OrderByDescending(a => a.Count).ToList();

            return sortedResult.Select(a => a.SeriesId).ToList();
        }

        public async Task<List<string>> GetTopSeriesIdsMonthly()
        {
            var filter = Builders<MonthlyFollow>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-30));

            var aggregateGroupResult = await _monthlyFollowCollection.Aggregate()
                .Match(filter)
                .Group(a => a.SeriesId,
                    g => new
                    {
                        SeriesId = g.First().SeriesId,
                        Count = g.Count()
                    }).SortByDescending(a => a.Count).Limit(LimitNumberSeries).ToListAsync();

            var sortedResult = aggregateGroupResult.OrderByDescending(a => a.Count).ToList();

            return sortedResult.Select(a => a.SeriesId).ToList();
        }

        // ------------------------------------------------------------------------------------------------------

        public async Task<List<string>> GetTopProducersIdsWeekly()
        {
            var filter = Builders<WeeklyFollow>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-7));

            var aggregateGroupResult = await _weeklyFollowCollection.Aggregate()
                .Match(filter)
                .Group(a => a.ProducerId,
                    g => new
                    {
                        ProducerId = g.First().ProducerId,
                        Count = g.Count()
                    }).SortByDescending(a => a.Count).Limit(LimitNumberProducer).ToListAsync();

            var sortedResult = aggregateGroupResult.OrderByDescending(a => a.Count).ToList();

            return sortedResult.Select(a => a.ProducerId).ToList();
        }

        public async Task<List<string>> GetTopProducersIdsMonthly()
        {
            var filter = Builders<MonthlyFollow>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-30));

            var aggregateGroupResult = await _monthlyFollowCollection.Aggregate()
                .Match(filter)
                .Group(a => a.ProducerId,
                    g => new
                    {
                        ProducerId = g.First().ProducerId,
                        Count = g.Count()
                    }).SortByDescending(a => a.Count).Limit(LimitNumberProducer).ToListAsync();

            var sortedResult = aggregateGroupResult.OrderByDescending(a => a.Count).ToList();

            return sortedResult.Select(a => a.ProducerId).ToList();
        }
    }
}
