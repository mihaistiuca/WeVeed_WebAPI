
namespace WeVeed.Application.Dtos
{
    public class ProducerListViewDto
    {
        public string Id { get; set; }

        public string ProducerName { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool IsNew { get; set; } = true;
    }
}
