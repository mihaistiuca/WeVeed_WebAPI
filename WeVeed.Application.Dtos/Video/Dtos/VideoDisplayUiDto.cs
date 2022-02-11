using System;

namespace WeVeed.Application.Dtos
{
    public class VideoDisplayUiDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int? Season { get; set; }

        public int? Episode { get; set; }

        public string ThumbnailUrl { get; set; }

        public decimal Length { get; set; }

        public long NumberOfViews { get; set; }

        public int NumberOfLikes { get; set; }

        public string UserProducerId { get; set; }


        public string SeriesId { get; set; }

        public string SeriesName { get; set; }

        public string SeriesThumbnail { get; set; }

        // AFTER ENCODING KEY
        public string EncodedVideoKey { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
