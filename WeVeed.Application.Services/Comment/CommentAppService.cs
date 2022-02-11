using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Services;

namespace WeVeed.Application.Services.Comment
{
    public class CommentAppService : ICommentAppService
    {
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;

        public CommentAppService(ICommentService commentService, IUserService userService)
        {
            _commentService = commentService;
            _userService = userService;
        }

        public async Task<string> CreateAsync(string userId, CommentCreateInput input)
        {
            var result = await _commentService.CreateAsync(userId, input);
            return result;
        }

        public async Task<bool> UpdateAsync(string userId, CommentUpdateInput input)
        {
            var result = await _commentService.UpdateAsync(userId, input);
            return result;
        }

        public async Task<bool> DeleteAsync(string userId, string commentId)
        {
            var result = await _commentService.DeleteAsync(userId, commentId);
            return result;
        }

        public async Task<List<CommentDisplayUiDto>> GetAllByVideoPaginatedAsync(VideoCommentPaginationInput input)
        {
            var comments = await _commentService.GetAllByVideoPaginatedAsync(input);
            var commentsDtos = comments.Select(a => Mapper.Map<CommentDisplayUiDto>(a)).ToList();

            foreach (var a in commentsDtos)
            {
                if (string.IsNullOrWhiteSpace(a.UserId))
                {
                    continue;
                }

                var user = await _userService.GetByIdAsync(a.UserId);

                if (user == null)
                {
                    continue;
                }

                a.UserIsProducer = user.UserType == "producer";
                a.UserName = a.UserIsProducer ? user.ProducerName : user.FirstName + " " + user.LastName;
                a.UserProfileImageUrl = user.ProfileImageUrl;
            }

            return commentsDtos;
        }
    }
}
