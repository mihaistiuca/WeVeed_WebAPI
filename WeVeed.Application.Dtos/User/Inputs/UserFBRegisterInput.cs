
namespace WeVeed.Application.Dtos
{
    public class UserFBRegisterInput
    {
        public string FacebookUserId { get; set; }

        public string FBToken { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProducerName { get; set; }

        public string Email { get; set; }

        public string UserType { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
