using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeVeed.Application.Dtos;
using WeVeed.Domain.Entities;

namespace WeVeed.Domain.Services
{
    public interface IUserService
    {
        Task<bool> ValidateProducerByAdmin(ValidateProducerByAdminInput input);

        Task<List<User>> SearchProducerAsync(string word);

        User GetById(string id);

        Task<User> GetByIdAsync(string id);

        Task<User> GetByEmailAsync(string email);

        Task<List<User>> GetAllByIdsList(List<string> idsList, bool ignoreSeriesWithProducerNotValidated = false);

        Task<User> GetByFacebookIdAsync(string id);

        Task<List<User>> GetMostPopularProducersAsync();

        Task<List<User>> GetMostRecentProducersAsync();

        Task<string> CreateAsync(UserRegisterInput input, Guid guid);

        Task<string> CreateWithFBAsync(UserFBRegisterInput input);

        Task<bool> UpdateUserAsync(string id, UserUpdateInfoInput input);

        Task<bool> UpdateProducerAsync(string id, ProducerUpdateInfoInput input);

        Task<bool> BecomeProducerAsync(string id, UserBecomeProducerInput input);

        Task<bool> DeleteAsync(string id);

        Task<IEnumerable<User>> GetAllAsync();

        Task<bool> AddSeriesInUserFollowedSeries(string userId, SeriesFollowInput input);

        Task<bool> RemoveSeriesFromUserFollowedSeries(string userId, SeriesFollowInput input);

        Task<bool> ConfirmAccountAsync(string guid);

        Task<User> AuthenticateAsync(string email, string password);

        Task<bool> IsProducerNameUnique(string producerName, string id);

        Task<bool> IsEmailUnique(string email, string id);

        Task<bool> IncrementProducerSeriesFollowersCount(string producerId);

        Task<bool> DecrementProducerSeriesFollowersCount(string producerId);

        Task<List<User>> GetMostFollowedProducers();

        Task<List<User>> GetDiscoverMostRecentProducersAsync();

        Task<bool> IncrementProducerViewsCount(string producerId);

        Task<List<User>> GetMostViewedProducers();



        Task<List<string>> GetNotActivatedUserIds();

        Task<List<string>> GetActivatedUserIds();


        Task<bool> SetResetPasswordInformationAsync(string userId, string resetToken);

        Task<User> GetByResetTokenAsync(string resetToken);

        Task<bool> ResetPasswordAsync(string userId, string newPassword);
    }
}
