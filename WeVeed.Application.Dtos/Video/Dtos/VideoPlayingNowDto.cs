
using System;

namespace WeVeed.Application.Dtos
{
    public class VideoPlayingNowDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Season { get; set; }

        public int? Episode { get; set; }

        public string VideoUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public decimal Length { get; set; }


        public long NumberOfViews { get; set; }


        public string SeriesId { get; set; }

        public string SeriesName { get; set; }

        public string SeriesThumbnail { get; set; }


        public string ProducerId { get; set; }

        public string ProducerName { get; set; }

        public string ProducerThumbnail { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
