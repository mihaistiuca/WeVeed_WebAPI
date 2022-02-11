
namespace WeVeed.Application.Dtos
{
    public class VideoCreateInput
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int? Season { get; set; }

        public int? Episode { get; set; }

        public string VideoUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string SeriesId { get; set; }

        public string SeriesCategory { get; set; }

        public decimal Length { get; set; }


        public string ControlbarThumbnailsUrl { get; set; }
    }
}
