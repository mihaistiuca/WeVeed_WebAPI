
namespace WeVeed.Application.Dtos
{
    public class ProducerViewDto
    {
        public string Id { get; set; }

        public string ProducerName { get; set; }

        public string ProducerDescription { get; set; }

        public int NumberOfSeries { get; set; }

        public int NumberOfSeriesEpisodes { get; set; }

        public long NumberOfSeriesFollowers { get; set; }

        public string ProfileImageUrl { get; set; }


        public string EmailContact { get; set; }

        public string FacebookContactUrl { get; set; }

        public string InstaContactUrl { get; set; }

    }
}
