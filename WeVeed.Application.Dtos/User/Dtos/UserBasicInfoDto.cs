
using System.Collections.Generic;

namespace WeVeed.Application.Dtos
{
    public class UserBasicInfoDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserType { get; set; }

        public string ProfileImageUrl { get; set; }

        public string ProducerName { get; set; }

        public string ProducerDescription { get; set; }

        public string EmailContact { get; set; }

        public string FacebookContactUrl { get; set; }

        public string InstaContactUrl { get; set; }

        public bool IsProducerValidatedByAdmin { get; set; } = false;


        public List<string> SeriesFollowed { get; set; } = new List<string>();


        public string Token { get; set; }
    }
}
