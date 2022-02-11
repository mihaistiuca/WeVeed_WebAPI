
namespace WeVeed.Application.Dtos
{
    public class VideoCommentPaginationInput
    {
        public string VideoId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
