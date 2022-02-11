
namespace WeVeed.Application.Dtos
{
    public class SeriesViewDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Category { get; set; }


        // this is also SeasonCount 
        public int LastSeason { get; set; }

        //this one is good!!! the plural 
        public int EpisodesCount { get; set; }

        public long FollowersCount { get; set; }


        public string ProducerId { get; set; }

        public string ProducerName { get; set; }

        public string ProducerProfileImageUrl { get; set; }
    }
}
