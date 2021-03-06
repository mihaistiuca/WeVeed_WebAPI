using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services.Comment
{
    public interface ICommentAppService
    {
        Task<string> CreateAsync(string userId, CommentCreateInput input);

        Task<bool> UpdateAsync(string userId, CommentUpdateInput input);

        Task<bool> DeleteAsync(string userId, string commentId);

        Task<List<CommentDisplayUiDto>> GetAllByVideoPaginatedAsync(VideoCommentPaginationInput input);
    }
}
