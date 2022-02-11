
namespace WeVeed.Application.Dtos
{
    public class UserUpdateInfoInput
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool HasProfileImageChanged { get; set; }
    }
}
