
namespace WeVeed.Application.Dtos
{
    public class UserAuthenticateDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Token { get; set; }

        public string UserType { get; set; }

        public string ProfileImageUrl { get; set; }

        public string ProducerName { get; set; }

        public string ProducerDescription { get; set; }


        public string EmailContact { get; set; }

        public string FacebookContactUrl { get; set; }

        public string InstaContactUrl { get; set; }
    }
}
