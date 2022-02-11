
namespace WeVeed.Application.Dtos
{
    public class VideoUpdateDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public int? Season { get; set; }

        public int? Episode { get; set; }

        public string UserProducerId { get; set; }
    }
}
