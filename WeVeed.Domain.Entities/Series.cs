
namespace WeVeed.Domain.Entities
{
    public class Series : EntityBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Category { get; set; }

        public string UserId { get; set; }

        public int LastEpisode { get; set; }

        public int LastSeason { get; set; }

        public int EpisodesCount { get; set; }


        public long FollowersCount { get; set; }

        // IMPORTANT 
        public long NumberOfViewsOnVideos { get; set; }

        public bool IsProducerValidatedByAdmin { get; set; } = false;
    }
}
