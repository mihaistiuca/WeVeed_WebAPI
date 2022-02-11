using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public interface ICommentService
    {
        Task<string> CreateAsync(string userId, CommentCreateInput input);

        Task<bool> UpdateAsync(string userId, CommentUpdateInput input);

        Task<bool> DeleteAsync(string userId, string commentId);

        Task<List<Comment>> GetAllByVideoPaginatedAsync(VideoCommentPaginationInput input);
    }
}
