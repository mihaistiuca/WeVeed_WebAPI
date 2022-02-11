using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources.Base.AuthUtils;
using Resources.Base.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Application.Services.Comment;
using WeVeedWebAPI.Extensions;

namespace WeVeedWebAPI.Controllers
{
    [Route("[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentAppService _commentAppService;

        public CommentController(ICommentAppService commentAppService)
        {
            _commentAppService = commentAppService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CommentCreateInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var commentString = await _commentAppService.CreateAsync(id, input);
            var response = new BaseResponse<string>(commentString);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] CommentUpdateInput input)
        {
            var id = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (id == null)
            {
                return Response.ValidationError(new BaseResponse(false));
            }

            var result = await _commentAppService.UpdateAsync(id, input);
            var response = new BaseResponse(result);
            return Response.Ok(response);
        }

        [Authorize]
        [HttpDelete("delete/{commentId}")]
        public async Task<IActionResult> Delete(string commentId)
        {
            var userId = User.Claims.FirstOrDefault(a => a.Type == AppClaims.UserId)?.Value;
            if (userId == null)
            {
                return Response.Unauthorized(new BaseResponse(false));
            }

            var commentDeleted = await _commentAppService.DeleteAsync(userId, commentId);
            var response = new BaseResponse(commentDeleted);
            return Response.Ok(response);
        }

        [HttpPost("getAllByVideoPaginated")]
        public async Task<IActionResult> GetAllByVideoPaginated([FromBody] VideoCommentPaginationInput input)
        {
            var comments = await _commentAppService.GetAllByVideoPaginatedAsync(input);
            var response = new BaseResponse<List<CommentDisplayUiDto>>(comments);
            return Response.Ok(response);
        }
    }
}
