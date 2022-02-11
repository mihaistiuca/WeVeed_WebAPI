using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class ViewsFilterService : IViewsFilterService
    {
        const int LimitNumberSeries = 30;
        const int LimitNumberProducer = 30;

        private IMongoCollection<ViewsFilter> _viewsFilterCollection;

        public ViewsFilterService(IMongoDatabase mongoDatabase)
        {
            _viewsFilterCollection = mongoDatabase.GetCollection<ViewsFilter>("viewsfilter");
        }

        public async Task AddViewsFilterAsync(string videoId, string seriesId, string producerId, string seriesCategory)
        {
            if(string.IsNullOrWhiteSpace(videoId) || string.IsNullOrWhiteSpace(seriesId) || string.IsNullOrWhiteSpace(producerId))
            {
                return;
            }

            var newViewsFilter = new ViewsFilter
            {
                VideoId = videoId,
                SeriesId = seriesId,
                ProducerId = producerId,
                SeriesCategory = seriesCategory,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            await _viewsFilterCollection.InsertOneAsync(newViewsFilter);
        }

        // ------------------------------------------------------------------------------------------------------

        public async Task<List<string>> GetMostViewedSeriesIdsWeekly()
        {
            var filter = Builders<ViewsFilter>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-7));

            var aggregateGroupResult = await _viewsFilterCollection.Aggregate()
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

        public async Task<List<string>> GetMostViewedSeriesIdsMonthly()
        {
            var filter = Builders<ViewsFilter>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-30));

            var aggregateGroupResult = await _viewsFilterCollection.Aggregate()
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

        public async Task<List<string>> GetMostViewedProducersIdsWeekly()
        {
            var filter = Builders<ViewsFilter>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-7));

            var aggregateGroupResult = await _viewsFilterCollection.Aggregate()
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

        public async Task<List<string>> GetMostViewedProducersIdsMonthly()
        {
            var filter = Builders<ViewsFilter>.Filter.Where(a => a.CreatedDate > DateTime.Now.AddDays(-30));

            var aggregateGroupResult = await _viewsFilterCollection.Aggregate()
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
