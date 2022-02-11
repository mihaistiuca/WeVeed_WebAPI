
namespace WeVeed.Application.Dtos
{
    public class ProducerUpdateInfoInput
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool HasProfileImageChanged { get; set; }


        public string ProducerName { get; set; }

        public string ProducerDescription { get; set; }

        public string EmailContact { get; set; }

        public string FacebookContactUrl { get; set; }

        public string InstaContactUrl { get; set; }
    }
}
