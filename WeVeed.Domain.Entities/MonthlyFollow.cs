
namespace WeVeed.Domain.Entities
{
    public class MonthlyFollow : EntityBase
    {
        public string SeriesId { get; set; }

        public string ProducerId { get; set; }

        public string UserId { get; set; }

        public string SeriesCategory { get; set; }
    }
}
