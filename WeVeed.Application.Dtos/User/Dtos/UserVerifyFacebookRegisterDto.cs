
namespace WeVeed.Application.Dtos
{
    public class UserVerifyFacebookRegisterDto
    {
        public bool DoesUserAlreadyExist { get; set; }

        public UserBasicInfoDto LoggedUser { get; set; }
    }
}
