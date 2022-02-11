
namespace WeVeed.Application.Dtos
{
    public class ProducerSeriesDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Category { get; set; }


        public string ProducerId { get; set; }

        public string ProducerName { get; set; }

        public string ProducerProfileImageUrl { get; set; }

        public long FollowersCount { get; set; }


        public bool IsNew { get; set; }


        public int LastSeason { get; set; }

        public int EpisodesCount { get; set; }
    }
}
