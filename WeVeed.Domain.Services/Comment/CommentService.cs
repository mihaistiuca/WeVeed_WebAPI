using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public class CommentService : ICommentService
    {
        private IMongoCollection<Comment> _commentCollection;

        public CommentService(IMongoDatabase mongoDatabase)
        {
            _commentCollection = mongoDatabase.GetCollection<Comment>("comment");
        }

        public async Task<string> CreateAsync(string userId, CommentCreateInput input)
        {
            input.Text = input.Text.Trim();
            if (string.IsNullOrWhiteSpace(input.Text))
            {
                return null;
            }

            if (input.Text.Length >= 500)
            {
                return null;
            }

            var dbComment = new Comment
            {
                Text = input.Text,
                VideoId = input.VideoId,
                UserId = userId,
                CommentTime = DateTime.Now
            };

            await _commentCollection.InsertOneAsync(dbComment);

            return dbComment.Id.ToString();
        }

        public async Task<bool> UpdateAsync(string userId, CommentUpdateInput input)
        {
            input.Text = input.Text.Trim();
            if (string.IsNullOrWhiteSpace(input.Text))
            {
                return false;
            }

            if (input.Text.Length >= 500)
            {
                return false;
            }

            var filter = Builders<Comment>.Filter.Eq(a => a.Id, new ObjectId(input.CommentId));
            var comment = (await _commentCollection.FindAsync(filter)).FirstOrDefault();

            if(comment == null)
            {
                return false;
            }

            if (comment.UserId != userId)
            {
                return false;
            }

            var update = Builders<Comment>.Update.Set(a => a.Text, input.Text)
                                                .Set(a => a.ModifiedDate, DateTime.Now);

            var updateResult = await _commentCollection.UpdateOneAsync(filter, update);
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string userId, string commentId)
        {
            var filter = Builders<Comment>.Filter.Eq(a => a.Id, new ObjectId(commentId));
            var comment = (await _commentCollection.FindAsync(filter)).FirstOrDefault();

            if(comment == null || userId != comment.UserId)
            {
                return false;
            }

            var deleteResult = await _commentCollection.DeleteOneAsync(filter);
            return (deleteResult.DeletedCount > 0);
        }

        public async Task<List<Comment>> GetAllByVideoPaginatedAsync(VideoCommentPaginationInput input)
        {
            if (input.PageSize == 0)
            {
                input.PageSize = 10;
            }

            if (input.Page == 0)
            {
                input.Page = 1;
            }

            var filter = Builders<Comment>.Filter.Eq(a => a.VideoId, input.VideoId);
            var sort = Builders<Comment>.Sort.Descending(a => a.CommentTime);
            var options = new FindOptions<Comment>
            {
                Sort = sort,
                Skip = (input.Page - 1) * input.PageSize,
                Limit = input.PageSize
            };

            var comments = (await _commentCollection.FindAsync(filter, options)).ToList();
            return comments;
        }
    }
}
