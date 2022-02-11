using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;

namespace WeVeed.Application.Services
{
    public interface IUserAppService
    {
        Task<bool> ValidateProducerByAdmin(ValidateProducerByAdminInput input);

        Task<List<ProducerListViewDto>> SearchProducerAsync(string word);

        Task<List<ProducerListViewDto>> GetDiscoverProducersAsync();

        Task<UserBasicInfoDto> GetBasicInfoById(string id);

        Task<UserBasicInfoDto> GetBasicInfoByFacebookId(string id);

        Task<ProducerViewDto> GetProducerViewDtoAsync(string producerId);

        Task<bool> RegisterAsync(UserRegisterInput input);

        Task<UserBasicInfoDto> RegisterWithFBAsync(UserFBRegisterInput input);

        Task<bool> UpdateUserInfoAsync(string id, UserUpdateInfoInput input);

        Task<bool> UpdateProducerInfoAsync(string id, ProducerUpdateInfoInput input);

        Task<bool> UserBecomeProducerAsync(string id, UserBecomeProducerInput input);

        Task<bool> ConfirmAccountAsync(string guid);

        Task<UserBasicInfoDto> AuthenticateAsync(UserLoginInput input);

        Task<bool> IsProducerNameUnique(string producerName, string id);

        Task<bool> IsEmailUnique(string email, string id);

        Task<List<ProducerListViewDto>> GetDiscoverProducersFollowedWeekly();

        Task<List<ProducerListViewDto>> GetDiscoverProducersFollowedMonthly();

        Task<List<ProducerListViewDto>> GetDiscoverProducersFollowedAllTime();

        Task<List<ProducerListViewDto>> GetDiscoverMostRecentProducers();

        Task<List<ProducerListViewDto>> GetDiscoverProducersMostViewedWeekly();

        Task<List<ProducerListViewDto>> GetDiscoverProducersMostViewedMonthly();

        Task<List<ProducerListViewDto>> GetDiscoverProducersMostViewedAllTime();


        Task DeleteNotValidated();

        Task<bool> SendResetPasswordEmailAsync(ResetPasswordSendEmailInput input);

        Task<bool> ResetPasswordAsync(ResetPasswordInput input);
    }
}
