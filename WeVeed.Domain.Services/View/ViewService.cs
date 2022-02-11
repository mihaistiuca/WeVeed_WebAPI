using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class ViewService : IViewService
    {
        private IMongoCollection<View> _viewCollection;
        const int MinutesToIncrementViews = 10;

        public ViewService(IMongoDatabase mongoDatabase)
        {
            _viewCollection = mongoDatabase.GetCollection<View>("view");
        }

        public async Task<string> CreateAsync(string sessionId, string videoId, string seriesCategory)
        {
            var view = new View
            {
                SessionId = sessionId,
                VideoId = videoId,
                SeriesCategory = seriesCategory,
                ViewTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            await _viewCollection.InsertOneAsync(view);
            return view.Id.ToString();
        }

        public async Task<View> GetBySessionAndVideoIdAsync(string sessionId, string videoId)
        {
            var filter = Builders<View>.Filter.Where(a => a.SessionId == sessionId && a.VideoId == videoId);
            var view = (await _viewCollection.FindAsync(filter)).FirstOrDefault();
            return view;
        }

        public async Task<bool> UpdateViewTimeAsync(string sessionId, string videoId)
        {
            var filter = Builders<View>.Filter.Where(a => a.SessionId == sessionId && a.VideoId == videoId);
            var update = Builders<View>.Update.Set(a => a.ViewTime, DateTime.Now);

            var updateResult = await _viewCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }
    }
}
