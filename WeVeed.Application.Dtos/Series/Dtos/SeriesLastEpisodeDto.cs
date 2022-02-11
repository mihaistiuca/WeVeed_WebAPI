
namespace WeVeed.Application.Dtos
{
    public class SeriesLastEpisodeDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Category { get; set; }

        public int LastEpisode { get; set; }

        public int LastSeason { get; set; }

        public int EpisodesCount { get; set; }
    }
}
