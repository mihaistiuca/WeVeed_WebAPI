
namespace WeVeed.Application.Dtos
{
    public class VideoWatchDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Season { get; set; }

        public int? Episode { get; set; }

        public string VideoUrl { get; set; }

        // AFTER ENCODING KEY
        public string EncodedVideoKey { get; set; }

        public string ThumbnailUrl { get; set; }

        public string UserProducerId { get; set; }

        public decimal Length { get; set; }

        public string SeriesCategory { get; set; }


        public long NumberOfViews { get; set; }

        public int NumberOfLikes { get; set; }



        public string SeriesId { get; set; }

        public string SeriesName { get; set; }

        public string SeriesThumbnail { get; set; }


        public string ProducerId { get; set; }

        public string ProducerName { get; set; }

        public string ProducerThumbnail { get; set; }


        public string ControlbarThumbnailsUrl { get; set; }
    }
}
