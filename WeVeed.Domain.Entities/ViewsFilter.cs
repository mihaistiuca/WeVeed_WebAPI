
namespace WeVeed.Domain.Entities
{
    public class ViewsFilter : EntityBase
    {
        public string VideoId { get; set; }

        public string SeriesId { get; set; }

        public string ProducerId { get; set; }

        public string SeriesCategory { get; set; }
    }
}
